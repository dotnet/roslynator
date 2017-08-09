// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration
{
    public class Generator
    {
        private ImmutableArray<AnalyzerDescriptor> _analyzers;
        private ImmutableArray<RefactoringDescriptor> _refactorings;
        private ImmutableArray<CodeFixDescriptor> _codeFixes;
        private ImmutableArray<CompilerDiagnosticDescriptor> _compilerDiagnostics;

        public Generator(string rootPath, StringComparer comparer = null)
        {
            RootPath = rootPath;
            Comparer = comparer ?? StringComparer.InvariantCulture;
        }

        public string RootPath { get; }

        public StringComparer Comparer { get; }

        public ImmutableArray<AnalyzerDescriptor> Analyzers
        {
            get
            {
                if (_analyzers.IsDefault)
                    _analyzers = MetadataFile.ReadAllAnalyzers(GetPath(@"Analyzers\Analyzers.xml"));

                return _analyzers;
            }
        }

        public ImmutableArray<RefactoringDescriptor> Refactorings
        {
            get
            {
                if (_refactorings.IsDefault)
                    _refactorings = MetadataFile.ReadAllRefactorings(GetPath(@"Refactorings\Refactorings.xml"));

                return _refactorings;
            }
        }

        public ImmutableArray<CodeFixDescriptor> CodeFixes
        {
            get
            {
                if (_codeFixes.IsDefault)
                    _codeFixes = MetadataFile.ReadAllCodeFixes(GetPath(@"CodeFixes\CodeFixes.xml"));

                return _codeFixes;
            }
        }

        public ImmutableArray<CompilerDiagnosticDescriptor> CompilerDiagnostics
        {
            get
            {
                if (_compilerDiagnostics.IsDefault)
                    _compilerDiagnostics = MetadataFile.ReadAllCompilerDiagnostics(GetPath(@"CodeFixes\Diagnostics.xml"));

                return _compilerDiagnostics;
            }
        }

        public string GetPath(string relativePath)
        {
            return Path.Combine(RootPath, relativePath);
        }
    }
}
