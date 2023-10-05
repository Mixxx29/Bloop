using Bloop.CodeAnalysis.Binding;
using Bloop.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using Bloop.CodeAnalysis.Lowering;
using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis
{
    public class Compilation
    {
        private BoundGlobalScope? _globalScope;

        public Compilation()
        {
            Evaluator = new Evaluator();
        }

        public delegate void CompileHandler();
        public event CompileHandler? OnCompile;

        public Compilation? Previous { get; }
        public SyntaxTree SyntaxTree { get; private set; }
        public Evaluator Evaluator { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; private set; }

        public void Compile(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;

            var globalScope = Binder.BindGlobalScope(null, syntaxTree.Root);

            var statement = Lowerer.Lower(globalScope.Statement);

            Diagnostics = SyntaxTree.Diagnostics.Concat(globalScope.Diagnostics).ToImmutableArray();

            OnCompile?.Invoke();

            if (Diagnostics.Any())
                return;

            var variables = new Dictionary<VariableSymbol, object?>();
            Evaluator.Evaluate(statement, variables);
        }

        public void Subscribe(CompilationSubscriber subscriber)
        {
            OnCompile += subscriber.OnCompile;
            Evaluator.Subscribe(subscriber);
        }
    }
}