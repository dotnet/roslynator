// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

        context.RegisterSyntaxNodeAction(
            f => AnalyzeInitializerExpression(f),
            SyntaxKind.ArrayInitializerExpression,
            SyntaxKind.ObjectInitializerExpression,
            SyntaxKind.CollectionInitializerExpression);

        context.RegisterSyntaxNodeAction(f => AnalyzeEnumDeclaration(f), SyntaxKind.EnumDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeAnonymousObjectCreationExpression(f), SyntaxKind.AnonymousObjectCreationExpression);
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
                SyntaxToken token = expressions.GetSeparator(count - 1);

                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AddOrRemoveTrailingComma, token, "Remove");
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                ExpressionSyntax lastExpression = expressions.Last();

                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.AddOrRemoveTrailingComma,
                    Location.Create(lastExpression.SyntaxTree, new TextSpan(lastExpression.Span.End, 0)),
                    "Add");
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

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                SyntaxToken token = members.GetSeparator(count - 1);

                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AddOrRemoveTrailingComma, token, "Remove");
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                EnumMemberDeclarationSyntax lastExpression = members.Last();

                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.AddOrRemoveTrailingComma,
                    Location.Create(lastExpression.SyntaxTree, new TextSpan(lastExpression.Span.End, 0)),
                    "Add");
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

        if (count == separatorCount)
        {
            if (style == TrailingCommaStyle.Omit)
            {
                SyntaxToken token = initializers.GetSeparator(count - 1);

                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AddOrRemoveTrailingComma, token, "Remove");
            }
        }
        else if (separatorCount == count - 1)
        {
            if (style == TrailingCommaStyle.Include)
            {
                AnonymousObjectMemberDeclaratorSyntax lastExpression = initializers.Last();

                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.AddOrRemoveTrailingComma,
                    Location.Create(lastExpression.SyntaxTree, new TextSpan(lastExpression.Span.End, 0)),
                    "Add");
            }
        }
    }
}
