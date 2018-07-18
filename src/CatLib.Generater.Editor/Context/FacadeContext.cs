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
    public class FacadeContext : Context
    {
        /// <summary>
        /// 门面上下文构建
        /// </summary>
        /// <param name="original"></param>
        public FacadeContext(Type original) 
            : base(original)
        {
            Using.Add("CatLib");
        }
    }
}
