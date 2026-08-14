// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.FindSymbols;
using Xunit;

namespace Roslynator.Documentation.Tests;

public static class ConsumerVisibleInterfaceTests
{
    [Fact]
    public static void IsVisibleToConsumers_OmitsInternalInterfaceOnPublicType()
    {
        Compilation compilation = CreateCompilation("""
internal interface IInternal { }
public interface IPublic { }
public class C : IInternal, IPublic { }
""");

        INamedTypeSymbol type = compilation.GetTypeByMetadataName("C")!;
        INamedTypeSymbol internalInterface = compilation.GetTypeByMetadataName("IInternal")!;
        INamedTypeSymbol publicInterface = compilation.GetTypeByMetadataName("IPublic")!;

        Assert.False(SymbolExtensions.IsVisibleToConsumers(type, internalInterface));
        Assert.True(SymbolExtensions.IsVisibleToConsumers(type, publicInterface));
    }

    [Fact]
    public static void GetImplementedInterfaces_OmitsInternalInterfaceOnPublicType()
    {
        Compilation compilation = CreateCompilation("""
internal interface IInternal { }
public interface IPublic { }
public class C : IInternal, IPublic { }
""");

        INamedTypeSymbol type = compilation.GetTypeByMetadataName("C")!;
        var model = new TypeDocumentationModel(type, SymbolFilterOptions.Default);

        string[] names = model.GetImplementedInterfaces().Select(f => f.Name).ToArray();

        Assert.DoesNotContain("IInternal", names);
        Assert.Contains("IPublic", names);
    }

    [Fact]
    public static void FilterConsumerVisibleInterfaceMembers_OmitsInternalInterfaceMembers()
    {
        Compilation compilation = CreateCompilation("""
internal interface IInternal
{
    void M();
}

public interface IPublic
{
    void M();
}

public class C : IInternal, IPublic
{
    public void M() { }
}
""");

        INamedTypeSymbol type = compilation.GetTypeByMetadataName("C")!;
        IMethodSymbol method = type.GetMembers("M").OfType<IMethodSymbol>().Single();

        ISymbol[] implemented = SymbolExtensions.FilterConsumerVisibleInterfaceMembers(method).ToArray();

        Assert.DoesNotContain(implemented, f => f.ContainingType.Name == "IInternal");
        Assert.Contains(implemented, f => f.ContainingType.Name == "IPublic");
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { tree },
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Diagnostic[] errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToArray();

        Assert.True(errors.Length == 0, string.Join("\n", errors.Select(d => d.ToString())));

        return compilation;
    }
}
