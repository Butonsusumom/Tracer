using System.Collections.Generic;
using System.Threading;

namespace TracerLib
{
    public class Tracer : ITracer
    {
        public Tracer()
        {
            ThreadsInfo = new List<ThreadInfo>();
            ThreadTracers = new List<ThreadTracer>();
        }

        private readonly List<ThreadTracer> ThreadTracers;

        private readonly List<ThreadInfo> ThreadsInfo;

        static object Locker = new object();

        public TraceResult GetTraceResult()
        {
            return new TraceResult(ThreadsInfo);
        }

        private ThreadInfo GetThreadInfoById(int threadId)
        {
            lock (Locker)
            {
                foreach (ThreadInfo threadInfo in ThreadsInfo)
                {
                    if (threadId == threadInfo.Id)
                    {
                        return threadInfo;
                    }
                }
            }
            return null;
        }

        private ThreadTracer GetCurrentThreadTracer()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (Locker)
            {
                foreach (ThreadTracer threadTracer in ThreadTracers)
                {
                    if (threadId == threadTracer.TracedThreadId)
                    {
                        return threadTracer;
                    }
                }
            }            
            return null;
        }

        public void StartTrace()
        {
            ThreadTracer threadTracer = GetCurrentThreadTracer();
            if (threadTracer == null)
            {
                int currentThreadId = Thread.CurrentThread.ManagedThreadId;
                threadTracer = new ThreadTracer(currentThreadId);
                lock (Locker)
                {
                    ThreadTracers.Add(threadTracer);
                }
                
            }
            threadTracer.StartTrace();
        }

        public void StopTrace()
        {
            ThreadTracer threadTracer = GetCurrentThreadTracer();
            threadTracer.StopTrace();
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            ThreadInfo threadInfo = GetThreadInfoById(currentThreadId);
            if (threadInfo == null)
            {
                List<MethodInfo> threadMethodInfos = threadTracer.GetThreadMethodList();
                threadInfo = new ThreadInfo(currentThreadId, threadMethodInfos);
                lock (Locker)
                {
                    ThreadsInfo.Add(threadInfo);
                }
            }
        }
    }
}
