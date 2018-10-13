// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal readonly struct EnumFieldInfo
    {
        public EnumFieldInfo(IFieldSymbol symbol, ulong value)
        {
            Symbol = symbol;
            Value = value;
        }

        public IFieldSymbol Symbol { get; }

        public ulong Value { get; }

        public string Name => Symbol?.Name;

        public bool IsDefault => Symbol == null;
    }
}
