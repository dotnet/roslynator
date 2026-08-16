// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Xunit;

namespace Roslynator.Testing.CSharp;

public static class ObjectPoolTests
{
    [Fact]
    public static void Rent_Free_ReusesInstance()
    {
        PooledBuffer first;
        using (PooledObject<PooledBuffer> pooled = ObjectPool<PooledBuffer>.Rent())
        {
            first = pooled.Value;
        }

        using PooledObject<PooledBuffer> pooled2 = ObjectPool<PooledBuffer>.Rent();

        Assert.Same(first, pooled2.Value);
    }

    [Fact]
    public static void Free_DropsOversizedInstance()
    {
        PooledBuffer first;
        using (PooledObject<PooledBuffer> pooled = ObjectPool<PooledBuffer>.Rent())
        {
            first = pooled.Value;
            first.Grow(ObjectPool.MaxCachedBufferSize + 1);
        }

        using PooledObject<PooledBuffer> pooled2 = ObjectPool<PooledBuffer>.Rent();

        Assert.NotSame(first, pooled2.Value);
    }

    [Fact]
    public static void Dispose_SecondCallOnSameLocal_IsNoOp()
    {
        PooledObject<PooledBuffer> pooled = ObjectPool<PooledBuffer>.Rent();
        PooledBuffer first = pooled.Value;

        pooled.Dispose();
        pooled.Dispose();

        using PooledObject<PooledBuffer> pooled2 = ObjectPool<PooledBuffer>.Rent();

        Assert.Same(first, pooled2.Value);
    }

    [Fact]
    public static void Free_DroppedInstance_DoesNotRecache()
    {
        PooledBuffer first;
        using (PooledObject<PooledBuffer> pooled = ObjectPool<PooledBuffer>.Rent())
        {
            first = pooled.Value;
            first.Grow(ObjectPool.MaxCachedBufferSize + 1);
        }

        ObjectPool<PooledBuffer>.Free(first);

        using PooledObject<PooledBuffer> pooled2 = ObjectPool<PooledBuffer>.Rent();

        Assert.NotSame(first, pooled2.Value);
    }

    private sealed class PooledBuffer : IResettable
    {
        public List<int> Items { get; } = [];

        public void Grow(int count)
        {
            for (int i = 0; i < count; i++)
                Items.Add(i);
        }

        public bool Reset()
        {
            bool canBeCached = Items.Capacity <= ObjectPool.MaxCachedBufferSize;
            Items.Clear();
            return canBeCached;
        }
    }
}
