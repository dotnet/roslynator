// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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

        public static void ChangeReturnType(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode methodDeclarationOrLocalFunction,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            Debug.Assert(methodDeclarationOrLocalFunction.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement));

            if (methodDeclarationOrLocalFunction is MethodDeclarationSyntax methodDeclaration)
            {
                ChangeMemberDeclarationType(context, diagnostic, methodDeclaration, typeSymbol, semanticModel, additionalKey);
            }
            else if (methodDeclarationOrLocalFunction is LocalFunctionStatementSyntax localFunction)
            {
                ChangeLocalFunctionReturnType(context, diagnostic, localFunction, typeSymbol, semanticModel, additionalKey);
            }
        }

        public static void ChangeMemberDeclarationType(
            CodeFixContext context,
            Diagnostic diagnostic,
            MemberDeclarationSyntax memberDeclaration,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            if (typeSymbol.IsErrorType())
                return;

            Document document = context.Document;

            string typeName = SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, memberDeclaration.SpanStart, SymbolDisplayFormats.Default);

            string title = (memberDeclaration.IsKind(SyntaxKind.MethodDeclaration))
                ? $"Change return type to '{typeName}'"
                : $"Change type to '{typeName}'";

            CodeAction codeAction = CodeAction.Create(
                title,
                cancellationToken => RefactorAsync(cancellationToken),
                EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);

            Task<Document> RefactorAsync(CancellationToken cancellationToken)
            {
                TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, memberDeclaration.SpanStart);

                switch (memberDeclaration.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        {
                            var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                            MethodDeclarationSyntax newNode = methodDeclaration.WithReturnType(newType.WithTriviaFrom(methodDeclaration.ReturnType));

                            return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
                        }
                    case SyntaxKind.PropertyDeclaration:
                        {
                            var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                            PropertyDeclarationSyntax newNode = propertyDeclaration.WithType(newType.WithTriviaFrom(propertyDeclaration.Type));

                            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
                        }
                    case SyntaxKind.IndexerDeclaration:
                        {
                            var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                            IndexerDeclarationSyntax newNode = indexerDeclaration.WithType(newType.WithTriviaFrom(indexerDeclaration.Type));

                            return document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken);
                        }
                    case SyntaxKind.EventDeclaration:
                        {
                            var eventDeclaration = (EventDeclarationSyntax)memberDeclaration;

                            EventDeclarationSyntax newNode = eventDeclaration.WithType(newType.WithTriviaFrom(eventDeclaration.Type));

                            return document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken);
                        }
                    case SyntaxKind.EventFieldDeclaration:
                        {
                            var eventDeclaration = (EventFieldDeclarationSyntax)memberDeclaration;

                            VariableDeclarationSyntax declaration = eventDeclaration.Declaration;

                            EventFieldDeclarationSyntax newNode = eventDeclaration.WithDeclaration(declaration.WithType(newType.WithTriviaFrom(declaration.Type)));

                            return document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken);
                        }
                    default:
                        {
                            Debug.Fail(memberDeclaration.Kind().ToString());
                            return Task.FromResult(document);
                        }
                }
            }
        }

        public static void ChangeLocalFunctionReturnType(
            CodeFixContext context,
            Diagnostic diagnostic,
            LocalFunctionStatementSyntax localFunction,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            if (typeSymbol.IsErrorType())
                return;

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                $"Change return type to '{SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, localFunction.SpanStart, SymbolDisplayFormats.Default)}'",
                cancellationToken =>
                {
                    TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, localFunction.SpanStart);

                    LocalFunctionStatementSyntax newNode = localFunction.WithReturnType(newType.WithTriviaFrom(localFunction.ReturnType));

                    return document.ReplaceNodeAsync(localFunction, newNode, cancellationToken);
                },
                EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
