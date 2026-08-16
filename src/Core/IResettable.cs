// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator;

internal interface IResettable
{
    /// <summary>
    /// Clears instance state. Returns <c>true</c> if the instance is worth retaining.
    /// Implementations must snapshot buffer size (capacity when available) before clearing
    /// and return <c>false</c> when it exceeded <see cref="ObjectPool.MaxCachedBufferSize"/>.
    /// </summary>
    bool Reset();
}
