using Bloop.CodeAnalysis.Binding;
using Bloop.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using Bloop.CodeAnalysis.Lowering;

namespace Bloop.CodeAnalysis
{
    public class Compilation
    {
        private BoundGlobalScope? _globalScope;

        public Compilation(SyntaxTree syntaxTree)
            : this(null, syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public Compilation(Compilation? previous, SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }

        BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(null, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public Compilation? Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        public EvaluationResult Evaluate()
        {
            var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null, null);

            var variables = new Dictionary<VariableSymbol, object?>();
            var statement = Lowerer.Lower(GlobalScope.Statement);
            var evaluator = new Evaluator(statement, variables);
            var result = evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, result, statement);
        }
    }
}
