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

namespace CatLib.Generater.Editor.Context
{
    /// <summary>
    /// 门面构建上下文
    /// </summary>
    public sealed class FacadeContext : Context
    {
        /// <summary>
        /// 门面上下文构建
        /// </summary>
        /// <param name="environment">运行环境</param>
        /// <param name="original">原始类型</param>
        public FacadeContext(IEnvironment environment, Type original) 
            : base(environment, original)
        {
            Using.Add("CatLib");
        }
    }
}
