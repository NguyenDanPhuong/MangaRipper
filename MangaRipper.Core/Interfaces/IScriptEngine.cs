namespace MangaRipper.Core.Interfaces
{
    public interface IScriptEngine
    {
        void Execute(string code);
        TResult CallGlobalFunction<TResult>(string functionName, params object[] argumentValues);
    }
}
