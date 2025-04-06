// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.CodeStyle;

[Flags]
public enum TargetBracesStyle
{
    None,
    Opening,
    Closing,
    Both = Opening | Closing,
}