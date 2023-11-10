// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation;

internal class TypeKindComparer : IComparer<TypeKind>, IEqualityComparer<TypeKind>, IComparer, IEqualityComparer
{
    public static TypeKindComparer Instance { get; } = new();

    public int Compare(object x, object y)
    {
        if (x == y)
            return 0;

        if (x is null)
            return -1;

        if (y is null)
            return 1;

        if (x is TypeKind a
            && y is TypeKind b)
        {
            return Compare(a, b);
        }

        throw new ArgumentException("", nameof(x));
    }

    new public bool Equals(object x, object y)
    {
        if (x == y)
            return true;

        if (x is null)
            return false;

        if (y is null)
            return false;

        if (x is TypeKind a
            && y is TypeKind b)
        {
            return Equals(a, b);
        }

        throw new ArgumentException("", nameof(x));
    }

    public int GetHashCode(object obj)
    {
        if (obj is null)
            return 0;

        if (obj is TypeKind symbol)
            return GetHashCode(symbol);

        throw new ArgumentException("", nameof(obj));
    }

    public int Compare(TypeKind x, TypeKind y)
    {
        return GetRank(x).CompareTo(GetRank(y));
    }

    public bool Equals(TypeKind x, TypeKind y)
    {
        return x == y;
    }

    private static int GetRank(TypeKind typeKind)
    {
        switch (typeKind)
        {
            case TypeKind.Class:
                return 1;
            case TypeKind.Struct:
                return 2;
            case TypeKind.Interface:
                return 3;
            case TypeKind.Enum:
                return 4;
            case TypeKind.Delegate:
                return 5;
        }

        Debug.Fail(typeKind.ToString());

        return 0;
    }

    public int GetHashCode(TypeKind obj)
    {
        return obj.GetHashCode();
    }
}
