// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CodeFixes.ConsoleHelpers;

namespace Roslynator.CodeFixes
{
    internal sealed class AnalyzerFileCache
    {
        private readonly Dictionary<string, AnalyzerFile> _analyzerFiles;

        public AnalyzerFileCache()
        {
            _analyzerFiles = new Dictionary<string, AnalyzerFile>();
        }

        internal bool Contains(string fullName)
        {
            return _analyzerFiles.ContainsKey(fullName);
        }

        internal void LoadFrom(IEnumerable<string> paths)
        {
            foreach (string path in paths)
                LoadFrom(path);
        }

        internal void LoadFrom(string path)
        {
            if (File.Exists(path))
            {
                AddAnalyzerFileIfNotEmpty(path);
            }
            else if (Directory.Exists(path))
            {
                using (IEnumerator<string> en = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).GetEnumerator())
                {
                    while (true)
                    {
                        try
                        {
                            if (en.MoveNext())
                            {
                                AddAnalyzerFileIfNotEmpty(en.Current);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (IOException)
                        {
                        }
                        catch (SecurityException)
                        {
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }
                }
            }

            void AddAnalyzerFileIfNotEmpty(string filePath)
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.LoadFrom(filePath);
                }
                catch (Exception ex)
                {
                    if (ex is FileLoadException
                        || ex is BadImageFormatException
                        || ex is SecurityException)
                    {
#if DEBUG
                        WriteLine($"Cannot load assembly '{filePath}'", ConsoleColor.Yellow);
#endif
                        return;
                    }
                    else
                    {
                        throw;
                    }
                }

                AnalyzerFile analyzerFile = AnalyzerFile.Create(assembly);

                if (analyzerFile.Analyzers.Count > 0
                    || analyzerFile.Fixers.Count > 0)
                {
                    Add(analyzerFile);
                }
            }
        }

        public bool Add(AnalyzerFile analyzerFile)
        {
            if (!_analyzerFiles.ContainsKey(analyzerFile.Assembly.FullName))
            {
                AddImpl(analyzerFile);
                return true;
            }

            return false;
        }

        private void AddImpl(AnalyzerFile analyzerFile)
        {
            WriteLine($"Add analyzer assembly '{analyzerFile.Assembly.FullName}'");

            _analyzerFiles.Add(analyzerFile.Assembly.FullName, analyzerFile);
        }

        public AnalyzerFile GetOrAdd(Assembly assembly)
        {
            if (!_analyzerFiles.TryGetValue(assembly.FullName, out AnalyzerFile analyzerFile))
            {
                analyzerFile = AnalyzerFile.Create(assembly);
                AddImpl(analyzerFile);
                return analyzerFile;
            }

            return analyzerFile;
        }

        public ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(string language)
        {
            return GetAnalyzers(_analyzerFiles.Values, language);
        }

        public ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(IEnumerable<Assembly> assemblies, string language)
        {
            return GetAnalyzers(assemblies.Select(GetOrAdd), language);
        }

        private static ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(IEnumerable<AnalyzerFile> analyzerFiles, string language)
        {
            return analyzerFiles
                .SelectMany(f => f.Analyzers)
                .Where(f => f.Key == language)
                .SelectMany(f => f.Value)
                .ToImmutableArray();
        }

        public ImmutableArray<CodeFixProvider> GetFixers(string language)
        {
            return GetFixers(_analyzerFiles.Values, language);
        }

        public ImmutableArray<CodeFixProvider> GetFixers(IEnumerable<Assembly> assemblies, string language)
        {
            return GetFixers(assemblies.Select(GetOrAdd), language);
        }

        public static ImmutableArray<CodeFixProvider> GetFixers(IEnumerable<AnalyzerFile> analyzerFiles, string language)
        {
            return analyzerFiles
                .SelectMany(f => f.Fixers)
                .Where(f => f.Key == language)
                .SelectMany(f => f.Value)
                .ToImmutableArray();
        }
    }
}
