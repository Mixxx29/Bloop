namespace Bloop.CodeAnalysis.Symbol
{
    public abstract class Symbol
    {
        private protected Symbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public abstract SymbolType SymbolType { get; }

        public override string ToString() => Name;
    }
}
