// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator;

internal interface IResettable
{
    void Reset();

    /// <summary>
    /// Gets a value indicating whether the instance can be retained by <see cref="ObjectPool{T}"/>.
    /// Implementations should return <c>false</c> when the instance holds a buffer that grew too large to be worth retaining.
    /// </summary>
    bool CanBeCached { get; }
}
