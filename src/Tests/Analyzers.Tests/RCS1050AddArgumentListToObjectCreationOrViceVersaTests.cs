// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1050AddArgumentListToObjectCreationOrViceVersaTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddArgumentListToObjectCreationOrViceVersa;

        protected override DiagnosticAnalyzer Analyzer { get; } = new AddArgumentListToObjectCreationOrViceVersaAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AddArgumentListToObjectCreationOrViceVersaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddArgumentListToObjectCreationOrViceVersa)]
        public async Task Test_AddArgumentList()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>[||] { ""a"", ""b"", ""c"" };
}
", @"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>() { ""a"", ""b"", ""c"" };
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddArgumentListToObjectCreationOrViceVersa)]
        public async Task Test_RemoveArgumentList()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>[|()|] { ""a"", ""b"", ""c"" };
}
", @"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string> { ""a"", ""b"", ""c"" };
}
", options: Options.WithEnabled(AnalyzerOptions.RemoveArgumentListFromObjectCreation));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddArgumentListToObjectCreationOrViceVersa)]
        public async Task TestNoDiagnostic_AddArgumentList()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>() { ""a"", ""b"", ""c"" };
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddArgumentListToObjectCreationOrViceVersa)]
        public async Task TestNoDiagnostic_RemoveArgumentList()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>() { ""a"", ""b"", ""c"" };
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddArgumentListToObjectCreationOrViceVersa)]
        public async Task TestNoDiagnostic_RemoveArgumentList_NoInitializer()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>();
}
", options: Options.WithEnabled(AnalyzerOptions.RemoveArgumentListFromObjectCreation));
        }
    }
}