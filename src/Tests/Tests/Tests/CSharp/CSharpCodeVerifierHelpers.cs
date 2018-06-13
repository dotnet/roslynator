// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Roslynator.RuntimeMetadataReference;

namespace Roslynator.Tests.CSharp
{
    internal static class CSharpCodeVerifierHelpers
    {
        private static Project _defaultProject;

        public static Project DefaultProject
        {
            get { return _defaultProject ?? (_defaultProject = CreateDefaultProject()); }
        }

        private static Project CreateDefaultProject()
        {
            Project project = new AdhocWorkspace()
            .CurrentSolution
            .AddProject("TestProject", "TestAssembly", LanguageNames.CSharp)
            .WithMetadataReferences(ImmutableArray.Create(
                CorLibReference,
                CreateFromAssemblyName("System.Core.dll"),
                CreateFromAssemblyName("System.Linq.dll"),
                CreateFromAssemblyName("System.Linq.Expressions.dll"),
                CreateFromAssemblyName("System.Runtime.Serialization.Formatters.dll"),
                CreateFromAssemblyName("System.Runtime.dll"),
                CreateFromAssemblyName("System.Collections.dll"),
                CreateFromAssemblyName("System.Collections.Immutable.dll"),
                CreateFromAssemblyName("System.Text.RegularExpressions.dll"),
                CreateFromAssemblyName("Microsoft.CodeAnalysis.dll"),
                CreateFromAssemblyName("Microsoft.CodeAnalysis.CSharp.dll")));

            var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

            CSharpCompilationOptions newCompilationOptions = compilationOptions
                .WithAllowUnsafe(true)
                .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

            var parseOptions = (CSharpParseOptions)project.ParseOptions;

            CSharpParseOptions newParseOptions = parseOptions
                .WithLanguageVersion(LanguageVersion.Latest);

            return project
                .WithCompilationOptions(newCompilationOptions)
                .WithParseOptions(newParseOptions);
        }

        public static string CreateFileName(int index = 0)
        {
            return (index == 0) ? "Test.cs" : Path.ChangeExtension("Test" + (index + 1), "cs");
        }
    }
}
