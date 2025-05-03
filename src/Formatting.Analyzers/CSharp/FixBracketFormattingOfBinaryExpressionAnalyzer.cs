#nullable enable

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FixBracketFormattingOfBinaryExpressionAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.FixBracketFormattingOfBinaryExpression);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            f => AnalyzeBinaryExpression(f),
            SyntaxKind.AddExpression,
            SyntaxKind.SubtractExpression,
            SyntaxKind.MultiplyExpression,
            SyntaxKind.DivideExpression,
            SyntaxKind.ModuloExpression,
            SyntaxKind.LeftShiftExpression,
            SyntaxKind.RightShiftExpression,
            SyntaxKind.LogicalOrExpression,
            SyntaxKind.LogicalAndExpression,
            SyntaxKind.BitwiseOrExpression,
            SyntaxKind.BitwiseAndExpression,
            SyntaxKind.ExclusiveOrExpression,
            SyntaxKind.EqualsExpression,
            SyntaxKind.NotEqualsExpression,
            SyntaxKind.LessThanExpression,
            SyntaxKind.LessThanOrEqualExpression,
            SyntaxKind.GreaterThanExpression,
            SyntaxKind.GreaterThanOrEqualExpression,
            SyntaxKind.IsExpression,
            SyntaxKind.AsExpression
        );
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        TargetBracesStyle bracesStyle = context.GetTargetBracesStyle();

        if (bracesStyle == TargetBracesStyle.None)
        {
            return;
        }

        CancellationToken cancellationToken = context.CancellationToken;

        BinaryExpressionSyntax binaryExpression = (BinaryExpressionSyntax)context.Node;

        ExpressionSyntax left = binaryExpression.Left;

        if (left.IsMissing)
        {
            return;
        }

        ExpressionSyntax right = binaryExpression.Right;

        if (right.IsMissing)
        {
            return;
        }

        SyntaxToken? openBracket;
        SyntaxToken? closeBracket;
        SyntaxNode? parent = FindBrackets(right, out openBracket, out closeBracket);

        if (parent == null || parent.IsSingleLine() || openBracket is null || closeBracket is null)
        {
            return;
        }

        var anyFirstDescendant = parent.DescendantNodesAndTokens();

        if (
            ShouldFixOpeningBracket(bracesStyle, openBracket.Value, left, cancellationToken)
            || ShouldFixClosingBracket(bracesStyle, parent, closeBracket.Value, right, cancellationToken)
        )
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.FixBracketFormattingOfBinaryExpression,
                Location.Create(
                    parent.SyntaxTree,
                    TextSpan.FromBounds(openBracket.Value.SpanStart, closeBracket.Value.Span.End)
                ),
                GetTitle(parent)
            );
        }
    }

    private static SyntaxNode? FindBrackets(
        SyntaxNode syntaxNode,
        out SyntaxToken? openBracket,
        out SyntaxToken? closeBracket
    )
    {
        SyntaxNode? parent = syntaxNode.FirstAncestor<SyntaxNode>();
        //while (parent != null)
        {
            parent = parent.FirstAncestor<SyntaxNode>();
            switch (parent)
            {
                case IfStatementSyntax ifStatement:
                    openBracket = ifStatement.OpenParenToken;
                    closeBracket = ifStatement.CloseParenToken;
                    return parent;
            }
        }

        openBracket = null;
        closeBracket = null;
        return null;
    }

    private static string GetTitle(SyntaxNode listNode) =>
        listNode.Kind() switch
        {
            SyntaxKind.IfStatement
                => "an 'if' statement",

            _ => throw new InvalidOperationException()
        };

    private static bool ShouldFixOpeningBracket(
        TargetBracesStyle bracesStyle,
        SyntaxToken leftBracket,
        SyntaxNode first,
        CancellationToken cancellationToken
    )
    {
        if (!bracesStyle.HasFlag(TargetBracesStyle.Opening))
        {
            return false;
        }

        return leftBracket.GetSpanStartLine(cancellationToken) == first.GetSpanStartLine(cancellationToken);
    }

    private static bool ShouldFixClosingBracket(
        TargetBracesStyle bracesStyle,
        SyntaxNode listNode,
        SyntaxToken rightBracket,
        SyntaxNode last,
        CancellationToken cancellationToken
    )
    {
        if (!bracesStyle.HasFlag(TargetBracesStyle.Closing))
        {
            return false;
        }

        if (rightBracket.GetSpanEndLine(cancellationToken) == last.GetSpanEndLine(cancellationToken))
        {
            return true;
        }

        SyntaxTrivia listNodeIndent =
            SyntaxTriviaAnalysis.DetermineIndentation(listNode, searchInAccessors: false, cancellationToken);
        SyntaxTrivia bracketIndent =
            SyntaxTriviaAnalysis.DetermineIndentation(rightBracket, searchInAccessors: false, cancellationToken);
        return listNodeIndent.Span.Length != bracketIndent.Span.Length;
    }
}