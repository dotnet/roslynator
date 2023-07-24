// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeGeneration;
using Roslynator.CodeGeneration.CSharp;
using Roslynator.CodeGeneration.EditorConfig;
using Roslynator.Configuration;
using Roslynator.Metadata;

namespace Roslynator.CodeGenerator;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args is null || args.Length == 0)
        {
            args = new string[] { Environment.CurrentDirectory };
        }

        string rootPath = args[0];

        StringComparer comparer = StringComparer.InvariantCulture;

        var metadata = new RoslynatorMetadata(rootPath);

        ImmutableArray<RefactoringMetadata> refactorings = metadata.Refactorings;
        ImmutableArray<CodeFixMetadata> codeFixes = metadata.CodeFixes;
        ImmutableArray<CompilerDiagnosticMetadata> compilerDiagnostics = metadata.CompilerDiagnostics;
        ImmutableArray<ConfigOptionMetadata> options = metadata.ConfigOptions;

        WriteCompilationUnit(
            @"Refactorings\CSharp\RefactoringDescriptors.Generated.cs",
            RefactoringDescriptorsGenerator.Generate(refactorings.Where(f => !f.IsObsolete), comparer: comparer));

        WriteCompilationUnit(
            @"Refactorings\CSharp\RefactoringIdentifiers.Generated.cs",
            RefactoringIdentifiersGenerator.Generate(refactorings, obsolete: false, comparer: comparer));

        WriteCompilationUnit(
            @"Refactorings\CSharp\RefactoringIdentifiers.Deprecated.Generated.cs",
            RefactoringIdentifiersGenerator.Generate(refactorings, obsolete: true, comparer: comparer));

        WriteCompilationUnit(
            @"VisualStudio\RefactoringsOptionsPage.Generated.cs",
            RefactoringsOptionsPageGenerator.Generate(refactorings.Where(f => !f.IsObsolete), comparer));

        WriteDiagnostics(@"Analyzers\CSharp", metadata.CommonAnalyzers, @namespace: "Roslynator.CSharp", categoryName: nameof(DiagnosticCategories.Roslynator));

        WriteDiagnostics(@"CodeAnalysis.Analyzers\CSharp", metadata.CodeAnalysisAnalyzers, @namespace: "Roslynator.CodeAnalysis.CSharp", categoryName: nameof(DiagnosticCategories.Roslynator));

        WriteDiagnostics(@"Formatting.Analyzers\CSharp", metadata.FormattingAnalyzers, @namespace: "Roslynator.Formatting.CSharp", categoryName: nameof(DiagnosticCategories.Roslynator));

        WriteCompilationUnit(
            @"CodeFixes\CSharp\CompilerDiagnosticRules.Generated.cs",
            CompilerDiagnosticRulesGenerator.Generate(compilerDiagnostics, comparer: comparer, @namespace: "Roslynator.CSharp"),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            @"CodeFixes\CSharp\CodeFixDescriptors.Generated.cs",
            CodeFixDescriptorsGenerator.Generate(codeFixes.Where(f => !f.IsObsolete), comparer: comparer, @namespace: "Roslynator.CSharp"),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            @"CodeFixes\CSharp\CodeFixIdentifiers.Generated.cs",
            CodeFixIdentifiersGenerator.Generate(codeFixes, comparer));

        WriteCompilationUnit(
            @"VisualStudio\CodeFixesOptionsPage.Generated.cs",
            CodeFixesOptionsPageGenerator.Generate());

        WriteCompilationUnit(
            @"CSharp\CSharp\CompilerDiagnosticIdentifiers.Generated.cs",
            CompilerDiagnosticIdentifiersGenerator.Generate(compilerDiagnostics, comparer));

        WriteCompilationUnit(
            @"Common\ConfigOptions.Generated.cs",
            Roslynator.CodeGeneration.CSharp.CodeGenerator.GenerateConfigOptions(options, metadata.Analyzers),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            @"Common\LegacyConfigOptions.Generated.cs",
            Roslynator.CodeGeneration.CSharp.CodeGenerator.GenerateLegacyConfigOptions(metadata.Analyzers));

        WriteCompilationUnit(
            @"Common\ConfigOptionKeys.Generated.cs",
            Roslynator.CodeGeneration.CSharp.CodeGenerator.GenerateConfigOptionKeys(options),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            @"Common\ConfigOptionValues.Generated.cs",
            Roslynator.CodeGeneration.CSharp.CodeGenerator.GenerateConfigOptionValues(options),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            @"Tools\CodeGeneration\CSharp\Symbols.Generated.cs",
            SymbolsGetKindsGenerator.Generate());

        WriteCompilationUnit(
            @"CSharp\CSharp\SyntaxWalkers\CSharpSyntaxNodeWalker.cs",
            CSharpSyntaxNodeWalkerGenerator.Generate());

        File.WriteAllText(
            Path.Combine(rootPath, @"VisualStudioCode\package\src\configurationFiles.generated.ts"),
            @"export const configurationFileContent = {
	roslynatorconfig: `"
                + EditorConfigCodeAnalysisConfig.FileDefaultContent
                + EditorConfigGenerator.GenerateEditorConfig(metadata, commentOut: true)
                + @"`
};",
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        Console.WriteLine($"number of analyzers: {metadata.Analyzers.Count(f => !f.IsObsolete)}");
        Console.WriteLine($"number of common analyzers: {metadata.CommonAnalyzers.Count(f => !f.IsObsolete)}");
        Console.WriteLine($"number of code analysis analyzers: {metadata.CodeAnalysisAnalyzers.Count(f => !f.IsObsolete)}");
        Console.WriteLine($"number of formatting analyzers: {metadata.FormattingAnalyzers.Count(f => !f.IsObsolete)}");
        Console.WriteLine($"number of refactorings: {refactorings.Length}");
        Console.WriteLine($"number of code fixes: {codeFixes.Length}");
        Console.WriteLine($"number of fixable compiler diagnostics: {codeFixes.SelectMany(f => f.FixableDiagnosticIds).Distinct().Count()}");

        void WriteDiagnostics(
            string dirPath,
            IEnumerable<AnalyzerMetadata> analyzers,
            string @namespace,
            string categoryName,
            string descriptorsClassName = "DiagnosticRules",
            string identifiersClassName = "DiagnosticIdentifiers")
        {
            WriteCompilationUnit(
                Path.Combine(dirPath, $"{descriptorsClassName}.Generated.cs"),
                DiagnosticRulesGenerators.Default.Generate(analyzers, obsolete: false, comparer: comparer, @namespace: @namespace, className: descriptorsClassName, identifiersClassName: identifiersClassName, categoryName: categoryName),
                normalizeWhitespace: false);

            WriteCompilationUnit(
                Path.Combine(dirPath, $"{descriptorsClassName}.Deprecated.Generated.cs"),
                DiagnosticRulesGenerators.Default.Generate(analyzers, obsolete: true, comparer: comparer, @namespace: @namespace, className: descriptorsClassName, identifiersClassName: identifiersClassName, categoryName: categoryName),
                normalizeWhitespace: false);

            WriteCompilationUnit(
                Path.Combine(dirPath, $"{identifiersClassName}.Generated.cs"),
                DiagnosticIdentifiersGenerator.Generate(analyzers, obsolete: false, comparer: comparer, @namespace: @namespace, className: identifiersClassName));

            WriteCompilationUnit(
                Path.Combine(dirPath, $"{identifiersClassName}.Deprecated.Generated.cs"),
                DiagnosticIdentifiersGenerator.Generate(analyzers, obsolete: true, comparer: comparer, @namespace: @namespace, className: identifiersClassName));
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
                banner: "Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
                autoGenerated: autoGenerated,
                normalizeWhitespace: normalizeWhitespace,
                fileMustExist: fileMustExist,
                overwrite: overwrite);
        }
    }
}
