using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TracerLib {
    public class JsonTracerSerializer : ITracerSerializer {
        public string Serealize(TraceResult traceResult) {
            JArray threadJArray = new JArray();
            foreach (ThreadInfo thread in traceResult.ThreadsInfo) {
                JObject threadJObject = GetThreadJObject(thread);
                threadJArray.Add(threadJObject);
            }
            JObject resultJObject = new JObject {
                {"threads", threadJArray }
            };
            StringWriter stringWriter = new StringWriter();
            using (var jsonWriter = new JsonTextWriter(stringWriter)) {
                jsonWriter.Formatting = Formatting.Indented;
                resultJObject.WriteTo(jsonWriter);
            }
            return stringWriter.ToString();
        }

        private JObject GetMethodJObject(MethodInfo methodInfo) {
            return new JObject
            {
                {"name", methodInfo.Name },
                {"class", methodInfo.ClassName },
                {"time", methodInfo.ExecutionTime },
            };
        }

        private JObject GetMethodJObjectWithChildMethods(MethodInfo methodInfo) {
            JObject methodJObject = GetMethodJObject(methodInfo);
            JArray methodsJArray = new JArray();
            foreach (MethodInfo method in methodInfo.ChildMethods) {
                JObject childMethodJObject = GetMethodJObject(method);
                if (method.ChildMethods.Count > 0) {
                    childMethodJObject = GetMethodJObjectWithChildMethods(method);
                }
                methodsJArray.Add(childMethodJObject);
            }
            methodJObject.Add("methods", methodsJArray);
            return methodJObject;
        }

        private JObject GetThreadJObject(ThreadInfo threadInfo) {
            JArray methodJArray = new JArray();
            foreach (MethodInfo method in threadInfo.Methods) {
                JObject methodJObject = GetMethodJObjectWithChildMethods(method);

                methodJArray.Add(methodJObject);
            }
            return new JObject {
                {"id", threadInfo.Id },
                {"time", threadInfo.ExecutionTime },
                {"methods", methodJArray }
            };
        }
    }
}
