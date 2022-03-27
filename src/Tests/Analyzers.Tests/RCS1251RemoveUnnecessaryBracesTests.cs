// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    //TODO: remove double diagnostic (https://github.com/dotnet/roslyn/issues/53136)
    public class RCS1251RemoveUnnecessaryBracesTests : AbstractCSharpDiagnosticVerifier<RemoveUnnecessaryBracesAnalyzer, RecordDeclarationCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveUnnecessaryBraces;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
        public async Task Test_Record()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    record R(string Value)
    [|[|{|]|]
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
namespace N
{
    record R(string Value);
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
        public async Task Test_RecordStruct()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    record struct R(string Value)
    [|[|{|]|]
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
namespace N
{
    record struct R(string Value);
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
        public async Task Test_NoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
namespace N
{
    record struct R
    {
    }
}
");
        }
    }
}
