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
        /// 预编译
        /// </summary>
        Precompiled,

        /// <summary>
        /// 代码构建
        /// </summary>
        GenCode,

        /// <summary>
        /// 编译完成
        /// </summary>
        Compiled,
    }
}
