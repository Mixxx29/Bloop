using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    public class BoundProgram
    {
        public BoundProgram(BoundGlobalScope gloalScope, DiagnosticsPool diagnostics, Dictionary<FunctionSymbol, BoundBlockStatement> functionBodies)
        {
            GloalScope = gloalScope;
            Diagnostics = diagnostics;
            FunctionBodies = functionBodies;
        }

        public BoundGlobalScope GloalScope { get; }
        public DiagnosticsPool Diagnostics { get; }
        public Dictionary<FunctionSymbol, BoundBlockStatement> FunctionBodies { get; }
    }
}