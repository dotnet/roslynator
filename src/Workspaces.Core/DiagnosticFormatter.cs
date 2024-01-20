// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
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
        IFormatProvider? formatProvider = null,
        bool omitSpan = false)
    {
        StringBuilder sb = StringBuilderCache.GetInstance();

        FormatLocation(diagnostic.Location, baseDirectoryPath, ref sb, omitSpan);

        sb.Append(GetSeverityText(diagnostic.Severity));
        sb.Append(' ');
        sb.Append(diagnostic.Id);
        sb.Append(": ");
        sb.Append(diagnostic.GetMessage(formatProvider));

        return StringBuilderCache.GetStringAndFree(sb);

        static string GetSeverityText(DiagnosticSeverity severity)
        {
            return severity switch
            {
                DiagnosticSeverity.Hidden => "hidden",
                DiagnosticSeverity.Info => "info",
                DiagnosticSeverity.Warning => "warning",
                DiagnosticSeverity.Error => "error",
                _ => throw new InvalidOperationException(),
            };
        }
    }

    private static void FormatLocation(
        Location location,
        string? baseDirectoryPath,
        ref StringBuilder sb,
        bool omitSpan = false)
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

                        if (omitSpan)
                        {
                            sb.Append(": ");
                        }
                        else
                        {
                            LinePosition linePosition = span.StartLinePosition;

                            sb.Append('(');
                            sb.Append(linePosition.Line + 1);
                            sb.Append(',');
                            sb.Append(linePosition.Character + 1);
                            sb.Append("): ");
                        }
                    }

                    break;
                }
        }
    }

    public static string FormatSymbolDefinition(
        ISymbol symbol,
        string? baseDirectoryPath = null,
        string? indentation = null,
        SymbolDisplayFormat? format = null)
    {
        StringBuilder sb = StringBuilderCache.GetInstance();

        sb.Append(indentation);

        FormatLocation(symbol.Locations[0], baseDirectoryPath, ref sb);

        sb.Append(GetSymbolTitle(symbol));

        if (symbol.IsKind(SymbolKind.Parameter, SymbolKind.TypeParameter))
        {
            sb.Append(" '");
            sb.Append(symbol.Name);
            sb.Append("': ");

            if (symbol.ContainingSymbol is IMethodSymbol { MethodKind: MethodKind.LambdaMethod })
            {
                sb.Append("anonymous function");
            }
            else
            {
                Debug.Assert(symbol.ContainingSymbol.IsKind(SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property), symbol.Kind.ToString());

                sb.Append(symbol.ContainingSymbol.ToDisplayString(format));
            }
        }
        else
        {
            sb.Append(": ");
            sb.Append(symbol.ToDisplayString(format));
        }

        return StringBuilderCache.GetStringAndFree(sb);

        static string GetSymbolTitle(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                    {
                        return "event";
                    }
                case SymbolKind.Field:
                    {
                        return (symbol.ContainingType.TypeKind == TypeKind.Enum) ? "enum field" : "field";
                    }
                case SymbolKind.Local:
                    {
                        return "local";
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Ordinary:
                                return "method";
                            case MethodKind.LocalFunction:
                                return "local function";
                        }

                        Debug.Fail(methodSymbol.MethodKind.ToString());
                        break;
                    }
                case SymbolKind.NamedType:
                    {
                        var typeSymbol = (INamedTypeSymbol)symbol;

                        switch (typeSymbol.TypeKind)
                        {
                            case TypeKind.Class:
                                return "class";
                            case TypeKind.Delegate:
                                return "delegate";
                            case TypeKind.Enum:
                                return "enum";
                            case TypeKind.Interface:
                                return "interface";
                            case TypeKind.Struct:
                                return "struct";
                        }

                        Debug.Fail(typeSymbol.TypeKind.ToString());
                        break;
                    }
                case SymbolKind.Parameter:
                    {
                        return "parameter";
                    }
                case SymbolKind.Property:
                    {
                        return (((IPropertySymbol)symbol).IsIndexer) ? "indexer" : "property";
                    }
                case SymbolKind.TypeParameter:
                    {
                        return "type parameter";
                    }
            }

            Debug.Fail(symbol.Kind.ToString());
            return symbol.Kind.ToString();
        }
    }
}
