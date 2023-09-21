using Bloop.CodeAnalysis.Text;

namespace Bloop.CodeAnalysis
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan textSpan, string message)
        {
            TextSpan = textSpan;
            Message = message;
        }

        public TextSpan TextSpan { get; }
        public string Message { get; }

        public override string ToString() => Message;
    }
}
