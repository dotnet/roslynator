// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting;
using static Roslynator.Logger;

namespace Roslynator.CodeFixes
{
    internal class CodeFixer
    {
        private readonly AnalyzerAssemblyList _analyzerAssemblies = new AnalyzerAssemblyList();

        private readonly AnalyzerAssemblyList _analyzerReferences = new AnalyzerAssemblyList();

        private static readonly CompilationWithAnalyzersOptions _defaultCompilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: default(Action<Exception, DiagnosticAnalyzer, Diagnostic>),
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: false,
            reportSuppressedDiagnostics: false);

        public CodeFixer(
            Solution solution,
            AbstractSyntaxFactsServiceFactory syntaxFactsFactory,
            IEnumerable<string> analyzerAssemblies = null,
            IFormatProvider formatProvider = null,
            CodeFixerOptions options = null)
        {
            Workspace = solution.Workspace;
            SyntaxFactsFactory = syntaxFactsFactory;
            Options = options ?? CodeFixerOptions.Default;

            if (analyzerAssemblies != null)
                _analyzerAssemblies.LoadFrom(analyzerAssemblies);

            FormatProvider = formatProvider;
        }

        public Workspace Workspace { get; }

        public AbstractSyntaxFactsServiceFactory SyntaxFactsFactory { get; }

        public CodeFixerOptions Options { get; }

        public IFormatProvider FormatProvider { get; }

        private Solution CurrentSolution => Workspace.CurrentSolution;

        public async Task FixSolutionAsync(CancellationToken cancellationToken = default)
        {
            foreach (string id in Options.IgnoredCompilerDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore compiler diagnostic '{id}'", Verbosity.Diagnostic);

            foreach (string id in Options.IgnoredDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore diagnostic '{id}'", Verbosity.Diagnostic);

            ImmutableArray<ProjectId> projects = CurrentSolution
                .GetProjectDependencyGraph()
                .GetTopologicallySortedProjects(cancellationToken)
                .ToImmutableArray();

            var results = new List<ProjectFixResult>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            TimeSpan lastElapsed = TimeSpan.Zero;

            for (int i = 0; i < projects.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = CurrentSolution.GetProject(projects[i]);

                string language = project.Language;

                if (Options.IsSupportedProject(project))
                {
                    WriteLine($"Fix '{project.Name}' {$"{i + 1}/{projects.Length}"}", ConsoleColor.Cyan, Verbosity.Minimal);

                    ProjectFixResult result = await FixProjectAsync(project, cancellationToken).ConfigureAwait(false);

                    results.Add(result);

                    WriteFixSummary(
                        result.FixedDiagnostics,
                        result.UnfixedDiagnostics,
                        result.UnfixableDiagnostics,
                        indent: "  ",
                        formatProvider: FormatProvider,
                        verbosity: Verbosity.Detailed);

                    if (result.Kind == ProjectFixKind.CompilerError)
                        break;
                }
                else
                {
                    WriteLine($"Skip '{project.Name}' {$"{i + 1}/{projects.Length}"}", ConsoleColor.DarkGray, Verbosity.Minimal);

                    results.Add(ProjectFixResult.Skipped);
                }

                TimeSpan elapsed = stopwatch.Elapsed;

                WriteLine($"Done fixing '{project.Name}' in {elapsed - lastElapsed:mm\\:ss\\.ff}", Verbosity.Normal);

                lastElapsed = elapsed;
            }

            stopwatch.Stop();

            if (Options.Format)
            {
                int count = results.Sum(f => f.FormattedDocumentCount);
                WriteLine();
                WriteLine($"{count} {((count == 1) ? "document" : "documents")} formatted", Verbosity.Normal);
            }

            WriteFixSummary(
                results.SelectMany(f => f.FixedDiagnostics),
                results.SelectMany(f => f.UnfixedDiagnostics),
                results.SelectMany(f => f.UnfixableDiagnostics),
                addEmptyLine: true,
                formatProvider: FormatProvider,
                verbosity: Verbosity.Normal);

            WriteLine(Verbosity.Minimal);

            int fixedCount = results.Sum(f => f.FixedDiagnostics.Length);

            WriteLine($"{fixedCount} {((fixedCount == 1) ? "diagnostic" : "diagnostics")} fixed", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);

            WriteLine($"Done fixing solution '{CurrentSolution.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", Verbosity.Minimal);
        }

        public async Task<ProjectFixResult> FixProjectAsync(Project project, CancellationToken cancellationToken = default)
        {
            (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) = Utilities.GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: _analyzerAssemblies,
                analyzerReferences: _analyzerReferences,
                options: Options);

            ProjectFixResult fixResult = await FixProjectAsync(project, analyzers, fixers, cancellationToken).ConfigureAwait(false);

            Compilation compilation = await CurrentSolution.GetProject(project.Id).GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            Dictionary<string, ImmutableArray<CodeFixProvider>> fixersById = fixers
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (id, fixer: f)))
                .GroupBy(f => f.id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.fixer).Distinct().ToImmutableArray());

            ImmutableArray<Diagnostic> unfixableDiagnostics = await GetDiagnosticsAsync(
                analyzers,
                fixResult.FixedDiagnostics,
                compilation,
                f => !fixersById.TryGetValue(f.id, out ImmutableArray<CodeFixProvider> fixers2),
                cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> unfixedDiagnostics = await GetDiagnosticsAsync(
                analyzers,
                fixResult.FixedDiagnostics.Concat(unfixableDiagnostics),
                compilation,
                f => fixersById.TryGetValue(f.id, out ImmutableArray<CodeFixProvider> fixers2),
                cancellationToken).ConfigureAwait(false);

            if (Options.FileBannerLines.Length > 0)
                await AddFileBannerAsync(CurrentSolution.GetProject(project.Id), Options.FileBannerLines, cancellationToken).ConfigureAwait(false);

            int formattedDocumentCount = 0;

            if (Options.Format)
                formattedDocumentCount = await FormatProjectAsync(CurrentSolution.GetProject(project.Id), cancellationToken).ConfigureAwait(false);

            return new ProjectFixResult(
                kind: fixResult.Kind,
                fixedDiagnostics: fixResult.FixedDiagnostics,
                unfixedDiagnostics: unfixedDiagnostics,
                unfixableDiagnostics: unfixableDiagnostics,
                analyzers: fixResult.Analyzers,
                fixers: fixResult.Fixers,
                formattedDocumentCount: formattedDocumentCount);
        }

        private async Task<ProjectFixResult> FixProjectAsync(
            Project project,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            if (!analyzers.Any())
            {
                WriteLine($"  No analyzers found to analyze '{project.Name}'", ConsoleColor.DarkGray, Verbosity.Normal);
                return ProjectFixResult.NoAnalyzers;
            }

            if (!fixers.Any())
            {
                WriteLine($"  No fixers found to fix '{project.Name}'", ConsoleColor.DarkGray, Verbosity.Normal);
                return new ProjectFixResult(ProjectFixKind.NoFixers, analyzers: analyzers, fixers: fixers);
            }

            Dictionary<string, ImmutableArray<CodeFixProvider>> fixersById = fixers
                .Where(f =>
                {
                    if (f.HasFixAllProvider(FixAllScope.Project))
                        return true;

                    if (Options.DiagnosticIdsFixableOneByOne.Count > 0)
                    {
                        foreach (string diagnosticId in f.FixableDiagnosticIds)
                        {
                            if (Options.DiagnosticIdsFixableOneByOne.Contains(diagnosticId))
                                return true;
                        }
                    }

                    return false;
                })
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (id, fixer: f)))
                .GroupBy(f => f.id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.fixer).Distinct().ToImmutableArray());

            analyzers = analyzers
                .Where(analyzer => analyzer.SupportedDiagnostics.Any(descriptor => fixersById.ContainsKey(descriptor.Id)))
                .ToImmutableArray();

            if (!analyzers.Any())
                return new ProjectFixResult(ProjectFixKind.Success, analyzers: analyzers, fixers: fixers);

            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzersById = analyzers
                .SelectMany(f => f.SupportedDiagnostics.Select(d => (id: d.Id, analyzer: f)))
                .GroupBy(f => f.id, f => f.analyzer)
                .ToDictionary(g => g.Key, g => g.Select(analyzer => analyzer).Distinct().ToImmutableArray());

            WriteUsedAnalyzers(analyzers, ConsoleColor.DarkGray, Verbosity.Diagnostic);
            WriteUsedFixers(fixers, ConsoleColor.DarkGray, Verbosity.Diagnostic);

            ImmutableArray<Diagnostic>.Builder fixedDiagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;
            ImmutableArray<Diagnostic> previousPreviousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            var fixKind = ProjectFixKind.Success;

            int iterationCount = 1;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                project = CurrentSolution.GetProject(project.Id);

                WriteLine($"  Compile '{project.Name}'{((iterationCount > 1) ? $" iteration {iterationCount}" : "")}", Verbosity.Normal);

                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                if (!VerifyCompilerDiagnostics(compilerDiagnostics, project))
                    return new ProjectFixResult(ProjectFixKind.CompilerError, fixedDiagnostics, analyzers: analyzers, fixers: fixers);

                WriteLine($"  Analyze '{project.Name}'", Verbosity.Normal);

                ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzers, _defaultCompilationWithAnalyzersOptions, cancellationToken).ConfigureAwait(false);

                foreach (string message in diagnostics
                    .Where(f => f.IsAnalyzerExceptionDiagnostic())
                    .Select(f => f.ToString())
                    .Distinct())
                {
                    WriteLine(message, ConsoleColor.Yellow, Verbosity.Diagnostic);
                }

                IEnumerable<Diagnostic> fixableCompilerDiagnostics = compilerDiagnostics
                    .Where(f => f.Severity != DiagnosticSeverity.Error
                        && !Options.IgnoredCompilerDiagnosticIds.Contains(f.Id)
                        && fixersById.ContainsKey(f.Id));

                diagnostics = diagnostics
                    .Where(f => Options.IsSupportedDiagnostic(f)
                        && analyzersById.ContainsKey(f.Id)
                        && fixersById.ContainsKey(f.Id))
                    .Concat(fixableCompilerDiagnostics)
                    .ToImmutableArray();

                int length = diagnostics.Length;

                if (length == 0)
                    break;

                if (length == previousDiagnostics.Length
                    && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    break;
                }

                if (length == previousPreviousDiagnostics.Length
                    && !diagnostics.Except(previousPreviousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    WriteInfiniteLoopSummary(diagnostics, previousDiagnostics, project, FormatProvider);

                    fixKind = ProjectFixKind.InfiniteLoop;
                    break;
                }

                WriteLine($"  Found {length} {((length == 1) ? "diagnostic" : "diagnostics")} in '{project.Name}'", Verbosity.Normal);

                Dictionary<DiagnosticDescriptor, int> diagnosticCountByDescriptor = diagnostics
                    .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
                    .ToDictionary(f => f.Key, f => f.Count());

                foreach (DiagnosticDescriptor diagnosticDescriptor in diagnosticCountByDescriptor
                    .Select(f => f.Key)
                    .OrderBy(f => f, new DiagnosticDescriptorFixComparer(diagnosticCountByDescriptor, fixersById)))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string diagnosticId = diagnosticDescriptor.Id;

                    DiagnosticFixResult result = await FixDiagnosticsAsync(
                        diagnosticDescriptor,
                        (diagnosticDescriptor.CustomTags.Contains(WellKnownDiagnosticTags.Compiler))
                            ? default(ImmutableArray<DiagnosticAnalyzer>)
                            : analyzersById[diagnosticId],
                        fixersById[diagnosticId],
                        CurrentSolution.GetProject(project.Id),
                        cancellationToken).ConfigureAwait(false);

                    if (result.Kind == DiagnosticFixKind.Fixed)
                    {
                        fixedDiagnostics.AddRange(result.FixedDiagnostics);
                    }
                    else if (result.Kind == DiagnosticFixKind.CompilerError)
                    {
                        return new ProjectFixResult(ProjectFixKind.CompilerError, fixedDiagnostics, analyzers: analyzers, fixers: fixers);
                    }
                }

                if (iterationCount == Options.MaxIterations)
                    break;

                previousPreviousDiagnostics = previousDiagnostics;
                previousDiagnostics = diagnostics;
                iterationCount++;
            }

            return new ProjectFixResult(fixKind, fixedDiagnostics, analyzers: analyzers, fixers: fixers);
        }

        private async Task<DiagnosticFixResult> FixDiagnosticsAsync(
            DiagnosticDescriptor descriptor,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            Project project,
            CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic>.Builder fixedDiagostics = ImmutableArray.CreateBuilder<Diagnostic>();

            ImmutableArray<Diagnostic> diagnostics = ImmutableArray<Diagnostic>.Empty;
            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;
            ImmutableArray<Diagnostic> previousDiagnosticsToFix = ImmutableArray<Diagnostic>.Empty;

            int length = 0;
            var fixKind = DiagnosticFixKind.NotFixed;

            while (true)
            {
                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                if (!VerifyCompilerDiagnostics(compilerDiagnostics, project))
                {
                    fixKind = DiagnosticFixKind.CompilerError;

                    if (!previousDiagnostics.Any())
                        break;
                }

                if (analyzers.IsDefault)
                {
                    diagnostics = compilerDiagnostics;
                }
                else
                {
                    diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzers, _defaultCompilationWithAnalyzersOptions, cancellationToken).ConfigureAwait(false);
                }

                diagnostics = diagnostics
                    .Where(f => f.Id == descriptor.Id && f.Severity >= Options.SeverityLevel)
                    .ToImmutableArray();

                if (fixKind == DiagnosticFixKind.CompilerError)
                {
                    break;
                }
                else if (fixKind == DiagnosticFixKind.Fixed)
                {
                    if (Options.BatchSize <= 0
                        || length <= Options.BatchSize)
                    {
                        break;
                    }
                }
                else if (previousDiagnostics.Any()
                    && fixKind != DiagnosticFixKind.PartiallyFixed)
                {
                    break;
                }

                length = diagnostics.Length;

                if (length == 0)
                    break;

                if (length == previousDiagnostics.Length
                    && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    break;
                }

                fixedDiagostics.AddRange(previousDiagnosticsToFix.Except(diagnostics, DiagnosticDeepEqualityComparer.Instance));

                previousDiagnostics = diagnostics;

                if (Options.BatchSize > 0
                    && length > Options.BatchSize)
                {
                    diagnostics = ImmutableArray.CreateRange(diagnostics, 0, Options.BatchSize, f => f);
                }

                fixKind = await FixDiagnosticsAsync(diagnostics, descriptor, fixers, project, cancellationToken).ConfigureAwait(false);

                previousDiagnosticsToFix = diagnostics;

                project = CurrentSolution.GetProject(project.Id);
            }

            fixedDiagostics.AddRange(previousDiagnosticsToFix.Except(diagnostics, DiagnosticDeepEqualityComparer.Instance));

            return new DiagnosticFixResult(fixKind, fixedDiagostics.ToImmutableArray());
        }

        private async Task<DiagnosticFixKind> FixDiagnosticsAsync(
            ImmutableArray<Diagnostic> diagnostics,
            DiagnosticDescriptor descriptor,
            ImmutableArray<CodeFixProvider> fixers,
            Project project,
            CancellationToken cancellationToken)
        {
            string diagnosticId = descriptor.Id;

            WriteLine($"  Fix {diagnostics.Length} {diagnosticId} '{descriptor.Title}'", diagnostics[0].Severity.GetColor(), Verbosity.Normal);

            WriteDiagnostics(diagnostics, baseDirectoryPath: Path.GetDirectoryName(project.FilePath), formatProvider: FormatProvider, indentation: "    ", verbosity: Verbosity.Detailed);

            CodeFixProvider fixer = null;
            CodeAction fix = null;

            for (int i = 0; i < fixers.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CodeAction fixCandidate = await GetFixAsync(
                    diagnostics,
                    descriptor,
                    fixers[i],
                    project,
                    cancellationToken).ConfigureAwait(false);

                if (fixCandidate != null)
                {
                    if (fix == null)
                    {
                        if (Options.DiagnosticFixerMap.IsEmpty
                            || !Options.DiagnosticFixerMap.TryGetValue(diagnosticId, out string fullTypeName)
                            || string.Equals(fixers[i].GetType().FullName, fullTypeName, StringComparison.Ordinal))
                        {
                            fix = fixCandidate;
                            fixer = fixers[i];
                        }
                    }
                    else if (Options.DiagnosticFixerMap.IsEmpty
                        || !Options.DiagnosticFixerMap.ContainsKey(diagnosticId))
                    {
                        WriteMultipleFixersSummary(diagnosticId, fixer, fixers[i]);
                        return DiagnosticFixKind.MultipleFixers;
                    }
                }
            }

            if (fix != null)
            {
                ImmutableArray<CodeActionOperation> operations = await fix.GetOperationsAsync(cancellationToken).ConfigureAwait(false);

                if (operations.Length == 1)
                {
                    operations[0].Apply(Workspace, cancellationToken);

                    return (diagnostics.Length != 1 && fixer.GetFixAllProvider() == null)
                        ? DiagnosticFixKind.PartiallyFixed
                        : DiagnosticFixKind.Fixed;
                }
                else if (operations.Length > 1)
                {
                    WriteMultipleOperationsSummary(fix);
                }
            }

            return DiagnosticFixKind.NotFixed;
        }

        private async Task<CodeAction> GetFixAsync(
            ImmutableArray<Diagnostic> diagnostics,
            DiagnosticDescriptor descriptor,
            CodeFixProvider fixer,
            Project project,
            CancellationToken cancellationToken)
        {
            if (diagnostics.Length == 1)
                return await GetFixAsync(diagnostics[0], fixer, project, cancellationToken).ConfigureAwait(false);

            FixAllProvider fixAllProvider = fixer.GetFixAllProvider();

            if (fixAllProvider == null)
            {
                if (Options.DiagnosticIdsFixableOneByOne.Contains(descriptor.Id))
                    return await GetFixAsync(diagnostics[0], fixer, project, cancellationToken).ConfigureAwait(false);

                WriteLine($"  '{fixer.GetType().FullName}' does not have FixAllProvider", ConsoleColor.Yellow, Verbosity.Diagnostic);
                return null;
            }

            if (!fixAllProvider.GetSupportedFixAllDiagnosticIds(fixer).Any(f => f == descriptor.Id))
            {
                WriteLine($"  '{fixAllProvider.GetType().FullName}' does not support diagnostic '{descriptor.Id}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                return null;
            }

            if (!fixAllProvider.GetSupportedFixAllScopes().Any(f => f == FixAllScope.Project))
            {
                WriteLine($"  '{fixAllProvider.GetType().FullName}' does not support scope '{FixAllScope.Project}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                return null;
            }

            var multipleFixesInfos = new HashSet<MultipleFixesInfo>();

            CodeAction action = null;

            Options.DiagnosticFixMap.TryGetValue(descriptor.Id, out string equivalenceKey);

            foreach (Diagnostic diagnostic in diagnostics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!diagnostic.Location.IsInSource)
                    continue;

                Document document = project.GetDocument(diagnostic.Location.SourceTree);

                if (document == null)
                    continue;

                CodeAction fixCandidate = await GetFixAsync(diagnostic, fixer, document, multipleFixesInfos, cancellationToken).ConfigureAwait(false);

                if (fixCandidate == null)
                    continue;

                if (equivalenceKey != null
                    && equivalenceKey != fixCandidate.EquivalenceKey)
                {
                    break;
                }

                action = fixCandidate;

                var fixAllContext = new FixAllContext(
                    document,
                    fixer,
                    FixAllScope.Project,
                    action.EquivalenceKey,
                    new string[] { descriptor.Id },
                    new FixAllDiagnosticProvider(diagnostics),
                    cancellationToken);

                CodeAction fix = await fixAllProvider.GetFixAsync(fixAllContext).ConfigureAwait(false);

                if (fix != null)
                {
                    WriteLine($"  CodeFixProvider: '{fixer.GetType().FullName}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    if (!string.IsNullOrEmpty(action.EquivalenceKey))
                        WriteLine($"  EquivalenceKey:  '{action.EquivalenceKey}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    WriteLine($"  FixAllProvider:  '{fixAllProvider.GetType().FullName}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return fix;
                }

                WriteLine($"  Fixer '{fixer.GetType().FullName}' registered no action for diagnostic '{descriptor.Id}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                WriteDiagnostics(diagnostics, baseDirectoryPath: Path.GetDirectoryName(project.FilePath), formatProvider: FormatProvider, indentation: "    ", maxCount: 10, verbosity: Verbosity.Diagnostic);
            }

            return null;
        }

        private async Task<CodeAction> GetFixAsync(
            Diagnostic diagnostic,
            CodeFixProvider fixer,
            Project project,
            CancellationToken cancellationToken)
        {
            if (!diagnostic.Location.IsInSource)
                return null;

            Document document = project.GetDocument(diagnostic.Location.SourceTree);

            Debug.Assert(document != null, "");

            if (document == null)
                return null;

            CodeAction action = await GetFixAsync(diagnostic, fixer, document, default, cancellationToken).ConfigureAwait(false);

            if (action == null)
            {
                WriteLine($"  Fixer '{fixer.GetType().FullName}' registered no action for diagnostic '{diagnostic.Id}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                WriteDiagnostic(diagnostic, baseDirectoryPath: Path.GetDirectoryName(project.FilePath), formatProvider: FormatProvider, indentation: "    ", verbosity: Verbosity.Diagnostic);
            }

            return action;
        }

        private async Task<CodeAction> GetFixAsync(
            Diagnostic diagnostic,
            CodeFixProvider fixer,
            Document document,
            HashSet<MultipleFixesInfo> multipleFixesInfos,
            CancellationToken cancellationToken)
        {
            CodeAction action = null;

            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, _) =>
                {
                    if (action == null)
                    {
                        action = a;
                    }
                    else if (!string.Equals(a.EquivalenceKey, action.EquivalenceKey, StringComparison.Ordinal)
                        && (Options.DiagnosticFixMap.IsEmpty || !Options.DiagnosticFixMap.ContainsKey(diagnostic.Id)))
                    {
                        var multipleFixesInfo = new MultipleFixesInfo(diagnostic.Id, fixer, action.EquivalenceKey, a.EquivalenceKey);

                        if (multipleFixesInfos == null)
                            multipleFixesInfos = new HashSet<MultipleFixesInfo>();

                        if (multipleFixesInfos.Add(multipleFixesInfo))
                        {
                            WriteMultipleActionsSummary(multipleFixesInfo);
                        }

                        action = null;
                    }
                },
                cancellationToken);

            await fixer.RegisterCodeFixesAsync(context).ConfigureAwait(false);

            return action;
        }

        private bool VerifyCompilerDiagnostics(ImmutableArray<Diagnostic> diagnostics, Project project)
        {
            const string indentation = "    ";

            using (IEnumerator<Diagnostic> en = diagnostics
                .Where(f => f.Severity == DiagnosticSeverity.Error
                    && !Options.IgnoredCompilerDiagnosticIds.Contains(f.Id))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    Write(indentation);
                    WriteLine("Compilation errors:");

                    string baseDirectoryPath = Path.GetDirectoryName(project.FilePath);

                    const int maxCount = 10;

                    int count = 0;

                    do
                    {
                        count++;

                        if (count <= maxCount)
                        {
                            WriteDiagnostic(
                                en.Current,
                                baseDirectoryPath: baseDirectoryPath,
                                formatProvider: FormatProvider,
                                indentation: indentation,
                                verbosity: Verbosity.Normal);
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (en.MoveNext());

                    count = 0;

                    bool plus = false;

                    while (en.MoveNext())
                    {
                        count++;

                        if (count == 1000)
                        {
                            plus = true;
                            break;
                        }
                    }

                    if (count > maxCount)
                    {
                        Write(indentation);
                        WriteLine($"and {count}{((plus) ? "+" : "")} more errors", verbosity: Verbosity.Normal);
                    }

                    if (!Options.IgnoreCompilerErrors)
                    {
#if DEBUG
                        Console.Write("Stop (Y/N)? ");

                        if (char.ToUpperInvariant((char)Console.Read()) == 'Y')
                            return false;
#else
                        return false;
#endif
                    }
                }
            }

            return true;
        }

        private async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            IEnumerable<Diagnostic> except,
            Compilation compilation,
            Func<(string id, DiagnosticAnalyzer analyzer), bool> predicate,
            CancellationToken cancellationToken)
        {
            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzersById = analyzers
                .SelectMany(f => f.SupportedDiagnostics.Select(d => (id: d.Id, analyzer: f)))
                .Where(predicate)
                .GroupBy(f => f.id, f => f.analyzer)
                .ToDictionary(g => g.Key, g => g.Select(analyzer => analyzer).Distinct().ToImmutableArray());

            analyzers = analyzersById
                .SelectMany(f => f.Value)
                .Distinct()
                .ToImmutableArray();

            if (!analyzers.Any())
                return ImmutableArray<Diagnostic>.Empty;

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzers, _defaultCompilationWithAnalyzersOptions, cancellationToken).ConfigureAwait(false);

            return diagnostics
                .Where(f => Options.IsSupportedDiagnostic(f)
                    && analyzersById.ContainsKey(f.Id))
                .Except(except, DiagnosticDeepEqualityComparer.Instance)
                .ToImmutableArray();
        }

        private async Task AddFileBannerAsync(
            Project project,
            ImmutableArray<string> banner,
            CancellationToken cancellationToken)
        {
            bool hasChanges = false;

            string solutionDirectory = Path.GetDirectoryName(project.Solution.FilePath);

            foreach (DocumentId documentId in project.DocumentIds)
            {
                Document document = project.GetDocument(documentId);

                if (GeneratedCodeUtility.IsGeneratedCodeFile(document.FilePath))
                    continue;

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxFactsService syntaxFacts = SyntaxFactsFactory.GetService(project.Language);

                if (syntaxFacts.BeginsWithAutoGeneratedComment(root))
                    continue;

                if (syntaxFacts.BeginsWithBanner(root, banner))
                    continue;

                SyntaxTriviaList leading = root.GetLeadingTrivia();

                SyntaxTriviaList newLeading = leading.InsertRange(0, banner.SelectMany(f => syntaxFacts.ParseLeadingTrivia(syntaxFacts.SingleLineCommentStart + f + Environment.NewLine)));

                if (!syntaxFacts.IsEndOfLineTrivia(leading.LastOrDefault()))
                    newLeading = newLeading.AddRange(syntaxFacts.ParseLeadingTrivia(Environment.NewLine));

                SyntaxNode newRoot = root.WithLeadingTrivia(newLeading);

                Document newDocument = document.WithSyntaxRoot(newRoot);

                WriteLine($"  Add banner to '{PathUtilities.TrimStart(document.FilePath, solutionDirectory)}'", ConsoleColor.DarkGray, Verbosity.Detailed);

                project = newDocument.Project;

                hasChanges = true;
            }

            if (hasChanges
                && !Workspace.TryApplyChanges(project.Solution))
            {
                Debug.Fail($"Cannot apply changes to solution '{project.Solution.FilePath}'");
                WriteLine($"Cannot apply changes to solution '{project.Solution.FilePath}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            }
        }

        private async Task<int> FormatProjectAsync(Project project, CancellationToken cancellationToken)
        {
            WriteLine($"  Format  '{project.Name}'", Verbosity.Normal);

            SyntaxFactsService syntaxFacts = SyntaxFactsFactory.GetService(project.Language);

            Project newProject = await CodeFormatter.FormatProjectAsync(project, syntaxFacts, cancellationToken).ConfigureAwait(false);

            string solutionDirectory = Path.GetDirectoryName(project.Solution.FilePath);

            ImmutableArray<DocumentId> formattedDocuments = await CodeFormatter.GetFormattedDocumentsAsync(project, newProject, syntaxFacts).ConfigureAwait(false);

            WriteFormattedDocuments(formattedDocuments, project, solutionDirectory);

            if (formattedDocuments.Length > 0
                && !Workspace.TryApplyChanges(newProject.Solution))
            {
                Debug.Fail($"Cannot apply changes to solution '{newProject.Solution.FilePath}'");
                WriteLine($"Cannot apply changes to solution '{newProject.Solution.FilePath}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            }

            return formattedDocuments.Length;
        }
    }
}
