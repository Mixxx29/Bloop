﻿using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class BLockStatementSyntax : StatementSyntax
    {
        public BLockStatementSyntax(SyntaxToken openBraceToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceToken = closeBraceToken;
        }

        public override SyntaxType Type => SyntaxType.BLOCK_STATEMENT;

        public SyntaxToken OpenBraceToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }
    }
}
