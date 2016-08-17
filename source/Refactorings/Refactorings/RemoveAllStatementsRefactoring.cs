// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class RemoveAllStatementsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                    {
                        if (CanRefactor(context, member))
                        {
                            context.RegisterRefactoring(
                                "Remove all statements",
                                cancellationToken => RefactorAsync(context.Document, member, cancellationToken));
                        }

                        break;
                    }
            }
        }

        public static bool CanRefactor(RefactoringContext context, MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)member;

                        return declaration.Body != null
                            && declaration.Body.Statements.Count > 0
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var declaration = (OperatorDeclarationSyntax)member;

                        return declaration.Body != null
                            && declaration.Body.Statements.Count > 0
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var declaration = (ConversionOperatorDeclarationSyntax)member;

                        return declaration.Body != null
                            && declaration.Body.Statements.Count > 0
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var declaration = (ConstructorDeclarationSyntax)member;

                        return declaration.Body != null
                            && declaration.Body.Statements.Count > 0
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax newNode = RemoveAllStatements(member);

            root = root.ReplaceNode(member, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static MemberDeclarationSyntax RemoveAllStatements(MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)member;

                        return declaration.WithBody(
                            declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var declaration = (OperatorDeclarationSyntax)member;

                        return declaration.WithBody(
                            declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var declaration = (ConversionOperatorDeclarationSyntax)member;

                        return declaration.WithBody(
                            declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var declaration = (ConstructorDeclarationSyntax)member;

                        return declaration.WithBody(
                            declaration.Body.WithStatements(List<StatementSyntax>()));
                    }
            }

            return member;
        }
    }
}
