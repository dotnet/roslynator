// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class AnalyzerTestGenerator
    {
        public static CompilationUnitSyntax Generate(AnalyzerDescriptor analyzer, string className)
        {
            string s = _sourceTemplate
                .Replace("$ClassName$", className)
                .Replace("$Identifier$", analyzer.Identifier);

            return ParseCompilationUnit(s);
        }

        private const string _sourceTemplate = @"
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class $ClassName$ : AbstractCSharpCodeFixVerifier
    {
        public $ClassName$()
        {
            Options = base.Options;
        }

        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.$Identifier$;

        public override DiagnosticAnalyzer Analyzer { get; } = new $Identifier$Analyzer();

        public override CodeFixProvider FixProvider { get; } = new $Identifier$CodeFixProvider();

        public override CodeVerificationOptions Options { get; }

        //[Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.$Identifier$)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@""
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
    {
        void M()
        {
        }
    }
"", @""
"");
        }

        //[Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.$Identifier$)]
        //[InlineData("""", """")]
        public async Task Test2(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@""
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
"", fromData, toData);
        }

        //[Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.$Identifier$)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@""
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
"");
        }

        //[Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.$Identifier$)]
        //[InlineData("""")]
        public async Task TestNoDiagnostic2(string fromData)
        {
            await VerifyNoDiagnosticAsync(@""
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
"", fromData);
        }
    }
}
";
    }
}
