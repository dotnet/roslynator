// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : Attribute
{
    public NotNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }

    public bool ReturnValue { get; }
}
