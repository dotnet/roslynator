// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    internal static class CSharpFactory2
    {
        public static ExpressionStatementSyntax VisitStatement(
            string methodName,
            string name,
            string propertyName = null)
        {
            ExpressionSyntax expression = IdentifierName(name);

            if (propertyName != null)
            {
                expression = SimpleMemberAccessExpression(
                    expression,
                    IdentifierName(propertyName));
            }

            return ExpressionStatement(
                InvocationExpression(
                IdentifierName(methodName),
                ArgumentList(
                Argument(
                expression))));
        }

        public static ForEachStatementSyntax ForEachVisitStatement(
            string typeName,
            string variableName,
            ExpressionSyntax expression,
            StatementSyntax statement,
            bool checkShouldVisit = false)
        {
            return ForEachStatement(
                IdentifierName(typeName),
                variableName,
                expression,
                (checkShouldVisit)
                    ? Block(IfNotShouldVisitReturnStatement(), statement)
                    : Block(statement));
        }

        public static StatementSyntax IfNotShouldVisitReturnStatement()
        {
            return IfStatement(LogicalNotExpression(IdentifierName("ShouldVisit")), Block(ReturnStatement()));
        }

        public static IfStatementSyntax IfNotEqualsToNullStatement(string name, StatementSyntax statement)
        {
            return IfStatement(
                NotEqualsExpression(
                    IdentifierName(name),
                    NullLiteralExpression()),
                (statement.IsKind(SyntaxKind.Block)) ? statement : Block(statement));
        }

        public static ThrowStatementSyntax ThrowNewInvalidOperationException(ExpressionSyntax expression = null)
        {
            ArgumentListSyntax argumentList;

            if (expression != null)
            {
                argumentList = ArgumentList(Argument(expression));
            }
            else
            {
                argumentList = ArgumentList();
            }

            return ThrowStatement(
                ObjectCreationExpression(
                IdentifierName("InvalidOperationException"), argumentList));
        }

        public static ThrowStatementSyntax ThrowNewArgumentException(ExpressionSyntax messageExpression, string parameterName)
        {
            ArgumentListSyntax argumentList = ArgumentList(
                Argument(messageExpression),
                Argument(NameOfExpression(IdentifierName(parameterName))));

            return ThrowStatement(
                ObjectCreationExpression(
                IdentifierName("ArgumentException"), argumentList));
        }

        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(
            ITypeSymbol typeSymbol,
            string name,
            string parameterName,
            string propertyName)
        {
            return CSharpFactory.LocalDeclarationStatement(
                typeSymbol.ToTypeSyntax(SymbolDisplayFormats.Default),
                name,
                SimpleMemberAccessExpression(
                    IdentifierName(parameterName),
                    IdentifierName(propertyName)));
        }
    }
}
