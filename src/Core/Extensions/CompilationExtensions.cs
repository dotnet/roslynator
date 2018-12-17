// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class CompilationExtensions
    {
        internal static bool IsAnalyzerSuppressed(this Compilation compilation, DiagnosticDescriptor descriptor)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("AnalyzerPerformance", "RS1012:Start action has no registered actions.", Justification = "<Pending>")]
        internal static bool AreAnalyzersSuppressed(this Compilation compilation, ImmutableArray<DiagnosticDescriptor> descriptors)
        {
            foreach (DiagnosticDescriptor descriptor in descriptors)
            {
                if (!compilation.IsAnalyzerSuppressed(descriptor))
                    return false;
            }

            return true;
        }

        internal static bool AreAnalyzersSuppressed(this Compilation compilation, DiagnosticDescriptor descriptor1, DiagnosticDescriptor descriptor2)
        {
            return IsAnalyzerSuppressed(compilation, descriptor1)
                && IsAnalyzerSuppressed(compilation, descriptor2);
        }

        internal static bool AreAnalyzersSuppressed(this Compilation compilation, DiagnosticDescriptor descriptor1, DiagnosticDescriptor descriptor2, DiagnosticDescriptor descriptor3)
        {
            return IsAnalyzerSuppressed(compilation, descriptor1)
                && IsAnalyzerSuppressed(compilation, descriptor2)
                && IsAnalyzerSuppressed(compilation, descriptor3);
        }
    }
}
