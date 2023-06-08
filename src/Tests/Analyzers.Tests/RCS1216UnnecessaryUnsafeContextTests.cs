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
    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Class()
    {
        await VerifyDiagnosticAndFixAsync(@"
    unsafe class C
    {
        void M()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    unsafe class C
    {
        void M()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Interface()
    {
        await VerifyDiagnosticAndFixAsync(@"
    unsafe interface C
    {
        void M()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    unsafe interface C
    {
        void M()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Record()
    {
        await VerifyDiagnosticAndFixAsync(@"
    unsafe record C
    {
        void M()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    unsafe record C
    {
        void M()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Struct()
    {
        await VerifyDiagnosticAndFixAsync(@"
    unsafe struct C
    {
        void M()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    unsafe struct C
    {
        void M()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_RecordStruct()
    {
        await VerifyDiagnosticAndFixAsync(@"
    unsafe record struct C
    {
        void M()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    unsafe record struct C
    {
        void M()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Method()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        unsafe void M()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    class C
    {
        unsafe void M()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_Constructor()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        unsafe C()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    class C
    {
        unsafe C()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_StaticMember()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        unsafe static void M()
        {
            [|unsafe|]
            {
                var x = 1;
            }
        }
    }
", @"
    class C
    {
        unsafe static void M()
        {
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryUnsafeContext)]
    public async Task Test_UnsafeLocalFunction()
    {
        await VerifyDiagnosticAndFixAsync(@"
    class C
    {
        unsafe void M()
        {
            for(int y = 0; y < 10; y ++)
            {
                [|unsafe|] void M2()
                {
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
            for(int y = 0; y < 10; y ++)
            {
                void M2()
                {
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
            unsafe
            {
                [|unsafe|]
                {
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
            unsafe
            {
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
                [|unsafe|]
                {
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
            [|unsafe|]
            {
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
                [|unsafe|]
                {
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
            unsafe
            {
                var x = 1;
            }
        }
    }
", options: Options.WithAllowUnsafe(true));
    }
}