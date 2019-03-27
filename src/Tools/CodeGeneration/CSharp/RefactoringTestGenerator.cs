// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class RefactoringTestGenerator
    {
        public static CompilationUnitSyntax Generate(RefactoringMetadata refactoring, string className)
        {
            string s = _sourceTemplate
                .Replace("$ClassName$", className)
                .Replace("$Id$", refactoring.Id)
                .Replace("$Identifier$", refactoring.Identifier);

            return ParseCompilationUnit(s);
        }

        private const string _sourceTemplate = @"
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    //TODO: Add tests for $Id$
    public class $ClassName$ : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.$Identifier$;

        //[Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.$Identifier$)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@""
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
"", equivalenceKey: RefactoringId);
        }

        //[Theory, Trait(Traits.Refactoring, RefactoringIdentifiers.$Identifier$)]
        //[InlineData("""", """")]
        public async Task Test2(string fromData, string toData)
        {
            await VerifyRefactoringAsync(@""
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
"", fromData, toData, equivalenceKey: RefactoringId);
        }

        //[Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.$Identifier$)]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@""
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
"", equivalenceKey: RefactoringId);
        }
    }
}
";
    }
}
