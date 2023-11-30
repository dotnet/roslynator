// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS0061BlankLineBetweenSwitchSectionsTests : AbstractCSharpDiagnosticVerifier<BlankLineBetweenSwitchSectionsAnalyzer, SwitchSectionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.BlankLineBetweenSwitchSections;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineBetweenSwitchSections)]
    public async Task Test_Statement_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
                return ""a"";[||]
            case ""b"":
                return ""b"";[||]
            default:
                return null;
        }
    }
}
", @"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
                return ""a"";

            case ""b"":
                return ""b"";

            default:
                return null;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenSwitchSections, ConfigOptionValues.BlankLineBetweenSwitchSections_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineBetweenSwitchSections)]
    public async Task Test_Statement_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
                return ""a"";
[||]
            case ""b"":
                return ""b"";
[||]
            default:
                return null;
        }
    }
}
", @"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
                return ""a"";
            case ""b"":
                return ""b"";
            default:
                return null;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenSwitchSections, ConfigOptionValues.BlankLineBetweenSwitchSections_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineBetweenSwitchSections)]
    public async Task Test_Statement_OmitAfterBlock()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }
[||]
            case ""b"":
            {
                return ""b"";
            }
[||]
            default:
                return null;
        }
    }
}
", @"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }
            case ""b"":
            {
                return ""b"";
            }
            default:
                return null;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenSwitchSections, ConfigOptionValues.BlankLineBetweenSwitchSections_OmitAfterBlock));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineBetweenSwitchSections)]
    public async Task Test_Block_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }[||]
            case ""b"":
            {
                return ""b"";
            }[||]
            default:
                return null;
        }
    }
}
", @"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }

            case ""b"":
            {
                return ""b"";
            }

            default:
                return null;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenSwitchSections, ConfigOptionValues.BlankLineBetweenSwitchSections_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineBetweenSwitchSections)]
    public async Task Test_Block_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }
[||]
            case ""b"":
            {
                return ""b"";
            }
[||]
            default:
                return null;
        }
    }
}
", @"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }
            case ""b"":
            {
                return ""b"";
            }
            default:
                return null;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenSwitchSections, ConfigOptionValues.BlankLineBetweenSwitchSections_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineBetweenSwitchSections)]
    public async Task Test_Block_OmitAfterBlock()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }
[||]
            case ""b"":
            {
                return ""b"";
            }
[||]
            default:
                return null;
        }
    }
}
", @"
class C
{
    string M()
    {
        string s = string.Empty;
        switch (s)
        {
            case ""a"":
            {
                return ""a"";
            }
            case ""b"":
            {
                return ""b"";
            }
            default:
                return null;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineBetweenSwitchSections, ConfigOptionValues.BlankLineBetweenSwitchSections_OmitAfterBlock));
    }
}
