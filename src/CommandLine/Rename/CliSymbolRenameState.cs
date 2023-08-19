// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.FindSymbols;
using Roslynator.Rename;
using static Roslynator.Logger;

namespace Roslynator.CommandLine.Rename;

internal class CliSymbolRenameState : SymbolRenameState
{
    public CliSymbolRenameState(
        Solution solution,
        Func<ISymbol, bool> predicate,
        Func<ISymbol, string> getNewName,
        bool ask,
        bool interactive,
        int codeContext,
        CliCompilationErrorResolution errorResolution,
        SymbolRenamerOptions options) : base(solution, predicate, getNewName, options: options)
    {
        Ask = ask;
        Interactive = interactive;
        CodeContext = codeContext;
        ErrorResolution = errorResolution;
        DryRun = Options.DryRun;
    }

    public bool Ask { get; set; }

    public bool Interactive { get; set; }

    public int CodeContext { get; }

    public bool DryRun { get; set; }

    public CliCompilationErrorResolution ErrorResolution { get; set; }

    public ConsoleDialog UserDialog { get; set; }

    protected override async Task RenameSymbolsAsync(ImmutableArray<ProjectId> projects, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        TimeSpan lastElapsed = TimeSpan.Zero;

        List<RenameScope> renameScopes = GetRenameScopes().ToList();
        for (int i = 0; i < renameScopes.Count; i++)
        {
            WriteLine($"Rename {GetScopePluralName(renameScopes[i])} {$"{i + 1}/{renameScopes.Count}"}", Verbosity.Minimal);

            for (int j = 0; j < projects.Length; j++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = CurrentSolution.GetProject(projects[j]);

                WriteLine($"  Rename {GetScopePluralName(renameScopes[i])} in '{project.Name}' {$"{j + 1}/{projects.Length}"}", ConsoleColors.Cyan, Verbosity.Minimal);

                await AnalyzeProjectAsync(project, renameScopes[i], cancellationToken);

                WriteLine($"  Done renaming {GetScopePluralName(renameScopes[i])} in '{project.Name}' in {stopwatch.Elapsed - lastElapsed:mm\\:ss\\.ff}", Verbosity.Normal);
            }
        }

        stopwatch.Stop();

        WriteLine($"Done renaming symbols in solution '{CurrentSolution.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", Verbosity.Minimal);
    }

    public override async Task RenameSymbolsAsync(Project project, CancellationToken cancellationToken = default)
    {
        List<RenameScope> renameScopes = GetRenameScopes().ToList();

        for (int i = 0; i < renameScopes.Count; i++)
        {
            WriteLine($"Rename {GetScopePluralName(renameScopes[i])} {$"{i + 1}/{renameScopes.Count}"}", Verbosity.Minimal);

            await AnalyzeProjectAsync(project, renameScopes[i], cancellationToken);
        }
    }

    protected override async Task<(string NewName, Solution NewSolution)> RenameSymbolAsync(
        ISymbol symbol,
        string symbolId,
        List<string> ignoreIds,
        IFindSymbolService findSymbolService,
        TextSpan span,
        Document document,
        CancellationToken cancellationToken)
    {
        LogHelpers.WriteSymbolDefinition(symbol, baseDirectoryPath: Path.GetDirectoryName(document.Project.FilePath), "    ", Verbosity.Normal);

        if (ShouldWrite(Verbosity.Detailed)
            || CodeContext >= 0)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken);
            Verbosity verbosity = (CodeContext >= 0) ? Verbosity.Normal : Verbosity.Detailed;
            LogHelpers.WriteLineSpan(span, CodeContext, sourceText, "    ", verbosity);
        }

        Solution newSolution = null;
        string newName = GetNewName?.Invoke(symbol) ?? symbol.Name;
        bool interactive = Interactive;
        int compilerErrorCount = 0;

        while (true)
        {
            string newName2 = GetSymbolNewName(newName, symbol, findSymbolService, interactive: interactive);

            if (newName2 is null)
            {
                ignoreIds?.Add(symbolId);
                return default;
            }

            newName = newName2;

            WriteLine(
                $"    Rename '{symbol.Name}' to '{newName}'",
                (DryRun) ? ConsoleColors.DarkGray : ConsoleColors.Green,
                Verbosity.Minimal);

            if (DryRun)
                return default;

            try
            {
                newSolution = await Microsoft.CodeAnalysis.Rename.Renamer.RenameSymbolAsync(
                    CurrentSolution,
                    symbol,
                    new Microsoft.CodeAnalysis.Rename.SymbolRenameOptions(RenameOverloads: true),
                    newName,
                    cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                WriteLine($"    Cannot rename '{symbol.Name}' to '{newName}': {ex.Message}", ConsoleColors.Yellow, Verbosity.Normal);
#if DEBUG
                WriteLine(ex.ToString());
#endif
                ignoreIds?.Add(symbolId);
                return default;
            }

            if (ErrorResolution != CliCompilationErrorResolution.None)
            {
                Project newProject = newSolution.GetDocument(document.Id).Project;
                Compilation compilation = await newProject.GetCompilationAsync(cancellationToken);
                ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken);

                compilerErrorCount = LogHelpers.WriteCompilerErrors(
                    diagnostics,
                    Path.GetDirectoryName(newProject.FilePath),
                    ignoredCompilerDiagnosticIds: Options.IgnoredCompilerDiagnosticIds,
                    indentation: "    ");
            }

            if (compilerErrorCount > 0
                && ErrorResolution != CliCompilationErrorResolution.List)
            {
                if (ErrorResolution == CliCompilationErrorResolution.Fix)
                {
                    interactive = true;
                    continue;
                }
                else if (ErrorResolution == CliCompilationErrorResolution.Skip)
                {
                    ignoreIds?.Add(symbolId);
                    return default;
                }
                else if (ErrorResolution == CliCompilationErrorResolution.Ask
                    && UserDialog is not null)
                {
                    switch (UserDialog.ShowDialog("Rename symbol?"))
                    {
                        case DialogResult.None:
                        case DialogResult.No:
                            {
                                ignoreIds?.Add(symbolId);
                                return default;
                            }
                        case DialogResult.NoToAll:
                            {
                                ErrorResolution = CliCompilationErrorResolution.Skip;
                                ignoreIds?.Add(symbolId);
                                return default;
                            }
                        case DialogResult.Yes:
                            {
                                break;
                            }
                        case DialogResult.YesToAll:
                            {
                                ErrorResolution = CliCompilationErrorResolution.None;
                                break;
                            }
                        default:
                            {
                                throw new InvalidOperationException();
                            }
                    }
                }
                else if (ErrorResolution == CliCompilationErrorResolution.Abort)
                {
                    throw new OperationCanceledException();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            break;
        }

        if (Ask
            && UserDialog is not null
            && (compilerErrorCount == 0 || ErrorResolution != CliCompilationErrorResolution.Ask))
        {
            switch (UserDialog.ShowDialog("Rename symbol?"))
            {
                case DialogResult.None:
                case DialogResult.No:
                    {
                        ignoreIds?.Add(symbolId);
                        return default;
                    }
                case DialogResult.NoToAll:
                    {
                        DryRun = true;
                        ignoreIds?.Add(symbolId);
                        return default;
                    }
                case DialogResult.Yes:
                    {
                        break;
                    }
                case DialogResult.YesToAll:
                    {
                        Ask = false;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        return (newName, newSolution);
    }

    private static string GetSymbolNewName(
        string newName,
        ISymbol symbol,
        IFindSymbolService findSymbolService,
        bool interactive)
    {
        if (interactive)
        {
            bool isAttribute = symbol is INamedTypeSymbol typeSymbol
                && typeSymbol.InheritsFrom(MetadataNames.System_Attribute);

            while (true)
            {
                string newName2 = ConsoleUtility.ReadUserInput(newName, "    New name: ");

                if (string.IsNullOrEmpty(newName2))
                    return null;

                if (string.Equals(newName, newName2, StringComparison.Ordinal))
                    break;

                bool isValidIdentifier = findSymbolService.SyntaxFacts.IsValidIdentifier(newName2);

                if (isValidIdentifier
                    && (!isAttribute || newName2.EndsWith("Attribute")))
                {
                    newName = newName2;
                    break;
                }

                ConsoleOut.WriteLine(
                    (!isValidIdentifier)
                        ? "    New name is invalid"
                        : "    New name is invalid, attribute name must end with 'Attribute'",
                    ConsoleColor.Yellow);
            }
        }

        if (string.Equals(symbol.Name, newName, StringComparison.Ordinal))
            return null;

        if (!interactive)
        {
            if (!findSymbolService.SyntaxFacts.IsValidIdentifier(newName))
            {
                WriteLine($"    New name is invalid: {newName}", ConsoleColors.Yellow, Verbosity.Minimal);
                return null;
            }

            if (symbol is INamedTypeSymbol typeSymbol
                && typeSymbol.InheritsFrom(MetadataNames.System_Attribute)
                && !newName.EndsWith("Attribute"))
            {
                WriteLine($"    New name is invalid: {newName}. Attribute name must end with 'Attribute'.", ConsoleColors.Yellow, Verbosity.Minimal);
                return null;
            }
        }

        return newName;
    }

    private static string GetScopePluralName(RenameScope scope)
    {
        return scope switch
        {
            RenameScope.Type => "types",
            RenameScope.Member => "members",
            RenameScope.Local => "locals",
            _ => throw new InvalidOperationException($"Unknown enum value '{scope}'."),
        };
    }
}
