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
    /// 属性提取策略
    /// </summary>
    public class PropertiesPolicy : IPolicy
    {
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="context">构建上下文</param>
        public void Factory(Context.Context context)
        {
            //ScanningType(context.Properties, context.Original);
            //ScanningInterface(context.Properties, context.Original);
        }

        /// <summary>
        /// 扫描指定类型中的属性
        /// </summary>
        /// <param name="properties">将提取的结果保存到当前参数</param>
        /// <param name="type">指定类型</param>
        private void ScanningType(IList<PropertyInfo> properties, Type type)
        {
            while (type != null)
            {
                Extract(properties, type);
                type = type.BaseType;
            }
        }

        /// <summary>
        /// 对接口进行扫描
        /// </summary>
        /// <param name="methods">将提取的结果保存到当前参数</param>
        /// <param name="type">提取当前类型的实现接口类型中的函数</param>
        private void ScanningInterface(IList<PropertyInfo> properties, Type type)
        {
            foreach (var baseInterface in type.GetInterfaces())
            {
                Extract(properties, baseInterface);
                ScanningInterface(properties, baseInterface);
            }
        }

        /// <summary>
        /// 为指定类型提取属性
        /// </summary>
        /// <param name="properties">将提取的结果保存到当前参数</param>
        /// <param name="type">需要提取属性的类型</param>
        private void Extract(IList<PropertyInfo> properties, Type type)
        {
            foreach (var property in type.GetProperties())
            {
                properties.Add(property);
                // todo:
            }
        }
    }
}
