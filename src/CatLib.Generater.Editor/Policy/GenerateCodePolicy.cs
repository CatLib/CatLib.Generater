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

using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace CatLib.Generater.Editor.Policy
{
    /// <summary>
    /// 代码生成策略
    /// </summary>
    public sealed class GenerateCodePolicy : IPolicy
    {
        /// <summary>
        /// 输出语言
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 代码生成配置
        /// </summary>
        public CodeGeneratorOptions Options { get; set; }

        /// <summary>
        /// 代码生成服务
        /// </summary>
        private CodeDomProvider provider;

        /// <summary>
        /// 构建一个新的代码生成策略
        /// </summary>
        public GenerateCodePolicy()
        {
            Language = "CSharp";
            Options = new CodeGeneratorOptions
            {
                ElseOnClosing = true,
                IndentString = "    "
            };
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            if (provider == null)
            {
                provider = CodeDomProvider.CreateProvider(Language);
            }

            using (var sw = new StringWriter(new StringBuilder()))
            {
                provider.GenerateCodeFromCompileUnit(context.CompileUnit, sw, Options);
                context.GenerateCode = sw.ToString().Trim();
            }
        }
    }
}
