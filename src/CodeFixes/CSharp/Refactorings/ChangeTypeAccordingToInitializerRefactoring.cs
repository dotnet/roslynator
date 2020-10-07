// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        public static CodeFixRegistrationResult ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            SemanticModel semanticModel)
        {
            if (!(expression.Parent is EqualsValueClauseSyntax equalsValueClause))
                return default;

            switch (equalsValueClause.Parent)
            {
                case VariableDeclaratorSyntax variableDeclarator:
                    {
                        return ComputeCodeFix(context, diagnostic, expression, variableDeclarator, semanticModel);
                    }
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        return ComputeCodeFix(context, diagnostic, expression, propertyDeclaration, semanticModel);
                    }
                default:
                    {
                        Debug.Fail(equalsValueClause.Parent.Kind().ToString());
                        break;
                    }
            }

            return default;
        }

        private static CodeFixRegistrationResult ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            VariableDeclaratorSyntax variableDeclarator,
            SemanticModel semanticModel)
        {
            if (!(variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration))
                return default;

            TypeSyntax type = variableDeclaration.Type;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if ((variableDeclaration.Parent is FieldDeclarationSyntax fieldDeclaration)
                && fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword)
                && typeSymbol?.SupportsConstantValue() != true)
            {
                return default;
            }

            CodeFixRegistrationResult result = default;
            CodeFixRegistrationResult result2;

            if (typeSymbol?.SupportsExplicitDeclaration() == true)
            {
                result2 = CodeFixRegistrator.ChangeType(context, diagnostic, type, typeSymbol, semanticModel, CodeFixIdentifiers.ChangeTypeAccordingToInitializer);

                result = result.CombineWith(result2);

                result2 = ComputeChangeTypeAndAddAwait();

                result = result.CombineWith(result2);
            }

            if (variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
            {
                result2 = CodeFixRegistrator.ChangeTypeToVar(context, diagnostic, type, CodeFixIdentifiers.ChangeTypeToVar);

                result = result.CombineWith(result2);
            }

            return result;

            CodeFixRegistrationResult ComputeChangeTypeAndAddAwait()
            {
                Func<CancellationToken, Task<Document>> createChangedDocument = DocumentRefactoringFactory.ChangeTypeAndAddAwait(
                    context.Document,
                    variableDeclaration,
                    variableDeclarator,
                    typeSymbol,
                    semanticModel,
                    context.CancellationToken);

                if (createChangedDocument == null)
                    return default;

                ITypeSymbol typeArgument = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];

                CodeAction codeAction = CodeAction.Create(
                    $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeArgument, semanticModel, variableDeclaration.Type.SpanStart)}' and add await",
                    createChangedDocument,
                    EquivalenceKey.Create(diagnostic, CodeFixIdentifiers.ChangeTypeAccordingToInitializer, "AddAwait"));

                context.RegisterCodeFix(codeAction, diagnostic);

                return new CodeFixRegistrationResult(true);
            }
        }

        private static CodeFixRegistrationResult ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            PropertyDeclarationSyntax propertyDeclaration,
            SemanticModel semanticModel)
        {
            TypeSyntax type = propertyDeclaration.Type;

            if (type == null)
                return default;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.SupportsExplicitDeclaration() != true)
                return default;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (symbol?.IsOverride != false)
                return default;

            if (symbol.ImplementsInterfaceMember())
                return default;

            return CodeFixRegistrator.ChangeType(context, diagnostic, type, typeSymbol, semanticModel, CodeFixIdentifiers.ChangeTypeAccordingToInitializer);
        }
    }
}