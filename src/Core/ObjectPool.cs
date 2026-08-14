// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator;

internal static class ObjectPool
{
    /// <summary>
    /// Maximum number of items a pooled object's buffer may have held to be worth retaining.
    /// Clearing a collection does not shrink its backing array, so an instance that grew beyond
    /// this size would keep that array alive for the lifetime of the thread.
    /// </summary>
    public const int MaxCachedBufferSize = 256;
}
