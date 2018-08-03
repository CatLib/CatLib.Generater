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

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 包装器类型
    /// </summary>
    public enum WrapperTypes
    {
        /// <summary>
        /// 函数
        /// </summary>
        Method = 0,

        /// <summary>
        /// 事件
        /// </summary>
        Event = 1,

        /// <summary>
        /// 属性
        /// </summary>
        Property = 2,
    }
}
