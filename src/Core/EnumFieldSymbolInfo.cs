// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct EnumFieldSymbolInfo
    {
        public EnumFieldSymbolInfo(IFieldSymbol symbol, ulong value)
        {
            Symbol = symbol;
            Value = value;
        }

        public IFieldSymbol Symbol { get; }

        public ulong Value { get; }

        public bool HasValue => Symbol?.HasConstantValue == true;

        public string Name => Symbol?.Name;

        public static EnumFieldSymbolInfo Create(IFieldSymbol fieldSymbol)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (!TryCreate(fieldSymbol, out EnumFieldSymbolInfo fieldInfo))
                throw new ArgumentException("", nameof(fieldSymbol));

            return fieldInfo;
        }

        public static bool TryCreate(IFieldSymbol fieldSymbol, out EnumFieldSymbolInfo fieldInfo)
        {
            if (fieldSymbol == null)
            {
                fieldInfo = default;
                return false;
            }

            ulong value = (fieldSymbol.HasConstantValue)
                ? SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, fieldSymbol.ContainingType)
                : 0;

            fieldInfo = new EnumFieldSymbolInfo(fieldSymbol, value);

            return true;
        }

        public bool HasCompositeValue()
        {
            return FlagsUtility<ulong>.Instance.IsComposite(Value);
        }

        public IEnumerable<ulong> DecomposeValue()
        {
            return FlagsUtility<ulong>.Instance.Decompose(Value);
        }
    }
}
