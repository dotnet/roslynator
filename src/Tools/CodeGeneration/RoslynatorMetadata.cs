// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration
{
    public class RoslynatorMetadata
    {
        public RoslynatorMetadata(string rootDirectoryPath)
        {
            RootDirectoryPath = rootDirectoryPath;
        }

        public string RootDirectoryPath { get; }

        private ImmutableArray<AnalyzerMetadata> _analyzers;
        private ImmutableArray<AnalyzerMetadata> _codeAnalysisAnalyzers;
        private ImmutableArray<AnalyzerMetadata> _formattingAnalyzers;
        private ImmutableArray<RefactoringMetadata> _refactorings;
        private ImmutableArray<CodeFixMetadata> _codeFixes;
        private ImmutableArray<CompilerDiagnosticMetadata> _compilerDiagnostics;

        public ImmutableArray<AnalyzerMetadata> Analyzers
        {
            get
            {
                if (_analyzers.IsDefault)
                    _analyzers = LoadAnalyzers(GetPath("Analyzers"));

                return _analyzers;
            }
        }

        public ImmutableArray<AnalyzerMetadata> CodeAnalysisAnalyzers
        {
            get
            {
                if (_codeAnalysisAnalyzers.IsDefault)
                    _codeAnalysisAnalyzers = LoadAnalyzers(GetPath("CodeAnalysis.Analyzers"));

                return _codeAnalysisAnalyzers;
            }
        }

        public ImmutableArray<AnalyzerMetadata> FormattingAnalyzers
        {
            get
            {
                if (_formattingAnalyzers.IsDefault)
                    _formattingAnalyzers = LoadAnalyzers(GetPath("Formatting.Analyzers"));

                return _formattingAnalyzers;
            }
        }

        public ImmutableArray<RefactoringMetadata> Refactorings
        {
            get
            {
                if (_refactorings.IsDefault)
                    _refactorings = LoadRefactorings(GetPath("Refactorings"));

                return _refactorings;
            }
        }

        public ImmutableArray<CodeFixMetadata> CodeFixes
        {
            get
            {
                if (_codeFixes.IsDefault)
                    _codeFixes = MetadataFile.ReadAllCodeFixes(GetPath(@"CodeFixes\CodeFixes.xml"));

                return _codeFixes;
            }
        }

        public ImmutableArray<CompilerDiagnosticMetadata> CompilerDiagnostics
        {
            get
            {
                if (_compilerDiagnostics.IsDefault)
                    _compilerDiagnostics = MetadataFile.ReadAllCompilerDiagnostics(GetPath(@"CodeFixes\Diagnostics.xml"));

                return _compilerDiagnostics;
            }
        }

        private static ImmutableArray<AnalyzerMetadata> LoadAnalyzers(string directoryPath)
        {
            IEnumerable<string> analyzers = Directory.EnumerateFiles(directoryPath, "*Analyzers.xml", SearchOption.TopDirectoryOnly)
                .Concat(Directory.EnumerateFiles(directoryPath, "Analyzers.*.xml", SearchOption.TopDirectoryOnly)
                    .Where(filePath => !filePath.EndsWith("Analyzers.Template.xml")));

            return analyzers.SelectMany(MetadataFile.ReadAnalyzers).ToImmutableArray();
        }

        private static ImmutableArray<RefactoringMetadata> LoadRefactorings(string directoryPath)
        {
            IEnumerable<RefactoringMetadata> refactorings = Directory
                .EnumerateFiles(directoryPath, "Refactorings.*.xml", SearchOption.TopDirectoryOnly)
                .Where(filePath => Path.GetFileName(filePath) != "Refactorings.Template.xml")
                .SelectMany(filePath => MetadataFile.ReadAllRefactorings(filePath));

            return MetadataFile.ReadAllRefactorings(Path.Combine(directoryPath, "Refactorings.xml")).AddRange(refactorings);
        }

        private string GetPath(string path)
        {
            return Path.Combine(RootDirectoryPath, path);
        }
    }
}
