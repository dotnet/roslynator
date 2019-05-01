// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
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

            if (options == null)
                options = CSharpCodeVerificationOptions.Default;

            return project
                .WithCompilationOptions(options.CompilationOptions)
                .WithParseOptions(options.ParseOptions);
        }
    }
}
