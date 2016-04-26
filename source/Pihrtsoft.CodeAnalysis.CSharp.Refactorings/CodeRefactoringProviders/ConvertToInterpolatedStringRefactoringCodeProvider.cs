// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ConvertToInterpolatedStringRefactoringCodeProvider))]
    public class ConvertToInterpolatedStringRefactoringCodeProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            InvocationExpressionSyntax invocation = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            if (invocation == null)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            ImmutableArray<ISymbol> formatMethods = GetFormatMethods(semanticModel);

            if (formatMethods.Length == 0)
                return;

            invocation = FindOutermostFormatMethod(context, semanticModel, formatMethods, invocation);

            if (invocation != null)
            {
                context.RegisterRefactoring(
                    "Convert to interpolated string",
                    cancellationToken => CreateInterpolatedStringAsync(context.Document, invocation, semanticModel, cancellationToken));
            }
        }

        private static InvocationExpressionSyntax FindOutermostFormatMethod(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            ImmutableArray<ISymbol> formatMethods,
            InvocationExpressionSyntax invocation)
        {
            while (invocation != null)
            {
                if (invocation.ArgumentList != null)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;

                    if (arguments.Count >= 2)
                    {
                        var firstArgument = arguments[0]?.Expression as LiteralExpressionSyntax;

                        if (firstArgument?.Token.IsKind(SyntaxKind.StringLiteralToken) == true)
                        {
                            ISymbol invocationSymbol = semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol;

                            if (formatMethods.Contains(invocationSymbol))
                                break;
                        }
                    }
                }

                invocation = invocation.FirstAncestor<InvocationExpressionSyntax>();
            }

            return invocation;
        }

        private static ImmutableArray<ISymbol> GetFormatMethods(SemanticModel semanticModel)
        {
            INamedTypeSymbol stringType = semanticModel.Compilation.GetTypeByMetadataName("System.String");

            if (stringType == null)
                return ImmutableArray<ISymbol>.Empty;

            return stringType
                .GetMembers("Format")
                .RemoveAll(symbol => !IsValidFormatMethod(symbol));
        }

        private static async Task<Document> CreateInterpolatedStringAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;

            ImmutableArray<ExpressionSyntax> expandedArguments = ImmutableArray.CreateRange(GetExpandedArguments(arguments, semanticModel));

            string formatText = ((LiteralExpressionSyntax)arguments[0].Expression).Token.ToString();

            var interpolatedString = (InterpolatedStringExpressionSyntax)ParseExpression("$" + formatText);

            InterpolatedStringExpressionSyntax newInterpolatedString = InterpolatedStringSyntaxRewriter.VisitNode(interpolatedString, expandedArguments);

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newRoot = root.ReplaceNode(invocation, newInterpolatedString);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IEnumerable<ExpressionSyntax> GetExpandedArguments(SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            for (int i = 1; i < arguments.Count; i++)
            {
                ITypeSymbol targetType = semanticModel.GetTypeInfo(arguments[i].Expression).ConvertedType;

                ExpressionSyntax expression = Cast(arguments[i].Expression, targetType);

                yield return Parenthesize(expression);
            }
        }

        private static ExpressionSyntax Parenthesize(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.ParenthesizedExpression))
                return expression;

            return ParenthesizedExpression(
                    Token(SyntaxTriviaList.Empty, SyntaxKind.OpenParenToken, SyntaxTriviaList.Empty),
                    expression,
                    Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, SyntaxTriviaList.Empty))
                .WithAdditionalAnnotations(Simplifier.Annotation);
        }

        private static ExpressionSyntax Cast(ExpressionSyntax expression, ITypeSymbol targetType)
        {
            if (targetType == null)
                return expression;

            TypeSyntax type = ParseTypeName(targetType.ToDisplayString());

            return CastExpression(type, Parenthesize(expression))
                .WithAdditionalAnnotations(Simplifier.Annotation);
        }

        private static bool IsValidFormatMethod(ISymbol symbol)
        {
            if (symbol.Kind != SymbolKind.Method)
                return false;

            if (!symbol.IsStatic)
                return false;

            var methodSymbol = (IMethodSymbol)symbol;

            if (methodSymbol.Parameters.Length == 0)
                return false;

            if (methodSymbol.Parameters[0]?.Name != "format")
                return false;

            return true;
        }
    }
}
