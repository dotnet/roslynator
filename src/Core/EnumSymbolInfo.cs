// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct EnumSymbolInfo
    {
        private EnumSymbolInfo(INamedTypeSymbol symbol, ImmutableArray<EnumFieldSymbolInfo> fields)
        {
            Symbol = symbol;
            Fields = fields;
        }

        public INamedTypeSymbol Symbol { get; }

        public ImmutableArray<EnumFieldSymbolInfo> Fields { get; }

        public bool Contains(ulong value)
        {
            foreach (EnumFieldSymbolInfo field in Fields)
            {
                if (field.HasValue
                    && field.Value == value)
                {
                    return true;
                }
            }

            return false;
        }

        public List<EnumFieldSymbolInfo> Decompose(in EnumFieldSymbolInfo fieldInfo)
        {
            return Decompose(fieldInfo.Value);
        }

        public List<EnumFieldSymbolInfo> Decompose(ulong value)
        {
            List<EnumFieldSymbolInfo> values = null;

            int i = Fields.Length - 1;

            while (i >= 0
                && Fields[i].Value >= value)
            {
                i--;
            }

            while (i >= 0)
            {
                ulong value2 = Fields[i].Value;

                if ((value & value2) == value2)
                {
                    (values ?? (values = new List<EnumFieldSymbolInfo>())).Add(Fields[i]);

                    value &= ~value2;

                    if (value == 0)
                        return values;
                }

                i--;
            }

            return null;
        }

        public static EnumSymbolInfo Create(INamedTypeSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (!TryCreate(symbol, out EnumSymbolInfo enumInfo))
                throw new ArgumentException("", nameof(symbol));

            return enumInfo;
        }

        public static bool TryCreate(INamedTypeSymbol symbol, out EnumSymbolInfo enumInfo)
        {
            if (symbol?.TypeKind != TypeKind.Enum)
            {
                enumInfo = default;
                return false;
            }

            ImmutableArray<ISymbol> members = symbol.GetMembers();

            ImmutableArray<EnumFieldSymbolInfo>.Builder fields = ImmutableArray.CreateBuilder<EnumFieldSymbolInfo>(members.Length);

            foreach (ISymbol member in members)
            {
                if (!member.IsImplicitlyDeclared
                    && member is IFieldSymbol fieldSymbol)
                {
                    fields.Add(EnumFieldSymbolInfo.Create(fieldSymbol));
                }
            }

            fields.Sort(EnumFieldSymbolInfoComparer.Instance);

            enumInfo = new EnumSymbolInfo(symbol, fields.ToImmutableArray());

            return true;
        }

        private class EnumFieldSymbolInfoComparer : IComparer<EnumFieldSymbolInfo>
        {
            public static EnumFieldSymbolInfoComparer Instance { get; } = new EnumFieldSymbolInfoComparer();

            public int Compare(EnumFieldSymbolInfo x, EnumFieldSymbolInfo y)
            {
                int result = x.Value.CompareTo(y.Value);

                if (result != 0)
                    return result;

                return string.CompareOrdinal(y.Name, x.Name);
            }
        }
    }
}
