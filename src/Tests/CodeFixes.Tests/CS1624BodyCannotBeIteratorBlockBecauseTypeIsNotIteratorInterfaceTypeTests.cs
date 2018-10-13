// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS1624BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceTypeTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public CS1624BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceTypeTests()
        {
            Options = base.Options.AddAllowedCompilerDiagnosticId(CompilerDiagnosticIdentifiers.CannotImplicitlyConvertType);
        }

        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceType;

        public override CodeFixProvider FixProvider { get; } = new MethodDeclarationOrLocalFunctionStatementCodeFixProvider();

        public override CodeVerificationOptions Options { get; }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceType)]
        public async Task Test_Method_String()
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    void M()
    {
        yield return default(string);
        yield return DateTime.Now;
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        yield return default(string);
        yield return DateTime.Now;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, "string"));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceType)]
        public async Task Test_LocalFunction_String()
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    void M()
    {
        void LF()
        {
            yield return default(string);
            yield return DateTime.Now;
        }
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    void M()
    {
        IEnumerable<string> LF()
        {
            yield return default(string);
            yield return DateTime.Now;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, "string"));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceType)]
        public async Task Test_Method_DateTime()
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    void M()
    {
        yield return default(string);
        yield return DateTime.Now;
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    IEnumerable<DateTime> M()
    {
        yield return default(string);
        yield return DateTime.Now;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, "DateTime"));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceType)]
        public async Task Test_LocalFunction_DateTime()
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    void M()
    {
        void LF()
        {
            yield return default(string);
            yield return DateTime.Now;
        }
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    void M()
    {
        IEnumerable<DateTime> LF()
        {
            yield return default(string);
            yield return DateTime.Now;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, "DateTime"));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceType)]
        public async Task TestNoFix()
        {
            await VerifyNoFixAsync(@"
class C
{
    void M()
    {
        yield return ;

        void LF()
        {
            yield return ;
        }
    }

    void M()
    {
        yield break;

        void LF()
        {
            yield break;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
