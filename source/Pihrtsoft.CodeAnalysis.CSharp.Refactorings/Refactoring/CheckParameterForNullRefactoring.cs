// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class CheckParameterForNullRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.CheckParameterForNull)
                && parameter.Identifier.Span.Contains(context.Span)
                && await CanRefactorAsync(context, parameter))
            {
                context.RegisterRefactoring(
                    "Check parameter for null",
                    cancellationToken => RefactorAsync(
                        context.Document,
                        parameter,
                        cancellationToken));
            }
        }

        public static async Task<bool> CanRefactorAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (!parameter.Identifier.IsMissing)
            {
                BlockSyntax body = GetBody(parameter);

                if (body != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync();

                    IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

                    return parameterSymbol?.Type?.IsReferenceType == true
                        && !ContainsParameterNullCheck(body, parameter, semanticModel, context.CancellationToken);
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ParameterSyntax parameter,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            BlockSyntax body = GetBody(parameter);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            int index = body.Statements
                .TakeWhile(f => IsParameterNullCheck(f, null, semanticModel, cancellationToken))
                .Count();

            IfStatementSyntax argumentNullCheck = IfNullThrowArgumentNullException(parameter.Identifier.ToString());

            if (index > 0)
                argumentNullCheck = argumentNullCheck.WithLeadingTrivia(CSharpFactory.NewLine);

            argumentNullCheck = argumentNullCheck.WithTrailingTrivia(CSharpFactory.NewLine, CSharpFactory.NewLine);

            BlockSyntax newBody = body
                .WithStatements(body.Statements.Insert(index, argumentNullCheck))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(body, newBody);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IfStatementSyntax IfNullThrowArgumentNullException(string identifier)
        {
            return IfStatement(
                BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    IdentifierName(identifier),
                    LiteralExpression(SyntaxKind.NullLiteralExpression)),
                ThrowArgumentNullException(identifier));
        }

        private static ThrowStatementSyntax ThrowArgumentNullException(string identifier)
        {
            NameSyntax type = ParseName("System.ArgumentNullException")
                .WithSimplifierAnnotation();

            ArgumentListSyntax argumentList =
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(CSharpFactory.NameOf(identifier))));

            return ThrowStatement(ObjectCreationExpression(
                type: type,
                argumentList: argumentList,
                initializer: null));
        }

        private static bool ContainsParameterNullCheck(
            BlockSyntax body,
            ParameterSyntax parameter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            for (int i = 0; i < body.Statements.Count; i++)
            {
                if (IsParameterNullCheck(body.Statements[i], null, semanticModel, cancellationToken))
                {
                    if (IsParameterNullCheck(body.Statements[i], parameter.Identifier.ToString()))
                        return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private static bool IsParameterNullCheck(StatementSyntax statement, string identifier)
        {
            var ifStatement = (IfStatementSyntax)statement;
            var binaryExpression = (BinaryExpressionSyntax)ifStatement.Condition;
            var identifierName = (IdentifierNameSyntax)binaryExpression.Left;

            return string.Equals(identifier, identifierName.Identifier.ToString(), StringComparison.Ordinal);
        }

        private static bool IsParameterNullCheck(StatementSyntax statement, string identifier, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (!statement.IsKind(SyntaxKind.IfStatement))
                return false;

            var ifStatement = (IfStatementSyntax)statement;

            var binaryExpression = ifStatement.Condition as BinaryExpressionSyntax;
            if (binaryExpression == null)
                return false;

            if (!binaryExpression.Left.IsKind(SyntaxKind.IdentifierName))
                return false;

            var identifierName = (IdentifierNameSyntax)binaryExpression.Left;
            if (!string.IsNullOrEmpty(identifier) && !string.Equals(identifier, identifierName.Identifier.ToString(), StringComparison.Ordinal))
                return false;

            if (!binaryExpression.Right.IsKind(SyntaxKind.NullLiteralExpression))
                return false;

            var throwStatement = SingleStatementOrDefault(ifStatement) as ThrowStatementSyntax;
            if (throwStatement == null)
                return false;

            if (!throwStatement.Expression.IsKind(SyntaxKind.ObjectCreationExpression))
                return false;

            var objectCreation = (ObjectCreationExpressionSyntax)throwStatement.Expression;

            INamedTypeSymbol argumentNullExceptionType = semanticModel.Compilation.GetTypeByMetadataName("System.ArgumentNullException");

            var type = semanticModel.GetSymbolInfo(objectCreation.Type, cancellationToken).Symbol as INamedTypeSymbol;

            return (type?.Equals(argumentNullExceptionType) == true);
        }

        private static StatementSyntax SingleStatementOrDefault(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            if (!ifStatement.Statement.IsKind(SyntaxKind.Block))
                return ifStatement.Statement;

            var block = (BlockSyntax)ifStatement.Statement;

            if (block.Statements.Count == 1)
                return block.Statements[0];

            return null;
        }

        private static BlockSyntax GetBody(ParameterSyntax parameter)
        {
            SyntaxNode parent = parameter.Parent;

            if (parent?.IsKind(SyntaxKind.ParameterList) == true)
            {
                parent = parent.Parent;

                switch (parent?.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        return ((MethodDeclarationSyntax)parent).Body;
                    case SyntaxKind.ConstructorDeclaration:
                        return ((ConstructorDeclarationSyntax)parent).Body;
                }
            }

            return null;
        }
    }
}
