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
    /// 门面代码生成器
    /// </summary>
    internal sealed class FacadeCodeGenerater : CodeGenerater
    {
        /// <summary>
        /// 默认的门面生成标记
        /// </summary>
        protected override Type GenerateAttribute
        {
            get { return typeof(FacadeGenerateAttribute); }
        }

        /// <summary>
        /// 开始生成代码
        /// </summary>
        protected override void BeginGenerate(Type[] generaterTypes)
        {
            
        }
    }
}
