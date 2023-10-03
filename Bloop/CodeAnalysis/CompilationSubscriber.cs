namespace Bloop.CodeAnalysis
{
    public interface CompilationSubscriber
    {
        void OnCompile();
        void OnPrint(string text);
        string OnRead();
    }
}
