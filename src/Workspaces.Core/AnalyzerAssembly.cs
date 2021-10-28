// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal sealed class AnalyzerAssembly : IEquatable<AnalyzerAssembly>
    {
        private AnalyzerAssembly(
            Assembly assembly,
            ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzersByLanguage,
            ImmutableDictionary<string, ImmutableArray<CodeFixProvider>> fixersByLanguage)
        {
            Assembly = assembly;

            AnalyzersByLanguage = analyzersByLanguage;
            FixersByLanguage = fixersByLanguage;
        }

        public Assembly Assembly { get; }

        internal string FullName => Assembly.FullName;

        internal string Name => Assembly.GetName().Name;

        public bool HasAnalyzers => AnalyzersByLanguage.Count > 0;

        public bool HasFixers => FixersByLanguage.Count > 0;

        internal bool IsEmpty => !HasAnalyzers && !HasFixers;

        public ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>> AnalyzersByLanguage { get; }

        public ImmutableDictionary<string, ImmutableArray<CodeFixProvider>> FixersByLanguage { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => FullName;

        internal IEnumerable<DiagnosticAnalyzer> GetAnalyzers()
        {
            return AnalyzersByLanguage
                .SelectMany(f => f.Value)
                .Distinct();
        }

        internal IEnumerable<CodeFixProvider> GetFixers()
        {
            return FixersByLanguage
                .SelectMany(f => f.Value)
                .Distinct();
        }

        public static AnalyzerAssembly Load(
            Assembly analyzerAssembly,
            bool loadAnalyzers = true,
            bool loadFixers = true,
            string language = null)
        {
            Debug.Assert(loadAnalyzers || loadFixers);

            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>.Builder> analyzers = null;
            Dictionary<string, ImmutableArray<CodeFixProvider>.Builder> fixers = null;

            TypeInfo[] types = null;
            try
            {
                types = analyzerAssembly.DefinedTypes.ToArray();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.OfType<TypeInfo>().ToArray();

                int count = ex.Types.Count(f => f == null);
                string message = $"Cannot load {count} type{((count == 1) ? "" : "s")} from assembly '{analyzerAssembly.FullName}'";

                if (!string.IsNullOrEmpty(analyzerAssembly.Location))
                    message += $" at '{analyzerAssembly.Location}'";

                WriteLine(message, ConsoleColors.DarkGray, Verbosity.Diagnostic);

                foreach (Exception loadeException in ex.LoaderExceptions)
                    WriteLine($"  {loadeException.Message}", ConsoleColors.DarkGray, Verbosity.Diagnostic);
            }

            foreach (TypeInfo typeInfo in types)
            {
                if (loadAnalyzers
                    && !typeInfo.IsAbstract
                    && typeInfo.IsSubclassOf(typeof(DiagnosticAnalyzer)))
                {
                    DiagnosticAnalyzerAttribute attribute = typeInfo.GetCustomAttribute<DiagnosticAnalyzerAttribute>();

                    if (attribute != null)
                    {
                        DiagnosticAnalyzer analyzer = CreateInstanceAndCatchIfThrows<DiagnosticAnalyzer>(typeInfo);

                        if (analyzer != null)
                        {
                            if (analyzers == null)
                                analyzers = new Dictionary<string, ImmutableArray<DiagnosticAnalyzer>.Builder>();

                            foreach (string language2 in attribute.Languages)
                            {
                                if (language == null
                                    || language == language2)
                                {
                                    if (!analyzers.TryGetValue(language2, out ImmutableArray<DiagnosticAnalyzer>.Builder value))
                                        analyzers[language2] = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

                                    analyzers[language2].Add(analyzer);
                                }
                            }
                        }
                    }
                }
                else if (loadFixers
                    && !typeInfo.IsAbstract
                    && typeInfo.IsSubclassOf(typeof(CodeFixProvider)))
                {
                    ExportCodeFixProviderAttribute attribute = typeInfo.GetCustomAttribute<ExportCodeFixProviderAttribute>();

                    if (attribute != null)
                    {
                        CodeFixProvider fixer = CreateInstanceAndCatchIfThrows<CodeFixProvider>(typeInfo);

                        if (fixer != null)
                        {
                            if (fixers == null)
                                fixers = new Dictionary<string, ImmutableArray<CodeFixProvider>.Builder>();

                            foreach (string language2 in attribute.Languages)
                            {
                                if (language == null
                                    || language == language2)
                                {
                                    if (!fixers.TryGetValue(language2, out ImmutableArray<CodeFixProvider>.Builder value))
                                        fixers[language2] = ImmutableArray.CreateBuilder<CodeFixProvider>();

                                    fixers[language2].Add(fixer);
                                }
                            }
                        }
                    }
                }
            }

            return new AnalyzerAssembly(
                analyzerAssembly,
                analyzers?.ToImmutableDictionary(f => f.Key, f => f.Value.ToImmutableArray()) ?? ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>>.Empty,
                fixers?.ToImmutableDictionary(f => f.Key, f => f.Value.ToImmutableArray()) ?? ImmutableDictionary<string, ImmutableArray<CodeFixProvider>>.Empty);
        }

        private static T CreateInstanceAndCatchIfThrows<T>(TypeInfo typeInfo)
        {
            try
            {
                return (T)Activator.CreateInstance(typeInfo.AsType());
            }
            catch (TargetInvocationException ex)
            {
                WriteLine($"Cannot create instance of type '{typeInfo.FullName}'", ConsoleColors.DarkGray, Verbosity.Diagnostic);
                WriteLine(ex.ToString(), ConsoleColors.DarkGray, Verbosity.Diagnostic);
            }

            return default;
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(FullName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AnalyzerAssembly);
        }

        public bool Equals(AnalyzerAssembly other)
        {
            return other != null
                && StringComparer.Ordinal.Equals(FullName, other.FullName);
        }
    }
}
