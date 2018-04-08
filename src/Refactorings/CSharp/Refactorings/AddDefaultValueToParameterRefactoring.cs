// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDefaultValueToParameterRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            SyntaxNode parent = parameter.Parent as BaseParameterListSyntax;

            if (parent == null)
                return;

            parent = parent.Parent;

            if (parent?.IsKind(
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression) != false)
            {
                return;
            }

            TypeSyntax type = parameter.Type;

            if (type == null)
                return;

            SyntaxToken identifier = parameter.Identifier;

            if (identifier.IsMissing)
                return;

            if (context.Span.Start < identifier.SpanStart)
                return;

            EqualsValueClauseSyntax @default = parameter.Default;

            if (@default?.Value?.IsMissing == false)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            context.RegisterRefactoring(
                "Add default value",
                cancellationToken => RefactorAsync(context.Document, parameter, typeSymbol, cancellationToken));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ParameterSyntax parameter,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParameterSyntax newParameter = GetNewParameter(parameter, typeSymbol);

            return document.ReplaceNodeAsync(parameter, newParameter, cancellationToken);
        }

        private static ParameterSyntax GetNewParameter(
            ParameterSyntax parameter,
            ITypeSymbol typeSymbol)
        {
            ExpressionSyntax value = typeSymbol.GetDefaultValueSyntax(parameter.Type.WithoutTrivia());

            EqualsValueClauseSyntax @default = EqualsValueClause(value);

            if (parameter.Default == null || parameter.IsMissing)
            {
                return parameter
                    .WithIdentifier(parameter.Identifier.WithoutTrailingTrivia())
                    .WithDefault(@default.WithTrailingTrivia(parameter.Identifier.TrailingTrivia));
            }
            else
            {
                return parameter
                    .WithDefault(@default.WithTriviaFrom(parameter.Default.EqualsToken));
            }
        }
    }
}
