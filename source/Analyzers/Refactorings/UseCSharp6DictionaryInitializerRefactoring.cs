// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCSharp6DictionaryInitializerRefactoring
    {
        public static void AnalyzeComplexElementInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var initializer = (InitializerExpressionSyntax)context.Node;

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            if (expressions.Count == 2)
            {
                SyntaxNode parent = initializer.Parent;

                if (parent?.IsKind(SyntaxKind.CollectionInitializerExpression) == true)
                {
                    parent = parent.Parent;

                    Debug.Assert(parent?.IsKind(SyntaxKind.ObjectCreationExpression) == true, parent.Kind().ToString());

                    if (parent?.IsKind(SyntaxKind.ObjectCreationExpression) == true)
                    {
                        var objectCreationExpression = (ObjectCreationExpressionSyntax)parent;

                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(objectCreationExpression, cancellationToken);

                        if (typeSymbol?.IsErrorType() == false)
                        {
                            foreach (ISymbol member in typeSymbol.GetMembers("this[]"))
                            {
                                var propertySymbol = (IPropertySymbol)member;

                                if (!propertySymbol.IsReadOnly
                                    && semanticModel.IsAccessible(objectCreationExpression.SpanStart, propertySymbol.SetMethod))
                                {
                                    ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

                                    if (parameters.Length == 1
                                        && IsIdentityOrImplicitConversion(expressions[0], parameters[0].Type, semanticModel)
                                        && IsIdentityOrImplicitConversion(expressions[1], propertySymbol.Type, semanticModel))
                                    {
                                        context.ReportDiagnostic(
                                            DiagnosticDescriptors.UseCSharp6DictionaryInitializer,
                                            initializer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsIdentityOrImplicitConversion(
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            Conversion conversion = semanticModel.ClassifyConversion(expression, destinationType);

            return conversion.IsIdentity
                || conversion.IsImplicit;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;
            SyntaxTriviaList openBraceTrailing = initializer.OpenBraceToken.TrailingTrivia;
            SyntaxTriviaList closeBraceLeading = initializer.CloseBraceToken.LeadingTrivia;

            SyntaxToken openBracket = Token(
                        initializer.OpenBraceToken.LeadingTrivia,
                        SyntaxKind.OpenBracketToken,
                        (openBraceTrailing.All(f => f.IsWhitespaceTrivia())) ? default(SyntaxTriviaList) : openBraceTrailing);

            ImplicitElementAccessSyntax implicitElementAccess = ImplicitElementAccess(
                BracketedArgumentList(
                    openBracket,
                    SingletonSeparatedList(Argument(expressions[0])),
                    CloseBracketToken()));

            SyntaxToken comma = initializer.ChildTokens().FirstOrDefault(f => f.IsKind(SyntaxKind.CommaToken));

            SyntaxTriviaList commaLeading = comma.LeadingTrivia;

            SyntaxToken equalsToken = Token(
                (commaLeading.Any()) ? commaLeading : TriviaList(Space),
                SyntaxKind.EqualsToken,
                comma.TrailingTrivia);

            ExpressionSyntax valueExpression = expressions[1];

            if (closeBraceLeading.Any(f => !f.IsWhitespaceTrivia()))
                valueExpression = valueExpression.AppendToTrailingTrivia(closeBraceLeading);

            AssignmentExpressionSyntax assignment = SimpleAssignmentExpression(implicitElementAccess, equalsToken, valueExpression)
                .WithTriviaFrom(initializer);

            return document.ReplaceNodeAsync(initializer, assignment, cancellationToken);
        }
    }
}