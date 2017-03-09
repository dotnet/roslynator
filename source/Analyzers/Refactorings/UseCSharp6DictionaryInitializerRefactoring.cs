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
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCSharp6DictionaryInitializerRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InitializerExpressionSyntax initializer)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            if (expressions.Count == 2)
            {
                SyntaxNode parent = initializer.Parent;

                Debug.Assert(parent?.IsKind(SyntaxKind.CollectionInitializerExpression) == true, parent.Kind().ToString());

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

        public static async Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            ImplicitElementAccessSyntax implicitElementAccess = ImplicitElementAccess(
                BracketedArgumentList(
                    OpenBracketToken().WithTriviaFrom(initializer.OpenBraceToken),
                    SingletonSeparatedList(Argument(expressions[0]).WithFormatterAnnotation()),
                    CloseBracketToken()));

            AssignmentExpressionSyntax assignment = SimpleAssignmentExpression(
                implicitElementAccess,
                EqualsToken().WithTriviaFrom(initializer.ChildTokens().FirstOrDefault()),
                expressions[1]
                    .AppendToTrailingTrivia(initializer.CloseBraceToken.GetLeadingAndTrailingTrivia())
                    .WithFormatterAnnotation());

            return await document.ReplaceNodeAsync(initializer, assignment, cancellationToken).ConfigureAwait(false);
        }
    }
}