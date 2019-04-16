// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class NotifyWhenPropertyChangeRefactoring
    {
        public static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax property)
        {
            AccessorDeclarationSyntax setter = property.Setter();

            if (setter == null)
                return;

            ExpressionSyntax expression = GetExpression();

            if (expression == null)
                return;

            SimpleAssignmentExpressionInfo simpleAssignment = SyntaxInfo.SimpleAssignmentExpressionInfo(expression);

            if (!simpleAssignment.Success)
                return;

            if (!simpleAssignment.Left.IsKind(SyntaxKind.IdentifierName))
                return;

            if (!(simpleAssignment.Right is IdentifierNameSyntax identifierName))
                return;

            if (identifierName.Identifier.ValueText != "value")
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            INamedTypeSymbol containingType = semanticModel
                .GetDeclaredSymbol(property, context.CancellationToken)?
                .ContainingType;

            if (containingType == null)
                return;

            if (!containingType.Implements(MetadataNames.System_ComponentModel_INotifyPropertyChanged, allInterfaces: true))
                return;

            IMethodSymbol methodSymbol = SymbolUtility.FindMethodThatRaisePropertyChanged(containingType, expression.SpanStart, semanticModel);

            if (methodSymbol == null)
                return;

            Document document = context.Document;

            context.RegisterRefactoring(
                "Notify when property change",
                ct => RefactorAsync(document, property, methodSymbol.Name, ct),
                RefactoringIdentifiers.NotifyWhenPropertyChange);

            ExpressionSyntax GetExpression()
            {
                BlockSyntax body = setter.Body;

                if (body != null)
                {
                    if (body.Statements.SingleOrDefault(shouldThrow: false) is ExpressionStatementSyntax expressionStatement)
                        return expressionStatement.Expression;
                }
                else
                {
                    return setter.ExpressionBody?.Expression;
                }

                return null;
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            string methodName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            AccessorDeclarationSyntax setter = property.Setter();

            string propertyName = property.Identifier.ValueText;

            ExpressionSyntax argumentExpression;
            if (document.SupportsLanguageFeature(CSharpLanguageFeature.NameOf))
            {
                argumentExpression = NameOfExpression(propertyName);
            }
            else
            {
                argumentExpression = StringLiteralExpression(propertyName);
            }

            IdentifierNameSyntax backingFieldName = GetBackingFieldIdentifierName(setter).WithoutTrivia();

            AccessorDeclarationSyntax newSetter = SetAccessorDeclaration(
                Block(
                    IfStatement(
                        NotEqualsExpression(
                            backingFieldName,
                            IdentifierName("value")),
                        Block(
                            SimpleAssignmentStatement(
                                backingFieldName,
                                IdentifierName("value")),
                            ExpressionStatement(
                                InvocationExpression(
                                    IdentifierName(methodName),
                                    ArgumentList(Argument(argumentExpression))))))));

            newSetter = newSetter
                .WithTriviaFrom(property)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(setter, newSetter, cancellationToken);
        }

        public static IdentifierNameSyntax GetBackingFieldIdentifierName(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                var expressionStatement = (ExpressionStatementSyntax)body.Statements[0];

                var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                return (IdentifierNameSyntax)assignment.Left;
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = accessor.ExpressionBody;

                var assignment = (AssignmentExpressionSyntax)expressionBody.Expression;

                return (IdentifierNameSyntax)assignment.Left;
            }
        }
    }
}
