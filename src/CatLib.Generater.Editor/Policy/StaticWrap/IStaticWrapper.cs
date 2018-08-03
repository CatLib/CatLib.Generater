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

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 静态包装构建器
    /// </summary>
    internal interface IStaticWrapper
    {
        /// <summary>
        /// 包装指定对象
        /// </summary>
        /// <param name="type">需要包装的类型</param>
        /// <returns>包装的成员</returns>
        CodeTypeMember[] Wrap(Type type);
    }
}
