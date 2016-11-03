// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddIdentifierToLocalDeclarationRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (context.SupportsSemanticModel)
            {
                TypeSyntax type = localDeclaration.Declaration?.Type;

                if (type?.IsVar == false)
                {
                    VariableDeclaratorSyntax declarator = localDeclaration.Declaration.Variables.FirstOrDefault();

                    if (declarator != null
                        && context.Span.Start >= type.Span.Start)
                    {
                        SyntaxTriviaList triviaList = type.GetTrailingTrivia();

                        if (triviaList.Any())
                        {
                            SyntaxTrivia trivia = triviaList
                                .SkipWhile(f => f.IsKind(SyntaxKind.WhitespaceTrivia))
                                .FirstOrDefault();

                            if (trivia.IsKind(SyntaxKind.EndOfLineTrivia)
                                && context.Span.End <= trivia.Span.Start)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type, context.CancellationToken).Type;

                                if (typeSymbol?.IsErrorType() == false)
                                {
                                    string name = SyntaxUtility.CreateIdentifier(typeSymbol, firstCharToLower: true);

                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        context.RegisterRefactoring(
                                            $"Add identifier '{name}'",
                                            c => RefactorAsync(context.Document, declarator, type, name, c));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionStatementSyntax expressionStatement)
        {
            if (context.SupportsSemanticModel)
            {
                var expression = expressionStatement.Expression as TypeSyntax;

                if (expression != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol typeSymbol = semanticModel
                        .GetTypeInfo(expression, context.CancellationToken).Type;

                    if (typeSymbol?.IsErrorType() == false)
                    {
                        string name = SyntaxUtility.CreateIdentifier(typeSymbol, firstCharToLower: true);

                        if (!string.IsNullOrEmpty(name))
                        {
                            context.RegisterRefactoring(
                                $"Add identifier '{name}'",
                                c => RefactorAsync(context.Document, expressionStatement, name, c));
                        }
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax declarator,
            TypeSyntax type,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SyntaxTrivia endOfLine = type.GetTrailingTrivia()
                .SkipWhile(f => f.IsKind(SyntaxKind.WhitespaceTrivia))
                .First();

            TextSpan span = TextSpan.FromBounds(type.Span.End, endOfLine.Span.Start);

            var textChange = new TextChange(span, " " + name);

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            LocalDeclarationStatementSyntax newNode = LocalDeclarationStatement(
                VariableDeclaration(
                    (TypeSyntax)expressionStatement.Expression,
                    VariableDeclarator(name)));

            if (expressionStatement.SemicolonToken.IsMissing)
            {
                newNode = newNode
                    .WithSemicolonToken(expressionStatement.SemicolonToken)
                    .WithTriviaFrom(expressionStatement.Expression);
            }
            else
            {
                newNode = newNode.WithTriviaFrom(expressionStatement);
            }

            root = root.ReplaceNode(expressionStatement, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}
