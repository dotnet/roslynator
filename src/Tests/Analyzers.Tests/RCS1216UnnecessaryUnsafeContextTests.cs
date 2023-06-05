using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1216UnnecessaryUnsafeContextTests : AbstractCSharpDiagnosticVerifier<UnnecessaryUnsafeContextAnalyzer, UnnecessaryUnsafeContextCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnnecessaryUnsafeContext;

    [Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    [Theory]
    [InlineData(@"class")]
    [InlineData(@"interface")]
    [InlineData(@"record")]
    [InlineData(@"struct")]
    [InlineData(@"record struct")]
    public async Task Test_Type(string typeKeyword)
    {
        var source = @"
    unsafe <type_keyword> C
    {
        void M()
        {
            [|unsafe|] {
                var x = 1;
            }
        }
    }
".Replace("<type_keyword>", typeKeyword);
        var expected = @"
    unsafe <type_keyword> C
    {
        void M()
        {
            {
                var x = 1;
            }
        }
    }
".Replace("<type_keyword>", typeKeyword);
        await VerifyDiagnosticAndFixAsync(source, expected, options: Options.WithAllowUnsafe(true));
    }

    [Theory]
    [InlineData(@"void M()")]
    [InlineData(@"C()")]
    [InlineData(@"static void M()")]
    public async Task Test_Member(string memberSignature)
    {
        var source = @"
    class C
    {
        unsafe <member_signature>
        {
            [|unsafe|] {
                var x = 1;
            }
        }
    }
".Replace("<member_signature>", memberSignature);
        var expected = @"
    class C
    {
        unsafe <member_signature>
        {
            {
                var x = 1;
            }
        }
    }
".Replace("<member_signature>", memberSignature);
        await VerifyDiagnosticAndFixAsync(source, expected, options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_UnsafeLocalFunction()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        unsafe void M()
        {
            for(int y = 0; y < 10; y ++){
                [|unsafe|] void M2() {
                    var x = 1;
                }
            }
        }
    }
", @"
    class C
    {
        unsafe void M()
        {
            for(int y = 0; y < 10; y ++){
                void M2() {
                    var x = 1;
                }
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_UnsafeBlock()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        void M()
        {
            unsafe {
                [|unsafe|] {
                    var x = 1;
                }
            }
        }
    }
", @"
    class C
    {
        void M()
        {
            unsafe {
                {
                    var x = 1;
                }
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Property()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        unsafe string X
        {
            get {
                [|unsafe|] {
                    var x = 1;
                }
                return ""1"";
            }
        }
    }
", @"
    class C
    {
        unsafe string X
        {
            get {
                {
                    var x = 1;
                }
                return ""1"";
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Operator()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        public unsafe static C operator +(C c1, C c2) 
        {
            [|unsafe|] {
                var x = 1;
            }
            return c1;
        }
    }
", @"
    class C
    {
        public unsafe static C operator +(C c1, C c2) 
        {
            {
                var x = 1;
            }
            return c1;
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Indexer()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        unsafe string this[int i]
        {
            get {
                [|unsafe|] {
                    var x = 1;
                }
                return ""1"";
            }
        }
    }
", @"
    class C
    {
        unsafe string this[int i]
        {
            get {
                {
                    var x = 1;
                }
                return ""1"";
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }



    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_NoDiagnostic_UnwrappedUnsafeBlock()
    {
        await VerifyNoDiagnosticAsync(@"
    class C
    {
        void M()
        {
            unsafe {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }


}