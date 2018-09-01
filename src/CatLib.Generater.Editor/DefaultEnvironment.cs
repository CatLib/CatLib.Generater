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
using System.IO;

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// 默认的环境构建器
    /// </summary>
    public sealed class DefaultEnvironment : IEnvironment
    {
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileSuffix { get; set; }

        /// <summary>
        /// 生成目录
        /// </summary>
        private readonly string generateDirectory;

        /// <summary>
        /// 备份目录
        /// </summary>
        private readonly string backupDirectory;

        /// <summary>
        /// 是否已经完成初始化
        /// </summary>
        private bool inited;

        /// <summary>
        /// 构建一个默认的环境构建器
        /// </summary>
        /// <param name="rootDirectory">文件根目录</param>
        public DefaultEnvironment(string rootDirectory)
        {
            generateDirectory = rootDirectory;
            backupDirectory =Path.Combine(Path.GetDirectoryName(rootDirectory), ".bak."
                              + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000 +
                              "." + Path.GetFileName(rootDirectory));
            FileSuffix = ".cs";
        }

        /// <summary>
        /// 开始预备环境
        /// </summary>
        public void Begin()
        {
            if (inited)
            {
                throw new Exception("Environment Unstable state");
            }

            if (File.Exists(generateDirectory))
            {
                throw new IOException("The directory is already occupied");
            }

            if (Directory.Exists(generateDirectory))
            {
                if (Directory.Exists(backupDirectory))
                {
                    Directory.Delete(backupDirectory, true);
                }
                Directory.Move(generateDirectory, backupDirectory);
            }

            Directory.CreateDirectory(generateDirectory);
            inited = true;
        }

        /// <summary>
        /// 提交环境变更
        /// </summary>
        public void Commit()
        {
            if (Directory.Exists(backupDirectory))
            {
                Directory.Delete(backupDirectory, true);
            }
        }

        /// <summary>
        /// 回滚环境
        /// </summary>
        public void Rollback()
        {
            if (!inited)
            {
                return;
            }

            try
            {
                if (Directory.Exists(generateDirectory))
                {
                    Directory.Delete(generateDirectory, true);
                }

                try
                {
                    if (Directory.Exists(backupDirectory))
                    {
                        Directory.Move(backupDirectory, generateDirectory);
                    }
                }
                catch
                {
                    if (Directory.Exists(backupDirectory))
                    {
                        Directory.Delete(backupDirectory, true);
                    }
                }
            }
            finally
            {
                inited = false;
            }
        }

        /// <summary>
        /// 创建一个文件写入流
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>可以被写入的流</returns>
        public Stream CreateStream(string filename)
        {
            if (File.Exists(Path.Combine(generateDirectory, filename + FileSuffix)))
            {
                throw new IOException("File [" + filename + "] is already exists.");
            }

            return new FileStream(Path.Combine(generateDirectory, filename + FileSuffix), FileMode.Create);
        }
    }
}
