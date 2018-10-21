// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Roslynator.CodeGeneration
{
    public class RoslynatorInfo
    {
        public RoslynatorInfo(
            Solution solution,
            ImmutableArray<ISymbol> diagnosticDescriptors,
            ImmutableArray<ISymbol> diagnosticIdentifiers,
            ImmutableArray<ISymbol> refactoringIdentifiers,
            ImmutableArray<ISymbol> compilerDiagnosticIdentifiers)
        {
            Solution = solution;
            DiagnosticDescriptors = diagnosticDescriptors;
            DiagnosticIdentifiers = diagnosticIdentifiers;
            RefactoringIdentifiers = refactoringIdentifiers;
            CompilerDiagnosticIdentifiers = compilerDiagnosticIdentifiers;

            SolutionDirectory = Path.GetDirectoryName(solution.FilePath);
        }

        public Solution Solution { get; }

        public string SolutionDirectory { get; }

        public ImmutableArray<ISymbol> DiagnosticDescriptors { get; }

        public ImmutableArray<ISymbol> DiagnosticIdentifiers { get; }

        public ImmutableArray<ISymbol> RefactoringIdentifiers { get; }

        public ImmutableArray<ISymbol> CompilerDiagnosticIdentifiers { get; }

        public static async Task<RoslynatorInfo> Create(Solution solution, CancellationToken cancellationToken = default)
        {
            Project analyzersProject = solution.Projects.First(f => f.Name == "Analyzers");

            Compilation analyzersCompilation = await analyzersProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<ISymbol> diagnosticDescriptors = analyzersCompilation.GetTypeByMetadataName("Roslynator.CSharp.DiagnosticDescriptors").GetMembers();

            ImmutableArray<ISymbol> diagnosticIdentifiers = analyzersCompilation.GetTypeByMetadataName("Roslynator.CSharp.DiagnosticIdentifiers").GetMembers();

            Project refactoringsProject = solution.Projects.First(f => f.Name == "Refactorings");

            Compilation refactoringsCompilation = await refactoringsProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<ISymbol> refactoringIdentifiers = refactoringsCompilation.GetTypeByMetadataName("Roslynator.CSharp.Refactorings.RefactoringIdentifiers").GetMembers();

            Project csharpProject = solution.Projects.First(f => f.Name == "CSharp");

            Compilation csharpCompilation = await csharpProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<ISymbol> compilerDiagnosticIdentifiers = csharpCompilation.GetTypeByMetadataName("Roslynator.CSharp.CompilerDiagnosticIdentifiers").GetMembers();

            return new RoslynatorInfo(solution, diagnosticDescriptors, diagnosticIdentifiers, refactoringIdentifiers, compilerDiagnosticIdentifiers);
        }

        public async Task<IEnumerable<string>> GetAnalyzerFilesAsync(string identifier, CancellationToken cancellationToken = default)
        {
            ISymbol diagnosticDescriptor = DiagnosticDescriptors.First(f => f.Name == identifier);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(diagnosticDescriptor, Solution, cancellationToken).ConfigureAwait(false);

            ISymbol diagnosticIdentifier = DiagnosticIdentifiers.First(f => f.Name == identifier);

            IEnumerable<ReferencedSymbol> referencedSymbols2 = await SymbolFinder.FindReferencesAsync(diagnosticIdentifier, Solution, cancellationToken).ConfigureAwait(false);

            return GetFilePaths(referencedSymbols.Concat(referencedSymbols2));
        }

        public async Task<IEnumerable<string>> GetRefactoringFilesAsync(string identifier, CancellationToken cancellationToken = default)
        {
            ISymbol symbol = RefactoringIdentifiers.First(f => f.Name == identifier);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(symbol, Solution, cancellationToken).ConfigureAwait(false);

            return GetFilePaths(referencedSymbols);
        }

        public async Task<IEnumerable<string>> GetCompilerDiagnosticFilesAsync(string identifier, CancellationToken cancellationToken = default)
        {
            ISymbol symbol = CompilerDiagnosticIdentifiers.First(f => f.Name == identifier);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(symbol, Solution, cancellationToken).ConfigureAwait(false);

            return GetFilePaths(referencedSymbols);
        }

        private IEnumerable<string> GetFilePaths(IEnumerable<ReferencedSymbol> referencedSymbols)
        {
            return referencedSymbols
                .SelectMany(f => f.Locations)
                .Where(f => !f.IsCandidateLocation && !f.IsImplicit && !GeneratedCodeUtility.IsGeneratedCodeFile(f.Document.FilePath))
                .Select(f => f.Document.FilePath.Replace(SolutionDirectory, ""))
                .Distinct()
                .OrderBy(f => f);
        }
    }
}
