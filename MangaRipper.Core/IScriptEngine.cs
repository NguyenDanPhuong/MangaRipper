using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public interface IScriptEngine
    {
        void Execute(string code);
        T CallGlobalFunction<T>(string functionName, params object[] argumentValues);
    }
}
