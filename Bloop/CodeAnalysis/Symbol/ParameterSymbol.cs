namespace Bloop.CodeAnalysis.Symbol
{
    public sealed class ParameterSymbol : VariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type)
            : base(name, true, type)
        {
        }

        public override SymbolType SymbolType => throw new NotImplementedException();
    }
}
