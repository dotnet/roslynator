// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CommandLine
{
    internal sealed class DiagnosticMap
    {
        private ImmutableDictionary<string, IEnumerable<DiagnosticAnalyzer>> _analyzersById;
        private ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;
        private ImmutableDictionary<string, ImmutableArray<DiagnosticDescriptor>> _supportedDiagnosticsByPrefix;

        private ImmutableDictionary<string, IEnumerable<CodeFixProvider>> _fixersById;
        private ImmutableArray<string> _fixableDiagnosticIds;
        private ImmutableDictionary<string, ImmutableArray<string>> _fixableDiagnosticIdsByPrefix;
        private ImmutableArray<FixAllProvider> _fixAllProviders;

        private ImmutableDictionary<string, DiagnosticDescriptor> _diagnosticsById;

        private DiagnosticMap(
            IEnumerable<DiagnosticAnalyzer> analyzers,
            IEnumerable<CodeFixProvider> fixers)
        {
            Analyzers = analyzers.ToImmutableArray();
            Fixers = fixers.ToImmutableArray();
        }

        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        public ImmutableArray<CodeFixProvider> Fixers { get; }

        public ImmutableDictionary<string, IEnumerable<DiagnosticAnalyzer>> AnalyzersById
        {
            get
            {
                if (_analyzersById == null)
                    Interlocked.CompareExchange(ref _analyzersById, LoadAnalyzersById(), null);

                return _analyzersById;
            }
        }

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, LoadSupportedDiagnostics());

                return _supportedDiagnostics;
            }
        }

        public ImmutableDictionary<string, ImmutableArray<DiagnosticDescriptor>> SupportedDiagnosticsByPrefix
        {
            get
            {
                if (_supportedDiagnosticsByPrefix == null)
                    Interlocked.CompareExchange(ref _supportedDiagnosticsByPrefix, LoadSupportedDiagnosticsByPrefix(), null);

                return _supportedDiagnosticsByPrefix;
            }
        }

        public ImmutableDictionary<string, IEnumerable<CodeFixProvider>> FixersById
        {
            get
            {
                if (_fixersById == null)
                    Interlocked.CompareExchange(ref _fixersById, LoadFixersById(), null);

                return _fixersById;
            }
        }

        public ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                if (_fixableDiagnosticIds.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _fixableDiagnosticIds, LoadFixableDiagnosticIds());

                return _fixableDiagnosticIds;
            }
        }

        public ImmutableDictionary<string, ImmutableArray<string>> FixableDiagnosticIdsByPrefix
        {
            get
            {
                if (_fixableDiagnosticIdsByPrefix == null)
                    Interlocked.CompareExchange(ref _fixableDiagnosticIdsByPrefix, LoadFixableDiagnosticIdsByPrefix(), null);

                return _fixableDiagnosticIdsByPrefix;
            }
        }

        public ImmutableDictionary<string, DiagnosticDescriptor> DiagnosticsById
        {
            get
            {
                if (_diagnosticsById == null)
                    Interlocked.CompareExchange(ref _diagnosticsById, LoadDiagnosticsById(), null);

                return _diagnosticsById;
            }
        }

        public ImmutableArray<FixAllProvider> FixAllProviders
        {
            get
            {
                if (_fixAllProviders.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _fixAllProviders, LoadFixAllProviders());

                return _fixAllProviders;
            }
        }

        public static DiagnosticMap Create(IEnumerable<DiagnosticAnalyzer> analyzers, IEnumerable<CodeFixProvider> fixers)
        {
            return new DiagnosticMap(analyzers, fixers);
        }

        public static DiagnosticMap Create(AnalyzerAssembly analyzerAssembly)
        {
            return Create(analyzerAssembly.GetAnalyzers(), analyzerAssembly.GetFixers());
        }

        public static DiagnosticMap Create(IEnumerable<AnalyzerAssembly> analyzerAssemblies)
        {
            return Create(
                analyzerAssemblies.SelectMany(f => f.GetAnalyzers()),
                analyzerAssemblies.SelectMany(f => f.GetFixers()));
        }

        private ImmutableDictionary<string, IEnumerable<DiagnosticAnalyzer>> LoadAnalyzersById()
        {
            return Analyzers
                .SelectMany(analyzer => analyzer.SupportedDiagnostics.Select(descriptor => (analyzer, descriptor)))
                .GroupBy(f => f.descriptor.Id)
                .ToImmutableDictionary(g => g.Key, g => g.Select(f => f.analyzer).Distinct());
        }

        private ImmutableArray<DiagnosticDescriptor> LoadSupportedDiagnostics()
        {
            return Analyzers
                .SelectMany(f => f.SupportedDiagnostics)
                .Distinct(DiagnosticDescriptorComparer.Id)
                .OrderBy(f => f, DiagnosticDescriptorComparer.Id)
                .ToImmutableArray();
        }

        private ImmutableDictionary<string, ImmutableArray<DiagnosticDescriptor>> LoadSupportedDiagnosticsByPrefix()
        {
            return SupportedDiagnostics
                .GroupBy(f => f, DiagnosticDescriptorComparer.IdPrefix)
                .ToImmutableDictionary(f => DiagnosticIdPrefix.GetPrefix(f.Key.Id), f => f.ToImmutableArray());
        }

        private ImmutableDictionary<string, IEnumerable<CodeFixProvider>> LoadFixersById()
        {
            return Fixers
                .SelectMany(fixer => fixer.FixableDiagnosticIds.Select(diagnosticId => (fixer, diagnosticId)))
                .GroupBy(f => f.diagnosticId)
                .ToImmutableDictionary(g => g.Key, g => g.Select(f => f.fixer).Distinct());
        }

        private ImmutableArray<string> LoadFixableDiagnosticIds()
        {
            return Fixers
                .SelectMany(f => f.FixableDiagnosticIds)
                .Distinct()
                .OrderBy(f => f)
                .ToImmutableArray();
        }

        private ImmutableDictionary<string, ImmutableArray<string>> LoadFixableDiagnosticIdsByPrefix()
        {
            return FixableDiagnosticIds
                .GroupBy(f => f, DiagnosticIdComparer.Prefix)
                .ToImmutableDictionary(f => DiagnosticIdPrefix.GetPrefix(f.Key), f => f.ToImmutableArray());
        }

        private ImmutableArray<FixAllProvider> LoadFixAllProviders()
        {
            return Fixers
                .Select(f => f.GetFixAllProvider())
                .Where(f => f != null)
                .Distinct()
                .ToImmutableArray();
        }

        private ImmutableDictionary<string, DiagnosticDescriptor> LoadDiagnosticsById()
        {
            ImmutableDictionary<string, DiagnosticDescriptor>.Builder diagnosticsById = ImmutableDictionary.CreateBuilder<string, DiagnosticDescriptor>();

            diagnosticsById.AddRange(Analyzers
                .SelectMany(f => f.SupportedDiagnostics)
                .Distinct(DiagnosticDescriptorComparer.Id)
                .OrderBy(f => f, DiagnosticDescriptorComparer.Id)
                .Select(f => new KeyValuePair<string, DiagnosticDescriptor>(f.Id, f)));

            foreach (CodeFixProvider fixer in Fixers)
            {
                foreach (string diagnosticId in fixer.FixableDiagnosticIds)
                {
                    if (!diagnosticsById.ContainsKey(diagnosticId))
                        diagnosticsById[diagnosticId] = null;
                }
            }

            return diagnosticsById.ToImmutable();
        }
    }
}
