// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestConsole
{
    internal static  class Program
    {
        private static void Main()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
using System;

public static class Foo
{
    public static void Bar()
    {
    }   
}");

            var compilation = CSharpCompilation.Create(
                "Test",
                syntaxTrees: new[] { tree },
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            SemanticModel semanticModel = compilation.GetSemanticModel(tree);

            SyntaxNode root = tree.GetRoot();
        }
    }
}
