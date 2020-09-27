using System.Collections.Generic;
using System.Diagnostics;

namespace TracerLib
{
    public class ThreadTracer
    {
        public ThreadTracer(int threadId)
        {
            TracedThreadId = threadId;
            MethodTracers = new Stack<MethodTracer>();
            MethodInfoList = new List<MethodInfo>();
        }

        public int TracedThreadId { get; private set; }

        private Stack<MethodTracer> MethodTracers;

        private MethodTracer CurrentMethodTracer;

        private List<MethodInfo> MethodInfoList;

        public List<MethodInfo> GetThreadMethodList()
        {
            return MethodInfoList;
        }

        public void StartTrace()
        {
            if (CurrentMethodTracer != null)
            {
                CurrentMethodTracer.StopTrace();
                MethodTracers.Push(CurrentMethodTracer);
            }
            CurrentMethodTracer = new MethodTracer();

            CurrentMethodTracer.StartTrace();
        }

        public void StopTrace()
        {
            CurrentMethodTracer.StopTrace();
            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(2).GetMethod().Name;
            string className = stackTrace.GetFrame(2).GetMethod().ReflectedType.Name;
            double methodExecutionTime = CurrentMethodTracer.GetExecutionTime();
            List<MethodInfo> methodInfos = CurrentMethodTracer.GetChildMethods();
            //List<MethodInfo> methodInfos = CurrentMethodTracer.ChildMethods;
            MethodInfo methodInfo = new MethodInfo(methodName, className, methodExecutionTime, methodInfos);
            if (MethodTracers.Count > 0)
            {
                CurrentMethodTracer = MethodTracers.Pop();
                CurrentMethodTracer.AddChildMethod(methodInfo);
                //CurrentMethodTracer.ChildMethods.Add(methodInfo);
            }
            else
            {
                MethodInfoList.Add(methodInfo);
                CurrentMethodTracer = null;
            }
        }
    }
}
