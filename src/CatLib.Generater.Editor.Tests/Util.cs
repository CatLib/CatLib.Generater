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

using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace CatLib.Generater.Editor.Tests
{
    /// <summary>
    /// 通用测试代码
    /// </summary>
    public class Util
    {
        /// <summary>
        /// 生成对应的代码
        /// </summary>
        /// <param name="codeCompile">代码模型</param>
        /// <returns>生成的代码</returns>

        public static string Generate(CodeCompileUnit codeCompile)
        {
            using (var sw = new StringWriter(new StringBuilder()))
            {
                CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromCompileUnit(
                    codeCompile, sw, new CodeGeneratorOptions
                    {
                        ElseOnClosing = true,
                        IndentString = "    "
                    });
                return sw.ToString().Trim();
            }
        }

        /// <summary>
        /// 生成对应的代码
        /// </summary>
        /// <param name="member">成员模型</param>
        /// <returns>生成的代码</returns>
        public static string GenerateFromMember(CodeTypeMember member)
        {
            using (var sw = new StringWriter(new StringBuilder()))
            {
                CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromMember(
                    member, sw, new CodeGeneratorOptions
                    {
                        ElseOnClosing = true,
                        IndentString = "    "
                    });
                return sw.ToString().Trim();
            }
        }
    }
}
