using System.Reflection;
using System.Collections.Concurrent;

namespace TracerLib
{
    public class TraceResult
    {
        private ConcurrentDictionary<int, ThreadInfo> threads;

        public TraceResult()
        {
            threads = new ConcurrentDictionary<int, ThreadInfo>();
        }

        public void StartTracing(int threadId,MethodBase method)
        {
            ThreadInfo currentThread = threads.GetOrAdd(threadId, new ThreadInfo(threadId));
            currentThread.StartTracingMethod(new MethodInfo(method));
        }

        public void StopTracing(int threadId)
        {
            threads.TryGetValue(threadId, out ThreadInfo currentThread);
            currentThread.StopTracingMethod();
        }

        public ConcurrentDictionary<int,ThreadInfo> GetResults()
        {
            return threads;
        }

    }
}
