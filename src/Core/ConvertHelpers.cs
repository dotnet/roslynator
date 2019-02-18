// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class ConvertHelpers
    {
        public static object Convert(ulong value, SpecialType numericType)
        {
            switch (numericType)
            {
                case SpecialType.System_SByte:
                    return System.Convert.ToSByte(value);
                case SpecialType.System_Byte:
                    return System.Convert.ToByte(value);
                case SpecialType.System_Int16:
                    return System.Convert.ToInt16(value);
                case SpecialType.System_UInt16:
                    return System.Convert.ToUInt16(value);
                case SpecialType.System_Int32:
                    return System.Convert.ToInt32(value);
                case SpecialType.System_UInt32:
                    return System.Convert.ToUInt32(value);
                case SpecialType.System_Int64:
                    return System.Convert.ToInt64(value);
                case SpecialType.System_UInt64:
                    return value;
                case SpecialType.System_Decimal:
                    return System.Convert.ToDecimal(value);
                case SpecialType.System_Single:
                    return System.Convert.ToSingle(value);
                case SpecialType.System_Double:
                    return System.Convert.ToDouble(value);
                default:
                    throw new ArgumentException("", nameof(numericType));
            }
        }

        public static bool CanConvert(ulong value, SpecialType specialType)
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
    }
}
