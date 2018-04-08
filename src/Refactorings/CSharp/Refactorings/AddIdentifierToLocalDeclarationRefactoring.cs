// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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
            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            TypeSyntax type = declaration?.Type;

            if (type?.IsVar != false)
                return;

            VariableDeclaratorSyntax declarator = declaration.Variables.FirstOrDefault();

            if (declarator == null)
                return;

            if (context.Span.Start < type.SpanStart)
                return;

            SyntaxTriviaList triviaList = type.GetTrailingTrivia();

            if (!triviaList.Any())
                return;

            SyntaxTrivia trivia = triviaList
                .SkipWhile(f => f.IsWhitespaceTrivia())
                .FirstOrDefault();

            if (!trivia.IsEndOfLineTrivia())
                return;

            if (context.Span.End > trivia.SpanStart)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            string name = NameGenerator.Default.CreateUniqueLocalName(
                typeSymbol,
                semanticModel,
                declarator.SpanStart,
                cancellationToken: context.CancellationToken);

            if (name == null)
                return;

            context.RegisterRefactoring(
                $"Add identifier '{name}'",
                c => RefactorAsync(context.Document, type, name, c));
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionStatementSyntax expressionStatement)
        {
            if (!(expressionStatement.Expression is TypeSyntax expression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetSymbol(expression, context.CancellationToken);

            if (symbol?.Kind != SymbolKind.NamedType)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            string name = NameGenerator.Default.CreateUniqueLocalName(
                typeSymbol,
                semanticModel,
                expression.SpanStart,
                cancellationToken: context.CancellationToken);

            if (name == null)
                return;

            context.RegisterRefactoring(
                $"Add identifier '{name}'",
                c => RefactorAsync(context.Document, expressionStatement, name, c));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            TypeSyntax type,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTrivia endOfLine = type.GetTrailingTrivia()
                .SkipWhile(f => f.IsWhitespaceTrivia())
                .First();

            TextSpan span = TextSpan.FromBounds(type.Span.End, endOfLine.SpanStart);

            var textChange = new TextChange(span, " " + name);

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
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

            return document.ReplaceNodeAsync(expressionStatement, newNode, cancellationToken);
        }
    }
}
