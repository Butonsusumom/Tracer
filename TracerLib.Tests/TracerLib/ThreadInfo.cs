using System.Collections.Generic;

namespace TracerLib
{
    public class ThreadInfo
    {
        public ThreadInfo(int id, List<MethodInfo> threadMethods)
        {
            Id = id;
            Methods = new List<MethodInfo>();
            Methods = threadMethods;
        }

        public int Id { get; private set; }

        private double executionTime;

        private double SummMethodsExecutionTime(MethodInfo methodInfo)
        {
            double time = 0;
            time += methodInfo.ExecutionTime;
            foreach (MethodInfo method in methodInfo.ChildMethods)
            {
                if (method.ChildMethods.Count > 0)
                {
                    time += SummMethodsExecutionTime(method);
                }
                time += method.ExecutionTime;
            }
            return time;
        }

        public double ExecutionTime
        {
            get
            {
                if (Methods.Count > 0)
                {
                    executionTime = SummMethodsExecutionTime(Methods[0]);
                }
                return executionTime;
            }
            private set
            {
                executionTime = value;
            }
        }

        public List<MethodInfo> Methods { get; private set; }
    }
}
