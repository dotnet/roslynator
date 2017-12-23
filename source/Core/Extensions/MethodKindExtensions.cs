// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp
{
    public static class MethodKindExtensions
    {
        public static bool Is(this MethodKind methodKind, MethodKind methodKind1, MethodKind methodKind2)
        {
            return methodKind == methodKind1
                || methodKind == methodKind2;
        }

        public static bool Is(this MethodKind methodKind, MethodKind methodKind1, MethodKind methodKind2, MethodKind methodKind3)
        {
            return methodKind == methodKind1
                || methodKind == methodKind2
                || methodKind == methodKind3;
        }

        public static bool Is(this MethodKind methodKind, MethodKind methodKind1, MethodKind methodKind2, MethodKind methodKind3, MethodKind methodKind4)
        {
            return methodKind == methodKind1
                || methodKind == methodKind2
                || methodKind == methodKind3
                || methodKind == methodKind4;
        }

        public static bool Is(this MethodKind methodKind, MethodKind methodKind1, MethodKind methodKind2, MethodKind methodKind3, MethodKind methodKind4, MethodKind methodKind5)
        {
            return methodKind == methodKind1
                || methodKind == methodKind2
                || methodKind == methodKind3
                || methodKind == methodKind4
                || methodKind == methodKind5;
        }

        public static bool Is(this MethodKind methodKind, MethodKind methodKind1, MethodKind methodKind2, MethodKind methodKind3, MethodKind methodKind4, MethodKind methodKind5, MethodKind methodKind6)
        {
            return methodKind == methodKind1
                || methodKind == methodKind2
                || methodKind == methodKind3
                || methodKind == methodKind4
                || methodKind == methodKind5
                || methodKind == methodKind6;
        }
    }
}
