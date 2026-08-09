// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Testing;

// Trimmed copy of Roslynator.Hash (src/Core/Hash.cs) containing only the members used here.
// Duplicated so that the testing framework does not depend on the Roslynator.Core package.
// http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function

internal static class Hash
{
    public const int OffsetBasis = unchecked((int)2166136261);

    public const int Prime = 16777619;

    public static int Combine(int value, int hash)
    {
        return unchecked((hash * Prime) + value);
    }
}
