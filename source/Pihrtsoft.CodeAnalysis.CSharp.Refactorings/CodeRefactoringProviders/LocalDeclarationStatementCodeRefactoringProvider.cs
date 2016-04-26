// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(LocalDeclarationStatementCodeRefactoringProvider))]
    public class LocalDeclarationStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            LocalDeclarationStatementSyntax localDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();

            if (localDeclaration == null)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel == null)
                return;

            AddUsingStatement(context, localDeclaration, semanticModel);
        }

        private static void AddUsingStatement(
            CodeRefactoringContext context,
            LocalDeclarationStatementSyntax localDeclaration,
            SemanticModel semanticModel)
        {
            if (GetInitializerValue(localDeclaration)?.IsKind(SyntaxKind.ObjectCreationExpression) != true)
                return;

            if (localDeclaration.Declaration?.Type == null)
                return;

            ITypeSymbol type = semanticModel.GetTypeInfo(localDeclaration.Declaration.Type).Type;

            if (type?.Kind != SymbolKind.NamedType)
                return;

            if (!((INamedTypeSymbol)type).Implements(SpecialType.System_IDisposable))
                return;

            context.RegisterRefactoring(
                "Add using statement",
                cancellationToken => AddUsingStatementAsync(context.Document, localDeclaration, cancellationToken));
        }

        private static async Task<Document> AddUsingStatementAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            UsingStatementSyntax usingStatement = SyntaxFactory
                .UsingStatement(localDeclaration.Declaration.WithoutTrivia(), null, SyntaxFactory.Block())
                .WithTriviaFrom(localDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(localDeclaration, usingStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ExpressionSyntax GetInitializerValue(LocalDeclarationStatementSyntax localDeclaration)
        {
            if (localDeclaration == null)
                throw new ArgumentNullException(nameof(localDeclaration));

            return localDeclaration
                .Declaration?
                .Variables
                .FirstOrDefault()?
                .Initializer?
                .Value;
        }
    }
}
