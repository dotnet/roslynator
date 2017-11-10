// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCSharp6DictionaryInitializerRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            if (!expressions.Any())
                return;

            if (!expressions.All(f => f.IsKind(SyntaxKind.ComplexElementInitializerExpression)))
                return;

            if (!(initializer.Parent is ObjectCreationExpressionSyntax objectCreationExpression))
                return;

            var complexElementInitializer = (InitializerExpressionSyntax)expressions.First();

            SeparatedSyntaxList<ExpressionSyntax> expressions2 = complexElementInitializer.Expressions;

            if (expressions2.Count != 2)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IPropertySymbol propertySymbol = FindIndexerSymbol(
                objectCreationExpression,
                expressions2,
                semanticModel,
                context.CancellationToken);

            if (propertySymbol == null)
                return;

            ITypeSymbol keyType = propertySymbol.Parameters[0].Type;
            ITypeSymbol valueType = propertySymbol.Type;

            for (int i = 1; i < expressions.Count; i++)
            {
                if (!CanRefactor(((InitializerExpressionSyntax)expressions[i]).Expressions, keyType, valueType, semanticModel))
                    return;
            }

            context.RegisterRefactoring(
                "Use C# 6.0 dictionary initializer",
                cancellationToken => RefactorAsync(context.Document, initializer, cancellationToken));
        }

        private static bool CanRefactor(
            SeparatedSyntaxList<ExpressionSyntax> expressions,
            ITypeSymbol keyType,
            ITypeSymbol valueType,
            SemanticModel semanticModel)
        {
            return expressions.Count == 2
                && semanticModel.ClassifyConversion(expressions[0], keyType).IsImplicit
                && semanticModel.ClassifyConversion(expressions[1], valueType).IsImplicit;
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

        private static AssignmentExpressionSyntax CreateNewExpression(InitializerExpressionSyntax initializer)
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