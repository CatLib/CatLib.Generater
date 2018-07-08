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
using System.Collections.Generic;
using System.Reflection;

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// CatLib 代码生成器
    /// </summary>
    public abstract class CodeGenerater
    {
        /// <summary>
        /// 程序集扫描器
        /// </summary>
        private Func<Assembly[]> assembliesFinder;

        /// <summary>
        /// 需要生成的类型列表，在这个列表中的类型一定会被生成
        /// </summary>
        private IList<Type> generateTypes;

        /// <summary>
        /// 设定一个程序集扫描器
        /// </summary>
        /// <param name="finder"></param>
        public void SetAssembliesFinder(Func<Assembly[]> finder)
        {
            assembliesFinder = finder;
        }

        /// <summary>
        /// 设定需要生成的类型列表，在这个列表中的类型一定会被生成
        /// </summary>
        /// <param name="generateList">需要生成的类型列表</param>
        public void SetGenerateTypes(IList<Type> generateList)
        {
            generateTypes = generateList;
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="outputPath">文件输出路径</param>
        public abstract IGenerateAsyncResult Generate(string outputPath);

        /// <summary>
        /// 获取需要进行扫描的程序集
        /// </summary>
        /// <returns>程序集</returns>
        protected Assembly[] GetAssemblies()
        {
            return assembliesFinder != null ? assembliesFinder.Invoke() : AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
