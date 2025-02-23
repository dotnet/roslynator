﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        if (args.Length == 0)
        {
            Console.WriteLine("Invalid number of arguments");
            return;
        }

        string rootPath = args[0];

        StringComparer comparer = StringComparer.InvariantCulture;

        var metadata = new RoslynatorMetadata(rootPath);

        ImmutableArray<RefactoringMetadata> refactorings = metadata.Refactorings;
        ImmutableArray<CodeFixMetadata> codeFixes = metadata.CodeFixes;
        ImmutableArray<CompilerDiagnosticMetadata> compilerDiagnostics = metadata.CompilerDiagnostics;
        ImmutableArray<AnalyzerOptionMetadata> options = metadata.ConfigOptions;

        WriteCompilationUnit(
            "Refactorings/CSharp/RefactoringDescriptors.Generated.cs",
            RefactoringDescriptorsGenerator.Generate(refactorings.Where(f => !f.IsObsolete), comparer: comparer));

        WriteCompilationUnit(
            "Refactorings/CSharp/RefactoringIdentifiers.Generated.cs",
            RefactoringIdentifiersGenerator.Generate(refactorings, obsolete: false, comparer: comparer));

        WriteCompilationUnit(
            "VisualStudio/RefactoringsOptionsPage.Generated.cs",
            RefactoringsOptionsPageGenerator.Generate(refactorings.Where(f => !f.IsObsolete), comparer));

        WriteDiagnostics("Common", metadata.CommonAnalyzers.Concat(metadata.FormattingAnalyzers), @namespace: "Roslynator", categoryName: nameof(DiagnosticCategories.Roslynator), descriptorsClassName: "DiagnosticRules", identifiersClassName: "DiagnosticIdentifiers");

        WriteDiagnostics("Common/CodeAnalysis", metadata.CodeAnalysisAnalyzers, @namespace: "Roslynator.CodeAnalysis", categoryName: nameof(DiagnosticCategories.Roslynator), descriptorsClassName: "CodeAnalysisDiagnosticRules", identifiersClassName: "CodeAnalysisDiagnosticIdentifiers");

        WriteCompilationUnit(
            "CodeFixes/CSharp/CompilerDiagnosticRules.Generated.cs",
            CompilerDiagnosticRulesGenerator.Generate(compilerDiagnostics, comparer: comparer, @namespace: "Roslynator.CSharp"),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            "CodeFixes/CSharp/CodeFixDescriptors.Generated.cs",
            CodeFixDescriptorsGenerator.Generate(codeFixes.Where(f => !f.IsObsolete), comparer: comparer, @namespace: "Roslynator.CSharp"),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            "CodeFixes/CSharp/CodeFixIdentifiers.Generated.cs",
            CodeFixIdentifiersGenerator.Generate(codeFixes, comparer));

        WriteCompilationUnit(
            "VisualStudio/CodeFixesOptionsPage.Generated.cs",
            CodeFixesOptionsPageGenerator.Generate());

        WriteCompilationUnit(
            "CSharp/CSharp/CompilerDiagnosticIdentifiers.Generated.cs",
            CompilerDiagnosticIdentifiersGenerator.Generate(compilerDiagnostics, comparer));

        WriteCompilationUnit(
            "Common/ConfigOptions.Generated.cs",
            Roslynator.CodeGeneration.CSharp.CodeGenerator.GenerateConfigOptions(options, metadata.Analyzers),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            "Common/ConfigOptionKeys.Generated.cs",
            Roslynator.CodeGeneration.CSharp.CodeGenerator.GenerateConfigOptionKeys(options),
            normalizeWhitespace: false);

        WriteCompilationUnit(
            "Common/ConfigOptionValues.Generated.cs",
            Roslynator.CodeGeneration.CSharp.CodeGenerator.GenerateConfigOptionValues(options),
            normalizeWhitespace: false);

        File.WriteAllText(
            Path.Combine(rootPath, "VisualStudioCode/package/src/configurationFiles.generated.ts"),
            @"export const configurationFileContent = {
	roslynatorconfig: `"
                + EditorConfigCodeAnalysisConfig.FileDefaultContent
                + EditorConfigGenerator.GenerateEditorConfig(metadata, commentOut: true)
                + @"`
};",
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        Console.WriteLine($"number of analyzers: {metadata.Analyzers.Count(f => f.Status == AnalyzerStatus.Enabled)}");
        Console.WriteLine($"number of common analyzers: {metadata.CommonAnalyzers.Count(f => f.Status == AnalyzerStatus.Enabled)}");
        Console.WriteLine($"number of code analysis analyzers: {metadata.CodeAnalysisAnalyzers.Count(f => f.Status == AnalyzerStatus.Enabled)}");
        Console.WriteLine($"number of formatting analyzers: {metadata.FormattingAnalyzers.Count(f => f.Status == AnalyzerStatus.Enabled)}");
        Console.WriteLine($"number of refactorings: {refactorings.Length}");
        Console.WriteLine($"number of code fixes: {codeFixes.Length}");
        Console.WriteLine($"number of fixable compiler diagnostics: {codeFixes.SelectMany(f => f.FixableDiagnosticIds).Distinct().Count()}");

        void WriteDiagnostics(
            string dirPath,
            IEnumerable<AnalyzerMetadata> analyzers,
            string @namespace,
            string categoryName,
            string descriptorsClassName,
            string identifiersClassName)
        {
            WriteCompilationUnit(
                Path.Combine(dirPath, $"{descriptorsClassName}.Generated.cs"),
                DiagnosticRulesGenerators.Default.Generate(analyzers.Where(f => f.Status != AnalyzerStatus.Disabled), comparer: comparer, @namespace: @namespace, className: descriptorsClassName, identifiersClassName: identifiersClassName, categoryName: categoryName),
                normalizeWhitespace: false);

            WriteCompilationUnit(
                Path.Combine(dirPath, $"{identifiersClassName}.Generated.cs"),
                DiagnosticIdentifiersGenerator.Generate(analyzers.Where(f => f.Status != AnalyzerStatus.Disabled), comparer: comparer, @namespace: @namespace, className: identifiersClassName));
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
                banner: "Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
                autoGenerated: autoGenerated,
                normalizeWhitespace: normalizeWhitespace,
                fileMustExist: fileMustExist,
                overwrite: overwrite);
        }
    }
}
