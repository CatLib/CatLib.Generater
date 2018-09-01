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
    /// 事件静态包装器
    /// </summary>
    public class EventStaticWrapper : IStaticWrapper
    {
        /// <summary>
        /// 事件代码模版
        /// </summary>
        private readonly string template =
@"        public static event {type} {event} {
            add {  
                {instance}.{event} += value;
            }
            remove {
                {instance}.{event} -= value;
            }
        }";

        /// <summary>
        /// 已经完成的包装
        /// </summary>
        private readonly List<CodeTypeMember> wraps;

        /// <summary>
        /// 构建一个事件静态包装器实例
        /// </summary>
        public EventStaticWrapper()
        {
            wraps = new List<CodeTypeMember>();
        }

        /// <summary>
        /// 包装指定对象
        /// </summary>
        /// <param name="type">需要包装的类型</param>
        /// <returns>包装的成员</returns>
        public CodeTypeMember[] Wrap(Type type)
        {
            wraps.Clear();

            foreach (var eventInfo in type.GetEvents())
            {
                var memeber = WrapEvent(eventInfo);
                if (memeber == null)
                {
                    continue;
                }

                wraps.Add(memeber);
            }

            return wraps.ToArray();
        }

        /// <summary>
        /// 包装指定的事件
        /// </summary>
        /// <param name="eventInfo">指定的事件</param>
        private CodeSnippetTypeMember WrapEvent(EventInfo eventInfo)
        {
            var generate = CreateEventMember(eventInfo.Name);

            var code = template.Replace("{type}", StaticWrapUtil.GenerateExpression(new CodeTypeReferenceExpression(
                new CodeTypeReference(eventInfo.EventHandlerType.ToString()))));
            code = code.Replace("{instance}", StaticWrapUtil.StaticInstance);
            code = code.Replace("{event}", eventInfo.Name);
            generate.Text = code;

            return generate;
        }

        /// <summary>
        /// 创建一个事件成员模型
        /// </summary>
        /// <param name="name">事件的名字</param>
        private CodeSnippetTypeMember CreateEventMember(string name)
        {
            var member = new CodeSnippetTypeMember
            {
                Name = name,
                Attributes = MemberAttributes.Static | MemberAttributes.Public,
            };
            return member;
        }
    }
}
