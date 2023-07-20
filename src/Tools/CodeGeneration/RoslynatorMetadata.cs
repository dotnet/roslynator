// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration;

public class RoslynatorMetadata
{
    public RoslynatorMetadata(string rootDirectoryPath)
    {
        RootDirectoryPath = rootDirectoryPath;
    }

    public string RootDirectoryPath { get; }

    private ImmutableArray<AnalyzerMetadata> _analyzers;
    private ImmutableArray<RefactoringMetadata> _refactorings;
    private ImmutableArray<CodeFixMetadata> _codeFixes;
    private ImmutableArray<CompilerDiagnosticMetadata> _compilerDiagnostics;
    private ImmutableArray<ConfigOptionMetadata> _configOptions;

    public ImmutableArray<AnalyzerMetadata> Analyzers
    {
        get
        {
            if (_analyzers.IsDefault)
                _analyzers = LoadAnalyzers();

            return _analyzers;
        }
    }

    public IEnumerable<AnalyzerMetadata> CommonAnalyzers => Analyzers.Where(f => f.Id.StartsWith("RCS1"));

    public IEnumerable<AnalyzerMetadata> CodeAnalysisAnalyzers => Analyzers.Where(f => f.Id.StartsWith("RCS9"));

    public IEnumerable<AnalyzerMetadata> FormattingAnalyzers => Analyzers.Where(f => f.Id.StartsWith("RCS0"));

    public ImmutableArray<RefactoringMetadata> Refactorings
    {
        get
        {
            if (_refactorings.IsDefault)
                _refactorings = LoadRefactorings();

            return _refactorings;
        }
    }

    public ImmutableArray<CodeFixMetadata> CodeFixes
    {
        get
        {
            if (_codeFixes.IsDefault)
                _codeFixes = MetadataFile.ReadCodeFixes(GetPath("CodeFixes.xml")).ToImmutableArray();

            return _codeFixes;
        }
    }

    public ImmutableArray<CompilerDiagnosticMetadata> CompilerDiagnostics
    {
        get
        {
            if (_compilerDiagnostics.IsDefault)
                _compilerDiagnostics = MetadataFile.ReadCompilerDiagnostics(GetPath("Diagnostics.xml")).ToImmutableArray();

            return _compilerDiagnostics;
        }
    }

    public ImmutableArray<ConfigOptionMetadata> ConfigOptions
    {
        get
        {
            if (_configOptions.IsDefault)
                _configOptions = MetadataFile.ReadOptions(GetPath("ConfigOptions.xml")).ToImmutableArray();

            return _configOptions;
        }
    }

    private ImmutableArray<AnalyzerMetadata> LoadAnalyzers()
    {
        IEnumerable<string> filePaths = Directory.EnumerateFiles(RootDirectoryPath, "Analyzers.xml", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(Path.Combine(RootDirectoryPath, "Analyzers"), "*.Analyzers.xml", SearchOption.TopDirectoryOnly))
            .Concat(Directory.EnumerateFiles(Path.Combine(RootDirectoryPath, "Formatting.Analyzers"), "*.Analyzers.xml", SearchOption.TopDirectoryOnly))
            .Concat(Directory.EnumerateFiles(Path.Combine(RootDirectoryPath, "CodeAnalysis.Analyzers"), "*.Analyzers.xml", SearchOption.TopDirectoryOnly))
            .Where(f => Path.GetFileName(f) != "Template.Analyzers.xml");

        foreach (string filePath in filePaths)
            MetadataFile.CleanAnalyzers(filePath);

        return filePaths.SelectMany(f => MetadataFile.ReadAnalyzers(f)).ToImmutableArray();
    }

    private ImmutableArray<RefactoringMetadata> LoadRefactorings()
    {
        return Directory.EnumerateFiles(RootDirectoryPath, "Refactorings.xml", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(Path.Combine(RootDirectoryPath, "Refactorings"), "*.Refactorings.xml", SearchOption.TopDirectoryOnly))
            .Where(filePath => Path.GetFileName(filePath) != "Template.Refactorings.xml")
            .SelectMany(filePath => MetadataFile.ReadRefactorings(filePath))
            .ToImmutableArray();
    }

    private string GetPath(string path)
    {
        return Path.Combine(RootDirectoryPath, path);
    }
}
