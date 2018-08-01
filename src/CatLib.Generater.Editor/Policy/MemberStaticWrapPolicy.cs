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
        /// 构建一个成员静态包装策略
        /// </summary>
        public MemberStaticWrapPolicy()
        {
            methods = new Dictionary<string, CodeTypeMember>();
            StaticInstance = "Instance";
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            this.context = context;
            methods.Clear();
            ScanningType(context, context.Original);
            ScanningInterface(context, context.Original);

            foreach (var method in methods)
            {
                context.Class.Members.Add(method.Value);
            }
        }

        /// <summary>
        /// 扫描指定类型中的函数
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="type">指定类型</param>
        private void ScanningType(Context.Context context, Type type)
        {
            while (type != null)
            {
                Imports(context, type);
                type = type.BaseType;
            }
        }

        /// <summary>
        /// 对接口进行扫描
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="type">提取当前类型的实现接口类型中的函数</param>
        private void ScanningInterface(Context.Context context, Type type)
        {
            foreach (var baseInterface in type.GetInterfaces())
            {
                Imports(context, baseInterface);
                ScanningInterface(context, baseInterface);
            }
;       }

        /// <summary>
        /// 为指定类型提取方法
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="type">需要提取函数的类型</param>
        private void Imports(Context.Context context, Type type)
        {
            foreach (var method in type.GetMethods())
            {
                ImportMethod(method);
            }
        }

        /// <summary>
        /// 导入方法
        /// </summary>
        /// <param name="method">需要都的函数</param>
        private void ImportMethod(MethodInfo method)
        {
            if (method.IsSpecialName)
            {
                // 导入一些特殊的成员，如：属性，事件
                ImportSpecialMethod(method);
            }

            Console.WriteLine(method.Name);
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

                    break;
                case "remove":

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
                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(context.Class.Name), StaticInstance),
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
                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(context.Class.Name), StaticInstance),
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
            var member = new CodeMemberProperty { Name = name };
            member.Attributes |= MemberAttributes.Static | MemberAttributes.Public;
            member.Type = new CodeTypeReference(method.ReturnType);
            return member;
        }
    }
}
