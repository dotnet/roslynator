// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                cancellationToken => RefactorAsync(context.Document, member, cancellationToken));
        }

        public static bool CanRefactor(MemberDeclarationSyntax member, TextSpan span)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        BlockSyntax body = ((MethodDeclarationSyntax)member).Body;

                        return body?.Statements.Any() == true
                            && BraceContainsSpan(body, span);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        BlockSyntax body = ((OperatorDeclarationSyntax)member).Body;

                        return body?.Statements.Any() == true
                            && BraceContainsSpan(body, span);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        BlockSyntax body = ((ConversionOperatorDeclarationSyntax)member).Body;

                        return body?.Statements.Any() == true
                            && BraceContainsSpan(body, span);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        BlockSyntax body = ((ConstructorDeclarationSyntax)member).Body;

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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberDeclarationSyntax newNode = RemoveAllStatements(member);

            return document.ReplaceNodeAsync(member, newNode, cancellationToken);
        }

        private static MemberDeclarationSyntax RemoveAllStatements(MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)member;

                        return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var declaration = (OperatorDeclarationSyntax)member;

                        return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var declaration = (ConversionOperatorDeclarationSyntax)member;

                        return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var declaration = (ConstructorDeclarationSyntax)member;

                        return declaration.WithBody(declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
            }

            return member;
        }
    }
}
