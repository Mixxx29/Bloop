using Bloop.CodeAnalysis.Binding;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bloop.CodeAnalysis
{
    public class EvaluationResult
    {
        public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object? value, BoundStatement? root)
        {
            Diagnostics = diagnostics;
            Value = value;
            Root = root;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public object? Value { get; }
        public BoundStatement? Root { get; }
    }
}
