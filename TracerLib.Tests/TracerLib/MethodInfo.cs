using System.Collections.Generic;

namespace TracerLib
{
    public class MethodInfo
    {
        public string Name { get; private set; }

        public string ClassName { get; private set; }

        public double ExecutionTime { get; private set; }

        public List<MethodInfo> ChildMethods { get; private set; }

        public MethodInfo(string name, string className, double executionTime, List<MethodInfo> childMethods)
        {
            Name = name;
            ClassName = className;
            ExecutionTime = executionTime;
            ChildMethods = new List<MethodInfo>(childMethods);
        }
    }
}
