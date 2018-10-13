// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct EnumFieldSymbolInfo
    {
        public EnumFieldSymbolInfo(IFieldSymbol symbol)
        {
            Symbol = symbol;
            UnderlyingType = symbol.ContainingType.EnumUnderlyingType.SpecialType;
            Value = symbol.ConstantValue;
        }

        public IFieldSymbol Symbol { get; }

        public object Value { get; }

        public SpecialType UnderlyingType { get; }

        public string Name => Symbol?.Name;

        public bool IsComposite()
        {
            switch (UnderlyingType)
            {
                case SpecialType.System_SByte:
                    return FlagsUtility<sbyte>.Instance.IsComposite((sbyte)Value);
                case SpecialType.System_Byte:
                    return FlagsUtility<byte>.Instance.IsComposite((byte)Value);
                case SpecialType.System_Int16:
                    return FlagsUtility<short>.Instance.IsComposite((short)Value);
                case SpecialType.System_UInt16:
                    return FlagsUtility<ushort>.Instance.IsComposite((ushort)Value);
                case SpecialType.System_Int32:
                    return FlagsUtility<int>.Instance.IsComposite((int)Value);
                case SpecialType.System_UInt32:
                    return FlagsUtility<uint>.Instance.IsComposite((uint)Value);
                case SpecialType.System_Int64:
                    return FlagsUtility<long>.Instance.IsComposite((long)Value);
                case SpecialType.System_UInt64:
                    return FlagsUtility<ulong>.Instance.IsComposite((ulong)Value);
            }

            return false;
        }

        public List<EnumFieldSymbolInfo> Decompose(ImmutableArray<EnumFieldSymbolInfo> infos)
        {
            if (UnderlyingType == SpecialType.System_Int32)
                return Decompose((int)Value, infos);

            switch (UnderlyingType)
            {
                case SpecialType.System_SByte:
                    return Decompose((sbyte)Value, infos);
                case SpecialType.System_Byte:
                    return Decompose((byte)Value, infos);
                case SpecialType.System_Int16:
                    return Decompose((short)Value, infos);
                case SpecialType.System_UInt16:
                    return Decompose((ushort)Value, infos);
                case SpecialType.System_UInt32:
                    return Decompose((uint)Value, infos);
                case SpecialType.System_Int64:
                    return Decompose((long)Value, infos);
                case SpecialType.System_UInt64:
                    return Decompose((ulong)Value, infos);
            }

            return null;
        }

        private static List<EnumFieldSymbolInfo> Decompose(int value, ImmutableArray<EnumFieldSymbolInfo> infos)
        {
            List<EnumFieldSymbolInfo> values = null;

            int i = infos.Length - 1;

            while (i >= 0
                && Convert.ToInt32(infos[i].Value) >= value)
            {
                i--;
            }

            while (i >= 0)
            {
                int value2 = Convert.ToInt32(infos[i].Value);

                if ((value & value2) == value2)
                {
                    (values ?? (values = new List<EnumFieldSymbolInfo>())).Add(infos[i]);

                    value &= ~value2;

                    if (value == 0)
                        return values;
                }

                i--;
            }

            return null;
        }

        private static List<EnumFieldSymbolInfo> Decompose(long value, ImmutableArray<EnumFieldSymbolInfo> infos)
        {
            List<EnumFieldSymbolInfo> values = null;

            int i = infos.Length - 1;

            while (i >= 0
                && Convert.ToInt64(infos[i].Value) >= value)
            {
                i--;
            }

            while (i >= 0)
            {
                long value2 = Convert.ToInt64(infos[i].Value);

                if ((value & value2) == value2)
                {
                    (values ?? (values = new List<EnumFieldSymbolInfo>())).Add(infos[i]);

                    value &= ~value2;

                    if (value == 0)
                        return values;
                }

                i--;
            }

            return null;
        }

        private static List<EnumFieldSymbolInfo> Decompose(ulong value, ImmutableArray<EnumFieldSymbolInfo> infos)
        {
            List<EnumFieldSymbolInfo> values = null;

            int i = infos.Length - 1;

            while (i >= 0
                && Convert.ToUInt64(infos[i].Value) >= value)
            {
                i--;
            }

            while (i >= 0)
            {
                ulong value2 = Convert.ToUInt64(infos[i].Value);

                if ((value & value2) == value2)
                {
                    (values ?? (values = new List<EnumFieldSymbolInfo>())).Add(infos[i]);

                    value &= ~value2;

                    if (value == 0)
                        return values;
                }

                i--;
            }

            return null;
        }

        public static ImmutableArray<EnumFieldSymbolInfo> CreateRange(INamedTypeSymbol enumSymbol)
        {
            ImmutableArray<EnumFieldSymbolInfo> infos = enumSymbol
                .GetMembers()
                .OfType<IFieldSymbol>()
                .Select(symbol => new EnumFieldSymbolInfo(symbol))
                .ToImmutableArray();

            foreach (EnumFieldSymbolInfo info in infos)
            {
                if (!info.Symbol.HasConstantValue)
                    return default(ImmutableArray<EnumFieldSymbolInfo>);
            }

            return infos.Sort((f, g) => ((IComparable)f.Value).CompareTo(g.Value));
        }
    }
}
