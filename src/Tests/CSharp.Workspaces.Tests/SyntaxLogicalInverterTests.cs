// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.CSharp.Workspaces.Tests;

public class SyntaxLogicallyInvertTests
{
    private readonly SyntaxLogicalInverter _inverter;

    public SyntaxLogicallyInvertTests()
    {
        _inverter = SyntaxLogicalInverter.Default;
    }

    [Theory]
    [InlineData("x", "!x")]
    [InlineData("!x", "x")]
    [InlineData(@"x is ""abc""", @"x is not ""abc""")]
    [InlineData("x is 1", "x is not 1")]
    [InlineData("x is null", "x is not null")]
    [InlineData("x is true", "x is false")]
    [InlineData("true", "false")]
    [InlineData("false", "true")]
    [InlineData("x >= 3", "x < 3")]
    [InlineData("x > 3", "x <= 3")]
    [InlineData("x <= 3", "x > 3")]
    [InlineData("x < 3", "x >= 3")]
    [InlineData("x == y", "x != y")]
    [InlineData("x != y", "x == y")]
    [InlineData("(bool)x || (bool)y", "!((bool)x) && !((bool)y)")]
    [InlineData("(bool)x && (bool)y", "!((bool)x) || !((bool)y)")]
    [InlineData("x ?? true", "x == false")]
    [InlineData("x ?? false", "x != true")]
    [InlineData("(bool)x ? y : z", "(bool)x ? !y : !z")]
    [InlineData("x[0]", "!x[0]")]
    [InlineData("default(bool)", "!default(bool)")]
    [InlineData("checked(x + y)", "!checked(x + y)")]
    [InlineData("unchecked(x + y)", "!unchecked(x + y)")]
    [InlineData("(bool)x", "!((bool)x)")]
    [InlineData("x & y", "!x | !y")]
    [InlineData("x ^ y", "!(x ^ y)")]
    [InlineData("x | y", "!x & !y")]
    [InlineData("x = y", "!(x = y)")]
    [InlineData("await x", "!(await x)")]
    [InlineData("x ?? y", "!(x ?? y)")]
    [InlineData("x.a", "!x.a")]
    [InlineData("x.a()", "!x.a()")]
    [InlineData("x?.a", "!x?.a")]
    public async Task LogicallyInvert(string source, string expected)
    {
        string sourceCode = $"class C {{ void M(dynamic x, dynamic y, dynamic z){{ if({source})return;}} }}";
        var workspace = new AdhocWorkspace();
        Project newProject = workspace.AddProject("TestProject", LanguageNames.CSharp);
        Document newDocument = workspace.AddDocument(newProject.Id, "TestDocument.cs", SourceText.From(sourceCode));
        SyntaxTree syntaxTree = await newDocument.GetSyntaxTreeAsync();
        Compilation compilation = await newDocument.Project.GetCompilationAsync();
        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

        ExpressionSyntax expression = syntaxTree.GetRoot().DescendantNodes().OfType<IfStatementSyntax>().Single().Condition;

        ExpressionSyntax result = _inverter.LogicallyInvert(expression, semanticModel, CancellationToken.None);
        Assert.Equal(expected, result.NormalizeWhitespace().ToFullString());
    }
}
