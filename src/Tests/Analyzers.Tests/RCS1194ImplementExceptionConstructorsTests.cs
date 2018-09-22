// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1194ImplementExceptionConstructorsTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.ImplementExceptionConstructors;

        public override DiagnosticAnalyzer Analyzer { get; } = new ImplementExceptionConstructorsAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ClassDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ImplementExceptionConstructors)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class [|C|] : Exception
{
}
", @"
using System;

class C : Exception
{
    public C() : base()
    {
    }

    public C(string message) : base(message)
    {
    }

    public C(string message, Exception innerException) : base(message, innerException)
    {
    }
}
");
        }
    }
}
