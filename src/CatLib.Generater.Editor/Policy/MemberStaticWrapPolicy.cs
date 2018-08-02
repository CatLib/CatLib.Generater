/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CatLib.Generater.Editor.Policy
{
    /// <summary>
    /// 成员静态包装策略
    /// </summary>
    public class MemberStaticWrapPolicy : IPolicy
    {
        /// <summary>
        /// 已经生成的类
        /// </summary>
        private readonly Dictionary<string, CodeTypeMember> methods;

        /// <summary>
        /// 上下文
        /// </summary>
        private Context.Context context;

        /// <summary>
        /// 静态Instance的名字
        /// </summary>
        public string StaticInstance;

        /// <summary>
        /// 事件模版
        /// </summary>
        private readonly string eventTemplate =
 @"    public static event {type} {name} {
        add {  
            {instance}.{event} += value;
        }
        remove {
            {instance}.{event} -= value;
        }
    }";

        /// <summary>
        /// CodeDom服务
        /// </summary>
        private readonly CodeDomProvider codeDomProvider;


        /// <summary>
        /// 构建一个成员静态包装策略
        /// </summary>
        public MemberStaticWrapPolicy()
        {
            methods = new Dictionary<string, CodeTypeMember>();
            StaticInstance = "Instance";
            codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            this.context = context;
            methods.Clear();
            ScanningType(context.Original);
            ScanningInterface(context.Original);

            foreach (var method in methods)
            {
                context.Class.Members.Add(method.Value);
            }
        }

        /// <summary>
        /// 扫描指定类型中的函数
        /// </summary>
        /// <param name="type">指定类型</param>
        private void ScanningType(Type type)
        {
            while (type != null)
            {
                Imports(type);
                type = type.BaseType;
            }
        }

        /// <summary>
        /// 对接口进行扫描
        /// </summary>
        /// <param name="type">提取当前类型的实现接口类型中的函数</param>
        private void ScanningInterface(Type type)
        {
            foreach (var baseInterface in type.GetInterfaces())
            {
                Imports(baseInterface);
                ScanningInterface(baseInterface);
            }
;       }

        /// <summary>
        /// 为指定类型提取方法
        /// </summary>
        /// <param name="type">需要提取函数的类型</param>
        private void Imports(Type type)
        {
            foreach (var method in type.GetMethods())
            {
                ImportMember(method);
            }
        }

        /// <summary>
        /// 导入成员，包括允许的特殊函数
        /// </summary>
        /// <param name="method">需要导入的成员函数</param>
        private void ImportMember(MethodInfo method)
        {
            if (method.IsSpecialName)
            {
                // 导入一些特殊的成员，如：属性，事件
                ImportSpecialMethod(method);
            }
            else
            {
                ImportMethod(method);
            }

            Console.WriteLine(method.Name);
        }

        /// <summary>
        /// 导入返回值为<see cref="Void"/>的函数
        /// </summary>
        /// <param name="method">需要导入的成员函数</param>
        private void ImportMethod(MethodInfo method)
        {
            if (methods.ContainsKey(method.Name))
            {
                return;
            }

            var generate = CreateMethod(method.Name, method);
            var parameters = AttachParameter(generate, method);

            if (method.ReturnType != typeof(void))
            {
                generate.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(GetInstance(), method.Name
                    ), parameters
                )));
            }
            else
            {
                generate.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(GetInstance(), method.Name
                    ), parameters
                ));
            }

            methods[method.Name] = generate;
        }

        /// <summary>
        /// 附上成员参数信息
        /// </summary>
        /// <param name="member">成员方法</param>
        /// <param name="method">需要导入的成员函数</param>
        private CodeExpression[] AttachParameter(CodeMemberMethod member, MethodInfo method)
        {
            var parameters = method.GetParameters();
            var result = new CodeExpression[parameters.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                var parameterDeclaration = new CodeParameterDeclarationExpression
                {
                    Name = parameter.Name,
                    Type = new CodeTypeReference(parameter.ParameterType.ToString().TrimEnd('&')),
                };

                var direction = string.Empty;
                // in, out, ref 参数处理
                if (parameter.IsOut)
                {
                    parameterDeclaration.Direction = FieldDirection.Out;
                    direction = "out ";
                }
                else if (parameter.IsIn)
                {
                    parameterDeclaration.Direction = FieldDirection.In;
                }
                else if (parameter.ParameterType.IsByRef)
                {
                    parameterDeclaration.Direction = FieldDirection.Ref;
                    direction = "ref ";
                }

                // 带有默认值的可选参数处理
                if (parameter.IsOptional)
                {
                    parameterDeclaration.Name += " = " + ToDefaultValueString(parameter.DefaultValue);
                }
                //else if (index == (parameters.Length - 1) && parameter.ParameterType.IsArray)
                //{
                //    parameterDeclaration.Type = new CodeTypeReference("params " + parameterDeclaration.Type.BaseType + "[]");
                //}

                member.Parameters.Add(parameterDeclaration);
                result[index] = new CodeVariableReferenceExpression
                {
                    VariableName = direction + parameter.Name
                };
            }

            return result;
        }

        /// <summary>
        /// 创建一个GetSet的特殊方法
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <param name="method">原始方法</param>
        private CodeMemberMethod CreateMethod(string name, MethodInfo method)
        {
            var member = new CodeMemberMethod
            {
                Name = name,
                Attributes = MemberAttributes.Static | MemberAttributes.Public,
                ReturnType = new CodeTypeReference(method.ReturnType.ToString())
            };
            return member;
        }

        /// <summary>
        /// 导入具备特殊名字的方法
        /// </summary>
        /// <param name="method">方法信息</param>
        private void ImportSpecialMethod(MethodInfo method)
        {
            var segment = method.Name.Split(new char[] { '_' }, 2);

            if (segment.Length < 2)
            {
                throw new GenerateException("Special method info is Invalid");
            }

            switch (segment[0])
            {
                case "add":
                case "remove":
                    ImportAddRemoveSpecialMethod(segment[1], method);
                    break;
                case "get":
                    ImportGetSetSpecialMethod(segment[1], method, true);
                    break;
                case "set":
                    ImportGetSetSpecialMethod(segment[1], method, false);
                    break;
                default:
                    throw new GenerateException("Unknow special method [" + segment[0] + "]");
            }
        }

        /// <summary>
        /// 生成一个AddRemove的特殊方法
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <param name="method">原始方法信息</param>
        private void ImportAddRemoveSpecialMethod(string name, MethodInfo method)
        {
            if (methods.ContainsKey(name))
            {
                return;
            }

            CodeSnippetTypeMember generate;
            methods[name] = generate = CreateAddRemoveSepcialMethod(name, method);

            var code = eventTemplate.Replace("{type}", GenerateExpression(new CodeTypeReferenceExpression(
                new CodeTypeReference(context.Original.GetEvent(name).EventHandlerType.ToString()))));
            code = code.Replace("{name}", name);
            code = code.Replace("{instance}", StaticInstance);
            code = code.Replace("{event}", name);
            generate.Text = code;
        }

        /// <summary>
        /// 创建一个GetSet的特殊方法
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <param name="method">原始方法</param>
        private CodeSnippetTypeMember CreateAddRemoveSepcialMethod(string name, MethodInfo method)
        {
            var member = new CodeSnippetTypeMember
            {
                Name = name,
                Attributes = MemberAttributes.Static | MemberAttributes.Public,
            };
            return member;
        }

        /// <summary>
        /// 导入GetSet的特殊方法
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <param name="method">方法信息</param>
        /// <param name="isGet">是否是Get策略</param>
        private void ImportGetSetSpecialMethod(string name, MethodInfo method, bool isGet)
        {
            CodeTypeMember member;
            if (!methods.TryGetValue(name, out member))
            {
                methods[name] = member = CreateGetSetSepcialMethod(name, method);
            }

            var generate = (CodeMemberProperty)member;

            if (isGet)
            {
                if (generate.HasGet)
                {
                    throw new GenerateException("Method [" + name + "] is alreay generate Get Attributes");
                }

                generate.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                        GetInstance(),
                        name)));
            }
            else
            {
                if (generate.HasSet)
                {
                    throw new GenerateException("Method [" + name + "] is alreay generate Set Attributes");
                }

                generate.SetStatements.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                        GetInstance(),
                        name
                    ), new CodePropertySetValueReferenceExpression()));
            }
        }

        /// <summary>
        /// 创建一个GetSet的特殊方法
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <param name="method">原始方法</param>
        private CodeTypeMember CreateGetSetSepcialMethod(string name, MethodInfo method)
        {
            var memberProperty = method.ReturnType;
            if (memberProperty == typeof(void))
            {
                var property = context.Original.GetProperty(name);
                if (property != null)
                {
                    memberProperty = property.PropertyType;
                }
            }

            if (memberProperty == null)
            {
                throw new GenerateException("Property [" + name + "] type cannot be null");
            }

            var member = new CodeMemberProperty
            {
                Name = name,
                Attributes = MemberAttributes.Static | MemberAttributes.Public,
                Type = new CodeTypeReference(memberProperty.ToString())
            };
            return member;
        }

        /// <summary>
        /// 获取实例表达式
        /// </summary>
        /// <returns></returns>
        private CodeExpression GetInstance()
        {
            return new CodeVariableReferenceExpression(StaticInstance);
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>代码结构</returns>
        private string GenerateExpression(CodeExpression expression)
        {
            using (var sw = new StringWriter(new StringBuilder()))
            {
                codeDomProvider.GenerateCodeFromExpression(expression, sw, new CodeGeneratorOptions
                {
                    ElseOnClosing = true,
                });
                return sw.ToString();
            }
        }

        /// <summary>
        /// 转为默认值所使用的字符串
        /// </summary>
        /// <param name="data">基础数据</param>
        /// <returns>被转换的字符串</returns>
        private string ToDefaultValueString(object data)
        {
            if (data == null)
            {
                return "null";
            }

            if (data is string)
            {
                return "\"" + data + "\"";
            }

            if (data is float)
            {
                return data + "f";
            }

            if (data is double)
            {
                return data + "d";
            }

            if (data is bool)
            {
                return data.ToString().ToLower();
            }

            return data.ToString();
        }
    }
}
