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

using System.CodeDom;

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 类型成员聚合器
    /// </summary>
    internal interface IMemberPolymerization
    {
        /// <summary>
        /// 聚合类型成员转为有效的成员模型列表
        /// </summary>
        /// <param name="typeMembers">需要筛选的类型成员列表</param>
        /// <returns>已经完成筛选的成员模型</returns>
        CodeTypeMember[] Polymerization(TypeMembers[] typeMembers);
    }
}
