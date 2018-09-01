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

using System.Text;

namespace CatLib.Generater.Editor.Policy
{
    /// <summary>
    /// 文件写入策略
    /// </summary>
    public sealed class FileWriterPolicy : IPolicy
    {
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 构建一个新的文件写入策略
        /// </summary>
        public FileWriterPolicy()
        {
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            if (string.IsNullOrEmpty(context.GenerateCode))
            {
                return;
            }

            using (var stream = context.Environment.CreateStream(context.Class.Name))
            {
                var data = Encoding.GetBytes(context.GenerateCode);
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
