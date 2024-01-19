// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator;

internal static class DiagnosticFormatter
{
    public static string FormatDiagnostic(
        Diagnostic diagnostic,
        string? baseDirectoryPath = null,
        IFormatProvider? formatProvider = null)
    {
        StringBuilder sb = StringBuilderCache.GetInstance();

        FormatLocation(diagnostic.Location, baseDirectoryPath, ref sb);

        AppendDiagnosticInfo(diagnostic, formatProvider, sb);

        return StringBuilderCache.GetStringAndFree(sb);
    }

    public static string FormatDiagnostic(
        Diagnostic diagnostic,
        string filePath,
        string? baseDirectoryPath = null,
        IFormatProvider? formatProvider = null)
    {
        StringBuilder sb = StringBuilderCache.GetInstance();

        sb.Append(PathUtilities.TrimStart(filePath, baseDirectoryPath));
        sb.Append(": ");

        AppendDiagnosticInfo(diagnostic, formatProvider, sb);

        return StringBuilderCache.GetStringAndFree(sb);
    }

    private static void AppendDiagnosticInfo(Diagnostic diagnostic, IFormatProvider? formatProvider, StringBuilder sb)
    {
        sb.Append(GetSeverityText(diagnostic.Severity));
        sb.Append(' ');
        sb.Append(diagnostic.Id);
        sb.Append(": ");
        sb.Append(diagnostic.GetMessage(formatProvider));
    }

    internal static void FormatLocation(
        Location location,
        string? baseDirectoryPath,
        ref StringBuilder sb)
    {
        switch (location.Kind)
        {
            case LocationKind.SourceFile:
            case LocationKind.XmlFile:
            case LocationKind.ExternalFile:
                {
                    FileLinePositionSpan span = location.GetMappedLineSpan();

                    if (span.IsValid)
                    {
                        sb.Append(PathUtilities.TrimStart(span.Path, baseDirectoryPath));

                        LinePosition linePosition = span.StartLinePosition;

                        sb.Append('(');
                        sb.Append(linePosition.Line + 1);
                        sb.Append(',');
                        sb.Append(linePosition.Character + 1);
                        sb.Append("): ");
                    }

                    break;
                }
        }
    }

    private static string GetSeverityText(DiagnosticSeverity diagnosticSeverity)
    {
        return diagnosticSeverity switch
        {
            DiagnosticSeverity.Hidden => "hidden",
            DiagnosticSeverity.Info => "info",
            DiagnosticSeverity.Warning => "warning",
            DiagnosticSeverity.Error => "error",
            _ => throw new InvalidOperationException(),
        };
    }
}
