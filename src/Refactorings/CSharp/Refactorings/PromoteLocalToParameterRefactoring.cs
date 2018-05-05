// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PromoteLocalToParameterRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            LocalDeclarationStatementSyntax localDeclaration,
            SemanticModel semanticModel)
        {
            if (!(semanticModel.GetEnclosingSymbol(localDeclaration.SpanStart, context.CancellationToken) is IMethodSymbol methodSymbol))
                return;

            if (methodSymbol.IsImplicitlyDeclared)
                return;

            if (!methodSymbol.MethodKind.Is(MethodKind.Ordinary, MethodKind.LocalFunction))
                return;

            if (methodSymbol.PartialImplementationPart != null)
                methodSymbol = methodSymbol.PartialImplementationPart;

            SyntaxNode methodOrLocalFunction = methodSymbol.GetSyntax(context.CancellationToken);

            if (!methodOrLocalFunction.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement))
                return;

            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            if (declaration == null)
                return;

            VariableDeclaratorSyntax variable = declaration
                .Variables
                .FirstOrDefault(f => !f.IsMissing && f.Identifier.Span.Contains(context.Span));

            if (variable == null)
                return;

            TypeSyntax type = declaration.Type;

            if (type == null)
                return;

            if (type.IsVar)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                if (typeSymbol?.SupportsExplicitDeclaration() == true)
                {
                    type = typeSymbol.ToTypeSyntax();
                }
                else
                {
                    return;
                }
            }

            context.RegisterRefactoring(
                $"Promote '{variable.Identifier.ValueText}' to parameter",
                cancellationToken =>
                {
                    return RefactorAsync(
                        context.Document,
                        methodOrLocalFunction,
                        localDeclaration,
                        type.WithoutTrivia().WithSimplifierAnnotation(),
                        variable,
                        cancellationToken);
                },
                RefactoringIdentifiers.PromoteLocalToParameter);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode methodOrLocalFunction,
            LocalDeclarationStatementSyntax localDeclaration,
            TypeSyntax type,
            VariableDeclaratorSyntax variable,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int variableCount = localDeclaration.Declaration.Variables.Count;
            ExpressionSyntax initializerValue = variable.Initializer?.Value;
            SyntaxToken identifier = variable.Identifier.WithoutTrivia();

            SyntaxNode newNode = methodOrLocalFunction;

            if (initializerValue != null)
            {
                ExpressionStatementSyntax expressionStatement = SimpleAssignmentStatement(
                    IdentifierName(identifier),
                    initializerValue);

                expressionStatement = expressionStatement.WithFormatterAnnotation();

                if (variableCount > 1)
                {
                    LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration.RemoveNode(
                        variable,
                        SyntaxRemoveOptions.KeepUnbalancedDirectives);

                    newNode = newNode.ReplaceNode(
                        localDeclaration,
                        new SyntaxNode[] { newLocalDeclaration, expressionStatement });
                }
                else
                {
                    newNode = newNode.ReplaceNode(
                        localDeclaration,
                        expressionStatement.WithTriviaFrom(localDeclaration));
                }
            }
            else if (variableCount > 1)
            {
                newNode = newNode.RemoveNode(variable, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            }
            else
            {
                newNode = newNode.RemoveNode(localDeclaration, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            }

            ParameterSyntax newParameter = Parameter(type, identifier).WithFormatterAnnotation();

            if (newNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration))
            {
                var methodDeclaration = (MethodDeclarationSyntax)newNode;
                newNode = methodDeclaration.AddParameterListParameters(newParameter);
            }
            else
            {
                var localFunction = (LocalFunctionStatementSyntax)newNode;
                newNode = localFunction.AddParameterListParameters(newParameter);
            }

            return document.ReplaceNodeAsync(methodOrLocalFunction, newNode, cancellationToken);
        }
    }
}
