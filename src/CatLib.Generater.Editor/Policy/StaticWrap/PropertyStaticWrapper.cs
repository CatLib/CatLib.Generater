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
    /// 属性包装器
    /// </summary>
    public class PropertyStaticWrapper : IStaticWrapper
    {
        /// <summary>
        /// 已经完成的包装
        /// </summary>
        private readonly List<CodeTypeMember> wraps;

        /// <summary>
        /// 构建一个事件静态包装器实例
        /// </summary>
        public PropertyStaticWrapper()
        {
            wraps = new List<CodeTypeMember>();
        }

        /// <summary>
        /// 包装指定类型中的所有属性
        /// </summary>
        /// <param name="type">需要包装的类型</param>
        /// <returns>包装的成员</returns>
        public CodeTypeMember[] Wrap(Type type)
        {
            wraps.Clear();

            foreach (var propertyInfo in type.GetProperties())
            {
                var memeber = WrapProperty(propertyInfo);
                if (memeber == null)
                {
                    continue;
                }

                wraps.Add(memeber);
            }

            return wraps.ToArray();
        }

        /// <summary>
        /// 包装指定的属性
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <returns>成员属性模型</returns>
        private CodeMemberProperty WrapProperty(PropertyInfo propertyInfo)
        {
            var generate = CreatePropertyMember(propertyInfo.Name, propertyInfo.PropertyType);

            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                return null;
            }

            if (propertyInfo.CanRead)
            {
                generate.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                        StaticWrapUtil.GetInstance(),
                        propertyInfo.Name)));
            }

            if (propertyInfo.CanWrite)
            {
                generate.SetStatements.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                        StaticWrapUtil.GetInstance(),
                        propertyInfo.Name
                    ), new CodePropertySetValueReferenceExpression()));
            }

            return generate;
        }

        /// <summary>
        /// 创建属性成员
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <param name="propertyType">属性的类型</param>
        private CodeMemberProperty CreatePropertyMember(string name, Type propertyType)
        {
            var member = new CodeMemberProperty
            {
                Name = name,
                Attributes = MemberAttributes.Static | MemberAttributes.Public,
                Type = new CodeTypeReference(propertyType.ToString())
            };
            return member;
        }
    }
}
