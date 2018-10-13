// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CodeFixes.ConsoleHelpers;

namespace Roslynator.CodeFixes
{
    //TODO: fix compiler diagnostics
    public class CodeFixer
    {
        private readonly AnalyzerFileCache _analyzerFiles;

        private readonly AnalyzerFileCache _analyzerFileCache = new AnalyzerFileCache();

        private static CompilationWithAnalyzersOptions DefaultCompilationWithAnalyzersOptions { get; } = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: null,
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: false,
            reportSuppressedDiagnostics: false);

        public CodeFixer(Workspace workspace, IEnumerable<string> analyzerAssemblies = null, CodeFixerOptions options = null)
        {
            Workspace = workspace;
            Options = options ?? CodeFixerOptions.Default;

            _analyzerFiles = new AnalyzerFileCache();

            if (analyzerAssemblies != null)
                _analyzerFiles.LoadFrom(analyzerAssemblies);
        }

        public Workspace Workspace { get; }

        public CodeFixerOptions Options { get; }

        private Solution CurrentSolution => Workspace.CurrentSolution;

        public async Task FixAsync(CancellationToken cancellationToken = default)
        {
            ImmutableArray<ProjectId> projects = CurrentSolution
                .GetProjectDependencyGraph()
                .GetTopologicallySortedProjects(cancellationToken)
                .ToImmutableArray();

            foreach (string projectName in Options.IgnoredProjectNames.OrderBy(f => f))
                WriteLine($"Project '{projectName}' will be ignored");

            foreach (string id in Options.IgnoredDiagnosticIds.OrderBy(f => f))
                WriteLine($"Diagnostic '{id}' will be ignored");

            foreach (string id in Options.IgnoredCompilerDiagnosticIds.OrderBy(f => f))
                WriteLine($"Compiler diagnostic '{id}' will be ignored");

            Stopwatch stopwatch = Stopwatch.StartNew();

            TimeSpan lastElapsed = TimeSpan.Zero;

            for (int i = 0; i < projects.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = CurrentSolution.GetProject(projects[i]);

                WriteLine($"Fix project {$"{i + 1}/{projects.Length}"} '{project.Name}'", ConsoleColor.Cyan);

                if (Options.IgnoredProjectNames.Contains(project.Name))
                {
                    WriteLine($"  Project '{project.Name}' is ignored");
                }
                else
                {
                    FixResult result = await FixProjectAsync(project, cancellationToken).ConfigureAwait(false);

                    if (result == FixResult.CompilerError)
                        break;
                }

                TimeSpan elapsed = stopwatch.Elapsed;

                WriteLine($"Done fixing project {$"{i + 1}/{projects.Length}"} {elapsed - lastElapsed:mm\\:ss\\.ff} '{project.Name}'", ConsoleColor.Green);

                lastElapsed = elapsed;
            }

            stopwatch.Stop();

            WriteLine($"Done fixing solution {stopwatch.Elapsed:mm\\:ss\\.ff} '{CurrentSolution.FilePath}'", ConsoleColor.Green);
        }

        private async Task<FixResult> FixProjectAsync(Project project, CancellationToken cancellationToken = default)
        {
            string language = project.Language;

            ImmutableArray<Assembly> assemblies = (Options.IgnoreAnalyzerReferences) ? ImmutableArray<Assembly>.Empty : project.AnalyzerReferences
                .Distinct()
                .OfType<AnalyzerFileReference>()
                .Select(f => f.GetAssembly())
                .Where(f => !_analyzerFiles.Contains(f.FullName))
                .ToImmutableArray();

            ImmutableArray<DiagnosticAnalyzer> analyzers = _analyzerFiles
                .GetAnalyzers(language)
                .AddRange(_analyzerFileCache.GetAnalyzers(assemblies, language));

            if (!analyzers.Any())
                return FixResult.NoAnalyzers;

            ImmutableArray<CodeFixProvider> fixers = _analyzerFiles
                .GetFixers(language)
                .AddRange(_analyzerFileCache.GetFixers(assemblies, language));

            if (!fixers.Any())
                return FixResult.NoFixers;

            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzersById = analyzers
                .SelectMany(f => f.SupportedDiagnostics.Select(id => (id.Id, analyzer: f)))
                .GroupBy(f => f.Id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.analyzer).Distinct().ToImmutableArray());

            Dictionary<string, ImmutableArray<CodeFixProvider>> fixersById = fixers
                .Where(f => f.GetFixAllProvider() != null)
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (id, fixer: f)))
                .GroupBy((f) => f.id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.fixer).ToImmutableArray());

            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            int iterationCount = 1;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                project = CurrentSolution.GetProject(project.Id);

                WriteLine($"  Compile '{project.Name}'{((iterationCount > 1) ? $" iteration {iterationCount}" : "")}");

                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                if (!VerifyCompilerDiagnostics(compilation, cancellationToken))
                    return FixResult.CompilerError;

                var compilationWithAnalyzers = new CompilationWithAnalyzers(compilation, analyzers, DefaultCompilationWithAnalyzersOptions);

                WriteLine($"  Analyze '{project.Name}'");

                ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);

                foreach (string message in diagnostics
                    .Where(f => f.Id.StartsWith("AD", StringComparison.Ordinal))
                    .Select(f => f.ToString())
                    .Distinct())
                {
                    WriteLine(message, ConsoleColor.Yellow);
                }

                diagnostics = diagnostics
                    .Where(f => f.Severity != DiagnosticSeverity.Hidden
                        && analyzersById.ContainsKey(f.Id)
                        && fixersById.ContainsKey(f.Id)
                        && !Options.IgnoredDiagnosticIds.Contains(f.Id))
                    .ToImmutableArray();

                int length = diagnostics.Length;

                if (length == 0)
                    break;

                if (length == previousDiagnostics.Length
                    && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    break;
                }

                WriteLine($"  Found {length} {((length == 1) ? "diagnostic" : "diagnostics")} in '{project.Name}'");

                foreach (string diagnosticId in diagnostics
                    .Select(f => f.Id)
                    .Distinct()
                    .OrderBy(f => f))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    FixResult result = await FixDiagnosticsAsync(
                        diagnosticId,
                        CurrentSolution.GetProject(project.Id),
                        analyzersById[diagnosticId],
                        fixersById[diagnosticId],
                        cancellationToken).ConfigureAwait(false);

                    if (result == FixResult.CompilerError)
                        return result;
                }

                previousDiagnostics = diagnostics;
                iterationCount++;
            }

            return FixResult.Success;
        }

        private async Task<FixResult> FixDiagnosticsAsync(
            string diagnosticId,
            Project project,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            while (true)
            {
                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                if (!VerifyCompilerDiagnostics(compilation, cancellationToken))
                    return FixResult.CompilerError;

                var compilationWithAnalyzers = new CompilationWithAnalyzers(compilation, analyzers, DefaultCompilationWithAnalyzersOptions);

                ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);

                diagnostics = diagnostics
                    .Where(f => f.Id == diagnosticId && f.Severity != DiagnosticSeverity.Hidden)
                    .ToImmutableArray();

                int length = diagnostics.Length;

                if (length == 0)
                    return FixResult.Success;

                if (length == previousDiagnostics.Length
                    && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    break;
                }

                previousDiagnostics = diagnostics;

                if (Options.BatchSize > 0
                    && length > Options.BatchSize)
                {
                    diagnostics = ImmutableArray.CreateRange(diagnostics, 0, Options.BatchSize, f => f);
                }

                await FixDiagnosticsAsync(diagnosticId, project, diagnostics, fixers, cancellationToken).ConfigureAwait(false);

                if (Options.BatchSize <= 0
                    || length <= Options.BatchSize)
                {
                    break;
                }

                project = CurrentSolution.GetProject(project.Id);
            }

            return FixResult.Success;
        }

        private async Task FixDiagnosticsAsync(
            string diagnosticId,
            Project project,
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            WriteLine($"  Fix {diagnostics.Length,4} {diagnosticId,10} '{diagnostics[0].Descriptor.Title}'");

            if (Options.BatchSize == 1)
                WriteLine($"  {diagnostics[0]}", ConsoleColor.DarkGray);

            CodeFixProvider fixer = null;
            CodeAction codeAction = null;

            for (int i = 0; i < fixers.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CodeAction codeAction2 = await GetFixAsync(
                    diagnosticId,
                    project,
                    diagnostics,
                    fixers[i],
                    cancellationToken).ConfigureAwait(false);

                if (codeAction2 != null)
                {
                    if (codeAction == null)
                    {
                        codeAction = codeAction2;
                        fixer = fixers[i];
                    }
                    else
                    {
#if DEBUG
                        WriteLine($"Diagnostic '{diagnosticId}' is fixable by multiple fixers", ConsoleColor.DarkYellow);
                        WriteLine($"  {fixer.GetType().Name}", ConsoleColor.DarkYellow);
                        WriteLine($"  {fixers[i].GetType().Name}", ConsoleColor.DarkYellow);
#endif
                        codeAction = null;
                        break;
                    }
                }
            }

            if (codeAction != null)
            {
                ImmutableArray<CodeActionOperation> operations = await codeAction.GetOperationsAsync(cancellationToken).ConfigureAwait(false);

                WriteLineIf(operations.Length > 1, $@"Code action has multiple operations
Title: {codeAction.Title}
Equivalence key: {codeAction.EquivalenceKey}", ConsoleColor.Magenta);

                if (operations.Length == 1)
                    operations[0].Apply(Workspace, cancellationToken);
            }
        }

        private static async Task<CodeAction> GetFixAsync(
            string diagnosticId,
            Project project,
            ImmutableArray<Diagnostic> diagnostics,
            CodeFixProvider fixer,
            CancellationToken cancellationToken)
        {
            FixAllProvider fixAll = fixer.GetFixAllProvider();

            if (!fixAll.GetSupportedFixAllDiagnosticIds(fixer).Any(f => f == diagnosticId))
                return null;

            if (!fixAll.GetSupportedFixAllScopes().Any(f => f == FixAllScope.Project))
                return null;

            foreach (Diagnostic diagnostic in diagnostics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!diagnostic.Location.IsInSource)
                    continue;

                Document document = project.GetDocument(diagnostic.Location.SourceTree);

                Debug.Assert(document != null, "");

                if (document == null)
                    continue;

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
                        else if (!string.Equals(a.EquivalenceKey, action.EquivalenceKey, StringComparison.Ordinal))
                        {
#if DEBUG
                            WriteLine($"'{fixer.GetType().Name}' registered multiple actions for diagnostic '{diagnosticId}'", ConsoleColor.DarkYellow);
                            WriteLine($"  {action.EquivalenceKey}", ConsoleColor.DarkYellow);
                            WriteLine($"  {a.EquivalenceKey}", ConsoleColor.DarkYellow);
#endif
                            action = null;
                        }
                    },
                    cancellationToken);

                await fixer.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                if (action == null)
                    continue;

                var fixAllContext = new FixAllContext(
                    document,
                    fixer,
                    FixAllScope.Project,
                    action.EquivalenceKey,
                    new string[] { diagnosticId },
                    new FixAllDiagnosticProvider(diagnostics),
                    cancellationToken);

                CodeAction fixAllAction = await fixAll.GetFixAsync(fixAllContext).ConfigureAwait(false);

                if (fixAllAction == null && diagnosticId.StartsWith("RCS"))
                {
                    WriteLine($"'{fixer.GetType().FullName}' registered no action for diagnostics:", ConsoleColor.Magenta);
                    Write(diagnostics, 10, ConsoleColor.Magenta);
                }

                return fixAllAction;
            }

            return null;
        }

        private bool VerifyCompilerDiagnostics(Compilation compilation, CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken);

            using (IEnumerator<Diagnostic> en = diagnostics
                .Where(f => f.Severity == DiagnosticSeverity.Error
                    && !Options.IgnoredCompilerDiagnosticIds.Contains(f.Id))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    const int maxCount = 10;

                    int count = 0;

                    do
                    {
                        count++;

                        if (count <= maxCount)
                        {
                            WriteLine(en.Current.ToString(), ConsoleColor.Red);
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
                        WriteLine($"and {count}{((plus) ? "+" : "")} more diagnostics", ConsoleColor.Red);
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
    }
}
