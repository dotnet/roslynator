// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeGeneration.CSharp;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
#if DEBUG
                args = new string[] { @"..\..\..\..\.." };
#else
                args = new string[] { Environment.CurrentDirectory };
#endif
            }

            string rootPath = args[0];

            StringComparer comparer = StringComparer.InvariantCulture;

            var metadata = new RoslynatorMetadata(rootPath);

            ImmutableArray<AnalyzerMetadata> analyzers = metadata.Analyzers;
            ImmutableArray<AnalyzerMetadata> codeAnalysisAnalyzers = metadata.CodeAnalysisAnalyzers;
            ImmutableArray<AnalyzerMetadata> formattingAnalyzers = metadata.FormattingAnalyzers;
            ImmutableArray<RefactoringMetadata> refactorings = metadata.Refactorings;
            ImmutableArray<CodeFixMetadata> codeFixes = metadata.CodeFixes;
            ImmutableArray<CompilerDiagnosticMetadata> compilerDiagnostics = metadata.CompilerDiagnostics;

            WriteCompilationUnit(
                @"Refactorings\CSharp\RefactoringIdentifiers.Generated.cs",
                RefactoringIdentifiersGenerator.Generate(refactorings, obsolete: false, comparer: comparer));

            WriteCompilationUnit(
                @"Refactorings\CSharp\RefactoringIdentifiers.Deprecated.Generated.cs",
                RefactoringIdentifiersGenerator.Generate(refactorings, obsolete: true, comparer: comparer));

            WriteCompilationUnit(
                @"VisualStudio.Common\RefactoringsOptionsPage.Generated.cs",
                RefactoringsOptionsPageGenerator.Generate(refactorings.Where(f => !f.IsObsolete), comparer));

            WriteDiagnostics(@"Analyzers\CSharp", analyzers, @namespace: "Roslynator.CSharp");

            WriteDiagnostics(@"CodeAnalysis.Analyzers\CSharp", codeAnalysisAnalyzers, @namespace: "Roslynator.CodeAnalysis.CSharp");

            WriteDiagnostics(@"Formatting.Analyzers\CSharp", formattingAnalyzers, @namespace: "Roslynator.Formatting.CSharp");

            WriteCompilationUnit(
                @"CodeFixes\CSharp\CompilerDiagnosticDescriptors.Generated.cs",
                CompilerDiagnosticDescriptorsGenerator.Generate(compilerDiagnostics, comparer: comparer, @namespace: "Roslynator.CSharp"),
                normalizeWhitespace: false);

            WriteCompilationUnit(
                @"CodeFixes\CSharp\CodeFixDescriptors.Generated.cs",
                CodeFixDescriptorsGenerator.Generate(codeFixes.Where(f => !f.IsObsolete), comparer: comparer, @namespace: "Roslynator.CSharp"),
                normalizeWhitespace: false);

            WriteCompilationUnit(
                @"CodeFixes\CSharp\CodeFixIdentifiers.Generated.cs",
                CodeFixIdentifiersGenerator.Generate(codeFixes, comparer));

            WriteCompilationUnit(
                @"VisualStudio.Common\CodeFixesOptionsPage.Generated.cs",
                CodeFixesOptionsPageGenerator.Generate(codeFixes, comparer));

            WriteCompilationUnit(
                @"CSharp\CSharp\CompilerDiagnosticIdentifiers.Generated.cs",
                CompilerDiagnosticIdentifiersGenerator.Generate(compilerDiagnostics, comparer));

            WriteCompilationUnit(
                @"Tools\CodeGeneration\CSharp\Symbols.Generated.cs",
                SymbolsGetKindsGenerator.Generate());

            WriteCompilationUnit(
                @"CSharp\CSharp\SyntaxWalkers\CSharpSyntaxNodeWalker.cs",
                CSharpSyntaxNodeWalkerGenerator.Generate());

            Console.WriteLine($"number of analyzers: {analyzers.Count(f => !f.IsObsolete)}");
            Console.WriteLine($"number of code analysis analyzers: {codeAnalysisAnalyzers.Count(f => !f.IsObsolete)}");
            Console.WriteLine($"number of formatting analyzers: {formattingAnalyzers.Count(f => !f.IsObsolete)}");
            Console.WriteLine($"number of refactorings: {refactorings.Length}");
            Console.WriteLine($"number of code fixes: {codeFixes.Length}");
            Console.WriteLine($"number of fixable compiler diagnostics: {codeFixes.SelectMany(f => f.FixableDiagnosticIds).Distinct().Count()}");

            void WriteDiagnostics(string dirPath, ImmutableArray<AnalyzerMetadata> analyzers2, string @namespace)
            {
                WriteCompilationUnit(
                    Path.Combine(dirPath, "DiagnosticDescriptors.Generated.cs"),
                    DiagnosticDescriptorsGenerator.Generate(analyzers2, obsolete: false, comparer: comparer, @namespace: @namespace),
                    normalizeWhitespace: false);

                WriteCompilationUnit(
                    Path.Combine(dirPath, "DiagnosticDescriptors.Deprecated.Generated.cs"),
                    DiagnosticDescriptorsGenerator.Generate(analyzers2, obsolete: true, comparer: comparer, @namespace: @namespace),
                    normalizeWhitespace: false);

                WriteCompilationUnit(
                    Path.Combine(dirPath, "DiagnosticIdentifiers.Generated.cs"),
                    DiagnosticIdentifiersGenerator.Generate(analyzers2, obsolete: false, comparer: comparer, @namespace: @namespace));

                WriteCompilationUnit(
                    Path.Combine(dirPath, "DiagnosticIdentifiers.Deprecated.Generated.cs"),
                    DiagnosticIdentifiersGenerator.Generate(analyzers2, obsolete: true, comparer: comparer, @namespace: @namespace));
            }

            void WriteCompilationUnit(
                string path,
                CompilationUnitSyntax compilationUnit,
                bool autoGenerated = true,
                bool normalizeWhitespace = true,
                bool fileMustExist = true,
                bool overwrite = true)
            {
                CodeGenerationHelpers.WriteCompilationUnit(
                    path: Path.Combine(rootPath, path),
                    compilationUnit: compilationUnit,
                    banner: "Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
                    autoGenerated: autoGenerated,
                    normalizeWhitespace: normalizeWhitespace,
                    fileMustExist: fileMustExist,
                    overwrite: overwrite);
            }
        }
    }
}
