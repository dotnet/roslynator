// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryRawStringLiteralAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UnnecessaryRawStringLiteral);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterCompilationStartAction(startContext =>
        {
            if (((CSharpCompilation)startContext.Compilation).LanguageVersion >= LanguageVersion.CSharp11)
            {
                startContext.RegisterSyntaxNodeAction(f => AnalyzeStringLiteralExpression(f), SyntaxKind.StringLiteralExpression);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeInterpolatedStringExpression(f), SyntaxKind.InterpolatedStringExpression);
            }
        });
    }

    private static void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context)
    {
        var literalExpression = (LiteralExpressionSyntax)context.Node;

        if (!RawStringLiteralInfo.TryCreate(literalExpression, out RawStringLiteralInfo info))
            return;

        string text = info.Text;

        if (ContainsBackSlashQuote(text, info.QuoteCount, text.Length - (info.QuoteCount * 2)))
            return;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UnnecessaryRawStringLiteral,
            Location.Create(literalExpression.SyntaxTree, new TextSpan(literalExpression.SpanStart, info.QuoteCount)));
    }

    private static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
    {
        var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

        if (!interpolatedString.StringStartToken.IsKind(SyntaxKind.InterpolatedSingleLineRawStringStartToken))
            return;

        foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
        {
            if (content is InterpolatedStringTextSyntax interpolatedStringText)
            {
                string text = interpolatedStringText.TextToken.Text;

                if (ContainsBackSlashQuote(text, 0, text.Length))
                    return;
            }
        }

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UnnecessaryRawStringLiteral,
            interpolatedString.StringStartToken);
    }

    private static bool ContainsBackSlashQuote(string text, int start, int length)
    {
        for (int pos = start; pos < start + length; pos++)
        {
            switch (text[pos])
            {
                case '\\':
                case '"':
                    {
                        return true;
                    }
            }
        }

        return false;
    }
}
