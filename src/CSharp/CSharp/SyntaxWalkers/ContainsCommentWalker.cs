// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxWalkers;

internal sealed class ContainsCommentWalker : CSharpSyntaxWalker
{
    public ContainsCommentWalker(TextSpan span)
        : base(SyntaxWalkerDepth.Trivia)
    {
        Span = span;
    }

    public bool Result { get; set; }

    public TextSpan Span { get; }

    public override void VisitTrivia(SyntaxTrivia trivia)
    {
        if (IsInSpan(trivia.Span)
            && CSharpFacts.IsCommentTrivia(trivia.Kind()))
        {
            Result = true;
        }

        base.VisitTrivia(trivia);
    }

    private bool IsInSpan(TextSpan span)
    {
        return Span.OverlapsWith(span)
            || (span.Length == 0 && Span.IntersectsWith(span));
    }

    public static bool ContainsComment(SyntaxNode node)
    {
        return ContainsComment(node, node.FullSpan);
    }

    public static bool ContainsComment(SyntaxNode node, TextSpan span)
    {
        var walker = new ContainsCommentWalker(span);

        walker.Visit(node);

        return walker.Result;
    }
}
