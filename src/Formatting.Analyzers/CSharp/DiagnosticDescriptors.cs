// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Formatting.CSharp
{
    public static partial class DiagnosticDescriptors
    {
        private static DiagnosticDescriptorFactory Factory { get; } = DiagnosticDescriptorFactory.CreateFromAssemblyLocation(typeof(DiagnosticDescriptors).Assembly.Location);
    }
}