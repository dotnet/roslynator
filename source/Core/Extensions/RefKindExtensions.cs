// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class RefKindExtensions
    {
        public static bool IsRefOrOut(this RefKind refKind)
        {
            return refKind == RefKind.Ref
                || refKind == RefKind.Out;
        }
    }
}
