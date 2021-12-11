// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnsafeStatementRefactoring
    {
        internal static void ComputeRefactorings(RefactoringContext context, UnsafeStatementSyntax node)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.MoveUnsafeContextToContainingDeclaration))
                MoveUnsafeContextToContainingDeclarationRefactoring.ComputeRefactoring(context, node);
        }
    }
}
