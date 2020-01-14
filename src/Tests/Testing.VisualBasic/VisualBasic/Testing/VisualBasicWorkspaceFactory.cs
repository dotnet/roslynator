// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Roslynator.Testing;

namespace Roslynator.VisualBasic.Testing
{
    internal class VisualBasicWorkspaceFactory : WorkspaceFactory
    {
        internal static VisualBasicWorkspaceFactory Instance { get; } = new VisualBasicWorkspaceFactory();

        public override string Language => LanguageNames.VisualBasic;

        public override string DefaultDocumentName => "Test.vb";

        public override Project AddProject(Solution solution, CodeVerificationOptions options)
        {
            return base.AddProject(solution, options)
                .WithCompilationOptions(options.CompilationOptions)
                .WithParseOptions(options.ParseOptions);
        }
    }
}
