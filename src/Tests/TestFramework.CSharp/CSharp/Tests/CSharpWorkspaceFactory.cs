// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Tests;

namespace Roslynator.CSharp.Tests
{
    public class CSharpWorkspaceFactory : WorkspaceFactory
    {
        internal static CSharpWorkspaceFactory Instance { get; } = new CSharpWorkspaceFactory();

        public override string Language => LanguageNames.CSharp;

        public override string DefaultDocumentName => "Test.cs";

        public override Project AddProject(Solution solution, CodeVerificationOptions options = null)
        {
            Project project = base.AddProject(solution, options);

            var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

            CSharpCompilationOptions newCompilationOptions = compilationOptions
                .WithAllowUnsafe(true)
                .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

            var parseOptions = (CSharpParseOptions)project.ParseOptions;

            CSharpCodeVerificationOptions csharpOptions = (options != null)
                ? ((CSharpCodeVerificationOptions)options)
                : CSharpCodeVerificationOptions.Default;

            CSharpParseOptions newParseOptions = parseOptions
                .WithLanguageVersion(csharpOptions.LanguageVersion)
                .WithPreprocessorSymbols(parseOptions.PreprocessorSymbolNames.Concat(new string[] { "DEBUG" }));

            return project
                .WithCompilationOptions(newCompilationOptions)
                .WithParseOptions(newParseOptions);
        }
    }
}
