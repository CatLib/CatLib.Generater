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
    /// 编译策略
    /// </summary>
    public interface IPolicy
    {
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        void Factory(Context.Context context);
    }
}
