// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Testing.CSharp;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public abstract class AbstractCSharpCompilerDiagnosticFixVerifier : CSharpCompilerDiagnosticFixVerifier
    {
        protected AbstractCSharpCompilerDiagnosticFixVerifier() : base(XunitAssert.Instance)
        {
        }
    }
}
