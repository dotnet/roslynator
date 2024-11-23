// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XmlNodeCodeFixProvider))]
[Shared]
public sealed class XmlNodeCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIdentifiers.FixDocumentationCommentTag);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out XmlNodeSyntax xmlNode, findInsideTrivia: true))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        XmlElementInfo elementInfo = SyntaxInfo.XmlElementInfo(xmlNode);

        CodeAction codeAction = CodeAction.Create(
            (elementInfo.GetTag() == XmlTag.C)
                ? "Rename tag to 'code'"
                : "Rename tag to 'c'",
            ct => FixDocumentationCommentTagAsync(document, elementInfo, ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
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
