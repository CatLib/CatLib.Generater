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
    /// 类型成员信息
    /// </summary>
    public struct TypeMembers
    {
        /// <summary>
        /// 当前类型
        /// </summary>
        public Type BaseType;

        /// <summary>
        /// 生成的成员模型
        /// </summary>
        public CodeTypeMember[][] Members;
    }
}
