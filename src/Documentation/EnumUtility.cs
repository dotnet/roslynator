// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class EnumUtility
    {
        public static OneOrMany<EnumFieldInfo> GetConstituentFields(object value, INamedTypeSymbol enumType)
        {
            ImmutableArray<EnumFieldInfo> fields = GetFields(enumType);

            ulong valueAsULong = SymbolUtility.GetEnumValueAsUInt64(value, enumType);

            if (!enumType.HasAttribute(MetadataNames.System_FlagsAttribute)
                || valueAsULong == 0)
            {
                return OneOrMany.Create(FindField(fields, valueAsULong));
            }

            return GetConstituentFields(valueAsULong, fields);
        }

        public static OneOrMany<EnumFieldInfo> GetConstituentFields(ulong value, ImmutableArray<EnumFieldInfo> fields)
        {
            ImmutableArray<EnumFieldInfo>.Builder builder = null;

            ulong result = value;

            for (int i = fields.Length - 1; i >= 0; i--)
            {
                ulong val = fields[i].Value;

                if (val != 0
                    && (result & val) == val)
                {
                    if (builder == null)
                    {
                        if (result == val)
                            return OneOrMany.Create(fields[i]);

                        builder = ImmutableArray.CreateBuilder<EnumFieldInfo>();
                    }

                    builder.Add(fields[i]);

                    result -= val;

                    if (result == 0)
                        break;
                }
            }

            if (result == 0
                && builder != null)
            {
                builder.Reverse();

                return OneOrMany.Create(builder.ToImmutableArray());
            }

            return default;
        }

        public static ImmutableArray<EnumFieldInfo> GetMinimalConstituentFields(IFieldSymbol fieldSymbol, ImmutableArray<EnumFieldInfo> fields)
        {
            ulong value = SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, fieldSymbol.ContainingType);

            return GetMinimalConstituentFields(value, fields);
        }

        public static ImmutableArray<EnumFieldInfo> GetMinimalConstituentFields(ulong value, ImmutableArray<EnumFieldInfo> fields)
        {
            ImmutableArray<EnumFieldInfo>.Builder builder = null;

            ulong result = value;

            for (int i = fields.Length - 1; i >= 0; i--)
            {
                ulong val = fields[i].Value;

                if (val != 0
                    && val != value
                    && (result & val) == val)
                {
                    (builder ?? (builder = ImmutableArray.CreateBuilder<EnumFieldInfo>())).Add(fields[i]);

                    result -= val;

                    if (result == 0)
                        break;
                }
            }

            if (result == 0
                && builder != null)
            {
                Debug.Assert(builder.Count > 1);

                builder.Reverse();

                return builder.ToImmutableArray();
            }

            return default;
        }

        public static ImmutableArray<EnumFieldInfo> GetFields(INamedTypeSymbol enumType)
        {
            ImmutableArray<ISymbol> members = enumType.GetMembers();

            ImmutableArray<EnumFieldInfo>.Builder builder = ImmutableArray.CreateBuilder<EnumFieldInfo>(members.Length);

            foreach (ISymbol member in members)
            {
                if (member.Kind == SymbolKind.Field)
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue)
                        builder.Add(new EnumFieldInfo(fieldSymbol, SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumType)));
                }
            }

            builder.Sort(EnumFieldInfoComparer.Instance);

            return builder.ToImmutableArray();
        }

        private static EnumFieldInfo FindField(ImmutableArray<EnumFieldInfo> fields, ulong value)
        {
            int start = 0;
            int end = fields.Length - 1;

            while (start <= end)
            {
                int i = (end + start) / 2;

                long order = unchecked((long)value - (long)fields[i].Value);

                if (order == 0)
                {
                    while (i < end
                        && fields[i + 1].Value == value)
                    {
                        i++;
                    }

                    return fields[i];
                }
                else if (order > 0)
                {
                    start = i + 1;
                }
                else
                {
                    end = i - 1;
                }
            }

            return default;
        }

        private class EnumFieldInfoComparer : IComparer<EnumFieldInfo>
        {
            public static EnumFieldInfoComparer Instance { get; } = new EnumFieldInfoComparer();

            public int Compare(EnumFieldInfo x, EnumFieldInfo y)
            {
                int result = x.Value.CompareTo(y.Value);

                if (result != 0)
                    return result;

                return string.CompareOrdinal(y.Name, x.Name);
            }
        }
    }
}
