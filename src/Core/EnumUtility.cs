// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class EnumUtility
    {
        public static OneOrMany<EnumFieldSymbolInfo> GetConstituentFields(object value, INamedTypeSymbol enumType)
        {
            EnumSymbolInfo enumInfo = EnumSymbolInfo.Create(enumType);

            ulong convertedValue = SymbolUtility.GetEnumValueAsUInt64(value, enumType);

            if (!enumType.HasAttribute(MetadataNames.System_FlagsAttribute)
                || convertedValue == 0)
            {
                return OneOrMany.Create(FindField(enumInfo, convertedValue));
            }

            return GetConstituentFields(convertedValue, enumInfo);
        }

        public static OneOrMany<EnumFieldSymbolInfo> GetConstituentFields(ulong value, in EnumSymbolInfo enumInfo)
        {
            ImmutableArray<EnumFieldSymbolInfo> fields = enumInfo.Fields;

            ImmutableArray<EnumFieldSymbolInfo>.Builder builder = null;

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

                        builder = ImmutableArray.CreateBuilder<EnumFieldSymbolInfo>();
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

        public static ImmutableArray<EnumFieldSymbolInfo> GetMinimalConstituentFields(IFieldSymbol fieldSymbol, in EnumSymbolInfo enumInfo)
        {
            if (!fieldSymbol.HasConstantValue)
                return ImmutableArray<EnumFieldSymbolInfo>.Empty;

            ulong value = SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumInfo.Symbol);

            return GetMinimalConstituentFields(value, enumInfo);
        }

        public static ImmutableArray<EnumFieldSymbolInfo> GetMinimalConstituentFields(ulong value, in EnumSymbolInfo enumInfo)
        {
            if (value == 0)
                return ImmutableArray<EnumFieldSymbolInfo>.Empty;

            ImmutableArray<EnumFieldSymbolInfo> fields = enumInfo.Fields;

            ImmutableArray<EnumFieldSymbolInfo>.Builder builder = null;

            ulong result = value;

            for (int i = fields.Length - 1; i >= 0; i--)
            {
                ulong val = fields[i].Value;

                if (val != 0
                    && val != value
                    && (result & val) == val)
                {
                    (builder ?? (builder = ImmutableArray.CreateBuilder<EnumFieldSymbolInfo>())).Add(fields[i]);

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

            return ImmutableArray<EnumFieldSymbolInfo>.Empty;
        }

        private static EnumFieldSymbolInfo FindField(in EnumSymbolInfo enumInfo, ulong value)
        {
            ImmutableArray<EnumFieldSymbolInfo> fields = enumInfo.Fields;

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
    }
}
