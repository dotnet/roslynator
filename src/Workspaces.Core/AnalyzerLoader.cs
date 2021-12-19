// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal class AnalyzerLoader
    {
        private readonly Dictionary<string, AnalyzerAssembly> _cache = new();
        private readonly Dictionary<string, AnalyzerAssembly> _defaultAssemblies = new();
        private readonly Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> _defaultAnalyzers = new();
        private readonly Dictionary<string, ImmutableArray<CodeFixProvider>> _defaultFixers = new();

        public AnalyzerLoader(IEnumerable<AnalyzerAssembly> defaultAssemblies, CodeAnalysisOptions options)
        {
            DefaultAssemblies = ImmutableArray<AnalyzerAssembly>.Empty;

            if (defaultAssemblies != null)
            {
                foreach (AnalyzerAssembly analyzerAssembly in defaultAssemblies)
                {
                    if (!_defaultAssemblies.ContainsKey(analyzerAssembly.FullName))
                    {
                        _defaultAssemblies.Add(analyzerAssembly.FullName, analyzerAssembly);
                        OnAnalyzerAssemblyAdded(new AnalyzerAssemblyEventArgs(analyzerAssembly));
                    }
                }

                DefaultAssemblies = _defaultAssemblies.Select(f => f.Value).ToImmutableArray();
            }

            Options = options;
        }

        public ImmutableArray<AnalyzerAssembly> DefaultAssemblies { get; }

        public CodeAnalysisOptions Options { get; }

        public event EventHandler<AnalyzerAssemblyEventArgs> AnalyzerAssemblyAdded;

        protected virtual void OnAnalyzerAssemblyAdded(AnalyzerAssemblyEventArgs e)
        {
            AnalyzerAssemblyAdded?.Invoke(this, e);
        }

        public ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(Project project)
        {
            (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> _) = GetAnalyzersAndFixers(project: project, loadFixers: false);

            return analyzers;
        }

        public (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(Project project)
        {
            return GetAnalyzersAndFixers(project: project, loadFixers: true);
        }

        private (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(
            Project project,
            bool loadFixers = true)
        {
            string language = project.Language;

            ImmutableArray<DiagnosticAnalyzer>.Builder analyzers = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();
            ImmutableArray<CodeFixProvider>.Builder fixers = ImmutableArray.CreateBuilder<CodeFixProvider>();

            if (_defaultAnalyzers.TryGetValue(language, out ImmutableArray<DiagnosticAnalyzer> defaultAnalyzers))
            {
                analyzers.AddRange(defaultAnalyzers);
            }
            else
            {
                foreach (AnalyzerAssembly analyzerAssembly in DefaultAssemblies)
                    LoadAnalyzers(analyzerAssembly, language, ref analyzers);

                _defaultAnalyzers.Add(language, analyzers.ToImmutableArray());
            }

            if (loadFixers)
            {
                if (_defaultFixers.TryGetValue(language, out ImmutableArray<CodeFixProvider> defaultFixers))
                {
                    fixers.AddRange(defaultFixers);
                }
                else
                {
                    foreach (AnalyzerAssembly analyzerAssembly in DefaultAssemblies)
                        LoadFixers(analyzerAssembly, language, ref fixers);

                    _defaultFixers.Add(language, fixers.ToImmutableArray());
                }
            }

            if (!Options.IgnoreAnalyzerReferences)
            {
                foreach (Assembly assembly in project.AnalyzerReferences
                    .Distinct()
                    .OfType<AnalyzerFileReference>()
                    .Select(f => f.GetAssembly())
                    .Where(f => !_defaultAssemblies.ContainsKey(f.FullName)))
                {
                    if (!_cache.TryGetValue(assembly.FullName, out AnalyzerAssembly analyzerAssembly))
                    {
                        analyzerAssembly = AnalyzerAssembly.Load(assembly);
                        _cache.Add(analyzerAssembly.FullName, analyzerAssembly);

                        OnAnalyzerAssemblyAdded(new AnalyzerAssemblyEventArgs(analyzerAssembly));
                    }

                    LoadAnalyzers(analyzerAssembly, language, ref analyzers);

                    if (loadFixers)
                        LoadFixers(analyzerAssembly, language, ref fixers);
                }
            }

            return (analyzers.ToImmutableArray(), fixers.ToImmutableArray());
        }

        private void LoadAnalyzers(AnalyzerAssembly analyzerAssembly, string language, ref ImmutableArray<DiagnosticAnalyzer>.Builder builder)
        {
            if (analyzerAssembly.AnalyzersByLanguage.TryGetValue(language, out ImmutableArray<DiagnosticAnalyzer> analyzers))
            {
                foreach (DiagnosticAnalyzer analyzer in analyzers)
                {
                    if (ShouldIncludeAnalyzer(analyzer))
                        builder.Add(analyzer);
                }
            }
        }

        private bool ShouldIncludeAnalyzer(DiagnosticAnalyzer analyzer)
        {
            if (Options.SupportedDiagnosticIds.Count > 0)
            {
                foreach (DiagnosticDescriptor supportedDiagnostic in analyzer.SupportedDiagnostics)
                {
                    if (Options.SupportedDiagnosticIds.Contains(supportedDiagnostic.Id))
                        return true;
                }

                return false;
            }
            else if (Options.IgnoredDiagnosticIds.Count > 0)
            {
                foreach (DiagnosticDescriptor supportedDiagnostic in analyzer.SupportedDiagnostics)
                {
                    if (!Options.IgnoredDiagnosticIds.Contains(supportedDiagnostic.Id))
                        return true;
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        private void LoadFixers(AnalyzerAssembly analyzerAssembly, string language, ref ImmutableArray<CodeFixProvider>.Builder builder)
        {
            if (analyzerAssembly.FixersByLanguage.TryGetValue(language, out ImmutableArray<CodeFixProvider> fixers))
            {
                foreach (CodeFixProvider fixer in fixers)
                {
                    if (ShouldIncludeFixer(fixer))
                        builder.Add(fixer);
                }
            }
        }

        private bool ShouldIncludeFixer(CodeFixProvider fixer)
        {
            if (Options.SupportedDiagnosticIds.Count > 0)
            {
                foreach (string fixableDiagnosticId in fixer.FixableDiagnosticIds)
                {
                    if (Options.SupportedDiagnosticIds.Contains(fixableDiagnosticId))
                        return true;
                }

                return false;
            }
            else if (Options.IgnoredDiagnosticIds.Count > 0)
            {
                foreach (string fixableDiagnosticId in fixer.FixableDiagnosticIds)
                {
                    if (!Options.IgnoredDiagnosticIds.Contains(fixableDiagnosticId))
                        return true;
                }

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
