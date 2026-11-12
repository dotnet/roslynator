// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public static async Task DryRunNamesNoParameterOfAnonymousMethodWithoutParameterList()
    {
        const string source = @"
class C
{
    public event System.EventHandler Changed = delegate { };

    string M()
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
            symbol => symbol is IParameterSymbol,
            symbol =>
            {
                named.Add(symbol);
                return symbol.Name + "2";
            },
            new SymbolRenamerOptions() { DryRun = true, SkipTypes = true, SkipMembers = true });

        Assert.Empty(named);
    }

    [Fact]
    public static async Task RenameLocalsCompletesWithAnonymousMethodWithoutParameterList()
    {
        const string source = @"
class C
{
    public event System.EventHandler Changed = delegate { };

    string M()
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

    [Fact]
    public static async Task DryRunNamesNestedLocalFunctionsAndLeavesSolutionUnchanged()
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
        var named = new List<ISymbol>();

        await SymbolRenamer.RenameSymbolsAsync(
            document.Project,
            symbol => symbol is IMethodSymbol { MethodKind: MethodKind.LocalFunction },
            symbol =>
            {
                named.Add(symbol);
                return symbol.Name + "2";
            },
            new SymbolRenamerOptions() { DryRun = true, SkipTypes = true, SkipMembers = true });

        Assert.Equal(2, named.Count);
        Assert.Equal(source, (await workspace.CurrentSolution.GetDocument(document.Id).GetTextAsync()).ToString());
    }

    [Fact]
    public static async Task RenameMembersUpdatesReferencesInOtherDocument()
    {
        string[] sources =
        [
            @"
class Foo
{
    public int Field = 1;
    public int Prop { get; set; }

    public int Bar(int p)
    {
        int local = Field + Prop + p;
        return local + Scale(local);
    }

    public int Scale(int q)
    {
        return q + Field;
    }
}
",
            @"
class User
{
    int Use(Foo foo)
    {
        int result = foo.Bar(foo.Field) + foo.Scale(foo.Prop);
        return result;
    }
}
",
        ];

        string[] results = await RenameDocumentsAsync(
            sources,
            symbol => symbol.Kind is SymbolKind.Field or SymbolKind.Property or SymbolKind.Method && !symbol.IsImplicitlyDeclared,
            symbol => symbol.Name + "Renamed",
            new SymbolRenamerOptions() { SkipTypes = true, SkipLocals = true });

        Assert.Equal(
            sources.Select(f => f.Replace("Field", "FieldRenamed").Replace("Prop", "PropRenamed").Replace("Bar", "BarRenamed").Replace("Scale", "ScaleRenamed").Replace("int Use(", "int UseRenamed(")),
            results);
    }

    [Fact]
    public static async Task RenameLocalsToLongerNamesUpdatesEveryReference()
    {
        const string source = @"
class C
{
    int M(int a, int b)
    {
        int c = a + b; int d = c * a; int e = d + c + b;
        return a + b + c + d + e;
    }
}
";
        const string expected = @"
class C
{
    int M(int first, int second)
    {
        int third = first + second; int fourth = third * first; int fifth = fourth + third + second;
        return first + second + third + fourth + fifth;
    }
}
";
        var names = new Dictionary<string, string>() { ["a"] = "first", ["b"] = "second", ["c"] = "third", ["d"] = "fourth", ["e"] = "fifth" };

        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol.Kind is SymbolKind.Local or SymbolKind.Parameter,
            symbol => names[symbol.Name],
            new SymbolRenamerOptions() { SkipTypes = true });

        Assert.Equal(expected, Assert.Single(results));
    }

    [Fact]
    public static async Task RenameLocalsToShorterNamesUpdatesEveryReference()
    {
        const string source = @"
class C
{
    int M(int first, int second)
    {
        int third = first + second; int fourth = third * first; int fifth = fourth + third + second;
        return first + second + third + fourth + fifth;
    }
}
";
        const string expected = @"
class C
{
    int M(int a, int b)
    {
        int c = a + b; int d = c * a; int e = d + c + b;
        return a + b + c + d + e;
    }
}
";
        var names = new Dictionary<string, string>() { ["first"] = "a", ["second"] = "b", ["third"] = "c", ["fourth"] = "d", ["fifth"] = "e" };

        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol.Kind is SymbolKind.Local or SymbolKind.Parameter,
            symbol => names[symbol.Name],
            new SymbolRenamerOptions() { SkipTypes = true });

        Assert.Equal(expected, Assert.Single(results));
    }

    [Fact]
    public static async Task RenameEscapedLocalsDropsTheEscapeWhenTheNewNameNeedsNone()
    {
        const string source = @"
class C
{
    int M()
    {
        var @default = 1;
        var @class = @default + 1;
        return @default + @class;
    }
}
";
        const string expected = @"
class C
{
    int M()
    {
        var value = 1;
        var category = value + 1;
        return value + category;
    }
}
";
        var names = new Dictionary<string, string>() { ["default"] = "value", ["class"] = "category" };

        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol.Kind == SymbolKind.Local,
            symbol => names[symbol.Name],
            new SymbolRenamerOptions() { SkipTypes = true, SkipMembers = true });

        Assert.Equal(expected, Assert.Single(results));
    }

    [Fact]
    public static async Task RenameEscapedMemberReferencedBeforeItsDeclarationDropsTheEscape()
    {
        const string source = @"
class C
{
    int M()
    {
        return @event + @event;
    }

    private int @event = 1;
}
";
        const string expected = @"
class C
{
    int M()
    {
        return handler + handler;
    }

    private int handler = 1;
}
";
        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol.Kind == SymbolKind.Field,
            _ => "handler",
            new SymbolRenamerOptions() { SkipTypes = true, SkipLocals = true });

        Assert.Equal(expected, Assert.Single(results));
    }

    [Fact]
    public static async Task RenameLambdaParametersUpdatesEveryReference()
    {
        const string source = @"
using System;
class C
{
    void M()
    {
        Func<int, int> f = x => x + 1;
        Action<int> g = (y) => { var z = y; Console.WriteLine(z + y); };
        Func<int, int, int> h = (p, q) => p + q;
        f(1); g(2); h(3, 4);
    }
}
";
        const string expected = @"
using System;
class C
{
    void M()
    {
        Func<int, int> f = number => number + 1;
        Action<int> g = (input) => { var z = input; Console.WriteLine(z + input); };
        Func<int, int, int> h = (left, right) => left + right;
        f(1); g(2); h(3, 4);
    }
}
";
        var names = new Dictionary<string, string>() { ["x"] = "number", ["y"] = "input", ["p"] = "left", ["q"] = "right" };

        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol.Kind == SymbolKind.Parameter,
            symbol => names[symbol.Name],
            new SymbolRenamerOptions() { SkipTypes = true, SkipMembers = true });

        Assert.Equal(expected, Assert.Single(results));
    }

    [Fact]
    public static async Task RenameLocalFunctionWithItsParameterAndTypeParameterUpdatesEveryReference()
    {
        const string source = @"
class C
{
    int M()
    {
        T Echo<T>(T value)
        {
            return value;
        }

        return Echo<int>(1) + Echo<int>(2);
    }
}
";
        const string expected = @"
class C
{
    int M()
    {
        TItem Repeat<TItem>(TItem item)
        {
            return item;
        }

        return Repeat<int>(1) + Repeat<int>(2);
    }
}
";
        var names = new Dictionary<string, string>() { ["Echo"] = "Repeat", ["T"] = "TItem", ["value"] = "item" };

        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol is IMethodSymbol { MethodKind: MethodKind.LocalFunction } or IParameterSymbol or ITypeParameterSymbol { TypeParameterKind: TypeParameterKind.Method },
            symbol => names[symbol.Name],
            new SymbolRenamerOptions() { SkipTypes = true, SkipMembers = true });

        Assert.Equal(expected, Assert.Single(results));
    }

    [Fact]
    public static async Task RenameNestedLocalFunctionParametersAndTypeParametersUpdatesEveryReference()
    {
        const string source = @"
class C
{
    int M()
    {
        int Outer<T>(int a)
        {
            int Inner<U>(int b)
            {
                return b + 1;
            }

            return Inner<T>(a);
        }

        return Outer<int>(1);
    }
}
";
        const string expected = @"
class C
{
    int M()
    {
        int Outer<TOuter>(int first)
        {
            int Inner<TInner>(int second)
            {
                return second + 1;
            }

            return Inner<TOuter>(first);
        }

        return Outer<int>(1);
    }
}
";
        var names = new Dictionary<string, string>() { ["T"] = "TOuter", ["U"] = "TInner", ["a"] = "first", ["b"] = "second" };

        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol is IParameterSymbol or ITypeParameterSymbol { TypeParameterKind: TypeParameterKind.Method },
            symbol => names[symbol.Name],
            new SymbolRenamerOptions() { SkipTypes = true, SkipMembers = true });

        Assert.Equal(expected, Assert.Single(results));
    }

    [Fact]
    public static async Task RenamePartialMethodToItsOwnNameCompletes()
    {
        string[] sources =
        [
            @"
partial class P
{
    public int A;
    partial void Hook();
}
",
            @"
partial class P
{
    public int B = 2;
    partial void Hook() { A = B; }
}
",
        ];

        string[] results = await RenameDocumentsAsync(
            sources,
            symbol => symbol is IMethodSymbol { Name: "Hook" },
            symbol => symbol.Name,
            new SymbolRenamerOptions() { SkipTypes = true, SkipLocals = true });

        Assert.Equal(sources, results);
    }

    [Fact]
    public static async Task SkipRenameThatCausesCompilerError()
    {
        const string source = @"
class C
{
    int M()
    {
        int a = 1;
        int b = 2;
        int c = 3;
        return a + b + c;
    }
}
";
        const string expected = @"
class C
{
    int M()
    {
        int same = 1;
        int b = 2;
        int c = 3;
        return same + b + c;
    }
}
";
        string[] results = await RenameDocumentsAsync(
            [source],
            symbol => symbol.Kind == SymbolKind.Local,
            _ => "same",
            new SymbolRenamerOptions() { SkipTypes = true, SkipMembers = true, CompilationErrorResolution = CompilationErrorResolution.Skip });

        Assert.Equal(expected, Assert.Single(results));
    }

    private static async Task<string[]> RenameDocumentsAsync(
        string[] sources,
        Func<ISymbol, bool> predicate,
        Func<ISymbol, string> getNewName,
        SymbolRenamerOptions options)
    {
        using var workspace = new AdhocWorkspace();
        Project project = workspace.AddProject(ProjectInfo.Create(
            ProjectId.CreateNewId(),
            VersionStamp.Create(),
            "Test",
            "Test",
            LanguageNames.CSharp,
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            metadataReferences: CSharpTestOptions.Default.MetadataReferences));

        var documentIds = new List<DocumentId>();

        for (int i = 0; i < sources.Length; i++)
            documentIds.Add(workspace.AddDocument(project.Id, $"Test{i}.cs", SourceText.From(sources[i])).Id);

        await SymbolRenamer.RenameSymbolsAsync(workspace.CurrentSolution.GetProject(project.Id), predicate, getNewName, options);

        var results = new string[sources.Length];

        for (int i = 0; i < results.Length; i++)
            results[i] = (await workspace.CurrentSolution.GetDocument(documentIds[i]).GetTextAsync()).ToString();

        return results;
    }
}
