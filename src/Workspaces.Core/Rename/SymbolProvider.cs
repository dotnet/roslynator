// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Roslynator.Rename;

#pragma warning disable RS1001, RS1022

internal class SymbolProvider
{
    public bool IncludeGeneratedCode { get; init; }

    public Matcher FileSystemMatcher { get; init; }

    public async Task<IEnumerable<ISymbol>> GetSymbolsAsync(
        Project project,
        RenameScope scope,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var compilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: default(Action<Exception, DiagnosticAnalyzer, Diagnostic>),
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: false,
            reportSuppressedDiagnostics: false);

        Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

        ImmutableArray<SymbolKind> symbolKinds = scope switch
        {
            RenameScope.Type => ImmutableArray.Create(SymbolKind.NamedType),
            RenameScope.Member => ImmutableArray.Create(SymbolKind.Field, SymbolKind.Event, SymbolKind.Method, SymbolKind.Property),
            RenameScope.Local => ImmutableArray.Create(SymbolKind.Field, SymbolKind.Event, SymbolKind.Method, SymbolKind.Property),
            _ => throw new InvalidOperationException(),
        };

        var analyzer = new Analyzer()
        {
            IncludeGeneratedCode = IncludeGeneratedCode,
            FileSystemMatcher = FileSystemMatcher,
        };

        analyzer.SymbolKinds.AddRange(symbolKinds);

        var compilationWithAnalyzers = new CompilationWithAnalyzers(
            compilation,
            ImmutableArray.Create((DiagnosticAnalyzer)analyzer),
            compilationWithAnalyzersOptions);

        ImmutableArray<Diagnostic> _ = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);

        return analyzer.Symbols;
    }

    private class Analyzer : DiagnosticAnalyzer
    {
        [SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking")]
        public static readonly DiagnosticDescriptor DiagnosticDescriptor = new(
            id: "Roslynator.FindSymbols.FindSymbolAnalyzer",
            title: "",
            messageFormat: "",
            category: "FindSymbols",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null,
            customTags: Array.Empty<string>());

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptor);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => _supportedDiagnostics;

        public ConcurrentBag<ISymbol> Symbols { get; } = new();

        public List<SymbolKind> SymbolKinds { get; } = new();

        public bool IncludeGeneratedCode { get; init; }

        public Matcher FileSystemMatcher { get; init; }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.ConfigureGeneratedCodeAnalysis((IncludeGeneratedCode)
                ? GeneratedCodeAnalysisFlags.Analyze
                : GeneratedCodeAnalysisFlags.None);

            context.RegisterSymbolAction(f => AnalyzeSymbol(f), SymbolKinds.ToArray());
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;

            if (symbol.IsImplicitlyDeclared)
                return;

            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.NamedType:
                case SymbolKind.Property:
                    {
                        AddSymbol(symbol);
                        break;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Ordinary:
                            case MethodKind.Constructor:
                            case MethodKind.UserDefinedOperator:
                            case MethodKind.Conversion:
                                {
                                    AddSymbol(methodSymbol);
                                    break;
                                }
                        }

                        break;
                    }
                default:
                    {
                        Debug.Fail(symbol.Kind.ToString());
                        break;
                    }
            }
        }

        private void AddSymbol(ISymbol symbol)
        {
            if (FileSystemMatcher?.IsMatch(symbol) != false)
                Symbols.Add(symbol);
        }
    }
}
