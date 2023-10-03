using System.Collections.Immutable;
using System.Reflection;

namespace Bloop.CodeAnalysis.Symbol
{
    public class BuiltinFunctions
    {
        public static readonly FunctionSymbol Print = new FunctionSymbol(
            "print",
            ImmutableArray.Create(
                new ParameterSymbol("text", TypeSymbol.String)
            ),
            TypeSymbol.Void
        );

        public static readonly FunctionSymbol Read = new FunctionSymbol(
            "read",
            ImmutableArray<ParameterSymbol>.Empty,
            TypeSymbol.String
        );

        public static readonly FunctionSymbol ParseInt = new FunctionSymbol(
            "parseInt",
            ImmutableArray.Create(
                new ParameterSymbol("text", TypeSymbol.String)
            ),
            TypeSymbol.Number
        );

        public static IEnumerable<FunctionSymbol?> GetAll()
            => typeof(BuiltinFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                                       .Where(f => f.FieldType == typeof(FunctionSymbol))
                                       .Select(f => (FunctionSymbol?)f.GetValue(null));
    }
}
