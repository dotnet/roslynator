// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Roslynator.Tests;

namespace Roslynator.VisualBasic.Tests
{
    public class VisualBasicWorkspaceFactory : WorkspaceFactory
    {
        internal static VisualBasicWorkspaceFactory Instance { get; } = new VisualBasicWorkspaceFactory();

        public override string Language => LanguageNames.VisualBasic;

        public override string DefaultDocumentName => "Test.vb";

        public override Project AddProject(Solution solution, CodeVerificationOptions options = null)
        {
            Project project = base.AddProject(solution, options);

            var compilationOptions = (VisualBasicCompilationOptions)project.CompilationOptions;

            VisualBasicCompilationOptions newCompilationOptions = compilationOptions
                .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

            var parseOptions = (VisualBasicParseOptions)project.ParseOptions;

            VisualBasicCodeVerificationOptions visualBasicOptions = (options != null)
                ? ((VisualBasicCodeVerificationOptions)options)
                : VisualBasicCodeVerificationOptions.Default;

            VisualBasicParseOptions newParseOptions = parseOptions
                .WithLanguageVersion(visualBasicOptions.LanguageVersion);

            return project
                .WithCompilationOptions(newCompilationOptions)
                .WithParseOptions(newParseOptions);
        }
    }
}
