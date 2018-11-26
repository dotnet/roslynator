// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator
{
    internal sealed class AnalyzerAssemblyList : IEnumerable<AnalyzerAssembly>
    {
        private readonly Dictionary<string, AnalyzerAssembly> _analyzerAssemblies;

        public AnalyzerAssemblyList()
        {
            _analyzerAssemblies = new Dictionary<string, AnalyzerAssembly>();
        }

        public int Count => _analyzerAssemblies.Count;

        internal bool ContainsAssembly(string fullName)
        {
            return _analyzerAssemblies.ContainsKey(fullName);
        }

        internal void LoadFrom(IEnumerable<string> paths, bool loadAnalyzers = true, bool loadFixers = true)
        {
            foreach (string path in paths)
                LoadFrom(path, loadAnalyzers: loadAnalyzers, loadFixers: loadFixers);
        }

        internal void LoadFrom(string path, bool loadAnalyzers = true, bool loadFixers = true)
        {
            foreach ((string filePath, AnalyzerAssembly analyzerAssembly) in AnalyzerAssembly.LoadFrom(path, loadAnalyzers: loadAnalyzers, loadFixers: loadFixers))
            {
                Add(analyzerAssembly);
            }
        }

        public bool Add(AnalyzerAssembly analyzerAssembly)
        {
            if (!_analyzerAssemblies.ContainsKey(analyzerAssembly.FullName))
            {
                AddImpl(analyzerAssembly);
                return true;
            }

            return false;
        }

        private void AddImpl(AnalyzerAssembly analyzerAssembly)
        {
            WriteLine($"Add analyzer assembly '{analyzerAssembly.FullName}'", ConsoleColor.DarkGray, Verbosity.Detailed);

            _analyzerAssemblies.Add(analyzerAssembly.FullName, analyzerAssembly);
        }

        public AnalyzerAssembly GetOrAdd(Assembly assembly)
        {
            if (!_analyzerAssemblies.TryGetValue(assembly.FullName, out AnalyzerAssembly analyzerAssembly))
            {
                analyzerAssembly = AnalyzerAssembly.Load(assembly);
                AddImpl(analyzerAssembly);
                return analyzerAssembly;
            }

            return analyzerAssembly;
        }

        public IEnumerable<DiagnosticAnalyzer> GetAnalyzers(string language)
        {
            return GetAnalyzers(_analyzerAssemblies.Values, language);
        }

        public IEnumerable<DiagnosticAnalyzer> GetOrAddAnalyzers(IEnumerable<Assembly> assemblies, string language)
        {
            return GetAnalyzers(assemblies.Select(GetOrAdd), language);
        }

        private static IEnumerable<DiagnosticAnalyzer> GetAnalyzers(IEnumerable<AnalyzerAssembly> analyzerAssemblies, string language)
        {
            return analyzerAssemblies
                .SelectMany(f => f.Analyzers)
                .Where(f => f.Key == language)
                .SelectMany(f => f.Value);
        }

        public IEnumerable<CodeFixProvider> GetFixers(string language)
        {
            return GetFixers(_analyzerAssemblies.Values, language);
        }

        public IEnumerable<CodeFixProvider> GetOrAddFixers(IEnumerable<Assembly> assemblies, string language)
        {
            return GetFixers(assemblies.Select(GetOrAdd), language);
        }

        private static IEnumerable<CodeFixProvider> GetFixers(IEnumerable<AnalyzerAssembly> analyzerAssemblies, string language)
        {
            return analyzerAssemblies
                .SelectMany(f => f.Fixers)
                .Where(f => f.Key == language)
                .SelectMany(f => f.Value);
        }

        public IEnumerator<AnalyzerAssembly> GetEnumerator()
        {
            return _analyzerAssemblies.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _analyzerAssemblies.Values.GetEnumerator();
        }
    }
}
