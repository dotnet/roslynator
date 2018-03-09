// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PromoteLocalToParameterRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetEnclosingSymbol(localDeclaration.SpanStart, context.CancellationToken);

            if (symbol?.IsImplicitlyDeclared == false
                && symbol.Kind == SymbolKind.Method)
            {
                var methodsymbol = (IMethodSymbol)symbol;

                if (methodsymbol.MethodKind == MethodKind.Ordinary)
                {
                    if (methodsymbol.PartialImplementationPart != null)
                        symbol = methodsymbol.PartialImplementationPart;

                    SyntaxNode node = await symbol
                        .DeclaringSyntaxReferences[0]
                        .GetSyntaxAsync(context.CancellationToken)
                        .ConfigureAwait(false);

                    if (node is MethodDeclarationSyntax method)
                    {
                        VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                        if (declaration != null)
                        {
                            VariableDeclaratorSyntax variable = declaration
                                .Variables
                                .FirstOrDefault(f => !f.IsMissing && f.Identifier.Span.Contains(context.Span));

                            if (variable != null)
                            {
                                TypeSyntax type = declaration.Type;

                                if (type != null)
                                {
                                    if (type.IsVar)
                                    {
                                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                                        if (typeSymbol?.SupportsExplicitDeclaration() == true)
                                        {
                                            type = typeSymbol.ToTypeSyntax();
                                        }
                                        else
                                        {
                                            type = null;
                                        }
                                    }

                                    if (type != null)
                                    {
                                        context.RegisterRefactoring(
                                            $"Promote '{variable.Identifier.ValueText}' to parameter",
                                            cancellationToken =>
                                            {
                                                return RefactorAsync(
                                                    context.Document,
                                                    method,
                                                    localDeclaration,
                                                    type.WithoutTrivia().WithSimplifierAnnotation(),
                                                    variable,
                                                    cancellationToken);
                                            });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax method,
            LocalDeclarationStatementSyntax localDeclaration,
            TypeSyntax type,
            VariableDeclaratorSyntax variable,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int variableCount = localDeclaration.Declaration.Variables.Count;
            ExpressionSyntax initializerValue = variable.Initializer?.Value;
            SyntaxToken identifier = variable.Identifier.WithoutTrivia();

            MethodDeclarationSyntax newMethod = method;

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

                    newMethod = newMethod.ReplaceNode(
                        localDeclaration,
                        new SyntaxNode[] { newLocalDeclaration, expressionStatement });
                }
                else
                {
                    newMethod = newMethod.ReplaceNode(
                        localDeclaration,
                        expressionStatement.WithTriviaFrom(localDeclaration));
                }
            }
            else if (variableCount > 1)
            {
                newMethod = newMethod.RemoveNode(variable, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            }
            else
            {
                newMethod = newMethod.RemoveNode(localDeclaration, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            }

            ParameterSyntax newParameter = Parameter(type, identifier).WithFormatterAnnotation();

            newMethod = newMethod.AddParameterListParameters(newParameter);

            return document.ReplaceNodeAsync(method, newMethod, cancellationToken);
        }
    }
}
