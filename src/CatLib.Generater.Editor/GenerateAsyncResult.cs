using System.Threading;

namespace CatLib.Generater.Editor
{
    /// <summary>
    /// 生成代码时的异步状态
    /// </summary>
    internal class GenerateAsyncResult : EventWaitHandle, IGenerateAsyncResult
    {
        public GenerateAsyncResult(bool initialState = true, EventResetMode mode = EventResetMode.AutoReset)
            : base(initialState, mode)
        {
        
        }

        /// <summary>
        /// 异步Handel
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get { return this; }
        }

        /// <summary>
        /// 执行完成时所在的线程是否就是调用线程
        /// </summary>
        public bool CompletedSynchronously
        {
            get { return false; }
        }

        /// <summary>
        /// 由开发者传递的异步状态
        /// </summary>
        public object AsyncState { get; set; }

        /// <summary>
        /// 是否已经完成了
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 加载进度
        /// </summary>
        public float Process { get; set; }
    }
}
