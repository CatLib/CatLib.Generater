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

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// 编译阶段
    /// </summary>
    public enum BuildStages
    {
        /// <summary>
        /// 代码编译之前
        /// </summary>
        BeforeGenerating,

        /// <summary>
        /// 代码编译中
        /// </summary>
        Generating,

        /// <summary>
        /// 代码编译之后
        /// </summary>
        AfterGenerating,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed,
    }
}
