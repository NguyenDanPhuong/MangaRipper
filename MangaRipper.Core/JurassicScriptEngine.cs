using Jurassic;
using MangaRipper.Core.Interfaces;

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

        public TResult CallGlobalFunction<TResult>(string functionName, params object[] argumentValues)
        {
            return engine.CallGlobalFunction<TResult>(functionName, argumentValues);
        }
    }
}
