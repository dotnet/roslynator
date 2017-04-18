// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidImplicitlyTypedArrayRefactoring
    {
        public static void AnalyzeImplicitArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (ImplicitArrayCreationExpressionSyntax)context.Node;

            SyntaxToken newKeyword = expression.NewKeyword;
            SyntaxToken openBracket = expression.OpenBracketToken;
            SyntaxToken closeBracket = expression.CloseBracketToken;

            if (!newKeyword.IsMissing
                && !openBracket.IsMissing
                && !closeBracket.IsMissing)
            {
                var typeSymbol = context.SemanticModel.GetTypeSymbol(expression) as IArrayTypeSymbol;

                if (typeSymbol?.ElementType?.IsErrorType() == false)
                {
                    TextSpan span = TextSpan.FromBounds(newKeyword.Span.Start, closeBracket.Span.End);

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AvoidImplicitlyTypedArray,
                        Location.Create(expression.SyntaxTree, span));
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ImplicitArrayCreationExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            var arrayType = typeSymbol.ToMinimalTypeSyntax(semanticModel, expression.SpanStart) as ArrayTypeSyntax;

            ArrayCreationExpressionSyntax newNode = SyntaxFactory.ArrayCreationExpression(
                expression.NewKeyword,
                arrayType.WithTrailingTrivia(expression.CloseBracketToken.TrailingTrivia),
                expression.Initializer);

            newNode = newNode.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(expression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
