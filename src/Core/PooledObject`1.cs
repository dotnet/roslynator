// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator;

internal struct PooledObject<T> : IDisposable where T : class, IResettable, new()
{
    private T? _value;

    internal PooledObject(T value)
    {
        _value = value;
    }

    public readonly T Value => _value!;

    public void Dispose()
    {
        T? value = _value;

        if (value is null)
            return;

        _value = null;
        ObjectPool<T>.Free(value);
    }
}
