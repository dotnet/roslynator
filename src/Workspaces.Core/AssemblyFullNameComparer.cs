// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Roslynator;

internal class AssemblyFullNameComparer : EqualityComparer<Assembly>
{
    public static AssemblyFullNameComparer Instance { get; } = new();

    public override bool Equals(Assembly x, Assembly y)
    {
        return StringComparer.Ordinal.Equals(x.FullName, y.FullName);
    }

    public override int GetHashCode(Assembly obj)
    {
        return StringComparer.Ordinal.GetHashCode(obj.FullName);
    }
}