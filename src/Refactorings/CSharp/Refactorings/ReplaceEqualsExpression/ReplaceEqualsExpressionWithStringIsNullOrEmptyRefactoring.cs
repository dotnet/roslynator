// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.ReplaceEqualsExpression
{
    internal sealed class ReplaceEqualsExpressionWithStringIsNullOrEmptyRefactoring : ReplaceEqualsExpressionRefactoring
    {
        private ReplaceEqualsExpressionWithStringIsNullOrEmptyRefactoring()
        {
        }

        public static ReplaceEqualsExpressionWithStringIsNullOrEmptyRefactoring Instance { get; } = new ReplaceEqualsExpressionWithStringIsNullOrEmptyRefactoring();

        public override string MethodName
        {
            get { return "IsNullOrEmpty"; }
        }

        public override string GetEquivalenceKey()
        {
            return RefactoringIdentifiers.ReplaceEqualsExpressionWithStringIsNullOrEmpty;
        }
    }
}