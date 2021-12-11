// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveImplementationFromAbstractMemberCodeFixProvider))]
    [Shared]
    public sealed class RemoveImplementationFromAbstractMemberCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0069_EventInInterfaceCannotHaveAddOrRemoveAccessors,
                    CompilerDiagnosticIdentifiers.CS0500_MemberCannotDeclareBodyBecauseItIsMarkedAbstract,
                    CompilerDiagnosticIdentifiers.CS0531_InterfaceMembersCannotHaveDefinition);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveImplementationFromAbstractMember, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f is MemberDeclarationSyntax || f is AccessorDeclarationSyntax))
                return;

            if (node.IsKind(SyntaxKind.AddAccessorDeclaration, SyntaxKind.RemoveAccessorDeclaration))
                node = node.Parent.Parent;

            CodeAction codeAction = CodeAction.Create(
                (node.IsKind(SyntaxKind.EventDeclaration)) ? "Remove accessor" : "Remove body",
                ct => RefactorAsync(context.Document, node, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        MethodDeclarationSyntax newNode = methodDeclaration
                            .WithBody(null)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SemicolonToken())
                            .WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;
                        ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                        PropertyDeclarationSyntax newNode = propertyDeclaration
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithAccessorList(AccessorList(AutoGetAccessorDeclaration()).WithTriviaFrom(expressionBody))
                            .WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                        IndexerDeclarationSyntax newNode = indexerDeclaration
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithAccessorList(AccessorList(AutoGetAccessorDeclaration()).WithTriviaFrom(expressionBody))
                            .WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)node;

                        EventFieldDeclarationSyntax eventFieldDeclaration = EventFieldDeclaration(
                            eventDeclaration.AttributeLists,
                            eventDeclaration.Modifiers,
                            eventDeclaration.EventKeyword,
                            VariableDeclaration(eventDeclaration.Type, VariableDeclarator(eventDeclaration.Identifier)),
                            SemicolonToken());

                        eventFieldDeclaration = eventFieldDeclaration.WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(eventDeclaration, eventFieldDeclaration, cancellationToken);
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        AccessorDeclarationSyntax newAccessor = accessor
                            .WithBody(null)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SemicolonToken())
                            .WithTrailingTrivia(
                                accessor.DescendantTrivia(
                                    TextSpan.FromBounds(
                                        accessor.BodyOrExpressionBody().SpanStart,
                                        accessor.Span.End)))
                            .WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(accessor, newAccessor, cancellationToken);
                    }
            }

            Debug.Fail("");

            return Task.FromResult(document);
        }
    }
}
