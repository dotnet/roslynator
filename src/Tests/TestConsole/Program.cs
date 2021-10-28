// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
#endregion usings

namespace Roslynator.Testing
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
            using (Workspace workspace = new AdhocWorkspace())
            {
                IEnumerable<PortableExecutableReference> metadataReferences = AppContext
                    .GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                    .ToString()
                    .Split(';')
                    .Select(f => MetadataReference.CreateFromFile(f));

                Project project = workspace.CurrentSolution
                    .AddProject("Test", "Test", LanguageNames.CSharp)
                    .WithMetadataReferences(metadataReferences);

                var compilationOptions = ((CSharpCompilationOptions)project.CompilationOptions);

                compilationOptions = ((CSharpCompilationOptions)project.CompilationOptions)
                    .WithAllowUnsafe(true)
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

                var parseOptions = ((CSharpParseOptions)project.ParseOptions);

                parseOptions = parseOptions
                    .WithLanguageVersion(LanguageVersion.CSharp8)
                    .WithPreprocessorSymbols(parseOptions.PreprocessorSymbolNames.Concat(new[] { "DEBUG" }));

                project = project
                    .WithCompilationOptions(compilationOptions)
                    .WithParseOptions(parseOptions);

                Document document = project.AddDocument("Document", SourceText.From(source));
                SemanticModel semanticModel = await document.GetSemanticModelAsync();
                SyntaxTree tree = await document.GetSyntaxTreeAsync();
                SyntaxNode root = await tree.GetRootAsync();

                string s = document.GetSyntaxRootAsync().Result.ToFullString();
                Console.WriteLine(s);
                Console.ReadKey();
            }
        }
    }
}
