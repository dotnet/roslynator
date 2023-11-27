// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddOrRemoveTrailingCommaCodeFixProvider))]
[Shared]
public sealed class AddOrRemoveTrailingCommaCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.AddOrRemoveTrailingComma); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            predicate: f => f.IsKind(
                SyntaxKind.ArrayInitializerExpression,
                SyntaxKind.ObjectInitializerExpression,
                SyntaxKind.CollectionInitializerExpression,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.AnonymousObjectCreationExpression)))
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];
        Document document = context.Document;

        if (node is InitializerExpressionSyntax initializer)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            int count = expressions.Count;

            if (count == expressions.SeparatorCount)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove comma",
                    ct => RemoveTrailingComma(document, expressions.GetSeparator(count - 1), ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Add comma",
                    ct => AddTrailingComma(document, expressions.Last(), ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
        else if (node is AnonymousObjectCreationExpressionSyntax objectCreation)
        {
            SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers = objectCreation.Initializers;

            int count = initializers.Count;

            if (count == initializers.SeparatorCount)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove comma",
                    ct => RemoveTrailingComma(document, initializers.GetSeparator(count - 1), ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Add comma",
                    ct => AddTrailingComma(document, initializers.Last(), ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
        else if (node is EnumDeclarationSyntax enumDeclaration)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int count = members.Count;

            if (count == members.SeparatorCount)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove comma",
                    ct => RemoveTrailingComma(document, members.GetSeparator(count - 1), ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Add comma",
                    ct => AddTrailingComma(document, members.Last(), ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }

    private static Task<Document> RemoveTrailingComma(
        Document document,
        SyntaxToken comma,
        CancellationToken cancellationToken)
    {
        return document.WithTextChangeAsync(new TextChange(comma.Span, ""), cancellationToken);
    }

    private static Task<Document> AddTrailingComma(
        Document document,
        SyntaxNode lastNode,
        CancellationToken cancellationToken)
    {
        return document.WithTextChangeAsync(new TextChange(new TextSpan(lastNode.Span.End, 0), ","), cancellationToken);
    }
}
