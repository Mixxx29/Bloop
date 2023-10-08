using Bloop.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Symbol
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol type, FunctionDeclarationSyntax? declaration = null)
            : base(name)
        {
            Parameters = parameters;
            Type = type;
            Declaration = declaration;
        }

        public override SymbolType SymbolType => throw new NotImplementedException();

        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol Type { get; }
        public FunctionDeclarationSyntax? Declaration { get; }
    }
}
