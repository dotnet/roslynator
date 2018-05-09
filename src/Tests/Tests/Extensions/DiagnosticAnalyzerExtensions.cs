// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal static class DiagnosticAnalyzerExtensions
    {
        public static bool Supports(this DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor)
        {
            return analyzer.SupportedDiagnostics.IndexOf(descriptor, DiagnosticDescriptorComparer.Id) != -1;
        }
    }
}
