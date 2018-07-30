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
using System.Reflection;

namespace CatLib.Generater.Editor.Policy
{
    /// <summary>
    /// 方法提取策略
    /// </summary>
    public class MethodsPolicy : IPolicy
    {
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            ScanningType(context, context.Original);
            ScanningInterface(context, context.Original);
        }

        /// <summary>
        /// 扫描指定类型中的函数
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="type">指定类型</param>
        private void ScanningType(Context.Context context, Type type)
        {
            while (type != null)
            {
                Imports(context, type);
                type = type.BaseType;
            }
        }

        /// <summary>
        /// 对接口进行扫描
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="type">提取当前类型的实现接口类型中的函数</param>
        private void ScanningInterface(Context.Context context, Type type)
        {
            foreach (var baseInterface in type.GetInterfaces())
            {
                Imports(context, baseInterface);
                ScanningInterface(context, baseInterface);
            }
;       }

        /// <summary>
        /// 为指定类型提取方法
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="type">需要提取函数的类型</param>
        private void Imports(Context.Context context, Type type)
        {
            foreach (var method in type.GetMethods())
            {
                ImportMethod(context.Class.Members.Add, method);
            }
        }

        /// <summary>
        /// 导入方法
        /// </summary>
        /// <param name="import">进行导入的方法</param>
        /// <param name="method">需要都的函数</param>
        private void ImportMethod(Func<CodeTypeMember, int> import, MethodInfo method)
        {
            Console.WriteLine(method.Name);
        }
    }
}
