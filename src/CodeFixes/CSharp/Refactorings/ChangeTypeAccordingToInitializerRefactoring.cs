// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            if (type == null)
                return default;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if ((variableDeclaration.Parent is FieldDeclarationSyntax fieldDeclaration)
                && fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword)
                && typeSymbol?.SupportsConstantValue() != true)
            {
                return default;
            }

            CodeFixRegistrationResult result = default;
            CodeFixRegistrationResult result2 = default;

            if (typeSymbol?.SupportsExplicitDeclaration() == true)
            {
                result2 = CodeFixRegistrator.ChangeType(context, diagnostic, type, typeSymbol, semanticModel, CodeFixIdentifiers.ChangeTypeAccordingToInitializer);

                result = result.CombineWith(result2);

                result2 = ComputeChangeTypeAndAddAwait(context, diagnostic, variableDeclaration, type, expression, typeSymbol, semanticModel);

                result = result.CombineWith(result2);
            }

            if (variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
            {
                result2 = CodeFixRegistrator.ChangeTypeToVar(context, diagnostic, type, CodeFixIdentifiers.ChangeTypeToVar);

                result = result.CombineWith(result2);
            }

            return result;
        }

        private static CodeFixRegistrationResult ComputeChangeTypeAndAddAwait(
            CodeFixContext context,
            Diagnostic diagnostic,
            VariableDeclarationSyntax variableDeclaration,
            TypeSyntax type,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel)
        {
            if (!typeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT())
                return default;

            if (!(semanticModel.GetEnclosingSymbol(variableDeclaration.SpanStart, context.CancellationToken) is IMethodSymbol methodSymbol))
                return default;

            if (!methodSymbol.MethodKind.Is(MethodKind.Ordinary, MethodKind.LocalFunction))
                return default;

            SyntaxNode node = GetSyntax();

            if (node == null)
                return default;

            SyntaxNode bodyOrExpressionBody = GetBodyOrExpressionBody();

            if (bodyOrExpressionBody == null)
                return default;

            foreach (SyntaxNode descendant in bodyOrExpressionBody.DescendantNodes())
            {
                if (descendant.IsKind(SyntaxKind.ReturnStatement))
                {
                    var returnStatement = (ReturnStatementSyntax)descendant;

                    if (returnStatement
                        .Expression?
                        .WalkDownParentheses()
                        .IsKind(SyntaxKind.AwaitExpression) == false)
                    {
                        return default;
                    }
                }
            }

            ITypeSymbol typeArgument = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];

            CodeAction codeAction = CodeAction.Create(
                $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeArgument, semanticModel, type.SpanStart, SymbolDisplayFormats.Default)}' and add await",
                cancellationToken => ChangeTypeAndAddAwait(context.Document, node, variableDeclaration, type, expression, typeArgument, semanticModel, cancellationToken),
                EquivalenceKey.Create(diagnostic, CodeFixIdentifiers.ChangeTypeAccordingToInitializer, "AddAwait"));

            context.RegisterCodeFix(codeAction, diagnostic);

            return new CodeFixRegistrationResult(true);

            SyntaxNode GetSyntax()
            {
                foreach (SyntaxReference syntaxReference in methodSymbol.DeclaringSyntaxReferences)
                {
                    SyntaxNode syntax = syntaxReference.GetSyntax(context.CancellationToken);

                    if (syntax.Contains(variableDeclaration))
                        return syntax;
                }

                return null;
            }

            SyntaxNode GetBodyOrExpressionBody()
            {
                switch (node.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        return ((MethodDeclarationSyntax)node).BodyOrExpressionBody();
                    case SyntaxKind.LocalFunctionStatement:
                        return ((LocalFunctionStatementSyntax)node).BodyOrExpressionBody();
                }

                Debug.Fail(node.Kind().ToString());

                return null;
            }
        }

        private static Task<Document> ChangeTypeAndAddAwait(
            Document document,
            SyntaxNode declaration,
            VariableDeclarationSyntax variableDeclaration,
            TypeSyntax type,
            ExpressionSyntax expression,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            AwaitExpressionSyntax newExpression = SyntaxFactory.AwaitExpression(expression).WithTriviaFrom(expression);

            VariableDeclarationSyntax newVariableDeclaration = variableDeclaration.ReplaceNode(expression, newExpression);

            TypeSyntax newType = newTypeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart).WithTriviaFrom(type);

            newVariableDeclaration = newVariableDeclaration.WithType(newType);

            if (!SyntaxInfo.ModifierListInfo(declaration).IsAsync)
            {
                SyntaxNode newDeclaration = declaration
                    .ReplaceNode(variableDeclaration, newVariableDeclaration)
                    .InsertModifier(SyntaxKind.AsyncKeyword);

                return document.ReplaceNodeAsync(declaration, newDeclaration, cancellationToken);
            }

            return document.ReplaceNodeAsync(variableDeclaration, newVariableDeclaration, cancellationToken);
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