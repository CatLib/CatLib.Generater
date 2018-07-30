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
    /// 类名构建策略
    /// </summary>
    public sealed class ClassNamePolicy : IPolicy
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            var facadeName = context.Original.Name;
            facadeName = facadeName.StartsWith("I", true, null) ? facadeName.Substring(1, facadeName.Length - 1) : facadeName;
            context.Class.Name = Prefix + facadeName + Suffix;
        }
    }
}
