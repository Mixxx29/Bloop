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

        public EvaluationResult Compile(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;

            var globalScope = Binder.BindGlobalScope(null, syntaxTree.Root);

            var statement = Lowerer.Lower(globalScope.Statement);

            OnCompile?.Invoke();

            var diagnostics = SyntaxTree.Diagnostics.Concat(globalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null, statement);

            var variables = new Dictionary<VariableSymbol, object?>();
            var result = Evaluator.Evaluate(statement, variables);
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, result, statement);
        }

        public void Subscribe(CompilationSubscriber subscriber)
        {
            OnCompile += subscriber.OnCompile;
            Evaluator.Subscribe(subscriber);
        }
    }
}