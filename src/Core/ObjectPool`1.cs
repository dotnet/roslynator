// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Roslynator;

internal static class ObjectPool<T> where T : class, IResettable, new()
{
    [ThreadStatic]
    private static T? _cachedInstance;

    public static PooledObject<T> Rent()
    {
        T? instance = _cachedInstance;

        _cachedInstance = null;

        return new PooledObject<T>(instance ?? new T());
    }

    internal static void Free(T instance)
    {
        Debug.Assert(!ReferenceEquals(_cachedInstance, instance), $"'{typeof(T).Name}' freed twice.");

        instance.Reset();

        if (instance.CanBeCached)
            _cachedInstance = instance;
    }
}
