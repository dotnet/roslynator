// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddCastExpressionRefactoring
    {
        public static void RegisterRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol newType,
            SemanticModel semanticModel)
        {
            if (!newType.IsErrorType()
                && !newType.IsVoid())
            {
                Conversion conversion = semanticModel.ClassifyConversion(
                    expression,
                    newType,
                    isExplicitInSource: false);

                WriteDebugOutput(expression, newType, semanticModel);

                if (conversion.IsExplicit)
                {
                    context.RegisterRefactoring(
                        $"Cast to '{newType.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                expression,
                                newType,
                                cancellationToken);
                        });
                }
            }
        }

        [Conditional("DEBUG")]
        private static void WriteDebugOutput(ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            Conversion conversion = semanticModel.ClassifyConversion(
                expression,
                destination,
                isExplicitInSource: false);

            Debug.WriteLine($"expression: {expression.Kind().ToString()}");
            Debug.WriteLine($"destination: {destination.ToDisplayString()}");

            if (conversion.Exists)
                Debug.WriteLine($"{nameof(conversion.Exists)}");

            if (conversion.IsAnonymousFunction)
                Debug.WriteLine($"{nameof(conversion.IsAnonymousFunction)}");

            if (conversion.IsBoxing)
                Debug.WriteLine($"{nameof(conversion.IsBoxing)}");

            if (conversion.IsConstantExpression)
                Debug.WriteLine($"{nameof(conversion.IsConstantExpression)}");

            if (conversion.IsDynamic)
                Debug.WriteLine($"{nameof(conversion.IsDynamic)}");

            if (conversion.IsEnumeration)
                Debug.WriteLine($"{nameof(conversion.IsEnumeration)}");

            if (conversion.IsExplicit)
                Debug.WriteLine($"{nameof(conversion.IsExplicit)}");

            if (conversion.IsIdentity)
                Debug.WriteLine($"{nameof(conversion.IsIdentity)}");

            if (conversion.IsImplicit)
                Debug.WriteLine($"{nameof(conversion.IsImplicit)}");

            if (conversion.IsInterpolatedString)
                Debug.WriteLine($"{nameof(conversion.IsInterpolatedString)}");

            if (conversion.IsIntPtr)
                Debug.WriteLine($"{nameof(conversion.IsIntPtr)}");

            if (conversion.IsMethodGroup)
                Debug.WriteLine($"{nameof(conversion.IsMethodGroup)}");

            if (conversion.IsNullable)
                Debug.WriteLine($"{nameof(conversion.IsNullable)}");

            if (conversion.IsNullLiteral)
                Debug.WriteLine($"{nameof(conversion.IsNullLiteral)}");

            if (conversion.IsNumeric)
                Debug.WriteLine($"{nameof(conversion.IsNumeric)}");

            if (conversion.IsPointer)
                Debug.WriteLine($"{nameof(conversion.IsPointer)}");

            if (conversion.IsReference)
                Debug.WriteLine($"{nameof(conversion.IsReference)}");

            if (conversion.IsUnboxing)
                Debug.WriteLine($"{nameof(conversion.IsUnboxing)}");

            if (conversion.IsUserDefined)
                Debug.WriteLine($"{nameof(conversion.IsUserDefined)}");

            Debug.WriteLine("*");
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            TypeSyntax type = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)
                .WithSimplifierAnnotation();

            CastExpressionSyntax castExpression = SyntaxFactory.CastExpression(type, expression)
                .WithTriviaFrom(expression);

            root = root.ReplaceNode(expression, castExpression);

            return document.WithSyntaxRoot(root);
        }
    }
}

