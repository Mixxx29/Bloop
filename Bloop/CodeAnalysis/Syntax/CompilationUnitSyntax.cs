using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    { 
        public CompilationUnitSyntax(ImmutableArray<MemberSyntax> members, SyntaxToken endOfFileToken)
        {
            Members = members;
            EndOfFileToken = endOfFileToken;
        }
        public override SyntaxType Type => SyntaxType.COMPILATION_UNIT;

        public ImmutableArray<MemberSyntax> Members { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
}
