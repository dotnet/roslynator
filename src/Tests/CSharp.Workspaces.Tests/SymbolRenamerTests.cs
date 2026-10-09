// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Rename;
using Xunit;

namespace Roslynator.Testing.CSharp;

public static class SymbolRenamerTests
{
    [Fact]
    public static async Task DryRunNamesLocalsAndLeavesSolutionUnchanged()
    {
        const string source = @"
class C
{
    string M()
    {
        var item = new object();
        return item.ToString();
    }

    string M2()
    {
        var item = new object();
        return item.ToString();
    }
}
";
        using var workspace = new AdhocWorkspace();
        Project project = workspace.AddProject(ProjectInfo.Create(
            ProjectId.CreateNewId(),
            VersionStamp.Create(),
            "Test",
            "Test",
            LanguageNames.CSharp,
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            metadataReferences: CSharpTestOptions.Default.MetadataReferences));
        Document document = workspace.AddDocument(project.Id, "Test.cs", SourceText.From(source));
        var named = new List<ISymbol>();

        await SymbolRenamer.RenameSymbolsAsync(
            document.Project,
            symbol => symbol is ILocalSymbol { Name: "item" },
            symbol =>
            {
                named.Add(symbol);
                return "element";
            },
            new SymbolRenamerOptions() { DryRun = true, SkipTypes = true, SkipMembers = true });

        Assert.Equal(2, named.Count);
        Assert.Equal(source, (await workspace.CurrentSolution.GetDocument(document.Id).GetTextAsync()).ToString());
    }

    [Fact]
    public static async Task RenameLocalsAppliesChangesToWorkspace()
    {
        const string source = @"
class C
{
    string M()
    {
        var item = new object();
        return item.ToString();
    }

    string M2()
    {
        var item = new object();
        return item.ToString();
    }
}
";
        using var workspace = new AdhocWorkspace();
        Project project = workspace.AddProject(ProjectInfo.Create(
            ProjectId.CreateNewId(),
            VersionStamp.Create(),
            "Test",
            "Test",
            LanguageNames.CSharp,
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            metadataReferences: CSharpTestOptions.Default.MetadataReferences));
        Document document = workspace.AddDocument(project.Id, "Test.cs", SourceText.From(source));

        await SymbolRenamer.RenameSymbolsAsync(
            document.Project,
            symbol => symbol is ILocalSymbol { Name: "item" },
            _ => "element",
            new SymbolRenamerOptions() { SkipTypes = true, SkipMembers = true });

        Assert.Equal(source.Replace("item", "element"), (await workspace.CurrentSolution.GetDocument(document.Id).GetTextAsync()).ToString());
    }

    [Fact]
    public static async Task RenameNestedLocalFunctionsCompletes()
    {
        const string source = @"
class C
{
    int M()
    {
        int Outer(int a)
        {
            int Inner(int b)
            {
                return b + 1;
            }

            return Inner(a);
        }

        return Outer(1);
    }
}
";
        using var workspace = new AdhocWorkspace();
        Project project = workspace.AddProject(ProjectInfo.Create(
            ProjectId.CreateNewId(),
            VersionStamp.Create(),
            "Test",
            "Test",
            LanguageNames.CSharp,
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            metadataReferences: CSharpTestOptions.Default.MetadataReferences));
        Document document = workspace.AddDocument(project.Id, "Test.cs", SourceText.From(source));

        await SymbolRenamer.RenameSymbolsAsync(
            document.Project,
            symbol => symbol is IMethodSymbol { MethodKind: MethodKind.LocalFunction },
            symbol => symbol.Name + "2",
            new SymbolRenamerOptions() { SkipTypes = true, SkipMembers = true });

        Assert.Equal(
            source.Replace("Outer", "Outer2").Replace("Inner", "Inner2"),
            (await workspace.CurrentSolution.GetDocument(document.Id).GetTextAsync()).ToString());
    }
}
