// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1050AddOrRemoveParenthesesWhenCreatingNewObjectTests : AbstractCSharpDiagnosticVerifier<AddOrRemoveParenthesesWhenCreatingNewObjectAnalyzer, AddOrRemoveParenthesesWhenCreatingNewObjectCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.IncludeParenthesesWhenCreatingNewObject;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.IncludeParenthesesWhenCreatingNewObject)]
        public async Task Test_AddParentheses()
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
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Include));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.IncludeParenthesesWhenCreatingNewObject)]
        public async Task Test_RemoveParentheses()
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
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Omit));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.IncludeParenthesesWhenCreatingNewObject)]
        public async Task TestNoDiagnostic_AddParentheses()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>() { ""a"", ""b"", ""c"" };
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Include));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.IncludeParenthesesWhenCreatingNewObject)]
        public async Task TestNoDiagnostic_RemoveParentheses()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>() { ""a"", ""b"", ""c"" };
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Include));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.IncludeParenthesesWhenCreatingNewObject)]
        public async Task TestNoDiagnostic_RemoveParentheses_NoInitializer()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

public class C
{
    List<string> items = new List<string>();
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Omit));
        }
    }
}
