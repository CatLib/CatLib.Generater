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

namespace CatLib.Generater.Editor.Policy.StaticWrap
{
    /// <summary>
    /// 静态包装通用信息
    /// </summary>
    public static class StaticWrapUtil
    {
        /// <summary>
        /// 静态实例的名字
        /// </summary>
        public static string StaticInstance { get; set; }

        /// <summary>
        /// 代码服务
        /// </summary>
        private static readonly CodeDomProvider codeDomProvider;

        /// <summary>
        /// 构建一个静态包装的通用信息
        /// </summary>
        static StaticWrapUtil()
        {
            codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
            StaticInstance = "Instance";
        }

        /// <summary>
        /// 获取实例表达式
        /// </summary>
        /// <returns></returns>
        public static CodeExpression GetInstance()
        {
            return new CodeVariableReferenceExpression(StaticInstance ?? "Instance");
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>代码结构</returns>
        public static string GenerateExpression(CodeExpression expression)
        {
            using (var sw = new StringWriter(new StringBuilder()))
            {
                codeDomProvider.GenerateCodeFromExpression(expression, sw, new CodeGeneratorOptions
                {
                    ElseOnClosing = true,
                });
                return sw.ToString();
            }
        }

        /// <summary>
        /// 转为默认值所使用的字符串
        /// </summary>
        /// <param name="data">基础数据</param>
        /// <returns>被转换的字符串</returns>
        public static string ToDefaultValueString(object data)
        {
            if (data == null)
            {
                return "null";
            }

            if (data is string)
            {
                return "\"" + data + "\"";
            }

            if (data is float)
            {
                return data + "f";
            }

            if (data is double)
            {
                return data + "d";
            }

            if (data is bool)
            {
                return data.ToString().ToLower();
            }

            return data.ToString();
        }
    }
}
