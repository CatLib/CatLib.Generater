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
using System.CodeDom;
using CatLib.Generater.Editor.Context;
using CatLib.Generater.Editor.Policy;
using CatLib.Generater.Editor.Policy.StaticWrap;

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// 门面代码生成器
    /// </summary>
    public sealed class FacadeCodeGenerater : CodeGenerater
    {
        /// <summary>
        /// 默认的门面生成标记
        /// </summary>
        protected override Type GenerateAttribute
        {
            get { return typeof(FacadeGenerateAttribute); }
        }

        /// <summary>
        /// 构建一个门面代码生成器
        /// </summary>
        public FacadeCodeGenerater()
            : base()
        {
            AddPolicy(BuildStages.Precompiled, new NameSpcaePolicy());
            AddPolicy(BuildStages.Precompiled, new ClassCreatePolicy());
            AddPolicy(BuildStages.Precompiled, new MemberStaticWrapPolicy());
        }

        /// <summary>
        /// 创建一个上下文
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override Context.Context CreateContext(Type type)
        {
            return new FacadeContext(Environment, type);
        }
    }
}
