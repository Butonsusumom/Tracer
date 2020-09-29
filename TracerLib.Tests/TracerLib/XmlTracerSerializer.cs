using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TracerLib
{
    public class XmlTracerSerializer : ITracerSerializer
    {
        public string Serealize(TraceResult traceResult)
        {
            List<ThreadInfo> threadsInfo = traceResult.ThreadsInfo;
            XDocument xDocument = new XDocument(new XElement("root"));

            foreach (ThreadInfo thread in threadsInfo)
            {
                XElement threadXElement = GetThreadXElement(thread);
                xDocument.Root.Add(threadXElement);
            }

            StringWriter stringWriter = new StringWriter();
            using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xDocument.WriteTo(xmlWriter);
            }
            return stringWriter.ToString();
        }

        private XElement GetMethodXElement(MethodInfo methodInfo)
        {
            return new XElement(
                "method",
                new XAttribute( "name", methodInfo.Name ),
                new XAttribute( "class", methodInfo.ClassName),
                new XAttribute( "time", methodInfo.ExecutionTime)
                );
        }

        private XElement GetMethodXElementWithChildMethods(MethodInfo methodInfo)
        {
            XElement methodXElement = GetMethodXElement(methodInfo);
            foreach (MethodInfo method in methodInfo.ChildMethods)
            {
                XElement childMethod = GetMethodXElement(method);
                if (method.ChildMethods.Count > 0)
                {
                    childMethod = GetMethodXElementWithChildMethods(method);
                }
                methodXElement.Add(childMethod);
            }
            return methodXElement;
        }

        private XElement GetThreadXElement(ThreadInfo threadInfo)
        {
            XElement threadXElement = new XElement(
                "thread",
                new XAttribute("id", threadInfo.Id),
                new XAttribute("time", threadInfo.ExecutionTime)
                );
            foreach (MethodInfo method in threadInfo.Methods)
            {
                XElement methodXElement = GetMethodXElementWithChildMethods(method);
                threadXElement.Add(methodXElement);
            }
            return threadXElement;
        }



    }
}
