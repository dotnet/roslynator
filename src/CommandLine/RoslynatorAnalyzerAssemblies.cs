// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace Roslynator.CommandLine
{
    internal static class RoslynatorAnalyzerAssemblies
    {
        public static AnalyzerAssembly Analyzers { get; } = LoadAnalyzerAssembly("Roslynator.CSharp.Analyzers.dll");

        public static AnalyzerAssembly CodeFixes { get; } = LoadAnalyzerAssembly("Roslynator.CSharp.Analyzers.CodeFixes.dll");

        public static ImmutableArray<AnalyzerAssembly> AnalyzersAndCodeFixes { get; } = ImmutableArray.Create(Analyzers, CodeFixes);

        private static AnalyzerAssembly LoadAnalyzerAssembly(string assemblyName)
        {
            string filePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                assemblyName);

            return AnalyzerAssemblyLoader.LoadFile(filePath);
        }
    }
}
