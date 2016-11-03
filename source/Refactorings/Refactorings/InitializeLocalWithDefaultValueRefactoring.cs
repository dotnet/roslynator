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
    internal static class InitializeLocalWithDefaultValueRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (localDeclaration.Declaration?.Variables.Count > 0)
            {
                VariableDeclaratorSyntax declarator = localDeclaration
                    .Declaration
                    .Variables
                    .FirstOrDefault(f => f.FullSpan.Contains(context.Span));

                if (declarator != null
                    && !declarator.Identifier.IsMissing
                    && (declarator.Initializer == null
                        || declarator.Initializer.IsMissing
                        || declarator.Initializer.Value == null
                        || declarator.Initializer.Value.IsMissing))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol typeSymbol = semanticModel
                        .GetTypeInfo(localDeclaration.Declaration.Type, context.CancellationToken)
                        .Type;

                    if (typeSymbol?.IsErrorType() == false)
                    {
                        context.RegisterRefactoring(
                            $"Initialize '{declarator.Identifier.ValueText}' with default value",
                            cancellationToken =>
                            {
                                return RefactorAsync(
                                    context.Document,
                                    localDeclaration,
                                    declarator,
                                    typeSymbol,
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            VariableDeclaratorSyntax declarator,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            VariableDeclaratorSyntax newDeclarator = GetNewDeclarator(
                declarator,
                localDeclaration.Declaration.Type.WithoutTrivia(),
                typeSymbol);

            LocalDeclarationStatementSyntax newNode = localDeclaration.ReplaceNode(declarator, newDeclarator);

            if (localDeclaration.SemicolonToken.IsMissing
                && localDeclaration.Declaration.Variables.Last().Equals(declarator))
            {
                newNode = newNode.WithSemicolonToken(SemicolonToken().WithTrailingTrivia(newDeclarator.GetTrailingTrivia()));
            }

            root = root.ReplaceNode(localDeclaration, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static VariableDeclaratorSyntax GetNewDeclarator(
            VariableDeclaratorSyntax declarator,
            TypeSyntax type,
            ITypeSymbol typeSymbol)
        {
            ExpressionSyntax value = SyntaxUtility.CreateDefaultValue(typeSymbol, type);

            EqualsValueClauseSyntax @default = EqualsValueClause(value);

            if (declarator.Initializer == null || declarator.Initializer.IsMissing)
            {
                return declarator
                    .WithIdentifier(declarator.Identifier.WithoutTrailingTrivia())
                    .WithInitializer(@default.WithTrailingTrivia(declarator.Identifier.TrailingTrivia));
            }
            else
            {
                return declarator
                    .WithInitializer(@default.WithTriviaFrom(declarator.Initializer.EqualsToken));
            }
        }
    }
}
