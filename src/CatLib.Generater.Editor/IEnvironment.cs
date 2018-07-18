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

using System.IO;

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// 生成环境
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// 初始化环境
        /// </summary>
        void Init();

        /// <summary>
        /// 创建一个文件写入流
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>可以被写入的流</returns>
        Stream CreateStream(string filename);
    }
}
