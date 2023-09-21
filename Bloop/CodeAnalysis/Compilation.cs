using Bloop.CodeAnalysis.Binding;
using Bloop.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Bloop.CodeAnalysis
{
    public class Compilation
    {
        public Compilation(SyntaxTree syntaxTree) 
        {
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(SyntaxTree.Node);

            var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null);

            var evaluator = new Evaluator(boundExpression, variables);
            var result = evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, result);
        }
    }
}
