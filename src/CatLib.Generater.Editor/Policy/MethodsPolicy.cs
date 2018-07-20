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
            ScanningType(context.Methods, context.Original);
            ScanningInterface(context.Methods, context.Original);
        }

        /// <summary>
        /// 扫描指定类型中的函数
        /// </summary>
        /// <param name="methods">将提取的结果保存到当前参数</param>
        /// <param name="type">指定类型</param>
        private void ScanningType(IList<MethodInfo> methods, Type type)
        {
            while (type != null)
            {
                Extract(methods, type);
                type = type.BaseType;
            }
        }

        /// <summary>
        /// 对接口进行扫描
        /// </summary>
        /// <param name="methods">将提取的结果保存到当前参数</param>
        /// <param name="type">提取当前类型的实现接口类型中的函数</param>
        private void ScanningInterface(IList<MethodInfo> methods, Type type)
        {
            foreach (var baseInterface in type.GetInterfaces())
            {
                Extract(methods, baseInterface);
                ScanningInterface(methods, baseInterface);
            }
;       }

        /// <summary>
        /// 为指定类型提取方法
        /// </summary>
        /// <param name="methods">将提取的结果保存到当前参数</param>
        /// <param name="type">需要提取函数的类型</param>
        private void Extract(IList<MethodInfo> methods, Type type)
        {
            foreach (var method in type.GetMethods())
            {
                methods.Add(method);
                // todo:
            }
        }
    }
}
