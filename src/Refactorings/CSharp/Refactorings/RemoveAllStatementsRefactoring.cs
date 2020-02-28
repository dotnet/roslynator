// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveAllStatementsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (!CanRefactor(member, context.Span))
                return;

            context.RegisterRefactoring(
                "Remove all statements",
                cancellationToken => RefactorAsync(context.Document, member, cancellationToken),
                RefactoringIdentifiers.RemoveAllStatements);
        }

        public static bool CanRefactor(MemberDeclarationSyntax member, TextSpan span)
        {
            switch (member)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        BlockSyntax body = methodDeclaration.Body;

                        return body?.Statements.Any() == true
                            && BraceContainsSpan(body, span);
                    }
                case OperatorDeclarationSyntax operatorDeclaration:
                    {
                        BlockSyntax body = operatorDeclaration.Body;

                        return body?.Statements.Any() == true
                            && BraceContainsSpan(body, span);
                    }
                case ConversionOperatorDeclarationSyntax conversionOperatorDeclaration:
                    {
                        BlockSyntax body = conversionOperatorDeclaration.Body;

                        return body?.Statements.Any() == true
                            && BraceContainsSpan(body, span);
                    }
                case ConstructorDeclarationSyntax constructorDeclaration:
                    {
                        BlockSyntax body = constructorDeclaration.Body;

                        return body?.Statements.Any() == true
                            && BraceContainsSpan(body, span);
                    }
            }

            return false;
        }

        private static bool BraceContainsSpan(BlockSyntax body, TextSpan span)
        {
            return body.OpenBraceToken.Span.Contains(span)
                || body.CloseBraceToken.Span.Contains(span);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default)
        {
            MemberDeclarationSyntax newNode = RemoveAllStatements(member);

            return document.ReplaceNodeAsync(member, newNode, cancellationToken);
        }

        private static MemberDeclarationSyntax RemoveAllStatements(MemberDeclarationSyntax member)
        {
            switch (member)
            {
                case MethodDeclarationSyntax declaration:
                    return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
                case OperatorDeclarationSyntax declaration:
                    return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
                case ConversionOperatorDeclarationSyntax declaration:
                    return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
                case ConstructorDeclarationSyntax declaration:
                    return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
            }

            return member;
        }
    }
}
