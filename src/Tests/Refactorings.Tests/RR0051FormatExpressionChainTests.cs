// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0051FormatExpressionChainTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.FormatExpressionChain;

        [Theory, Trait(Traits.Refactoring, RefactoringIdentifiers.FormatExpressionChain)]
        [InlineData("[||]x.M().M()", @"x
                .M()
                .M()")]
        [InlineData("[||]x?.M()?.M()", @"x?
                .M()?
                .M()")]
        [InlineData("[||]x[0].M()[0].M()[0]", @"x[0]
                .M()[0]
                .M()[0]")]
        [InlineData("[||]x?[0]?.M()[0]?.M()[0]", @"x?[0]?
                .M()[0]?
                .M()[0]")]
        [InlineData("[||]x.P.P", @"x
                .P
                .P")]
        [InlineData("[||]x?.P?.P", @"x?
                .P?
                .P")]
        [InlineData("[||]x[0].P[0].P[0]", @"x[0]
                .P[0]
                .P[0]")]
        [InlineData("[||]x?[0]?.P[0]?.P[0]", @"x?[0]?
                .P[0]?
                .P[0]")]
        [InlineData("[||]x.M(x.M().M()).M(x.M().M())", @"x
                .M(x.M().M())
                .M(x.M().M())")]
        [InlineData("[||]this.M().M().M()", @"this.M()
                .M()
                .M()")]
        [InlineData("[||]A.B.Foo.SM().M()", @"A.B.Foo
                .SM()
                .M()")]
        public async Task TestRefactoring(string fixableCode, string fixedCode)
        {
            await VerifyRefactoringAsync(@"
namespace A.B
{
    class Foo
    {
        Foo M(Foo foo = null)
        {
            var x = new Foo();

            x = [||];
                
            return null;
        }

        public static Foo SM() => null;

        public Foo F;
        public Foo SF;

        public Foo P { get; }
        public Foo SP { get; }

        public Foo this[int index] => null;
    }
}
", fixableCode, fixedCode, equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.FormatExpressionChain)]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
namespace A.B
{
    class Foo
    {
        Foo M()
        {
            var x = new Foo();

            x = x.[||]M();
            x = x.[||]P;
            x = x.[||]P[0];

            x = x?.[||]M();
            x = x?.[||]P;
            x = x?.[||]P[0];

            x = x
                .M() //
                .[||]M();
                
            return null;
        }

        public Foo SM() => null;

        public Foo F;
        public Foo SF;

        public Foo P { get; }
        public Foo SP { get; }

        public Foo this[int index] => null;
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
