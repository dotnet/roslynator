// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
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

            if (options == null)
                options = VisualBasicCodeVerificationOptions.Default;

            return project
                .WithCompilationOptions(options.CompilationOptions)
                .WithParseOptions(options.ParseOptions);
        }
    }
}
