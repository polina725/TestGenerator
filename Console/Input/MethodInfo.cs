using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace TracerLib
{
    public class MethodInfo
    {
        private readonly List<MethodInfo> methods;
        private Stopwatch stopwatch;

        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonPropertyName("class")]
        public string ClassName { get; private set; }

        [JsonPropertyName("time")]
        public long ExecutionTime { get; private set; }

        [JsonPropertyName("methods")]
        public List<MethodInfo> Methods { get { return methods; } private set { } }

        public MethodInfo(MethodBase method)
        {
            methods = new List<MethodInfo>();
            Name = method.Name;
            ClassName = method.DeclaringType.Name;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void Add(MethodInfo method)
        {
            methods.Add(method);
        }

        public long Stop()
        {
            stopwatch.Stop();
            return ExecutionTime = stopwatch.ElapsedMilliseconds;
        }  
        
        public List<MethodInfo> GetMethodsList()
        {
            return methods;
        }
    }
}
