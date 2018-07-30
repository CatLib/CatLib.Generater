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

namespace CatLib.Generater.Editor.Policy
{
    /// <summary>
    /// 命名空间策略
    /// </summary>
    public sealed class NameSpcaePolicy : IPolicy
    {
        /// <summary>
        /// 默认的命名空间
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// 构建一个新的命名空间策略
        /// </summary>
        public NameSpcaePolicy()
        {
            Default = "CatLib.Facade";
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            var segment = context.Original.Namespace.Split('.');
            if (segment.Length >= 1)
            {
                context.Namespace.Name = segment[0] + ".Facade";
            }
            else
            {
                context.Namespace.Name = Default;
            }
        }
    }
}
