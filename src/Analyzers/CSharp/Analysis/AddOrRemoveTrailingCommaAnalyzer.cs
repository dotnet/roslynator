// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddOrRemoveTrailingCommaAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddOrRemoveTrailingComma);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeEnumDeclaration(f), SyntaxKind.EnumDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeAnonymousObjectCreationExpression(f), SyntaxKind.AnonymousObjectCreationExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeSwitchExpression(f), SyntaxKind.SwitchExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzePropertyPatternClause(f), SyntaxKind.PropertyPatternClause);
#if ROSLYN_4_7
        context.RegisterSyntaxNodeAction(f => AnalyzeCollectionExpression(f), SyntaxKind.CollectionExpression);
#endif

        context.RegisterSyntaxNodeAction(
            f => AnalyzeInitializerExpression(f),
            SyntaxKind.ArrayInitializerExpression,
            SyntaxKind.ObjectInitializerExpression,
            SyntaxKind.CollectionInitializerExpression);
    }

    private static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
    {
        TrailingCommaStyle style = context.GetTrailingCommaStyle();

        if (style == TrailingCommaStyle.None)
            return;

        var initializer = (InitializerExpressionSyntax)context.Node;

        SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

        if (!expressions.Any())
            return;

        int count = expressions.Count;
        int separatorCount = expressions.SeparatorCount;

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                ReportRemove(context, expressions.GetSeparator(count - 1));
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && initializer.IsSingleLine(cancellationToken: context.CancellationToken))
            {
                ReportRemove(context, expressions.GetSeparator(count - 1));
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                ReportAdd(context, expressions.Last());
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && !initializer.IsSingleLine(cancellationToken: context.CancellationToken))
            {
                ReportAdd(context, expressions.Last());
            }
        }
    }

    private static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
    {
        TrailingCommaStyle style = context.GetTrailingCommaStyle();

        if (style == TrailingCommaStyle.None)
            return;

        var enumDeclaration = (EnumDeclarationSyntax)context.Node;

        SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

        if (!members.Any())
            return;

        int count = members.Count;
        int separatorCount = members.SeparatorCount;
        TextSpan bracesSpan = TextSpan.FromBounds(enumDeclaration.OpenBraceToken.SpanStart, enumDeclaration.CloseBraceToken.Span.End);

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                ReportRemove(context, members.GetSeparator(count - 1));
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && bracesSpan.IsSingleLine(enumDeclaration.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportRemove(context, members.GetSeparator(count - 1));
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                ReportAdd(context, members.Last());
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && !bracesSpan.IsSingleLine(enumDeclaration.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportAdd(context, members.Last());
            }
        }
    }

    private static void AnalyzeAnonymousObjectCreationExpression(SyntaxNodeAnalysisContext context)
    {
        TrailingCommaStyle style = context.GetTrailingCommaStyle();

        if (style == TrailingCommaStyle.None)
            return;

        var objectCreation = (AnonymousObjectCreationExpressionSyntax)context.Node;

        SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers = objectCreation.Initializers;

        if (!initializers.Any())
            return;

        int count = initializers.Count;
        int separatorCount = initializers.SeparatorCount;
        TextSpan bracesSpan = TextSpan.FromBounds(objectCreation.OpenBraceToken.SpanStart, objectCreation.CloseBraceToken.Span.End);

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                ReportRemove(context, initializers.GetSeparator(count - 1));
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && bracesSpan.IsSingleLine(objectCreation.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportRemove(context, initializers.GetSeparator(count - 1));
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                ReportAdd(context, initializers.Last());
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && !bracesSpan.IsSingleLine(objectCreation.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportAdd(context, initializers.Last());
            }
        }
    }

    private static void AnalyzeSwitchExpression(SyntaxNodeAnalysisContext context)
    {
        TrailingCommaStyle style = context.GetTrailingCommaStyle();

        if (style == TrailingCommaStyle.None)
            return;

        var switchExpression = (SwitchExpressionSyntax)context.Node;

        SeparatedSyntaxList<SwitchExpressionArmSyntax> arms = switchExpression.Arms;

        if (!arms.Any())
            return;

        int count = arms.Count;
        int separatorCount = arms.SeparatorCount;
        TextSpan bracesSpan = TextSpan.FromBounds(switchExpression.OpenBraceToken.SpanStart, switchExpression.CloseBraceToken.Span.End);

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                ReportRemove(context, arms.GetSeparator(count - 1));
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && bracesSpan.IsSingleLine(switchExpression.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportRemove(context, arms.GetSeparator(count - 1));
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                ReportAdd(context, arms.Last());
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && !bracesSpan.IsSingleLine(switchExpression.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportAdd(context, arms.Last());
            }
        }
    }

    private static void AnalyzePropertyPatternClause(SyntaxNodeAnalysisContext context)
    {
        TrailingCommaStyle style = context.GetTrailingCommaStyle();

        if (style == TrailingCommaStyle.None)
            return;

        var patternClause = (PropertyPatternClauseSyntax)context.Node;

        SeparatedSyntaxList<SubpatternSyntax> subpatterns = patternClause.Subpatterns;

        if (!subpatterns.Any())
            return;

        int count = subpatterns.Count;
        int separatorCount = subpatterns.SeparatorCount;
        TextSpan bracesSpan = TextSpan.FromBounds(patternClause.OpenBraceToken.SpanStart, patternClause.CloseBraceToken.Span.End);

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                ReportRemove(context, subpatterns.GetSeparator(count - 1));
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && bracesSpan.IsSingleLine(patternClause.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportRemove(context, subpatterns.GetSeparator(count - 1));
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                ReportAdd(context, subpatterns.Last());
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && !bracesSpan.IsSingleLine(patternClause.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportAdd(context, subpatterns.Last());
            }
        }
    }

#if ROSLYN_4_7
    private static void AnalyzeCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        TrailingCommaStyle style = context.GetTrailingCommaStyle();

        if (style == TrailingCommaStyle.None)
            return;

        var collectionExpression = (CollectionExpressionSyntax)context.Node;

        SeparatedSyntaxList<CollectionElementSyntax> elements = collectionExpression.Elements;

        if (!elements.Any())
            return;

        int count = elements.Count;
        int separatorCount = elements.SeparatorCount;
        TextSpan bracesSpan = TextSpan.FromBounds(collectionExpression.OpenBracketToken.SpanStart, collectionExpression.CloseBracketToken.Span.End);

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                ReportRemove(context, elements.GetSeparator(count - 1));
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && bracesSpan.IsSingleLine(collectionExpression.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportRemove(context, elements.GetSeparator(count - 1));
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                ReportAdd(context, elements.Last());
            }
            else if (style == TrailingCommaStyle.OmitWhenSingleLine
                && !bracesSpan.IsSingleLine(collectionExpression.SyntaxTree, cancellationToken: context.CancellationToken))
            {
                ReportAdd(context, elements.Last());
            }
        }
    }
#endif

    private static void ReportAdd(SyntaxNodeAnalysisContext context, SyntaxNode lastNode)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.AddOrRemoveTrailingComma,
            Location.Create(lastNode.SyntaxTree, new TextSpan(lastNode.Span.End, 0)),
            "Add");
    }

    private static void ReportRemove(SyntaxNodeAnalysisContext context, SyntaxToken token)
    {
        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AddOrRemoveTrailingComma, token, "Remove");
    }
}
