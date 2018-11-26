// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class AnalyzeAssemblyCommandExecutor
    {
        public AnalyzeAssemblyCommandExecutor(string language = null)
        {
            Language = language;
        }

        public string Language { get; }

        public CommandResult Execute(AnalyzeAssemblyCommandLineOptions options)
        {
            var assemblies = new HashSet<Assembly>();

            foreach ((string filePath, AnalyzerAssembly analyzerAssembly) in options.GetPaths()
                .SelectMany(path => AnalyzerAssembly.LoadFrom(
                    path: path,
                    loadAnalyzers: !options.NoAnalyzers,
                    loadFixers: !options.NoFixers,
                    language: Language))
                .OrderBy(f => f.analyzerAssembly.GetName().Name)
                .ThenBy(f => f.filePath))
            {
                if (assemblies.Add(analyzerAssembly.Assembly))
                {
                    Write($"{analyzerAssembly.FullName}", ConsoleColor.Cyan, Verbosity.Minimal);
                    WriteLine($" [{filePath}]", Verbosity.Minimal);
                }
                else
                {
                    Write($"{analyzerAssembly.FullName}", ConsoleColor.DarkGray, Verbosity.Minimal);
                    WriteLine($" [{filePath}]", ConsoleColor.DarkGray, Verbosity.Minimal);
                    continue;
                }

                DiagnosticAnalyzer[] analyzers = analyzerAssembly
                    .Analyzers
                    .SelectMany(f => f.Value)
                    .Distinct()
                    .ToArray();

                if (analyzers.Length > 0)
                {
                    Write($"  {analyzers.Length} DiagnosticAnalyzers (", Verbosity.Normal);

                    using (IEnumerator<KeyValuePair<string, ImmutableArray<DiagnosticAnalyzer>>> en = analyzerAssembly.Analyzers.OrderBy(f => f.Key).GetEnumerator())
                    {
                        if (en.MoveNext())
                        {
                            while (true)
                            {
                                Write($"{en.Current.Value.Length} {Utilities.GetShortLanguageName(en.Current.Key)}", Verbosity.Normal);

                                if (en.MoveNext())
                                {
                                    Write(", ");
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    WriteLine(")", Verbosity.Normal);

                    foreach (DiagnosticAnalyzer analyzer in analyzers.OrderBy(f => f.GetType().FullName))
                    {
                        Type type = analyzer.GetType();

                        DiagnosticAnalyzerAttribute attribute = type.GetCustomAttribute<DiagnosticAnalyzerAttribute>();

                        WriteLine($"    {type.FullName}", Verbosity.Detailed);
                        WriteLine($"      Supported Languages:   {string.Join(", ", attribute.Languages.Select(f => Utilities.GetShortLanguageName(f)).OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Detailed);
                        WriteLine($"      Supported Diagnostics: {string.Join(", ", analyzer.SupportedDiagnostics.Select(f => f.Id).OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Detailed);
                    }
                }

                CodeFixProvider[] fixers = analyzerAssembly
                    .Fixers
                    .SelectMany(f => f.Value)
                    .Distinct()
                    .ToArray();

                if (fixers.Length > 0)
                {
                    Write($"  {fixers.Length} CodeFixProviders (", Verbosity.Normal);

                    using (IEnumerator<KeyValuePair<string, ImmutableArray<CodeFixProvider>>> en = analyzerAssembly.Fixers.OrderBy(f => f.Key).GetEnumerator())
                    {
                        if (en.MoveNext())
                        {
                            while (true)
                            {
                                Write($"{en.Current.Value.Length} {Utilities.GetShortLanguageName(en.Current.Key)}", Verbosity.Normal);

                                if (en.MoveNext())
                                {
                                    Write(", ");
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    WriteLine(")", Verbosity.Normal);

                    foreach (CodeFixProvider fixer in fixers.OrderBy(f => f.GetType().FullName))
                    {
                        Type type = fixer.GetType();

                        ExportCodeFixProviderAttribute attribute = type.GetCustomAttribute<ExportCodeFixProviderAttribute>();

                        WriteLine($"    {type.FullName}", Verbosity.Detailed);
                        WriteLine($"      Supported Languages: {string.Join(", ", attribute.Languages.Select(f => Utilities.GetShortLanguageName(f)).OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Detailed);
                        WriteLine($"      Fixable Diagnostics: {string.Join(", ", fixer.FixableDiagnosticIds.OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Detailed);

                        Write("      FixAllProvider:      ", ConsoleColor.DarkGray, Verbosity.Detailed);

                        FixAllProvider fixAllProvider = fixer.GetFixAllProvider();

                        if (fixAllProvider != null)
                        {
                            WriteLine($"{fixAllProvider.GetType().FullName} ({string.Join(", ", fixAllProvider.GetSupportedFixAllScopes().Select(f => f.ToString()).OrderBy(f => f))})", ConsoleColor.DarkGray, Verbosity.Detailed);
                        }
                        else
                        {
                            WriteLine("-", ConsoleColor.DarkGray, Verbosity.Detailed);
                        }
                    }
                }
            }

            WriteLine(Verbosity.Minimal);
            WriteLine($"{assemblies.Count} analyzer {((assemblies.Count == 1) ? "assembly" : "assemblies")} found", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);

            return CommandResult.Success;
        }
    }
}
