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

using CatLib.Generater.Editor.Policy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// CatLib 代码生成器
    /// </summary>
    public abstract class CodeGenerater : ICodeGenerater
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
        /// 当前正在处理的进度
        /// </summary>
        private GenerateAsyncResult generating;

        /// <summary>
        /// 文件处理器
        /// </summary>
        protected IEnvironment Environment { get; private set; }

        /// <summary>
        /// 当生成完成时触发
        /// </summary>
        public event Action OnCompleted;

        /// <summary>
        /// 当出现异常时触发
        /// </summary>
        public event Action<Exception> OnException;

        /// <summary>
        /// 编译策略
        /// </summary>
        private readonly LinkedList<IPolicy>[] stages;

        /// <summary>
        /// 编译进度（0-1表示）
        /// </summary>
        public float Process
        {
            get
            {
                return IsGenerating ? generating.Process : 0;
            }
        }

        /// <summary>
        /// 是否在编译中
        /// </summary>
        public bool IsGenerating
        {
            get
            {
                return generating != null;
            } 
        }

        /// <summary>
        /// 构建一个代码生成器
        /// </summary>
        protected CodeGenerater()
        {
            stages = new LinkedList<IPolicy>[Enum.GetNames(typeof(BuildStages)).Length];
            for (var i = 0; i < stages.Length; i++)
            {
                stages[i] = new LinkedList<IPolicy>();
            }
        }

        /// <summary>
        /// 设定一个程序集扫描器
        /// </summary>
        /// <param name="finder"></param>
        public void SetAssembliesFinder(Func<Assembly[]> finder)
        {
            assembliesFinder = finder;
        }

        /// <summary>
        /// 默认的生成标记
        /// </summary>
        protected abstract Type GenerateAttribute { get; } 

        /// <summary>
        /// 设定需要生成的类型列表，在这个列表中的类型一定会被生成
        /// </summary>
        /// <param name="generateList">需要生成的类型列表</param>
        public void SetGenerateTypes(Type[] generateList)
        {
            generateTypes = generateList;
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="environment">文件写入器</param>
        public IGenerateAsyncResult Generate(IEnvironment environment)
        {
            lock (generating)
            {
                if (generating != null)
                {
                    return generating;
                }
                generating = new GenerateAsyncResult();
                Environment = environment;
            }

            ThreadPool.QueueUserWorkItem(BeginGenerate);
            return generating;
        }

        /// <summary>
        /// 开始生成代码
        /// </summary>
        private void BeginGenerate(object state)
        {
            var isExceotion = false;
            try
            {
                Environment.Begin();
                GenerateTypes(GetGeneraterTypes());
            }
            catch (Exception ex)
            {
                isExceotion = true;
                Environment.Rollback();
                if (OnException != null)
                {
                    OnException.Invoke(ex);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                if (!isExceotion)
                {
                    Environment.Commit();
                    if (OnCompleted != null)
                    {
                        OnCompleted.Invoke();
                    }
                }

                if (Environment is IDisposable)
                {
                    ((IDisposable)Environment).Dispose();
                }

                Environment = null;
                generating = null;
            }
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="types">需要生成的代码类型</param>
        private void GenerateTypes(Type[] types)
        {
            for (var i = 0; i < types.Length; i++)
            {
                var context = CreateContext(types[i]);
                foreach (var stage in stages)
                {
                    foreach (var policy in stage)
                    {
                        policy.Factory(context);
                    }
                }
                generating.Process = (float) i / types.Length;
            }
        }

        /// <summary>
        /// 创建一个上下文
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected abstract Context.Context CreateContext(Type type); 

        /// <summary>
        /// 添加一个编译策略
        /// </summary>
        /// <param name="stage">编译阶段</param>
        /// <param name="policy">编译策略</param>
        public void AddPolicy(BuildStages stage, IPolicy policy)
        {
            stages[(int) stage].AddLast(policy);
        }

        /// <summary>
        /// 获取需要生成的类型
        /// </summary>
        /// <returns>类型列表</returns>
        private Type[] GetGeneraterTypes()
        {
            var results = new List<Type>();

            GeneraterTypesFromAssemblies(ref results);
            if (generateTypes != null)
            {
                results.AddRange(generateTypes);
            }

            return results.ToArray();
        }

        /// <summary>
        /// 从Assembly中获取需要被生成的类型
        /// </summary>
        /// <returns>类型</returns>
        private void GeneraterTypesFromAssemblies(ref List<Type> output)
        {
            if (output == null)
            {
                output = new List<Type>();
            }

            foreach (var assembly in GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsDefined(GenerateAttribute, false))
                    {
                        continue;
                    }
                    output.Add(type);
                }
            }
        }

        /// <summary>
        /// 获取需要进行扫描的程序集
        /// </summary>
        /// <returns>程序集</returns>
        private IEnumerable<Assembly> GetAssemblies()
        {
            return assembliesFinder != null ? assembliesFinder.Invoke() : AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
