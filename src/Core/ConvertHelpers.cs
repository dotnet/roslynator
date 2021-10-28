// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class ConvertHelpers
    {
        public static object ConvertFromUInt64(ulong value, SpecialType numericType)
        {
            switch (numericType)
            {
                case SpecialType.System_SByte:
                    return Convert.ToSByte(value);
                case SpecialType.System_Byte:
                    return Convert.ToByte(value);
                case SpecialType.System_Int16:
                    return Convert.ToInt16(value);
                case SpecialType.System_UInt16:
                    return Convert.ToUInt16(value);
                case SpecialType.System_Int32:
                    return Convert.ToInt32(value);
                case SpecialType.System_UInt32:
                    return Convert.ToUInt32(value);
                case SpecialType.System_Int64:
                    return Convert.ToInt64(value);
                case SpecialType.System_UInt64:
                    return value;
                case SpecialType.System_Decimal:
                    return Convert.ToDecimal(value);
                case SpecialType.System_Single:
                    return Convert.ToSingle(value);
                case SpecialType.System_Double:
                    return Convert.ToDouble(value);
                default:
                    throw new ArgumentException("", nameof(numericType));
            }
        }

        public static bool CanConvertFromUInt64(ulong value, SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_SByte:
                    return value <= (ulong)sbyte.MaxValue;
                case SpecialType.System_Byte:
                    return value <= byte.MaxValue;
                case SpecialType.System_Int16:
                    return value <= (ulong)short.MaxValue;
                case SpecialType.System_UInt16:
                    return value <= ushort.MaxValue;
                case SpecialType.System_Int32:
                    return value <= int.MaxValue;
                case SpecialType.System_UInt32:
                    return value <= uint.MaxValue;
                case SpecialType.System_Int64:
                    return value <= long.MaxValue;
                case SpecialType.System_UInt64:
                    return true;
                case SpecialType.System_Decimal:
                    return value <= decimal.MaxValue;
                case SpecialType.System_Single:
                    return value <= float.MaxValue;
                case SpecialType.System_Double:
                    return value <= double.MaxValue;
                default:
                    throw new ArgumentException("", nameof(specialType));
            }
        }

        public static ulong ConvertToUInt64(object value, SpecialType numericType)
        {
            switch (numericType)
            {
                case SpecialType.System_SByte:
                    return (ulong)(sbyte)value;
                case SpecialType.System_Byte:
                    return (byte)value;
                case SpecialType.System_Int16:
                    return (ulong)(short)value;
                case SpecialType.System_UInt16:
                    return (ushort)value;
                case SpecialType.System_Int32:
                    return (ulong)(int)value;
                case SpecialType.System_UInt32:
                    return (uint)value;
                case SpecialType.System_Int64:
                    return (ulong)(long)value;
                case SpecialType.System_UInt64:
                    return (ulong)value;
                case SpecialType.System_Decimal:
                    return (ulong)(decimal)value;
                case SpecialType.System_Single:
                    return (ulong)(float)value;
                case SpecialType.System_Double:
                    return (ulong)(double)value;
                default:
                    throw new ArgumentException("", nameof(numericType));
            }
        }
    }
}
