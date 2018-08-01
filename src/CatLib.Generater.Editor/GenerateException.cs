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
    /// 生成异常
    /// </summary>
    public class GenerateException : Exception
    {
        /// <summary>
        /// 构建一个生成异常
        /// </summary>
        /// <param name="message"></param>
        public GenerateException(string message)
            : base(message)
        {
            
        }
    }
}
