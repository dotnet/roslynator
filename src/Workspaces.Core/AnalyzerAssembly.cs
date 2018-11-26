// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class AnalyzerAssembly : IEquatable<AnalyzerAssembly>
    {
        private AnalyzerAssembly(
            Assembly assembly,
            ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzers,
            ImmutableDictionary<string, ImmutableArray<CodeFixProvider>> fixers)
        {
            Assembly = assembly;
            Analyzers = analyzers;
            Fixers = fixers;
        }

        public Assembly Assembly { get; }

        public string FullName => Assembly.FullName;

        public string Location => Assembly.Location;

        public ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>> Analyzers { get; }

        public ImmutableDictionary<string, ImmutableArray<CodeFixProvider>> Fixers { get; }

        internal bool IsEmpty => Analyzers.Count == 0 && Fixers.Count == 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => FullName;

        public AssemblyName GetName() => Assembly.GetName();

        public static AnalyzerAssembly Load(
            Assembly analyzerAssembly,
            bool loadAnalyzers = true,
            bool loadFixers = true,
            string language = null)
        {
            Debug.Assert(loadAnalyzers || loadFixers);

            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>.Builder> analyzers = null;
            Dictionary<string, ImmutableArray<CodeFixProvider>.Builder> fixers = null;

            try
            {
                foreach (TypeInfo typeInfo in analyzerAssembly.DefinedTypes)
                {
                    if (loadAnalyzers
                        && !typeInfo.IsAbstract
                        && typeInfo.IsSubclassOf(typeof(DiagnosticAnalyzer)))
                    {
                        DiagnosticAnalyzerAttribute attribute = typeInfo.GetCustomAttribute<DiagnosticAnalyzerAttribute>();

                        if (attribute != null)
                        {
                            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(typeInfo.AsType());

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
                    else if (loadFixers
                        && !typeInfo.IsAbstract
                        && typeInfo.IsSubclassOf(typeof(CodeFixProvider)))
                    {
                        ExportCodeFixProviderAttribute attribute = typeInfo.GetCustomAttribute<ExportCodeFixProviderAttribute>();

                        if (attribute != null)
                        {
                            var fixer = (CodeFixProvider)Activator.CreateInstance(typeInfo.AsType());

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
            catch (ReflectionTypeLoadException)
            {
                WriteLine($"Cannot load types from assembly '{analyzerAssembly.Location}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);
            }

            return new AnalyzerAssembly(
                analyzerAssembly,
                analyzers?.ToImmutableDictionary(f => f.Key, f => f.Value.ToImmutableArray()) ?? ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>>.Empty,
                fixers?.ToImmutableDictionary(f => f.Key, f => f.Value.ToImmutableArray()) ?? ImmutableDictionary<string, ImmutableArray<CodeFixProvider>>.Empty);
        }

        public static AnalyzerAssembly LoadFile(
            string filePath,
            bool loadAnalyzers = true,
            bool loadFixers = true,
            string language = null)
        {
            Assembly assembly = Assembly.LoadFrom(filePath);

            return Load(assembly, loadAnalyzers: loadAnalyzers, loadFixers: loadFixers, language: language);
        }

        public static IEnumerable<(string filePath, AnalyzerAssembly analyzerAssembly)> LoadFrom(
            string path,
            bool loadAnalyzers = true,
            bool loadFixers = true,
            string language = null)
        {
            if (File.Exists(path))
            {
                AnalyzerAssembly analyzerAssembly = Load(path);

                if (analyzerAssembly?.IsEmpty == false)
                    yield return (path, analyzerAssembly);
            }
            else if (Directory.Exists(path))
            {
                using (IEnumerator<string> en = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).GetEnumerator())
                {
                    while (true)
                    {
                        string filePath = null;
                        AnalyzerAssembly analyzerAssembly = null;

                        try
                        {
                            if (en.MoveNext())
                            {
                                filePath = en.Current;
                                analyzerAssembly = Load(filePath);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (IOException)
                        {
                            continue;
                        }
                        catch (SecurityException)
                        {
                            continue;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            continue;
                        }

                        if (analyzerAssembly?.IsEmpty == false)
                            yield return (filePath, analyzerAssembly);
                    }
                }
            }
            else
            {
                WriteLine($"File or directory not found: '{path}'", ConsoleColor.DarkGray, Verbosity.Normal);
            }

            AnalyzerAssembly Load(string filePath)
            {
                try
                {
                    return LoadFile(filePath, loadAnalyzers, loadFixers, language);
                }
                catch (Exception ex)
                {
                    if (ex is FileLoadException
                        || ex is BadImageFormatException
                        || ex is SecurityException)
                    {
                        WriteLine($"Cannot load assembly '{filePath}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
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
