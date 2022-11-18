// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0060AddEmptyLineAfterFileScopedNamespaceTests : AbstractCSharpDiagnosticVerifier<AddEmptyLineAfterFileScopedNamespaceAnalyzer, FileScopedNamespaceDeclarationCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddEmptyLineAfterFileScopedNamespace;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace A.B;
[||]class C
{
}
", @"
namespace A.B;

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace)]
        public async Task Test2()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  
[||]class C
{
}
", @"
namespace A.B;  

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace)]
        public async Task Test3()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  //x
[||]class C
{
}
", @"
namespace A.B;  //x

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace)]
        public async Task Test4()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace A.B;//x
[||]class C
{
}
", @"
namespace A.B;//x

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace)]
        public async Task Test5()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace A.B;[||]class C
{
}
", @"
namespace A.B;

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace)]
        public async Task Test6()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  [||]class C
{
}
", @"
namespace A.B;  

class C
{
}
");
        }
    }
}
