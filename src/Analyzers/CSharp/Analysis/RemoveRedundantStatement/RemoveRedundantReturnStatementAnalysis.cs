// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal class RemoveRedundantReturnStatementAnalysis : RemoveRedundantStatementAnalysis<ReturnStatementSyntax>
    {
        protected override bool IsFixable(ReturnStatementSyntax statement)
        {
            return statement.Expression == null
                && base.IsFixable(statement);
        }

        protected override bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind)
        {
            if (!parentKind.Is(
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.DestructorDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.SetAccessorDeclaration,
                SyntaxKind.LocalFunctionStatement))
            {
                return false;
            }

            if (parentKind == SyntaxKind.MethodDeclaration)
                return ((MethodDeclarationSyntax)block.Parent).ReturnType?.IsVoid() == true;

            if (parentKind == SyntaxKind.LocalFunctionStatement)
                return ((LocalFunctionStatementSyntax)block.Parent).ReturnType?.IsVoid() == true;

            return true;
        }
    }
}
