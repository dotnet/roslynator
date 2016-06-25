// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class WrapInUsingStatementRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (localDeclaration.Declaration?.Variables.Count == 1
                && localDeclaration.Declaration.Variables[0].Initializer?.Value?.IsKind(SyntaxKind.ObjectCreationExpression) == true
                && localDeclaration.Declaration.Type != null)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ITypeSymbol type = semanticModel
                    .GetTypeInfo(localDeclaration.Declaration.Type)
                    .Type;

                if (type?.Kind == SymbolKind.NamedType
                    && ((INamedTypeSymbol)type).Implements(SpecialType.System_IDisposable))
                {
                    context.RegisterRefactoring(
                        "Wrap in using statement",
                        cancellationToken => RefactorAsync(context.Document, localDeclaration, cancellationToken));
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            UsingStatementSyntax usingStatement = SyntaxFactory
                .UsingStatement(localDeclaration.Declaration.WithoutTrivia(), null, SyntaxFactory.Block())
                .WithTriviaFrom(localDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(localDeclaration, usingStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
