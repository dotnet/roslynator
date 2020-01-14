// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Roslynator.Testing;

namespace Roslynator.CSharp.Testing
{
    internal class CSharpWorkspaceFactory : WorkspaceFactory
    {
        internal static CSharpWorkspaceFactory Instance { get; } = new CSharpWorkspaceFactory();

        public override string Language => LanguageNames.CSharp;

        public override string DefaultDocumentName => "Test.cs";

        public override Project AddProject(Solution solution, CodeVerificationOptions options)
        {
            return base.AddProject(solution, options)
                .WithCompilationOptions(options.CompilationOptions)
                .WithParseOptions(options.ParseOptions);
        }
    }
}
