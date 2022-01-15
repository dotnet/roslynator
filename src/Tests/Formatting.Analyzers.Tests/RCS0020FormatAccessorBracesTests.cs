// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0020FormatAccessorBracesTests : AbstractCSharpDiagnosticVerifier<FormatAccessorBracesAnalyzer, AccessorDeclarationCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FormatAccessorBraces;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_Getter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get [|{|] return _p; }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_MultiLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_Getter_ToSingleLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get
        [|{|]
            return _p;
        }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get { return _p; }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_SingleLineWhenExpressionIsOnSingleLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_Setter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        set [|{|] _p = value; }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        set
        {
            _p = value;
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_MultiLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_InitSetter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p;
        }

        init [|{|] _p = value; }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p;
        }

        init
        {
            _p = value;
        }
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS0518")
                .AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_MultiLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_Event()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    event EventHandler E
    {
        add [|{|] }
        remove [|{|] }
    }
}
", @"
using System;

class C
{
    event EventHandler E
    {
        add
        {
        }
        remove
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_MultiLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_Event_ToSingleLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    event EventHandler E
    {
        add
        [|{|]
        }
        remove
        [|{|]
        }
    }
}
", @"
using System;

class C
{
    event EventHandler E
    {
        add { }
        remove { }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_SingleLineWhenExpressionIsOnSingleLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_AllowSingeLineGetter()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string _p;

    string P
    {
        get { return _p; }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_SingleLineWhenExpressionIsOnSingleLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task Test_MultiLineStatement()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p
                .ToString();
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.AccessorBracesStyle, ConfigOptionValues.AccessorBracesStyle_SingleLineWhenExpressionIsOnSingleLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBraces)]
        public async Task TestNoDiagnostic_AutoProperty()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string P { get; set; }
}
");
        }
    }
}
