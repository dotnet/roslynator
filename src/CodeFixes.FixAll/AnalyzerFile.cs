// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CodeFixes.ConsoleHelpers;

namespace Roslynator.CodeFixes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class AnalyzerFile : IEquatable<AnalyzerFile>
    {
        static AnalyzerFile()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private AnalyzerFile(
            Assembly assembly,
            ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzers,
            ImmutableDictionary<string, ImmutableArray<CodeFixProvider>> fixers)
        {
            Assembly = assembly;
            Analyzers = analyzers;
            Fixers = fixers;
        }

        public Assembly Assembly { get; }

        public ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>> Analyzers { get; }

        public ImmutableDictionary<string, ImmutableArray<CodeFixProvider>> Fixers { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Assembly.FullName;

        public static AnalyzerFile Create(Assembly analyzerAssembly)
        {
            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>.Builder> analyzers = null;
            Dictionary<string, ImmutableArray<CodeFixProvider>.Builder> fixers = null;

            try
            {
                foreach (TypeInfo typeInfo in analyzerAssembly.DefinedTypes)
                {
                    if (typeInfo.IsSubclassOf(typeof(DiagnosticAnalyzer)))
                    {
                        DiagnosticAnalyzerAttribute attribute = typeInfo.GetCustomAttribute<DiagnosticAnalyzerAttribute>();

                        if (attribute != null)
                        {
                            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(typeInfo.AsType());

                            if (analyzers == null)
                                analyzers = new Dictionary<string, ImmutableArray<DiagnosticAnalyzer>.Builder>();

                            foreach (string language in attribute.Languages)
                            {
                                if (!analyzers.TryGetValue(language, out ImmutableArray<DiagnosticAnalyzer>.Builder value))
                                    analyzers[language] = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

                                analyzers[language].Add(analyzer);
                            }
                        }
                    }
                    else if (typeInfo.IsSubclassOf(typeof(CodeFixProvider)))
                    {
                        ExportCodeFixProviderAttribute attribute = typeInfo.GetCustomAttribute<ExportCodeFixProviderAttribute>();

                        if (attribute != null)
                        {
                            var fixer = (CodeFixProvider)Activator.CreateInstance(typeInfo.AsType());

                            if (fixers == null)
                                fixers = new Dictionary<string, ImmutableArray<CodeFixProvider>.Builder>();

                            foreach (string language in attribute.Languages)
                            {
                                if (!fixers.TryGetValue(language, out ImmutableArray<CodeFixProvider>.Builder value))
                                    fixers[language] = ImmutableArray.CreateBuilder<CodeFixProvider>();

                                fixers[language].Add(fixer);
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
#if DEBUG
                    WriteLine($"Cannot load types from assembly '{analyzerAssembly.Location}'", ConsoleColor.Yellow);
#endif
            }

            return new AnalyzerFile(
                analyzerAssembly,
                analyzers?.ToImmutableDictionary(f => f.Key, f => f.Value.ToImmutableArray()) ?? ImmutableDictionary<string, ImmutableArray<DiagnosticAnalyzer>>.Empty,
                fixers?.ToImmutableDictionary(f => f.Key, f => f.Value.ToImmutableArray()) ?? ImmutableDictionary<string, ImmutableArray<CodeFixProvider>>.Empty);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Assembly.FullName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AnalyzerFile);
        }

        public bool Equals(AnalyzerFile other)
        {
            return other != null
                && StringComparer.Ordinal.Equals(Assembly.FullName, other.Assembly.FullName);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);

            switch (assemblyName.Name)
            {
                case "Microsoft.CodeAnalysis":
                case "Microsoft.CodeAnalysis.CSharp":
                case "Microsoft.CodeAnalysis.VisualBasic":
                case "Microsoft.CodeAnalysis.Workspaces":
                case "Microsoft.CodeAnalysis.CSharp.Workspaces":
                case "Microsoft.CodeAnalysis.VisualBasic.Workspaces":
                case "System.Collections.Immutable":
                case "System.Composition.AttributedModel":
                case "System.Composition.Runtime":
                case "System.Composition.TypedParts":
                case "System.Composition.Hosting":
                    {
                        Assembly assembly = FindLoadedAssembly();

                        if (assembly != null)
                            return assembly;

                        break;
                    }
            }

            if (!assemblyName.Name.EndsWith(".resources"))
                throw new InvalidOperationException($"Unable to resolve assembly '{assemblyName.FullName}'.");

            return null;

            Assembly FindLoadedAssembly()
            {
                Assembly result = null;

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    AssemblyName an = assembly.GetName();

                    if (assemblyName.Name == an.Name
                        && assemblyName.Version <= an.Version)
                    {
                        if (result == null)
                        {
                            result = assembly;
                        }
                        else if (result.GetName().Version < an.Version)
                        {
                            result = assembly;
                        }
                    }
                }

                return result;
            }
        }
    }
}
