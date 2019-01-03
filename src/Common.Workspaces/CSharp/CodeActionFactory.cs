// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal static class CodeActionFactory
    {
        public static CodeAction ChangeTypeToVar(
            Document document,
            TypeSyntax type,
            string title = null,
            string equivalenceKey = null)
        {
            return CodeAction.Create(
                title ?? "Change type to 'var'",
                ct => DocumentRefactorings.ChangeTypeToVarAsync(document, type, ct),
                equivalenceKey);
        }

        public static CodeAction ChangeType(
            Document document,
            TypeSyntax type,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel,
            string title = null,
            string equivalenceKey = null)
        {
            if (title == null)
            {
                string newTypeName = SymbolDisplay.ToMinimalDisplayString(newTypeSymbol, semanticModel, type.SpanStart);

                if ((type.Parent is MethodDeclarationSyntax methodDeclaration && methodDeclaration.ReturnType == type)
                    || (type.Parent is LocalFunctionStatementSyntax localFunction && localFunction.ReturnType == type))
                {
                    title = $"Change return type to '{newTypeName}'";
                }
                else
                {
                    title = $"Change type to '{newTypeName}'";
                }
            }

            return ChangeType(document, type, newTypeSymbol, title, equivalenceKey);
        }

        public static CodeAction ChangeType(
            Document document,
            TypeSyntax type,
            ITypeSymbol newTypeSymbol,
            string title,
            string equivalenceKey = null)
        {
            return CodeAction.Create(
                title,
                ct => DocumentRefactorings.ChangeTypeAsync(document, type, newTypeSymbol, ct),
                equivalenceKey);
        }

        public static CodeAction AddCastExpression(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel,
            string title = null,
            string equivalenceKey = null)
        {
            string typeName = SymbolDisplay.ToMinimalDisplayString(destinationType, semanticModel, expression.SpanStart);

            TypeSyntax newType = ParseTypeName(typeName);

            return CodeAction.Create(
                title ?? $"Cast to '{typeName}'",
                ct => DocumentRefactorings.AddCastExpressionAsync(document, expression, newType, ct),
                equivalenceKey);
        }

        public static CodeAction RemoveMemberDeclaration(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            string title = null,
            string equivalenceKey = null)
        {
            return CodeAction.Create(
                title ?? $"Remove {CSharpFacts.GetTitle(memberDeclaration)}",
                ct => document.RemoveMemberAsync(memberDeclaration, ct),
                equivalenceKey);
        }

        public static CodeAction RemoveStatement(
            Document document,
            StatementSyntax statement,
            string title = null,
            string equivalenceKey = null)
        {
            return CodeAction.Create(
                title ?? $"Remove {CSharpFacts.GetTitle(statement)}",
                ct => document.RemoveStatementAsync(statement, ct),
                equivalenceKey);
        }

        public static CodeAction ReplaceNullWithDefaultValue(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            string title = null,
            string equivalenceKey = null)
        {
            return CodeAction.Create(
                title ?? "Replace 'null' with default value",
                ct =>
                {
                    ExpressionSyntax defaultValue = typeSymbol
                        .GetDefaultValueSyntax(document.GetDefaultSyntaxOptions())
                        .WithTriviaFrom(expression);

                    return document.ReplaceNodeAsync(expression, defaultValue, ct);
                },
                equivalenceKey);
        }

        public static CodeAction RemoveAsyncAwait(
            Document document,
            SyntaxToken asyncKeyword,
            string title = null,
            string equivalenceKey = null)
        {
            return CodeAction.Create(
                title ?? "Remove async/await",
                ct => DocumentRefactorings.RemoveAsyncAwaitAsync(document, asyncKeyword, ct),
                equivalenceKey);
        }
    }
}