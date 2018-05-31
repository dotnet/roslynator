// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#region usings
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
#endregion usings

namespace Roslynator.Tests
{
    internal static class Program
    {
        internal static async Task Main()
        {
            const string source = @"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }   
}
";

            Project project = new AdhocWorkspace()
                .CurrentSolution
                .AddProject("TestProject", "TestProject", LanguageNames.CSharp)
                .WithMetadataReferences(
                    new MetadataReference[]
                    {
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Core.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Linq.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Linq.Expressions.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Runtime.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Collections.Immutable.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("Microsoft.CodeAnalysis.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("Microsoft.CodeAnalysis.CSharp.dll"),
                    });

            var parseOptions = (CSharpParseOptions)project.ParseOptions;

            Document document = project
                .WithParseOptions(parseOptions.WithLanguageVersion(LanguageVersion.Latest))
                .AddDocument("Test.cs", SourceText.From(source));

            SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
            SyntaxTree tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
            SyntaxNode root = await tree.GetRootAsync().ConfigureAwait(false);

            string s = document.GetSyntaxRootAsync().Result.ToFullString();
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}
