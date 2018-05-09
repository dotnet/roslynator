// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Roslynator.CSharp;

namespace Roslynator.Tests.CSharp
{
    public abstract class CSharpCompilerCodeFixVerifier : CompilerCodeFixVerifier
    {
        public override string Language
        {
            get { return LanguageNames.CSharp; }
        }

        protected override IWorkspaceFactory WorkspaceFactory
        {
            get { return CSharpWorkspaceFactory.Instance; }
        }
    }
}
