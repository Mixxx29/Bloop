﻿namespace Bloop.CodeAnalysis.Symbol
{
    public class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type)
            : base(name)
        {
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public bool IsReadOnly { get; }
        public TypeSymbol Type { get; }

        public override SymbolType SymbolType => SymbolType.VARIABLE;
    }
}