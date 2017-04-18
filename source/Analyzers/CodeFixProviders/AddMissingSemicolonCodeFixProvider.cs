// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddMissingSemicolonCodeFixProvider))]
    [Shared]
    public class AddMissingSemicolonCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddMissingSemicolon); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SyntaxNode>(f =>
                {
                    switch (f.Kind())
                    {
                        case SyntaxKind.MethodDeclaration:
                        case SyntaxKind.ConstructorDeclaration:
                        case SyntaxKind.DestructorDeclaration:
                        case SyntaxKind.OperatorDeclaration:
                        case SyntaxKind.ConversionOperatorDeclaration:
                        case SyntaxKind.PropertyDeclaration:
                        case SyntaxKind.IndexerDeclaration:
                        case SyntaxKind.GetAccessorDeclaration:
                        case SyntaxKind.SetAccessorDeclaration:
                        case SyntaxKind.AddAccessorDeclaration:
                        case SyntaxKind.RemoveAccessorDeclaration:
                        case SyntaxKind.FieldDeclaration:
                        case SyntaxKind.EventFieldDeclaration:
                        case SyntaxKind.BreakStatement:
                        case SyntaxKind.ContinueStatement:
                        case SyntaxKind.DoStatement:
                        case SyntaxKind.ExpressionStatement:
                        case SyntaxKind.GotoStatement:
                        case SyntaxKind.GotoCaseStatement:
                        case SyntaxKind.GotoDefaultStatement:
                        case SyntaxKind.LocalDeclarationStatement:
                        case SyntaxKind.ReturnStatement:
                        case SyntaxKind.ThrowStatement:
                        case SyntaxKind.YieldBreakStatement:
                        case SyntaxKind.YieldReturnStatement:
                        case SyntaxKind.UsingDirective:
                        case SyntaxKind.ExternAliasDirective:
                            return true;
                        default:
                            return false;
                    }
                });

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Add missing semicolon",
                cancellationToken => RefactorAsync(context.Document, node, cancellationToken),
                DiagnosticIdentifiers.AddMissingSemicolon + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            SyntaxToken token = node.GetLastToken();

            SyntaxNode newNode = node.ReplaceToken(token, token.WithoutTrailingTrivia());

            SyntaxToken semicolonToken = SemicolonToken().WithTrailingTrivia(token.TrailingTrivia);

            newNode = AddMissingToken(newNode, semicolonToken);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private SyntaxNode AddMissingToken(SyntaxNode node, SyntaxToken semicolonToken)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.BreakStatement:
                    return ((BreakStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.ContinueStatement:
                    return ((ContinueStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                    return ((GotoStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.LocalDeclarationStatement:
                    return ((LocalDeclarationStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.ThrowStatement:
                    return ((ThrowStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.YieldReturnStatement:
                    return ((YieldStatementSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.UsingDirective:
                    return ((UsingDirectiveSyntax)node).WithSemicolonToken(semicolonToken);
                case SyntaxKind.ExternAliasDirective:
                    return ((ExternAliasDirectiveSyntax)node).WithSemicolonToken(semicolonToken);
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return node;
                    }
            }
        }
    }
}
