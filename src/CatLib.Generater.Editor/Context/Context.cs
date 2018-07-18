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
using System.Collections.Generic;
using System.Reflection;

namespace CatLib.Generater.Editor.Context
{
    /// <summary>
    /// 构建上下文
    /// </summary>
    public abstract class Context
    {
        /// <summary>
        /// 原始类型
        /// </summary>
        public Type Original { get; private set; }

        /// <summary>
        /// 引用
        /// </summary>
        public virtual List<string> Using { get; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public virtual string Namespace { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public virtual string ClassName { get; set; }

        /// <summary>
        /// 继承的类
        /// </summary>
        public virtual string InheritClass { get; set; }

        /// <summary>
        /// 继承的接口
        /// </summary>
        public virtual List<string> InheritInterfaces { get; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public virtual List<PropertyInfo> Properties { get; }

        /// <summary>
        /// 函数信息
        /// </summary>
        public virtual List<MethodInfo> Methods { get; }

        /// <summary>
        /// 创建一个新的构建上下文
        /// </summary>
        /// <param name="original">原始类型</param>
        protected Context(Type original)
        {
            Original = original;
            Using = new List<string>();
            InheritInterfaces = new List<string>();
            Properties = new List<PropertyInfo>();
            Methods = new List<MethodInfo>();
        }
    }
}
