// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class SyntaxTreeAnalysisContextExtensions
    {
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            Diagnostic diagnostic = Diagnostic.Create(descriptor, location, messageArgs);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
