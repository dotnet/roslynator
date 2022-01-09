// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0014AddBlankLineBetweenSwitchSectionsTests : AbstractCSharpDiagnosticVerifier<AddBlankLineBetweenSwitchSectionsAnalyzer, SwitchSectionCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddBlankLineBetweenSwitchSections;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return A();[||]
            case ""b"":
                return B();[||]
            default:
                return null;
        }
    }

    public string A() => null;
    public string B() => null;
}
", @"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return A();

            case ""b"":
                return B();

            default:
                return null;
        }
    }

    public string A() => null;
    public string B() => null;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task Test_ClosingBraceAndSection()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    return """";
                }[||]
            case ""b"":
                return """";[||]
            default:
                return null;
        }
    }

    public string A() => null;
    public string B() => null;
}
", @"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    return """";
                }

            case ""b"":
                return """";

            default:
                return null;
        }
    }

    public string A() => null;
    public string B() => null;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task Test_ClosingBraceAndSection2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    return """";
                }[||]
            case ""b"":
                return """";
        }

        return null;
    }
}
", @"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    return """";
                }

            case ""b"":
                return """";
        }

        return null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenClosingBraceAndSwitchSection, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task Test_ClosingBraceAndSection3()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return """";[||]
            case ""b"":
                return """";
        }

        return null;
    }
}
", @"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return """";

            case ""b"":
                return """";
        }

        return null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenClosingBraceAndSwitchSection, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task TestNoDiagnostic_NoEmptyLineBetweenClosingBraceAndSwitchSection()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    return """";
                }
            case ""b"":
                return """";
        }

        return null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenClosingBraceAndSwitchSection, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task TestNoDiagnostic_NoEmptyLineBetweenClosingBraceAndSwitchSection2()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    return """";
                }

            case ""b"":
                return """";
        }

        return null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenClosingBraceAndSwitchSection, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task TestNoDiagnostic_NoEmptyLineBetweenClosingBraceAndSwitchSection3()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return """";

            case ""b"":
                return """";
        }

        return null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenClosingBraceAndSwitchSection, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task Test_Comment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return A(); //x[||]
            default:
                return null;
        }
    }

    public string A() => null;
}
", @"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return A(); //x

            default:
                return null;
        }
    }

    public string A() => null;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections)]
        public async Task TestNoDiagnostic_SingleSection()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                return A();
        }

        return null;
    }

    public string A() => null;
}
");
        }
    }
}
