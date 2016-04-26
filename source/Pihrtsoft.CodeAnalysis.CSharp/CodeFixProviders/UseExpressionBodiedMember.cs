// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseExpressionBodiedMemberCodeFixProvider))]
    [Shared]
    public class UseExpressionBodiedMemberCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.UseExpressionBodiedMember);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (declaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Use expression-bodied member",
                cancellationToken => UseExpressionBodiedMemberAsync(context.Document, declaration, cancellationToken),
                DiagnosticIdentifiers.UseExpressionBodiedMember + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> UseExpressionBodiedMemberAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newDeclaration = GetNewDeclaration(declaration)
                .WithTrailingTrivia(declaration.GetTrailingTrivia())
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(declaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberDeclarationSyntax GetNewDeclaration(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return GetNewDeclaration((MethodDeclarationSyntax)declaration);
                case SyntaxKind.OperatorDeclaration:
                    return GetNewDeclaration((OperatorDeclarationSyntax)declaration);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return GetNewDeclaration((ConversionOperatorDeclarationSyntax)declaration);
                case SyntaxKind.PropertyDeclaration:
                    return GetNewDeclaration((PropertyDeclarationSyntax)declaration);
                case SyntaxKind.IndexerDeclaration:
                    return GetNewDeclaration((IndexerDeclarationSyntax)declaration);
                default:
                    return declaration;
            }
        }

        private static MemberDeclarationSyntax GetNewDeclaration(MethodDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(GetExpression(declaration.Body)))
                .WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(OperatorDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(GetExpression(declaration.Body)))
                .WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(ConversionOperatorDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(GetExpression(declaration.Body)))
                .WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(PropertyDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(GetExpression(declaration.AccessorList)))
                .WithAccessorList(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(IndexerDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(GetExpression(declaration.AccessorList)))
                .WithAccessorList(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static ExpressionSyntax GetExpression(BlockSyntax block)
        {
            var returnStatement = (ReturnStatementSyntax)block.Statements[0];

            return returnStatement.Expression;
        }

        private static ExpressionSyntax GetExpression(AccessorListSyntax accessorList)
        {
            var returnStatement = (ReturnStatementSyntax)accessorList.Accessors[0].Body.Statements[0];

            return returnStatement.Expression;
        }
    }
}
