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

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 成员对象（事件，属性，函数）静态包装策略
    /// </summary>
    public class MemberStaticWrapPolicy : IPolicy
    {
        /// <summary>
        /// 已经生成的对象
        /// </summary>
        private readonly List<TypeMembers> wraps;

        /// <summary>
        /// 静态包装器
        /// </summary>
        private readonly IStaticWrapper[] wrappers;

        /// <summary>
        /// 聚合器
        /// </summary>
        private readonly IMemberPolymerization polymerization;

        /// <summary>
        /// 构建一个成员静态包装策略
        /// </summary>
        public MemberStaticWrapPolicy()
        {
            wraps = new List<TypeMembers>();
            polymerization = new MemberPolymerization();

            wrappers = new IStaticWrapper[Enum.GetValues(typeof(WrapperTypes)).Length];
            wrappers[(int)WrapperTypes.Method] = new MethodStaticWrapper();
            wrappers[(int)WrapperTypes.Event] = new EventStaticWrapper();
            wrappers[(int)WrapperTypes.Property] = new PropertyStaticWrapper();
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            wraps.Clear();
            ScanningType(context.Original);
            context.Class.Members.AddRange(polymerization.Polymerization(wraps.ToArray()));
        }

        /// <summary>
        /// 扫描类型
        /// </summary>
        /// <param name="type">扫描的类型</param>
        private void ScanningType(Type type)
        {
            var original = type;

            while (type != null)
            {
                wraps.Add(Wrap(type));
                type = type.BaseType;
            }

            foreach (var baseInterface in original.GetInterfaces())
            {
                ScanningType(baseInterface);
            }
        }

        /// <summary>
        /// 对指定类型进行包装
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns>成员模型</returns>
        private TypeMembers Wrap(Type type)
        {
            var typeMembers = new TypeMembers
            {
                BaseType = type,
                Members = new CodeTypeMember[wrappers.Length][]
            };

            for (var index = 0; index < wrappers.Length; index++)
            {
                if (wrappers[index] != null)
                {
                    typeMembers.Members[index] = wrappers[index].Wrap(type);
                }
            }

            return typeMembers;
        }
    }
}
