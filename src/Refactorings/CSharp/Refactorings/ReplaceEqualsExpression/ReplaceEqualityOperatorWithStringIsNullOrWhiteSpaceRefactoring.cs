// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.ReplaceEqualsExpression
{
    internal sealed class ReplaceEqualityOperatorWithStringIsNullOrWhiteSpaceRefactoring : ReplaceEqualityOperatorRefactoring
    {
        private ReplaceEqualityOperatorWithStringIsNullOrWhiteSpaceRefactoring()
        {
        }

        public static ReplaceEqualityOperatorWithStringIsNullOrWhiteSpaceRefactoring Instance { get; } = new ReplaceEqualityOperatorWithStringIsNullOrWhiteSpaceRefactoring();

        public override string MethodName
        {
            get { return "IsNullOrWhiteSpace"; }
        }

        public override string GetEquivalenceKey()
        {
            return RefactoringIdentifiers.ReplaceEqualityOperatorWithStringIsNullOrWhiteSpace;
        }
    }
}
