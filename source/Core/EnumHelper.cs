// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.Extensions;

namespace Roslynator
{
    public static class EnumHelper
    {
        public static IEnumerable<object> GetValues(ITypeSymbol enumSymbol)
        {
            foreach (IFieldSymbol fieldSymbol in enumSymbol.GetFields())
            {
                if (fieldSymbol.HasConstantValue)
                    yield return fieldSymbol.ConstantValue;
            }
        }

        public static bool IsValueDefined(INamedTypeSymbol enumSymbol, object value)
        {
            foreach (object value2 in GetValues(enumSymbol))
            {
                if (object.Equals(value2, value))
                    return true;
            }

            return false;
        }

        public static Optional<object> GetUniquePowerOfTwo(INamedTypeSymbol enumSymbol, bool startFromHighestExistingValue = false)
        {
            if (enumSymbol == null)
                throw new ArgumentNullException(nameof(enumSymbol));

            return GetUniquePowerOfTwo(
                enumSymbol.EnumUnderlyingType.SpecialType,
                GetValues(enumSymbol),
                startFromHighestExistingValue);
        }

        public static Optional<object> GetUniquePowerOfTwo(
            SpecialType enumUnderlyingType,
            IEnumerable<object> reservedValues,
            bool startFromHighestExistingValue = false)
        {
            switch (enumUnderlyingType)
            {
                case SpecialType.System_SByte:
                    {
                        Optional<sbyte> result = GetUniquePowerOfTwo(reservedValues.Cast<sbyte>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
                case SpecialType.System_Byte:
                    {
                        Optional<byte> result = GetUniquePowerOfTwo(reservedValues.Cast<byte>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
                case SpecialType.System_Int16:
                    {
                        Optional<short> result = GetUniquePowerOfTwo(reservedValues.Cast<short>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
                case SpecialType.System_UInt16:
                    {
                        Optional<ushort> result = GetUniquePowerOfTwo(reservedValues.Cast<ushort>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
                case SpecialType.System_Int32:
                    {
                        Optional<int> result = GetUniquePowerOfTwo(reservedValues.Cast<int>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
                case SpecialType.System_UInt32:
                    {
                        Optional<uint> result = GetUniquePowerOfTwo(reservedValues.Cast<uint>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
                case SpecialType.System_Int64:
                    {
                        Optional<long> result = GetUniquePowerOfTwo(reservedValues.Cast<long>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
                case SpecialType.System_UInt64:
                    {
                        Optional<ulong> result = GetUniquePowerOfTwo(reservedValues.Cast<ulong>(), startFromHighestExistingValue);

                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                        else
                        {
                            return default(Optional<object>);
                        }
                    }
            }

            return default(Optional<object>);
        }

        public static Optional<sbyte> GetUniquePowerOfTwo(IEnumerable<sbyte> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            sbyte[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                sbyte i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                sbyte i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<sbyte>);
        }

        public static bool IsZeroOrPowerOfTwo(sbyte x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(sbyte x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        public static Optional<byte> GetUniquePowerOfTwo(IEnumerable<byte> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            byte[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                byte i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                byte i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<byte>);
        }

        public static bool IsZeroOrPowerOfTwo(byte x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(byte x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        public static Optional<short> GetUniquePowerOfTwo(IEnumerable<short> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            short[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                short i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                short i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<short>);
        }

        public static bool IsZeroOrPowerOfTwo(short x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(short x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        public static Optional<ushort> GetUniquePowerOfTwo(IEnumerable<ushort> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            ushort[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                ushort i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                ushort i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<ushort>);
        }

        public static bool IsZeroOrPowerOfTwo(ushort x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(ushort x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        public static Optional<int> GetUniquePowerOfTwo(IEnumerable<int> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            int[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                int i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                int i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<int>);
        }

        public static bool IsZeroOrPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(int x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        public static Optional<uint> GetUniquePowerOfTwo(IEnumerable<uint> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            uint[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                uint i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                uint i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<uint>);
        }

        public static bool IsZeroOrPowerOfTwo(uint x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(uint x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        public static Optional<long> GetUniquePowerOfTwo(IEnumerable<long> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            long[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                long i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                long i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<long>);
        }

        public static bool IsZeroOrPowerOfTwo(long x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(long x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        public static Optional<ulong> GetUniquePowerOfTwo(IEnumerable<ulong> reservedValues, bool startFromHighestExistingValue = false)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            ulong[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (startFromHighestExistingValue)
            {
                ulong i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
            }
            else
            {
                ulong i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }

            return default(Optional<ulong>);
        }

        public static bool IsZeroOrPowerOfTwo(ulong x)
        {
            return (x & (x - 1)) == 0;
        }

        public static bool IsPowerOfTwo(ulong x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }
    }
}