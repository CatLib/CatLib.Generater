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
using System.Collections.Generic;
using System.Reflection;

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 方法静态包装器
    /// </summary>
    public class MethodStaticWrapper : IStaticWrapper
    {
        /// <summary>
        /// 已经完成的包装
        /// </summary>
        private readonly List<CodeTypeMember> wraps;

        /// <summary>
        /// 构建一个函数静态包装策略
        /// </summary>
        public MethodStaticWrapper()
        {
            wraps = new List<CodeTypeMember>();
        }

        /// <summary>
        /// 对方法进行包装
        /// </summary>
        /// <param name="type">需要包装的类型</param>
        /// <returns></returns>
        public CodeTypeMember[] Wrap(Type type)
        {
            wraps.Clear();
            foreach (var method in type.GetMethods())
            {
                if (method.IsSpecialName)
                {
                    continue;
                }

                var memeber = WrapMethod(method);
                if (memeber == null)
                {
                    continue;
                }

                wraps.Add(memeber);
            }
            return wraps.ToArray();
        }

        /// <summary>
        /// 导入成员函数
        /// </summary>
        /// <param name="method">需要导入的成员函数</param>
        private CodeMemberMethod WrapMethod(MethodInfo method)
        {
            var generate = CreateMethod(method.Name, method);
            var parameters = AttachParameter(generate, method);

            if (method.ReturnType != typeof(void))
            {
                generate.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(StaticWrapUtil.GetInstance(), method.Name
                    ), parameters
                )));
            }
            else
            {
                generate.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(StaticWrapUtil.GetInstance(), method.Name
                    ), parameters
                ));
            }

            return generate;
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
        /// 为成员方法模型附上成员参数信息
        /// </summary>
        /// <param name="member">成员方法模型</param>
        /// <param name="method">需要导入到成员方法模型的成员函数</param>
        /// <returns>成员顺序表达式</returns>
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
