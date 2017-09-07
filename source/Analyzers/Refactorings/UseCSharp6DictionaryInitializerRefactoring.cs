// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
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
        public static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var initializer = (InitializerExpressionSyntax)context.Node;

            if (!initializer.ContainsDiagnostics)
            {
                SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

                if (expressions.Any()
                    && expressions.All(f => f.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                    && initializer.IsParentKind(SyntaxKind.ObjectCreationExpression)))
                {
                    var objectCreationExpression = (ObjectCreationExpressionSyntax)initializer.Parent;

                    var complexElementInitializer = (InitializerExpressionSyntax)expressions.First();

                    SeparatedSyntaxList<ExpressionSyntax> expressions2 = complexElementInitializer.Expressions;

                    if (expressions2.Count == 2)
                    {
                        IPropertySymbol propertySymbol = FindIndexerSymbol(
                            objectCreationExpression,
                            expressions2,
                            context.SemanticModel,
                            context.CancellationToken);

                        if (propertySymbol != null)
                        {
                            ITypeSymbol keyType = propertySymbol.Parameters[0].Type;
                            ITypeSymbol valueType = propertySymbol.Type;

                            if (expressions
                                .Skip(1)
                                .All(f => CanRefactor(((InitializerExpressionSyntax)f).Expressions, keyType, valueType, context.SemanticModel)))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.UseCSharp6DictionaryInitializer,
                                    Location.Create(initializer.SyntaxTree, expressions.Span));
                            }
                        }
                    }
                }
            }
        }

        private static bool CanRefactor(
            SeparatedSyntaxList<ExpressionSyntax> expressions,
            ITypeSymbol keyType,
            ITypeSymbol valueType,
            SemanticModel semanticModel)
        {
            return expressions.Count == 2
                && IsIdentityOrImplicitConversion(expressions[0], keyType, semanticModel)
                && IsIdentityOrImplicitConversion(expressions[1], valueType, semanticModel);
        }

        private static IPropertySymbol FindIndexerSymbol(
            ObjectCreationExpressionSyntax objectCreationExpression,
            SeparatedSyntaxList<ExpressionSyntax> expressions,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(objectCreationExpression, cancellationToken);

            if (typeSymbol?.IsErrorType() == false)
            {
                foreach (ISymbol member in semanticModel.LookupSymbols(objectCreationExpression.SpanStart, typeSymbol, "this[]"))
                {
                    var propertySymbol = (IPropertySymbol)member;

                    if (!propertySymbol.IsReadOnly
                        && semanticModel.IsAccessible(objectCreationExpression.SpanStart, propertySymbol.SetMethod))
                    {
                        ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

                        if (parameters.Length == 1
                            && CanRefactor(expressions, parameters[0].Type, propertySymbol.Type, semanticModel))
                        {
                            return propertySymbol;
                        }
                    }
                }
            }

            return null;
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

            for (int i = 0; i < expressions.Count; i++)
                expressions = expressions.ReplaceAt(i, CreateNewExpression((InitializerExpressionSyntax)expressions[i]));

            InitializerExpressionSyntax newInitializer = initializer.WithExpressions(expressions);

            return document.ReplaceNodeAsync(initializer, newInitializer, cancellationToken);
        }

        public static ExpressionSyntax CreateNewExpression(InitializerExpressionSyntax initializer)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            SyntaxToken openBracket = Token(
                initializer.OpenBraceToken.LeadingTrivia,
                SyntaxKind.OpenBracketToken,
                initializer.OpenBraceToken.TrailingTrivia.EmptyIfWhitespace());

            ImplicitElementAccessSyntax implicitElementAccess = ImplicitElementAccess(
                BracketedArgumentList(
                    openBracket,
                    SingletonSeparatedList(Argument(expressions[0].TrimTrivia())),
                    CloseBracketToken()));

            SyntaxToken comma = initializer.ChildTokens().FirstOrDefault(f => f.IsKind(SyntaxKind.CommaToken));

            SyntaxTriviaList commaLeading = comma.LeadingTrivia;

            SyntaxToken equalsToken = Token(
                (commaLeading.Any()) ? commaLeading : TriviaList(Space),
                SyntaxKind.EqualsToken,
                comma.TrailingTrivia);

            ExpressionSyntax valueExpression = expressions[1];

            valueExpression = valueExpression.AppendToTrailingTrivia(initializer.CloseBraceToken.LeadingTrivia.EmptyIfWhitespace());

            return SimpleAssignmentExpression(implicitElementAccess, equalsToken, valueExpression)
                .WithTriviaFrom(initializer);
        }
    }
}