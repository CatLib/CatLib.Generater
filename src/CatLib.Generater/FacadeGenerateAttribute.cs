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

namespace CatLib.Generater
{
    /// <summary>
    /// 门面生成标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class FacadeGenerateAttribute : Attribute
    {
    }
}
