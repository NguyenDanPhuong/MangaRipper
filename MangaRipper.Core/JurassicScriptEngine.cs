using Jurassic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class JurassicScriptEngine: IScriptEngine
    {
        private ScriptEngine engine;

        public JurassicScriptEngine()
        {
            engine = new ScriptEngine();
        }
        public void Execute(string code)
        {
            engine.Execute(code);
        }

        public T CallGlobalFunction<T>(string functionName, params object[] argumentValues)
        {
            return engine.CallGlobalFunction<T>(functionName, argumentValues);
        }
    }
}
