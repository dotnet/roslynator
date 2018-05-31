// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Tests.CSharp
{
    public abstract class CSharpDiagnosticVerifier : DiagnosticVerifier
    {
        public override string Language
        {
            get { return LanguageNames.CSharp; }
        }

        public override CodeVerificationOptions Options
        {
            get { return CSharpCodeVerificationOptions.Default; }
        }

        protected override string CreateFileName(int index = 0)
        {
            return CSharpCodeVerifierHelpers.CreateFileName(index);
        }

        protected override Project CreateProject()
        {
            return CSharpCodeVerifierHelpers.DefaultProject;
        }
    }
}
