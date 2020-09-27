using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TracerLib;

namespace ConsoleApp
{
    class Program
    {
        public class Foo
        {
            private Bar _bar;
            private ITracer _tracer;

            internal Foo(ITracer tracer)
            {
                _tracer = tracer;
                _bar = new Bar(_tracer);
            }

            public void MyMethod()
            {
                _tracer.StartTrace();
                _bar.InnerMethod();
                _tracer.StopTrace();
            }
        }

        public class Bar
        {
            private ITracer _tracer;

            internal Bar(ITracer tracer)
            {
                _tracer = tracer;
            }

            public void InnerMethod()
            {
                _tracer.StartTrace();
                _tracer.StopTrace();
            }
        }

        public void Method(object o)
        {
            Tracer tracer = (Tracer)o;
            tracer.StartTrace();
            Thread.Sleep(100);
            tracer.StopTrace();
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            Thread thread = new Thread(new ParameterizedThreadStart(program.Method));

            Tracer tracer = new Tracer();
            Foo foo = new Foo(tracer);
            foo.MyMethod();
            thread.Start(tracer);
            thread.Join();

            XmlTracerSerializer xmlTracerSerializer = new XmlTracerSerializer();
            JsonTracerSerializer jsonTracerSerializer = new JsonTracerSerializer();
            TraceResult traceResult = tracer.GetTraceResult();
            string xml = xmlTracerSerializer.Serealize(traceResult);
            string json = jsonTracerSerializer.Serealize(traceResult);
            Console.WriteLine(xml);
            Console.WriteLine(json);
            File.WriteAllText("trace.json", json);
            File.WriteAllText("trace.xml", xml);
            Console.ReadKey();
        }
    }
}
