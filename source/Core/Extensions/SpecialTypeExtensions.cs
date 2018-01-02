// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SpecialTypeExtensions
    {
        public static bool Is(this SpecialType specialType, SpecialType specialType1, SpecialType specialType2)
        {
            return specialType == specialType1
                || specialType == specialType2;
        }

        public static bool Is(this SpecialType specialType, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3)
        {
            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3;
        }

        public static bool Is(this SpecialType specialType, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3, SpecialType specialType4)
        {
            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3
                || specialType == specialType4;
        }

        public static bool Is(this SpecialType specialType, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3, SpecialType specialType4, SpecialType specialType5)
        {
            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3
                || specialType == specialType4
                || specialType == specialType5;
        }

        public static bool Is(this SpecialType specialType, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3, SpecialType specialType4, SpecialType specialType5, SpecialType specialType6)
        {
            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3
                || specialType == specialType4
                || specialType == specialType5
                || specialType == specialType6;
        }

        public static bool SupportsConstantValue(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
            }

            return false;
        }
    }
}
