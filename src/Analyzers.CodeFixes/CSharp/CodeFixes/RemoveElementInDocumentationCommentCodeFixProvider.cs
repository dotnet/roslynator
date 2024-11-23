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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveElementInDocumentationCommentCodeFixProvider))]
[Shared]
public sealed class RemoveElementInDocumentationCommentCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.UnusedElementInDocumentationComment,
                DiagnosticIdentifiers.InvalidReferenceInDocumentationComment);
        }
    }

    public override FixAllProvider GetFixAllProvider()
    {
        return FixAllProvider.Create(async (context, document, diagnostics) => await FixAllAsync(document, diagnostics, context.CancellationToken).ConfigureAwait(false));

        static async Task<Document> FixAllAsync(
            Document document,
            ImmutableArray<Diagnostic> diagnostics,
            CancellationToken cancellationToken)
        {
            foreach (Diagnostic diagnostic in diagnostics.OrderByDescending(d => d.Location.SourceSpan.Start))
            {
                (Func<CancellationToken, Task<Document>> CreateChangedDocument, string) result
                    = await GetChangedDocumentAsync(document, diagnostic, cancellationToken).ConfigureAwait(false);

                document = await result.CreateChangedDocument(cancellationToken).ConfigureAwait(false);
            }

            return document;
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out XmlNodeSyntax xmlNode, findInsideTrivia: true))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        (Func<CancellationToken, Task<Document>> createChangedDocument, string name)
            = await GetChangedDocumentAsync(document, diagnostic, context.CancellationToken).ConfigureAwait(false);

        CodeAction codeAction = CodeAction.Create(
            $"Remove '{name}' element",
            ct => createChangedDocument(ct),
            GetEquivalenceKey(diagnostic, name));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static async Task<(Func<CancellationToken, Task<Document>>, string)> GetChangedDocumentAsync(
        Document document,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, diagnostic.Location.SourceSpan, out XmlNodeSyntax xmlNode, findInsideTrivia: true))
        {
            throw new InvalidOperationException();
        }

        XmlElementInfo elementInfo = SyntaxInfo.XmlElementInfo(xmlNode);
        string name = elementInfo.LocalName;

        return (ct => RemoveUnusedElementInDocumentationCommentAsync(document, elementInfo, ct), $"Remove '{name}' element");
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
}
