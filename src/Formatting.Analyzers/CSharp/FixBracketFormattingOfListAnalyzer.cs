using System;
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

        // Add if statement

        context.RegisterSyntaxNodeAction(f => AnalyzeParameterList(f), SyntaxKind.ParameterList);
        context.RegisterSyntaxNodeAction(f => AnalyzeBracketedParameterList(f), SyntaxKind.BracketedParameterList);
        context.RegisterSyntaxNodeAction(f => AnalyzeTypeParameterList(f), SyntaxKind.TypeParameterList);

        context.RegisterSyntaxNodeAction(f => AnalyzeArgumentList(f), SyntaxKind.ArgumentList);
        context.RegisterSyntaxNodeAction(f => AnalyzeBracketedArgumentList(f), SyntaxKind.BracketedArgumentList);
        context.RegisterSyntaxNodeAction(f => AnalyzeAttributeArgumentList(f), SyntaxKind.AttributeArgumentList);
        context.RegisterSyntaxNodeAction(f => AnalyzeTypeArgumentList(f), SyntaxKind.TypeArgumentList);

        context.RegisterSyntaxNodeAction(f => AnalyzeAttributeList(f), SyntaxKind.AttributeList);
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

        Analyze(context, parameterList.OpenParenToken, parameterList.CloseParenToken, parameterList.Parameters);
    }

    private static void AnalyzeBracketedParameterList(SyntaxNodeAnalysisContext context)
    {
        var parameterList = (BracketedParameterListSyntax)context.Node;

        Analyze(context, parameterList.OpenBracketToken, parameterList.CloseBracketToken, parameterList.Parameters);
    }

    private static void AnalyzeTypeParameterList(SyntaxNodeAnalysisContext context)
    {
        var parameterList = (TypeParameterListSyntax)context.Node;

        Analyze(context, parameterList.LessThanToken, parameterList.GreaterThanToken, parameterList.Parameters);
    }

    private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (ArgumentListSyntax)context.Node;

        Analyze(context, argumentList.OpenParenToken, argumentList.CloseParenToken, argumentList.Arguments);
    }

    private static void AnalyzeBracketedArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (BracketedArgumentListSyntax)context.Node;

        Analyze(context, argumentList.OpenBracketToken, argumentList.CloseBracketToken, argumentList.Arguments);
    }

    private static void AnalyzeAttributeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (AttributeArgumentListSyntax)context.Node;

        Analyze(context, argumentList.OpenParenToken, argumentList.CloseParenToken, argumentList.Arguments);
    }

    private static void AnalyzeTypeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (TypeArgumentListSyntax)context.Node;

        Analyze(context, argumentList.LessThanToken, argumentList.GreaterThanToken, argumentList.Arguments);
    }

    private static void AnalyzeAttributeList(SyntaxNodeAnalysisContext context)
    {
        var attributeList = (AttributeListSyntax)context.Node;

        Analyze(context, attributeList.OpenBracketToken, attributeList.CloseBracketToken, attributeList.Attributes);
    }

    private static void AnalyzeTupleType(SyntaxNodeAnalysisContext context)
    {
        var tupleType = (TupleTypeSyntax)context.Node;

        Analyze(context, tupleType.OpenParenToken, tupleType.CloseParenToken, tupleType.Elements);
    }

    private static void AnalyzeTupleExpression(SyntaxNodeAnalysisContext context)
    {
        var tupleExpression = (TupleExpressionSyntax)context.Node;

        Analyze(context, tupleExpression.OpenParenToken, tupleExpression.CloseParenToken, tupleExpression.Arguments);
    }

    private static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
    {
        var initializerExpression = (InitializerExpressionSyntax)context.Node;

        Analyze(context, initializerExpression.OpenBraceToken, initializerExpression.CloseBraceToken, initializerExpression.Expressions);
    }

    private static void Analyze<TNode>(
        SyntaxNodeAnalysisContext context,
        SyntaxToken openNodeOrToken,
        SyntaxToken closeNodeOrToken,
        SeparatedSyntaxList<TNode> nodes
    )
        where TNode : SyntaxNode
    {
        TargetBracesStyle bracesStyle = context.GetTargetBracesStyle();

        if (bracesStyle == TargetBracesStyle.None)
        {
            return;
        }

        TNode first = nodes.FirstOrDefault();

        if (first is null)
            return;

        SyntaxNode listNode = context.Node;
        TextSpan listSpan = listNode.GetSpan(includeExteriorTrivia: false);

        if (listSpan.IsSingleLine(listNode.SyntaxTree))
        {
            return;
        }

        TNode last = nodes.Last();

        if (
            ShouldFixOpeningBracket(bracesStyle, openNodeOrToken, first)
            || ShouldFixClosingBracket(bracesStyle, listNode, closeNodeOrToken, last)
        )
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.FixBracketFormattingOfList,
                Location.Create(listNode.SyntaxTree, listSpan),
                GetTitle(listNode)
            );
        }
    }

    private static string GetTitle(SyntaxNode listNode) =>
        listNode.Kind() switch
        {
            SyntaxKind.ParameterList
            or SyntaxKind.BracketedParameterList
            or SyntaxKind.TypeParameterList
                => "parameters",

            SyntaxKind.ArgumentList
            or SyntaxKind.BracketedArgumentList
            or SyntaxKind.AttributeArgumentList
            or SyntaxKind.TypeArgumentList
                => "arguments",

            SyntaxKind.AttributeList => "attributes",

            SyntaxKind.TupleType or SyntaxKind.TupleExpression => "a tuple",

            SyntaxKind.ArrayInitializerExpression
            or SyntaxKind.CollectionInitializerExpression
            or SyntaxKind.ComplexElementInitializerExpression
            or SyntaxKind.ObjectInitializerExpression
                => "an initializer",

            _ => throw new InvalidOperationException()
        };

    private static bool ShouldFixOpeningBracket(
        TargetBracesStyle bracesStyle,
        SyntaxToken leftBracket,
        SyntaxNode first
    )
    {
        if (!bracesStyle.HasFlag(TargetBracesStyle.Opening))
        {
            return false;
        }

        return leftBracket.GetSpanStartLine() == first.GetSpanStartLine();
    }

    private static bool ShouldFixClosingBracket(
        TargetBracesStyle bracesStyle,
        SyntaxNode listNode,
        SyntaxToken rightBracket,
        SyntaxNode last
    )
    {
        if (!bracesStyle.HasFlag(TargetBracesStyle.Closing))
        {
            return false;
        }

        if (rightBracket.GetSpanEndLine() == last.GetSpanEndLine())
        {
            return true;
        }

        SyntaxTrivia listNodeIndent = SyntaxTriviaAnalysis.DetermineIndentation(listNode);
        SyntaxTrivia bracketIndent = SyntaxTriviaAnalysis.DetermineIndentation(rightBracket);
        return listNodeIndent.Span.Length != bracketIndent.Span.Length;
    }
}