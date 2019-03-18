// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class CodeFixTestGenerator
    {
        public static CompilationUnitSyntax Generate(CompilerDiagnosticMetadata compilerDiagnostic, string className)
        {
            string s = _sourceTemplate
                .Replace("$ClassName$", className)
                .Replace("$Id$", compilerDiagnostic.Id)
                .Replace("$Identifier$", compilerDiagnostic.Identifier);

            return ParseCompilationUnit(s);
        }

        private const string _sourceTemplate = @"
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    //TODO: Add tests for $Id$
    public class $ClassName$ : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.$Identifier$;

        public override CodeFixProvider FixProvider { get; } = new $Identifier$CodeFixProvider();

        //[Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.$Identifier$)]
        public async Task Test()
        {
            await VerifyFixAsync(@""
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
"", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        //[Theory, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.$Identifier$)]
        //[InlineData("""", """")]
        public async Task Test(string fromData, string toData)
        {
            await VerifyFixAsync(@""
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
"", fromData, toData, equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        //[Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.$Identifier$)]
        public async Task TestNoFix()
        {
            await VerifyNoFixAsync(@""
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
"", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
";
    }
}
