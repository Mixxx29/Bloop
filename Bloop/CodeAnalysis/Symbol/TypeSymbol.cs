namespace Bloop.CodeAnalysis.Symbol
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Void = new TypeSymbol("void");
        public static readonly TypeSymbol Number = new TypeSymbol("number");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");
        public static readonly TypeSymbol String = new TypeSymbol("string");
        public static readonly TypeSymbol Error = new TypeSymbol("?");

        public TypeSymbol(string name)
            : base(name)
        {

        }

        public override SymbolType SymbolType => SymbolType.TYPE;
    }
}
