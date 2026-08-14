// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator;

internal readonly struct PooledObject<T> : IDisposable where T : class, IResettable, new()
{
    internal PooledObject(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public void Dispose()
    {
        ObjectPool<T>.Free(Value);
    }
}
