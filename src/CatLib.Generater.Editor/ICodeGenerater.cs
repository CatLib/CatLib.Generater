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
using System.Reflection;
using CatLib.Generater.Editor.Policy;

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// CatLib代码生成器
    /// </summary>
    public interface ICodeGenerater
    {
        /// <summary>
        /// 当完成时
        /// </summary>
        event Action OnCompleted;

        /// <summary>
        /// 当出现异常时触发
        /// </summary>
        event Action<Exception> OnException;

        /// <summary>
        /// 编译进度（0-1表示）
        /// </summary>
        float Process { get; }

        /// <summary>
        /// 是否在编译中
        /// </summary>
        bool IsGenerating { get; }

        /// <summary>
        /// 设定一个程序集扫描器
        /// </summary>
        /// <param name="finder">程序集扫描器</param>
        void SetAssembliesFinder(Func<Assembly[]> finder);

        /// <summary>
        /// 设定需要生成的类型列表，在这个列表中的类型一定会被生成
        /// </summary>
        /// <param name="generateList">需要生成的类型列表</param>
        void SetGenerateTypes(Type[] generateList);

        /// <summary>
        /// 添加一个编译策略
        /// </summary>
        /// <param name="stage">编译阶段</param>
        /// <param name="policy">编译策略</param>
        void AddPolicy(BuildStages stage, IPolicy policy);

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="environment">构建环境</param>
        IGenerateAsyncResult Generate(IEnvironment environment);
    }
}
