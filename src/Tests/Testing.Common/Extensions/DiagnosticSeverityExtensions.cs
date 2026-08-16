// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator.Testing;

// ToReportDiagnostic is a copy of Roslynator.DiagnosticsExtensions.ToReportDiagnostic
// (src/Core/Extensions/DiagnosticsExtensions.cs).
// Duplicated so that the testing framework does not depend on the Roslynator.Core package.
internal static class DiagnosticSeverityExtensions
{
    public static ReportDiagnostic ToReportDiagnostic(this DiagnosticSeverity diagnosticSeverity)
    {
        switch (diagnosticSeverity)
        {
            case DiagnosticSeverity.Hidden:
                return ReportDiagnostic.Hidden;
            case DiagnosticSeverity.Info:
                return ReportDiagnostic.Info;
            case DiagnosticSeverity.Warning:
                return ReportDiagnostic.Warn;
            case DiagnosticSeverity.Error:
                return ReportDiagnostic.Error;
            default:
                throw new ArgumentException($"Unknown value '{diagnosticSeverity}'.", nameof(diagnosticSeverity));
        }
    }
}
