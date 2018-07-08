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

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// 代码生成异步结果
    /// </summary>
    public interface IGenerateAsyncResult : IAsyncResult
    {
        /// <summary>
        /// 执行进度
        /// </summary>
        float Process { get; }
    }
}
