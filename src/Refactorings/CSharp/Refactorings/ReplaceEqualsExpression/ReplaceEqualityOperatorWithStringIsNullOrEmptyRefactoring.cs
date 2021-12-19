// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.ReplaceEqualsExpression
{
    internal sealed class ReplaceEqualityOperatorWithStringIsNullOrEmptyRefactoring : ReplaceEqualityOperatorRefactoring
    {
        private ReplaceEqualityOperatorWithStringIsNullOrEmptyRefactoring()
        {
        }

        public static ReplaceEqualityOperatorWithStringIsNullOrEmptyRefactoring Instance { get; } = new();

        public override string MethodName
        {
            get { return "IsNullOrEmpty"; }
        }

        public override RefactoringDescriptor GetDescriptor()
        {
            return RefactoringDescriptors.ReplaceEqualityOperatorWithStringIsNullOrEmpty;
        }
    }
}
