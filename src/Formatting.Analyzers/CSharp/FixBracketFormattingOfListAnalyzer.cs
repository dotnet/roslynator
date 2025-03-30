﻿using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FixBracketFormattingOfListAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.FixBracketFormattingOfList);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeParameterList(f), SyntaxKind.ParameterList);
        context.RegisterSyntaxNodeAction(f => AnalyzeBracketedParameterList(f), SyntaxKind.BracketedParameterList);
        context.RegisterSyntaxNodeAction(f => AnalyzeTypeParameterList(f), SyntaxKind.TypeParameterList);

        context.RegisterSyntaxNodeAction(f => AnalyzeArgumentList(f), SyntaxKind.ArgumentList);
        context.RegisterSyntaxNodeAction(f => AnalyzeBracketedArgumentList(f), SyntaxKind.BracketedArgumentList);
        context.RegisterSyntaxNodeAction(f => AnalyzeAttributeArgumentList(f), SyntaxKind.AttributeArgumentList);
        context.RegisterSyntaxNodeAction(f => AnalyzeTypeArgumentList(f), SyntaxKind.TypeArgumentList);

        context.RegisterSyntaxNodeAction(f => AnalyzeAttributeList(f), SyntaxKind.AttributeList);
        context.RegisterSyntaxNodeAction(f => AnalyzeBaseList(f), SyntaxKind.BaseList);
        context.RegisterSyntaxNodeAction(f => AnalyzeTupleType(f), SyntaxKind.TupleType);
        context.RegisterSyntaxNodeAction(f => AnalyzeTupleExpression(f), SyntaxKind.TupleExpression);

        context.RegisterSyntaxNodeAction(
            f => AnalyzeInitializerExpression(f),
            SyntaxKind.ArrayInitializerExpression,
            SyntaxKind.CollectionInitializerExpression,
            SyntaxKind.ComplexElementInitializerExpression,
            SyntaxKind.ObjectInitializerExpression
        );
    }

    private static void AnalyzeParameterList(SyntaxNodeAnalysisContext context)
    {
        var parameterList = (ParameterListSyntax)context.Node;

        Analyze(context, parameterList.OpenParenToken, parameterList.Parameters);
    }

    private static void AnalyzeBracketedParameterList(SyntaxNodeAnalysisContext context)
    {
        var parameterList = (BracketedParameterListSyntax)context.Node;

        Analyze(context, parameterList.OpenBracketToken, parameterList.Parameters);
    }

    private static void AnalyzeTypeParameterList(SyntaxNodeAnalysisContext context)
    {
        var parameterList = (TypeParameterListSyntax)context.Node;

        Analyze(context, parameterList.LessThanToken, parameterList.Parameters);
    }

    private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (ArgumentListSyntax)context.Node;

        Analyze(context, argumentList.OpenParenToken, argumentList.Arguments);
    }

    private static void AnalyzeBracketedArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (BracketedArgumentListSyntax)context.Node;

        Analyze(context, argumentList.OpenBracketToken, argumentList.Arguments);
    }

    private static void AnalyzeAttributeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (AttributeArgumentListSyntax)context.Node;

        Analyze(context, argumentList.OpenParenToken, argumentList.Arguments);
    }

    private static void AnalyzeTypeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (TypeArgumentListSyntax)context.Node;

        Analyze(context, argumentList.LessThanToken, argumentList.Arguments);
    }

    private static void AnalyzeAttributeList(SyntaxNodeAnalysisContext context)
    {
        var attributeList = (AttributeListSyntax)context.Node;

        Analyze(context, attributeList.OpenBracketToken, attributeList.Attributes);
    }

    private static void AnalyzeBaseList(SyntaxNodeAnalysisContext context)
    {
        var baseList = (BaseListSyntax)context.Node;

        Analyze(context, baseList.ColonToken, baseList.Types);
    }

    private static void AnalyzeTupleType(SyntaxNodeAnalysisContext context)
    {
        var tupleType = (TupleTypeSyntax)context.Node;

        Analyze(context, tupleType.OpenParenToken, tupleType.Elements);
    }

    private static void AnalyzeTupleExpression(SyntaxNodeAnalysisContext context)
    {
        var tupleExpression = (TupleExpressionSyntax)context.Node;

        Analyze(context, tupleExpression.OpenParenToken, tupleExpression.Arguments);
    }

    private static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
    {
        var initializerExpression = (InitializerExpressionSyntax)context.Node;

        Analyze(context, initializerExpression.OpenBraceToken, initializerExpression.Expressions);
    }

    private static void Analyze<TNode>(
        SyntaxNodeAnalysisContext context,
        SyntaxNodeOrToken openNodeOrToken,
        SeparatedSyntaxList<TNode> nodes
    )
        where TNode : SyntaxNode
    {
        TargetBracesStyle style = context.GetTargetBracesStyle();

        if (style == TargetBracesStyle.None)
        {
            return;
        }

        TNode first = nodes.FirstOrDefault();

        if (first is null)
            return;

        TextSpan span = nodes.GetSpan(includeExteriorTrivia: false);

        if (span.IsSingleLine(first.SyntaxTree))
        {
            if (!TriviaBlock.FromTrailing(openNodeOrToken).IsWrapped)
                return;

            int indentationLength = IndentationAnalysis.Create(openNodeOrToken.Parent, context.GetConfigOptions()).IncreasedIndentationLength;

            if (indentationLength == 0)
                return;

            if (ShouldFixIndentation(first.GetLeadingTrivia(), indentationLength))
                ReportDiagnostic();
        }
        else
        {
            TextLineCollection lines = null;
            IndentationAnalysis indentationAnalysis = IndentationAnalysis.Create(openNodeOrToken.Parent, context.GetConfigOptions());
            int indentationLength = indentationAnalysis.IncreasedIndentationLength;

            if (indentationLength == 0)
                return;

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                SyntaxNodeOrToken nodeOrToken = (i == 0)
                    ? openNodeOrToken
                    : nodes.GetSeparator(i - 1);

                if (TriviaBlock.FromTrailing(nodeOrToken).IsWrapped)
                {
                    if (AnalyzeBlock(nodes, indentationAnalysis, SyntaxKind.OpenBracketToken, SyntaxKind.CloseBracketToken))
                        return;

                    if (ShouldFixIndentation(nodes[i].GetLeadingTrivia(), indentationLength))
                    {
                        ReportDiagnostic();
                        return;
                    }
                }
                else
                {
                    if (nodes.Count > 1
                        && ShouldWrapAndIndent(context.Node, i))
                    {
                        ReportDiagnostic();
                        return;
                    }

                    if (AnalyzeBlock(nodes, indentationAnalysis, SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken))
                        return;

                    lines ??= first.SyntaxTree.GetText().Lines;

                    int lineIndex = lines.IndexOf(span.Start);
                    if (lineIndex < lines.Count - 1)
                    {
                        int lineStartIndex = lines[lineIndex + 1].Start;

                        if (first.Span.Contains(lineStartIndex))
                        {
                            SyntaxToken token = first.FindToken(lineStartIndex);

                            if (!token.IsKind(SyntaxKind.None))
                            {
                                SyntaxTriviaList leading = token.LeadingTrivia;

                                if (leading.Any())
                                {
                                    if (leading.FullSpan.Contains(lineStartIndex))
                                    {
                                        SyntaxTrivia trivia = leading.Last();

                                        if (trivia.IsWhitespaceTrivia()
                                            && trivia.SpanStart == lineStartIndex
                                            && trivia.Span.Length != indentationLength)
                                        {
                                            ReportDiagnostic();
                                            return;
                                        }
                                    }
                                }
                                else if (lineStartIndex == token.SpanStart)
                                {
                                    ReportDiagnostic();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        void ReportDiagnostic()
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.FixBracketFormattingOfList,
                Location.Create(first.SyntaxTree, nodes.Span),
                GetTitle()
            );
        }

        static bool ShouldFixIndentation(SyntaxTriviaList leading, int indentationLength)
        {
            SyntaxTriviaList.Reversed.Enumerator en = leading.Reverse().GetEnumerator();

            if (!en.MoveNext())
                return true;

            switch (en.Current.Kind())
            {
                case SyntaxKind.WhitespaceTrivia:
                {
                    if (en.Current.Span.Length != indentationLength)
                    {
                        if (!en.MoveNext()
                            || en.Current.IsEndOfLineTrivia())
                        {
                            return true;
                        }
                    }

                    break;
                }
                case SyntaxKind.EndOfLineTrivia:
                {
                    return true;
                }
            }

            return false;
        }

        string GetTitle()
        {
            switch (context.Node.Kind())
            {
                case SyntaxKind.ParameterList:
                case SyntaxKind.BracketedParameterList:
                case SyntaxKind.TypeParameterList:
                    return "parameters";
                case SyntaxKind.ArgumentList:
                case SyntaxKind.BracketedArgumentList:
                case SyntaxKind.AttributeArgumentList:
                case SyntaxKind.TypeArgumentList:
                    return "arguments";
                case SyntaxKind.AttributeList:
                    return "attributes";
                case SyntaxKind.BaseList:
                    return "base types";
                case SyntaxKind.TupleType:
                case SyntaxKind.TupleExpression:
                    return "a tuple";
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ComplexElementInitializerExpression:
                case SyntaxKind.ObjectInitializerExpression:
                    return "an initializer";
                default:
                    throw new InvalidOperationException();
            }
        }

        bool AnalyzeBlock(
            SeparatedSyntaxList<TNode> nodes,
            IndentationAnalysis indentationAnalysis,
            SyntaxKind kind1,
            SyntaxKind kind2)
        {
            if (nodes.Count == 1
                && nodes[0].IsKind(SyntaxKind.Argument))
            {
                BracesBlock block = GetBracesBlock(nodes[0]);

                if (block.Token.IsKind(kind1, kind2))
                {
                    SyntaxToken token = block.Token;
                    SyntaxTriviaList leading = token.LeadingTrivia;

                    if (leading.Any())
                    {
                        SyntaxTrivia trivia = leading.Last();

                        if (trivia.IsWhitespaceTrivia()
                            && trivia.SpanStart == block.LineStartIndex
                            && trivia.Span.Length != indentationAnalysis.IndentationLength)
                        {
                            ReportDiagnostic();
                        }
                    }
                    else if (block.LineStartIndex == token.SpanStart)
                    {
                        ReportDiagnostic();
                    }
                    return true;
                }
            }

            return false;
        }
    }

    internal static BracesBlock GetBracesBlock(SyntaxNode node)
    {
        TextLineCollection lines = node.SyntaxTree.GetText().Lines;
        TextLine line = lines.GetLineFromPosition(node.SpanStart);

        int startIndex = line.End;

        if (!node.FullSpan.Contains(startIndex))
            return default;

        SyntaxToken openToken = node.FindToken(startIndex);
        SyntaxNode block = null;
        var isOpenBraceAtEndOfLine = false;

        if (AnalyzeToken(openToken, isOpen: true))
        {
            SyntaxTriviaList trailing = openToken.TrailingTrivia;

            if (trailing.Any()
                && trailing.Span.Contains(startIndex))
            {
                block = openToken.Parent;
                isOpenBraceAtEndOfLine = true;
            }
        }

        if (block is null)
        {
            startIndex = line.EndIncludingLineBreak;
            openToken = node.FindToken(startIndex);

            if (AnalyzeToken(openToken, isOpen: true))
            {
                SyntaxTriviaList leading = openToken.LeadingTrivia;

                if ((leading.Any() && leading.Span.Contains(startIndex))
                    || (!leading.Any() && openToken.SpanStart == startIndex))
                {
                    block = openToken.Parent;
                }
            }
        }

        if (block is not null)
        {
            int endIndex = lines.GetLineFromPosition(node.Span.End).Start;
            SyntaxToken closeToken = node.FindToken(endIndex);

            if (AnalyzeToken(closeToken, isOpen: false)
                && object.ReferenceEquals(block, closeToken.Parent))
            {
                SyntaxTriviaList leading = closeToken.LeadingTrivia;

                if ((leading.Any() && leading.Span.Contains(endIndex))
                    || (!leading.Any() && closeToken.SpanStart == endIndex))
                {
                    return new BracesBlock(
                        (isOpenBraceAtEndOfLine) ? closeToken : openToken,
                        (isOpenBraceAtEndOfLine) ? endIndex : startIndex);
                }
            }
        }

        return default;

        static bool AnalyzeToken(SyntaxToken token, bool isOpen)
        {
            if (token.IsKind((isOpen) ? SyntaxKind.OpenBraceToken : SyntaxKind.CloseBraceToken))
            {
                if (token.IsParentKind(SyntaxKind.Block)
                    && (CSharpFacts.IsAnonymousFunctionExpression(token.Parent.Parent.Kind())))
                {
                    return true;
                }

                if (token.IsParentKind(SyntaxKind.SwitchExpression))
                    return true;

                if (token.IsParentKind(SyntaxKind.ObjectInitializerExpression)
                    && token.Parent.Parent.IsKind(
                        SyntaxKind.ObjectCreationExpression,
                        SyntaxKind.AnonymousObjectCreationExpression,
                        SyntaxKind.ImplicitObjectCreationExpression))
                {
                    return true;
                }

                if (token.IsParentKind(SyntaxKind.ArrayInitializerExpression)
                    && token.Parent.Parent.IsKind(
                        SyntaxKind.ArrayCreationExpression,
                        SyntaxKind.ImplicitArrayCreationExpression))
                {
                    return true;
                }
            }
#if ROSLYN_4_7
            return token.IsKind((isOpen) ? SyntaxKind.OpenBracketToken : SyntaxKind.CloseBracketToken)
                   && token.IsParentKind(SyntaxKind.CollectionExpression);
#else
            return false;
#endif
        }
    }

    internal static bool ShouldWrapAndIndent(SyntaxNode node, int index)
    {
        if (index == 0)
        {
            if (node.IsKind(SyntaxKind.AttributeList))
            {
                return false;
            }
            else if (node.IsKind(SyntaxKind.TupleExpression)
                     && node.Parent is AssignmentExpressionSyntax assignmentExpression
                     && object.ReferenceEquals(node, assignmentExpression.Left))
            {
                return false;
            }
        }

        return true;
    }

    internal readonly struct BracesBlock
    {
        public BracesBlock(SyntaxToken token, int lineStartIndex)
        {
            Token = token;
            LineStartIndex = lineStartIndex;
        }

        public SyntaxToken Token { get; }

        public int LineStartIndex { get; }
    }
}