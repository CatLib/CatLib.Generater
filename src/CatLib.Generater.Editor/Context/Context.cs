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
        /// 环境
        /// </summary>
        public IEnvironment Environment { get; private set; }

        /// <summary>
        /// 代码编译单元
        /// </summary>
        public CodeCompileUnit CompileUnit { get; private set; }

        /// <summary>
        /// 代码命名空间
        /// </summary>
        public CodeNamespace Namespace { get; private set; }

        /// <summary>
        /// 类信息
        /// </summary>
        public CodeTypeDeclaration Class { get; private set; }

        /// <summary>
        /// 创建一个新的构建上下文
        /// </summary>
        /// <param name="enviroment">运行环境</param>
        /// <param name="original">原始类型</param>
        protected Context(IEnvironment enviroment, Type original)
        {
            Environment = enviroment;
            Original = original;

            CompileUnit = new CodeCompileUnit();
            Namespace = new CodeNamespace();
            Class = new CodeTypeDeclaration();
            CompileUnit.Namespaces.Add(Namespace);
            Namespace.Types.Add(Class);
        }
    }
}
