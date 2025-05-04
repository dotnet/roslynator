#nullable enable

using System;
using System.Collections.Generic;
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

        SyntaxToken firstToken = binaryExpression.GetFirstToken();
        SyntaxToken lastToken = binaryExpression.GetLastToken();

        IEnumerable<(SyntaxNode Parent, SyntaxToken OpenBracket, SyntaxToken CloseBracket)> nodesWithBrackets =
            FindNodesWithBrackets(binaryExpression);

        SemanticModel semanticModel = context.SemanticModel;

        foreach ((SyntaxNode parent, SyntaxToken openBracket, SyntaxToken closeBracket) in nodesWithBrackets)
        {
            if (openBracket.GetSpanStartLine(cancellationToken) == closeBracket.GetSpanEndLine(cancellationToken))
            {
                continue;
            }

            if (
                ShouldFixOpeningBracket(bracesStyle, openBracket, firstToken, cancellationToken)
                || ShouldFixClosingBracket(bracesStyle, parent, closeBracket, lastToken, cancellationToken)
            )
            {
                TextSpan span = TextSpan.FromBounds(openBracket.SpanStart, closeBracket.Span.End);

                Diagnostic? existingDiagnostic =
                    semanticModel.GetDiagnostic(
                        DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression,
                        span,
                        cancellationToken
                    );

                if (existingDiagnostic is not null)
                {
                    continue;
                }

                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.FixBracketFormattingOfBinaryExpression,
                    Location.Create(
                        parent.SyntaxTree,
                        span
                    ),
                    GetTitle(parent)
                );
            }
        }
    }

    private static IEnumerable<(SyntaxNode Parent, SyntaxToken OpenBracket, SyntaxToken CloseBracket)>
        FindNodesWithBrackets(SyntaxNode syntaxNode)
    {
        // Braced nodes can be in braces and in braces and ...
        // if the node itself is not ParenthesizedExpressionSyntax or the first parent doesn't have parentheses
        // then further processing should be stopped, otherwise multiple diagnostics for the same place will be reported

        SyntaxNode? parent = syntaxNode;
        int depth = 0;
        while (parent != null)
        {
            bool stop = false;

            switch (parent)
            {
                case ParenthesizedExpressionSyntax parenthesizedExpressionSyntax:
                    yield return (
                        parent,
                        parenthesizedExpressionSyntax.OpenParenToken,
                        parenthesizedExpressionSyntax.CloseParenToken
                    );
                    break;

                case IfStatementSyntax ifStatement:
                    yield return (
                        parent,
                        ifStatement.OpenParenToken,
                        ifStatement.CloseParenToken
                    );
                    // If-statement is considered as a final node.
                    stop = true;
                    break;

                case WhileStatementSyntax whileStatement:
                    yield return (
                        parent,
                        whileStatement.OpenParenToken,
                        whileStatement.CloseParenToken
                    );
                    // While-statement is considered as a final node.
                    stop = true;
                    break;

                case DoStatementSyntax doWhileStatement:
                    yield return (
                        parent,
                        doWhileStatement.OpenParenToken,
                        doWhileStatement.CloseParenToken
                    );
                    // Do-while-statement is considered as a final node.
                    stop = true;
                    break;

                default:
                    if (depth > 0)
                    {
                        stop = true;
                    }
                    break;
            }

            depth++;

            if (stop)
            {
                break;
            }

            parent = parent.FirstAncestor<SyntaxNode>();
        }
    }

    private static string GetTitle(SyntaxNode node) =>
        node.Kind() switch
        {
            SyntaxKind.IfStatement
                => "an 'if' statement",

            SyntaxKind.ParenthesizedExpression
                => "a parenthesized expression",

            SyntaxKind.WhileStatement
                => "a 'while' statement",

            SyntaxKind.DoStatement
                => "a 'do-while' statement",

            _ => throw new InvalidOperationException()
        };

    private static bool ShouldFixOpeningBracket(
        TargetBracesStyle bracesStyle,
        SyntaxToken leftBracket,
        SyntaxToken first,
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
        SyntaxToken last,
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