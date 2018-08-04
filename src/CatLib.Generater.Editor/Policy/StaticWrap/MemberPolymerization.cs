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
using System.Text;

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 成员聚合器
    /// </summary>
    public class MemberPolymerization : IMemberPolymerization
    {
        // 防歧义处理方案
        // 生成应该自上而下生成
        // 首层生成的事件(属性，函数)会禁止后续的同名属性(事件，函数)的生成
        // (种类间互斥，属性，事件，函数)
        // 
        // 继承的不同接口间出现同名的事件和属性会导致二义性，所以忽略生成
        // 继承的不同接口间出现同名的事件和函数，函数优先
        // 继承的不同接口间出现同名的属性和函数，函数优先
        // 继承关系中函数重载不会导致歧义，但相同的重载只生成一次
        // 如果函数返回值不同重载的参数表相同，则引发二义性异常


        // 先按照 函数，事件，属性的顺序生成首层的成员。成员保存到临时变量中
        // 将首层的成员名字加入禁止使用的成员名字列表
        // 再生成继承的 函数，事件，属性。保存到临时变量中
        // 将函数导入到上下文，同时建立排他名字hash。后续事件和属性的导入会检查排他hash，如果存在则列外。

        /// <summary>
        /// 成员类型关系
        /// </summary>
        private readonly Dictionary<string, int> membersTypeRelation;

        /// <summary>
        /// 根成员
        /// </summary>
        private readonly HashSet<string> rootMembers;

        /// <summary>
        /// 函数重载名称表[重载名，[字段方向关系, 函数返回值类型]]
        /// </summary>
        private readonly Dictionary<string, KeyValuePair<string, string>> methodsOverrideName;

        /// <summary>
        /// 成员黑名单
        /// </summary>
        private readonly Dictionary<string, WrapperTypes> memberBlacklist;

        /// <summary>
        /// 需要被聚合(导出)的成员
        /// </summary>
        private readonly List<CodeTypeMember> exports;

        /// <summary>
        /// 构建一个成员聚合器
        /// </summary>
        public MemberPolymerization()
        {
            membersTypeRelation = new Dictionary<string, int>();
            methodsOverrideName = new Dictionary<string, KeyValuePair<string, string>>();
            memberBlacklist = new Dictionary<string, WrapperTypes>();
            rootMembers = new HashSet<string>();
            exports = new List<CodeTypeMember>();
        }

        /// <summary>
        /// 聚合类型成员转为有效的成员模型列表
        /// </summary>
        /// <param name="typeMembers">需要筛选的类型成员列表</param>
        /// <returns>已经完成筛选的成员模型</returns>
        public CodeTypeMember[] Polymerization(TypeMembers[] typeMembers)
        {
            methodsOverrideName.Clear();
            memberBlacklist.Clear();
            exports.Clear();

            BuildTypeRelation(typeMembers);

            foreach (var typeMember in typeMembers)
            {
                for (var typeIndex = 0; typeIndex < typeMember.Members.Length; typeIndex++)
                {
                    if (typeMember.Members[typeIndex] == null)
                    {
                        continue;
                    }

                    foreach (var member in typeMember.Members[typeIndex])
                    {
                        Export(typeMember.BaseType, member, (WrapperTypes) typeIndex);
                    }
                }
            }

            return exports.ToArray();
        }

        /// <summary>
        /// 编译类型关系，标记指定名字的成员的类型关系
        /// </summary>
        /// <param name="typeMembers">成员列表</param>
        private void BuildTypeRelation(IEnumerable<TypeMembers> typeMembers)
        {
            rootMembers.Clear();
            membersTypeRelation.Clear();

            var isRoot = true;
            foreach (var members in typeMembers)
            {
                for (var typeIndex = 0; typeIndex < members.Members.Length; typeIndex++)
                {
                    if (members.Members[typeIndex] == null)
                    {
                        continue;
                    }

                    foreach (var member in members.Members[typeIndex])
                    {
                        int value;
                        if (membersTypeRelation.TryGetValue(member.Name, out value))
                        {
                            membersTypeRelation[member.Name] = value | (1 << typeIndex);
                        }
                        else
                        {
                            membersTypeRelation[member.Name] = 1 << typeIndex;
                        }

                        if (isRoot && !rootMembers.Contains(member.Name))
                        {
                            rootMembers.Add(member.Name);
                        }
                    }
                }
                isRoot = false;
            }
        }

        /// <summary>
        /// 对成员导出
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="member">检测的成员</param>
        /// <param name="wrapperTypes">包装器类型</param>
        /// <returns>是否可以被导出</returns>
        private void Export(Type parent,CodeTypeMember member, WrapperTypes wrapperTypes)
        {
            switch (wrapperTypes)
            {
                case WrapperTypes.Method:
                    ExportMethod(parent, member);
                    break;
                case WrapperTypes.Event:
                    ExportEvent(member);
                    break;
                case WrapperTypes.Property:
                    ExportProperty(member);
                    break;
            }
        }

        /// <summary>
        /// 导出函数
        /// </summary>
        /// <param name="member">方法模型</param>
        private void ExportMethod(Type parent, CodeTypeMember member)
        {
            WrapperTypes wrapperTypes;
            if (memberBlacklist.TryGetValue(member.Name, out wrapperTypes)
                && wrapperTypes != WrapperTypes.Method)
            {
                return;
            }

            var overrideName = ConvertToOverrideName(member);
            var directionName = ConvertToParamsDirectionName(member);
            var method = (CodeMemberMethod) member;

            KeyValuePair<string, string> overrideMethod;
            if (methodsOverrideName.TryGetValue(overrideName, out overrideMethod))
            {
                // 方法调用二义性检查
                if (overrideMethod.Key != directionName
                    || overrideMethod.Value != method.ReturnType.BaseType)
                {
                    throw new GenerateException("The method [" + parent +  "." + member.Name + "()] call has two meanings");
                }

                return;
            }

            exports.Add(member);
            memberBlacklist[member.Name] = WrapperTypes.Method;
            methodsOverrideName.Add(overrideName,
                new KeyValuePair<string, string>(directionName, method.ReturnType.BaseType));
        }

        // 继承的不同接口间出现同名的事件和属性会导致二义性，所以忽略生成
        // 继承的不同接口间出现同名的事件和函数，函数优先
        // 继承的不同接口间出现同名的属性和函数，函数优先
        // 继承关系中函数重载不会导致歧义，但相同的重载只生成一次

        /// <summary>
        /// 导出事件
        /// </summary>
        /// <param name="member">事件模型</param>
        private void ExportEvent(CodeTypeMember member)
        {
            if (memberBlacklist.ContainsKey(member.Name))
            {
                return;
            }

            if (!rootMembers.Contains(member.Name)
                && IsEventAndProperty(membersTypeRelation[member.Name]))
            {
                return;
            }

            exports.Add(member);
            memberBlacklist.Add(member.Name, WrapperTypes.Event);
        }

        /// <summary>
        /// 导出属性
        /// </summary>
        /// <param name="member"></param>
        private void ExportProperty(CodeTypeMember member)
        {
            if (memberBlacklist.ContainsKey(member.Name))
            {
                return;
            }

            if (!rootMembers.Contains(member.Name)
                && IsEventAndProperty(membersTypeRelation[member.Name]))
            {
                return;
            }

            exports.Add(member);
            memberBlacklist.Add(member.Name, WrapperTypes.Property);
        }

        /// <summary>
        /// 是否是事件以及属性
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        private bool IsEventAndProperty(int code)
        {
            return (code & (1 << (int) WrapperTypes.Event)) > 0 && (code & (1 << (int) WrapperTypes.Property)) > 0;
        }

        /// <summary>
        /// 转换为重载名字
        /// </summary>
        /// <returns></returns>
        private string ConvertToOverrideName(CodeTypeMember member)
        {
            if (!(member is CodeMemberMethod))
            {
                return member.Name + "[]";
            }

            var method = (CodeMemberMethod) member;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(member.Name);
            stringBuilder.Append("[");

            foreach (CodeParameterDeclarationExpression parameter in method.Parameters)
            {
                stringBuilder.Append(parameter.Type.BaseType);
                stringBuilder.Append(",");
            }

            if (method.Parameters.Count > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 转换为字段方向名
        /// </summary>
        /// <returns></returns>
        private string ConvertToParamsDirectionName(CodeTypeMember member)
        {
            if (!(member is CodeMemberMethod))
            {
                return "[]";
            }

            var method = (CodeMemberMethod)member;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");

            foreach (CodeParameterDeclarationExpression parameter in method.Parameters)
            {
                if (parameter.Direction == FieldDirection.Ref)
                {
                    stringBuilder.Append("ref");
                }else if (parameter.Direction == FieldDirection.Out)
                {
                    stringBuilder.Append("out");
                }
                else
                {
                    stringBuilder.Append("null");
                }
                stringBuilder.Append(",");
            }

            if (method.Parameters.Count > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
