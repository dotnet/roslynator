// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1050AddArgumentListToObjectCreationTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddArgumentListToObjectCreation;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddArgumentListToObjectCreationAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AddArgumentListToObjectCreationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddArgumentListToObjectCreation)]
        public async Task Test_List()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>[| |]{ ""a"", ""b"", ""c"" };
}
", @"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>() { ""a"", ""b"", ""c"" };
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddArgumentListToObjectCreation)]
        public async Task TestNoDiagnostic_ArgumentListPresent()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>() { ""a"", ""b"", ""c"" };
}
");
        }
    }
}