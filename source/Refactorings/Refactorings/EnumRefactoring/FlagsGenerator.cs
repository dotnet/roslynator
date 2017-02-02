// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings.EnumWithFlagsAttribute
{
    internal static class FlagsGenerator
    {
        public static Optional<object> GetNewValue(
            SpecialType specialType,
            IEnumerable<object> reservedValues,
            FlagsGenerationMode mode = FlagsGenerationMode.None)
        {
            switch (specialType)
            {
                case SpecialType.System_SByte:
                    {
                        Optional<sbyte> result = GetNewValue(reservedValues.Cast<sbyte>(), mode);

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
                        Optional<byte> result = GetNewValue(reservedValues.Cast<byte>(), mode);

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
                        Optional<short> result = GetNewValue(reservedValues.Cast<short>(), mode);

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
                        Optional<ushort> result = GetNewValue(reservedValues.Cast<ushort>(), mode);

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
                        Optional<int> result = GetNewValue(reservedValues.Cast<int>(), mode);

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
                        Optional<uint> result = GetNewValue(reservedValues.Cast<uint>(), mode);

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
                        Optional<long> result = GetNewValue(reservedValues.Cast<long>(), mode);

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
                        Optional<ulong> result = GetNewValue(reservedValues.Cast<ulong>(), mode);

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

        public static Optional<sbyte> GetNewValue(IEnumerable<sbyte> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
        {
            if (reservedValues == null)
                throw new ArgumentNullException(nameof(reservedValues));

            sbyte[] values = reservedValues.Where(f => f >= 0 && IsZeroOrPowerOfTwo(f)).ToArray();

            if (values.Length == 0)
                return 0;

            if (values.Length == 1 && values[0] == 0)
                return 1;

            if (mode == FlagsGenerationMode.None)
            {
                sbyte i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                sbyte i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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

        public static Optional<byte> GetNewValue(IEnumerable<byte> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
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

            if (mode == FlagsGenerationMode.None)
            {
                byte i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                byte i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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

        public static Optional<short> GetNewValue(IEnumerable<short> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
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

            if (mode == FlagsGenerationMode.None)
            {
                short i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                short i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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

        public static Optional<ushort> GetNewValue(IEnumerable<ushort> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
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

            if (mode == FlagsGenerationMode.None)
            {
                ushort i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                ushort i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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

        public static Optional<int> GetNewValue(IEnumerable<int> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
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

            if (mode == FlagsGenerationMode.None)
            {
                int i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                int i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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

        public static Optional<uint> GetNewValue(IEnumerable<uint> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
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

            if (mode == FlagsGenerationMode.None)
            {
                uint i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                uint i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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

        public static Optional<long> GetNewValue(IEnumerable<long> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
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

            if (mode == FlagsGenerationMode.None)
            {
                long i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                long i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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

        public static Optional<ulong> GetNewValue(IEnumerable<ulong> reservedValues, FlagsGenerationMode mode = FlagsGenerationMode.None)
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

            if (mode == FlagsGenerationMode.None)
            {
                ulong i = 1;

                while (i > 0)
                {
                    if (Array.IndexOf(values, i) == -1)
                        return i;

                    i *= 2;
                }
            }
            else if (mode == FlagsGenerationMode.FromHighestExistingValue)
            {
                ulong i = values.Max();

                i *= 2;

                if (i > 0)
                    return i;
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