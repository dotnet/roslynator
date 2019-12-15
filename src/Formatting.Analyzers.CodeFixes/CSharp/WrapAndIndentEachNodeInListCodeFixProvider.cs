// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WrapAndIndentEachNodeInListCodeFixProvider))]
    [Shared]
    internal class WrapAndIndentEachNodeInListCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.WrapAndIndentEachNodeInList); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f =>
                {
                    switch (f.Kind())
                    {
                        case SyntaxKind.TypeArgumentList:
                        case SyntaxKind.ArgumentList:
                        case SyntaxKind.BracketedArgumentList:
                        case SyntaxKind.AttributeList:
                        case SyntaxKind.AttributeArgumentList:
                        case SyntaxKind.BaseList:
                        case SyntaxKind.ParameterList:
                        case SyntaxKind.BracketedParameterList:
                        case SyntaxKind.TypeParameterList:
                            return true;
                    }

                    return false;
                }))
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                $"Wrap and indent each {GetTitle()}",
                ct => WrapAndIndentEachNodeInListAsync(document, node, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);

            string GetTitle()
            {
                switch (node.Kind())
                {
                    case SyntaxKind.TypeArgumentList:
                        return "type argument";
                    case SyntaxKind.ArgumentList:
                    case SyntaxKind.BracketedArgumentList:
                    case SyntaxKind.AttributeArgumentList:
                        return "argument";
                    case SyntaxKind.AttributeList:
                        return "attribute";
                    case SyntaxKind.BaseList:
                        return "type";
                    case SyntaxKind.ParameterList:
                    case SyntaxKind.BracketedParameterList:
                        return "parameter";
                    case SyntaxKind.TypeParameterList:
                        return "type parameter";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private static Task<Document> WrapAndIndentEachNodeInListAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            switch (node)
            {
                case TypeArgumentListSyntax typeArgumentList:
                    return WrapAndIndentEachNodeInListAsync(document, typeArgumentList.Arguments, cancellationToken);
                case BaseArgumentListSyntax argumentList:
                    return WrapAndIndentEachNodeInListAsync(document, argumentList.Arguments, cancellationToken);
                case AttributeListSyntax attributeList:
                    return WrapAndIndentEachNodeInListAsync(document, attributeList.Attributes, cancellationToken);
                case AttributeArgumentListSyntax attributeArgumentList:
                    return WrapAndIndentEachNodeInListAsync(document, attributeArgumentList.Arguments, cancellationToken);
                case BaseListSyntax baseList:
                    return WrapAndIndentEachNodeInListAsync(document, baseList.Types, cancellationToken);
                case BaseParameterListSyntax parameterList:
                    return WrapAndIndentEachNodeInListAsync(document, parameterList.Parameters, cancellationToken);
                case TypeParameterListSyntax typeParameterList:
                    return WrapAndIndentEachNodeInListAsync(document, typeParameterList.Parameters, cancellationToken);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static Task<Document> WrapAndIndentEachNodeInListAsync<TNode>(
            Document document,
            SeparatedSyntaxList<TNode> nodes,
            CancellationToken cancellationToken) where TNode : SyntaxNode
        {
            return document.WithTextChangesAsync(GetTextChanges().Where(f => f != default), cancellationToken);

            IEnumerable<TextChange> GetTextChanges()
            {
                string newText = SyntaxTriviaAnalysis.GetEndOfLine(nodes[0]).ToString() + GetIndentation();

                yield return GetTextChange(nodes[0].GetFirstToken().GetPreviousToken(), nodes[0]);

                for (int i = 1; i < nodes.Count; i++)
                {
                    yield return GetTextChange(nodes.GetSeparator(i - 1), nodes[i]);
                }

                TextChange GetTextChange(SyntaxToken token, SyntaxNode node)
                {
                    TextSpan span = TextSpan.FromBounds(token.Span.End, node.SpanStart);

                    return (node.SyntaxTree.IsSingleLineSpan(span))
                        ? new TextChange(span, newText)
                        : default;
                }
            }

            string GetIndentation()
            {
                foreach (TNode node in nodes)
                {
                    SyntaxTriviaList.Reversed.Enumerator en = node.GetLeadingTrivia().Reverse().GetEnumerator();

                    if (en.MoveNext()
                        && en.Current.IsWhitespaceTrivia())
                    {
                        SyntaxTrivia whitespaceTrivia = en.Current;

                        if (!en.MoveNext()
                            || en.Current.IsEndOfLineTrivia())
                        {
                            return whitespaceTrivia.ToString();
                        }
                    }
                }

                return nodes[0].GetIndentation(cancellationToken).ToString() + nodes[0].SyntaxTree.GetFirstIndentation(cancellationToken);
            }
        }
    }
}
