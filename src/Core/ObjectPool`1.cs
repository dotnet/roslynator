// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator;

internal static class ObjectPool<T> where T : class, IResettable, new()
{
    [ThreadStatic]
    private static T? _cachedInstance;

    public static PooledObject<T> Rent()
    {
        return new PooledObject<T>(RentInstance());
    }

    public static T RentInstance()
    {
        T? instance = _cachedInstance;

        _cachedInstance = null;

        return instance ?? new T();
    }

    internal static void Free(T? instance)
    {
        if (instance is null)
            return;

        if (ReferenceEquals(_cachedInstance, instance))
            return;

        if (instance.Reset())
            _cachedInstance = instance;
    }
}
