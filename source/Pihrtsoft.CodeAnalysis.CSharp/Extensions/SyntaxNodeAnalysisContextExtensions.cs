// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class SyntaxNodeAnalysisContextExtensions
    {
        [DebuggerStepThrough]
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            Diagnostic diagnostic = Diagnostic.Create(descriptor, location, messageArgs);

            context.ReportDiagnostic(diagnostic);
        }

        [DebuggerStepThrough]
        public static INamedTypeSymbol GetTypeByMetadataName(
            this SyntaxNodeAnalysisContext context,
            string fullyQualifiedMetadataName)
        {
            return context.SemanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }
    }
}
