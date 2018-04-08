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
    internal static class NotifyPropertyChangedRefactoring
    {
        public static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax property)
        {
            AccessorDeclarationSyntax setter = property.Setter();

            if (setter == null)
                return false;

            BlockSyntax body = setter.Body;

            if (body != null)
            {
                if (body.Statements.SingleOrDefault(shouldThrow: false) is ExpressionStatementSyntax expressionStatement)
                {
                    ExpressionSyntax expression = expressionStatement.Expression;

                    return expression != null
                        && await CanRefactorAsync(context, property, expression).ConfigureAwait(false);
                }
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = setter.ExpressionBody;

                if (expressionBody != null)
                {
                    ExpressionSyntax expression = expressionBody.Expression;

                    return expression != null
                        && await CanRefactorAsync(context, property, expression).ConfigureAwait(false);
                }
            }

            return false;
        }

        private static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax property,
            ExpressionSyntax expression)
        {
            SimpleAssignmentExpressionInfo simpleAssignment = SyntaxInfo.SimpleAssignmentExpressionInfo(expression);

            if (!simpleAssignment.Success)
                return false;

            if (simpleAssignment.Left.Kind() != SyntaxKind.IdentifierName)
                return false;

            if (simpleAssignment.Right.Kind() != SyntaxKind.IdentifierName)
                return false;

            var identifierName = (IdentifierNameSyntax)simpleAssignment.Right;

            if (identifierName.Identifier.ValueText != "value")
                return false;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            return semanticModel
                .GetDeclaredSymbol(property, context.CancellationToken)?
                .ContainingType?
                .Implements(semanticModel.GetTypeByMetadataName(MetadataNames.System_ComponentModel_INotifyPropertyChanged), allInterfaces: true) == true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            bool supportsCSharp6,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            AccessorDeclarationSyntax setter = property.Setter();

            AccessorDeclarationSyntax newSetter = CreateSetter(
                GetBackingFieldIdentifierName(setter).WithoutTrivia(),
                property.Identifier.ValueText,
                supportsCSharp6);

            newSetter = newSetter
                .WithTriviaFrom(property)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(setter, newSetter, cancellationToken);
        }

        private static AccessorDeclarationSyntax CreateSetter(IdentifierNameSyntax fieldIdentifierName, string propertyName, bool supportsCSharp6)
        {
            ExpressionSyntax argumentExpression;

            if (supportsCSharp6)
            {
                argumentExpression = NameOfExpression(propertyName);
            }
            else
            {
                argumentExpression = StringLiteralExpression(propertyName);
            }

            return SetAccessorDeclaration(
                Block(
                    IfStatement(
                        NotEqualsExpression(
                            fieldIdentifierName,
                            IdentifierName("value")),
                        Block(
                            SimpleAssignmentStatement(
                                fieldIdentifierName,
                                IdentifierName("value")),
                            ExpressionStatement(
                                InvocationExpression(
                                    IdentifierName("OnPropertyChanged"),
                                    ArgumentList(Argument(argumentExpression))))))));
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
