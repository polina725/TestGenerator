using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TracerLib
{
    public class ThreadInfo
    {
        private List<MethodInfo> methods;
        private Stack<MethodInfo> callStack;

        [JsonPropertyName("id")]
        public int Id { get; private set; }

        [JsonPropertyName("time")]
        public long LifeTime { get; private set; }

        [JsonPropertyName("methods")]
        public List<MethodInfo> Methods { get { return methods; } private set { } }

        public ThreadInfo(int id)
        {
            methods = new List<MethodInfo>();
            callStack = new Stack<MethodInfo>();
            LifeTime = 0;
            Id = id;
        }

        public void StartTracingMethod(MethodInfo method) 
        {

            if (callStack.Count == 0)
                methods.Add(method);
            else
                callStack.Peek().Add(method);
            callStack.Push(method);
        }

        public void StopTracingMethod()
        {
            LifeTime += callStack.Peek().Stop();
            callStack.Pop();
        }

        public List<MethodInfo> GetMethods()
        {
            return methods;
        }
    }
}
