// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.CodeFixes;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeTypeAccordingToInitializerRefactoring
    {
        public static void ComputeCodeFix(
             CodeFixContext context,
             Diagnostic diagnostic,
             ExpressionSyntax expression,
             SemanticModel semanticModel)
        {
            if (!(expression.Parent is EqualsValueClauseSyntax equalsValueClause))
                return;

            switch (equalsValueClause.Parent)
            {
                case VariableDeclaratorSyntax variableDeclarator:
                    {
                        ComputeCodeFix(context, diagnostic, expression, variableDeclarator, semanticModel);
                        break;
                    }
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        ComputeCodeFix(context, diagnostic, expression, propertyDeclaration, semanticModel);
                        break;
                    }
                default:
                    {
                        Debug.Fail(equalsValueClause.Parent.Kind().ToString());
                        break;
                    }
            }
        }

        private static void ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            VariableDeclaratorSyntax variableDeclarator,
            SemanticModel semanticModel)
        {
            if (!(variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration))
                return;

            TypeSyntax type = variableDeclaration.Type;

            if (type == null)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if ((variableDeclaration.Parent is FieldDeclarationSyntax fieldDeclaration)
                && fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword)
                && typeSymbol?.SupportsConstantValue() != true)
            {
                return;
            }

            if (typeSymbol?.SupportsExplicitDeclaration() == true)
            {
                CodeFixRegistrator.ChangeType(context, diagnostic, type, typeSymbol, semanticModel, CodeFixIdentifiers.ChangeTypeAccordingToInitializer);

                if (typeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT(semanticModel))
                {
                    ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(variableDeclaration.SpanStart, context.CancellationToken);

                    if (enclosingSymbol.IsAsyncMethod())
                    {
                        ITypeSymbol typeArgument = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];

                        ChangeTypeAndAddAwait(context, diagnostic, expression, variableDeclaration, type, typeArgument, semanticModel);
                    }
                }
            }

            if (variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
                CodeFixRegistrator.ChangeTypeToVar(context, diagnostic, type, CodeFixIdentifiers.ChangeTypeToVar);
        }

        private static void ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            PropertyDeclarationSyntax propertyDeclaration,
            SemanticModel semanticModel)
        {
            TypeSyntax type = propertyDeclaration.Type;

            if (type == null)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.SupportsExplicitDeclaration() != true)
                return;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (symbol?.IsOverride != false)
                return;

            if (symbol.ImplementsInterfaceMember())
                return;

            CodeFixRegistrator.ChangeType(context, diagnostic, type, typeSymbol, semanticModel, CodeFixIdentifiers.ChangeTypeAccordingToInitializer);
        }

        private static void ChangeTypeAndAddAwait(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            VariableDeclarationSyntax variableDeclaration,
            TypeSyntax type,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel)
        {
            AwaitExpressionSyntax newExpression = SyntaxFactory.AwaitExpression(expression).WithTriviaFrom(expression);

            VariableDeclarationSyntax newNode = variableDeclaration.ReplaceNode(expression, newExpression);

            TypeSyntax newType = newTypeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart).WithTriviaFrom(type);

            newNode = newNode.WithType(newType);

            string typeName = SymbolDisplay.ToMinimalDisplayString(newTypeSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.Default);

            CodeAction codeAction = CodeAction.Create(
                $"Change type to '{typeName}' and add await",
                cancellationToken => context.Document.ReplaceNodeAsync(variableDeclaration, newNode, cancellationToken),
                EquivalenceKey.Create(diagnostic, CodeFixIdentifiers.ChangeTypeAccordingToInitializer, "AddAwait"));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}