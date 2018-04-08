// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    internal static class CodeFixRegistrator
    {
        public static void ChangeType(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeSyntax type,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            string typeName = SymbolDisplay.ToMinimalDisplayString(newTypeSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.Default);

            string title = $"Change type to '{typeName}'";

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                title,
                cancellationToken =>
                {
                    TypeSyntax newType = newTypeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart).WithTriviaFrom(type);

                    return document.ReplaceNodeAsync(type, newType, cancellationToken);
                },
                EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void ChangeTypeToVar(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeSyntax type,
            string additionalKey = null)
        {
            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                "Change type to 'var'",
                cancellationToken =>
                {
                    TypeSyntax newType = CSharpFactory.VarType().WithTriviaFrom(type);

                    return document.ReplaceNodeAsync(type, newType, cancellationToken);
                },
                EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void AddCastExpression(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            string typeName = SymbolDisplay.ToMinimalDisplayString(destinationType, semanticModel, expression.SpanStart, SymbolDisplayFormats.Default);

            TypeSyntax newType = SyntaxFactory.ParseTypeName(typeName);

            CodeAction codeAction = CodeAction.Create(
                $"Cast to '{typeName}'",
                cancellationToken => AddCastExpressionRefactoring.RefactorAsync(context.Document, expression, newType, cancellationToken),
                EquivalenceKey.Create(diagnostic, CodeFixIdentifiers.AddCastExpression));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveMember(
            CodeFixContext context,
            Diagnostic diagnostic,
            MemberDeclarationSyntax memberDeclaration,
            string additionalKey = null)
        {
            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                $"Remove {CSharpFacts.GetTitle(memberDeclaration)}",
                cancellationToken => document.RemoveMemberAsync(memberDeclaration, cancellationToken),
                EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveStatement(
            CodeFixContext context,
            Diagnostic diagnostic,
            StatementSyntax statement,
            string title = null,
            string additionalKey = null)
        {
            if (statement.IsEmbedded())
                return;

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                title ?? $"Remove {CSharpFacts.GetTitle(statement)}",
                cancellationToken => document.RemoveStatementAsync(statement, cancellationToken),
                EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void ReplaceNullWithDefaultValue(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType;

            if (typeSymbol == null)
                return;

            ReplaceNullWithDefaultValue(context, diagnostic, expression, typeSymbol, semanticModel, additionalKey);
        }

        public static void ReplaceNullWithDefaultValue(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            if (!typeSymbol.SupportsExplicitDeclaration())
                return;

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                "Replace 'null' with default value",
                cancellationToken =>
                {
                    ExpressionSyntax newNode = typeSymbol.GetDefaultValueSyntax(semanticModel, expression.SpanStart);

                    newNode = newNode.WithTriviaFrom(expression);

                    return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
                },
                EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
