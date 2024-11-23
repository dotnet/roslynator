// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XmlNodeCodeFixProvider))]
[Shared]
public sealed class XmlNodeCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.UnusedElementInDocumentationComment,
                DiagnosticIdentifiers.InvalidReferenceInDocumentationComment,
                DiagnosticIdentifiers.FixDocumentationCommentTag);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out XmlNodeSyntax xmlNode, findInsideTrivia: true))
            return;

        Document document = context.Document;

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.UnusedElementInDocumentationComment:
                case DiagnosticIdentifiers.InvalidReferenceInDocumentationComment:
                    {
                        XmlElementInfo elementInfo = SyntaxInfo.XmlElementInfo(xmlNode);

                        string name = elementInfo.LocalName;

                        CodeAction codeAction = CodeAction.Create(
                            $"Remove '{name}' element",
                            ct => RemoveUnusedElementInDocumentationCommentAsync(document, elementInfo, ct),
                            GetEquivalenceKey(diagnostic, name));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.FixDocumentationCommentTag:
                    {
                        XmlElementInfo elementInfo = SyntaxInfo.XmlElementInfo(xmlNode);

                        CodeAction codeAction = CodeAction.Create(
                            (elementInfo.GetTag() == XmlTag.C)
                                ? "Rename tag to 'code'"
                                : "Rename tag to 'c'",
                            ct => FixDocumentationCommentTagAsync(document, elementInfo, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }

    private static Task<Document> RemoveUnusedElementInDocumentationCommentAsync(
        Document document,
        in XmlElementInfo elementInfo,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        XmlNodeSyntax element = elementInfo.Element;

        var documentationComment = (DocumentationCommentTriviaSyntax)element.Parent;

        SyntaxList<XmlNodeSyntax> content = documentationComment.Content;

        if (content.Count(f => f.IsKind(SyntaxKind.XmlElement, SyntaxKind.XmlEmptyElement)) == 1)
        {
            SyntaxNode declaration = documentationComment
                .GetParent(ascendOutOfTrivia: true)
                .FirstAncestorOrSelf(f => f is MemberDeclarationSyntax or LocalFunctionStatementSyntax);

            SyntaxNode newNode = SyntaxRefactorings.RemoveSingleLineDocumentationComment(declaration, documentationComment);
            return document.ReplaceNodeAsync(declaration, newNode, cancellationToken);
        }

        int start = element.FullSpan.Start;
        int end = element.FullSpan.End;

        int index = content.IndexOf(element);

        if (index > 0
            && content[index - 1].IsKind(SyntaxKind.XmlText))
        {
            start = content[index - 1].FullSpan.Start;

            if (index == 1)
            {
                SyntaxNode parent = documentationComment.GetParent(ascendOutOfTrivia: true);
                SyntaxTriviaList leadingTrivia = parent.GetLeadingTrivia();

                index = leadingTrivia.IndexOf(documentationComment.ParentTrivia);

                if (index > 0
                    && leadingTrivia[index - 1].IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    start = leadingTrivia[index - 1].FullSpan.Start;
                }

                SyntaxToken token = parent.GetFirstToken().GetPreviousToken(includeDirectives: true);
                parent = parent.FirstAncestorOrSelf(f => f.FullSpan.Contains(token.FullSpan));

                if (start > 0)
                {
                    SyntaxTrivia trivia = parent.FindTrivia(start - 1, findInsideTrivia: true);

                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia)
                        && start == trivia.FullSpan.End)
                    {
                        start = trivia.FullSpan.Start;
                    }
                }
            }
        }

        return document.WithTextChangeAsync(new TextChange(TextSpan.FromBounds(start, end), ""), cancellationToken);
    }

    private static Task<Document> FixDocumentationCommentTagAsync(
        Document document,
        in XmlElementInfo elementInfo,
        CancellationToken cancellationToken)
    {
        var element = (XmlElementSyntax)elementInfo.Element;

        XmlTag xmlTag = elementInfo.GetTag();

        XmlElementSyntax newElement;

        if (xmlTag == XmlTag.C)
        {
            newElement = element.UpdateName("code");
        }
        else if (xmlTag == XmlTag.Code)
        {
            newElement = element.UpdateName("c");
        }
        else
        {
            throw new InvalidOperationException();
        }

        return document.ReplaceNodeAsync(element, newElement, cancellationToken);
    }
}
