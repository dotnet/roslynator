// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

#pragma warning disable RS1012

namespace Roslynator
{
    internal static class Extensions
    {
        public static bool IsAnalyzerSuppressed(this CompilationStartAnalysisContext context, DiagnosticDescriptor descriptor)
        {
            return IsAnalyzerSuppressed(context.Compilation, descriptor);
        }

        public static bool IsAnalyzerSuppressed(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor)
        {
            return IsAnalyzerSuppressed(context.Compilation, descriptor);
        }

        public static bool IsAnalyzerSuppressed(this Compilation compilation, DiagnosticDescriptor descriptor)
        {
            ReportDiagnostic reportDiagnostic = compilation
                .Options
                .SpecificDiagnosticOptions
                .GetValueOrDefault(descriptor.Id);

            switch (reportDiagnostic)
            {
                case ReportDiagnostic.Default:
                    return !descriptor.IsEnabledByDefault;
                case ReportDiagnostic.Suppress:
                    return true;
                default:
                    return false;
            }
        }

        public static bool AreAnalyzersSuppressed(this CompilationStartAnalysisContext context, DiagnosticDescriptor descriptor1, DiagnosticDescriptor descriptor2)
        {
            return IsAnalyzerSuppressed(context, descriptor1)
                && IsAnalyzerSuppressed(context, descriptor2);
        }

        public static bool AreAnalyzersSuppressed(this CompilationStartAnalysisContext context, DiagnosticDescriptor descriptor1, DiagnosticDescriptor descriptor2, DiagnosticDescriptor descriptor3)
        {
            return IsAnalyzerSuppressed(context, descriptor1)
                && IsAnalyzerSuppressed(context, descriptor2)
                && IsAnalyzerSuppressed(context, descriptor3);
        }
    }
}
