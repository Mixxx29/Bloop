using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion Identity = new Conversion();
        public static readonly Conversion Explicit = new Conversion();
        public static readonly Conversion Implicit = new Conversion();
        public static readonly Conversion Invalid = new Conversion();

        public bool IsIdentity => this == Identity;
        public bool IsExplicit => this == Explicit;
        public bool IsImplicit => this == Implicit;
        public bool IsValid => this != Invalid;

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Identity;

            if (from == TypeSymbol.Number || from == TypeSymbol.Bool)
            {
                if (to == TypeSymbol.String)
                    return Explicit;
            }

            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Number || to == TypeSymbol.Bool)
                    return Explicit;
            }

            return Invalid;
        }
    }
}
