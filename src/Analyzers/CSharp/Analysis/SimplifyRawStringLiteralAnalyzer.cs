// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SimplifyRawStringLiteralAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.SimplifyRawStringLiteral);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeStringLiteralExpression(f), SyntaxKind.StringLiteralExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeInterpolatedStringExpression(f), SyntaxKind.InterpolatedStringExpression);
    }

    private static void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context)
    {
        var literalExpression = (LiteralExpressionSyntax)context.Node;

        RawStringLiteralInfo info = RawStringLiteralInfo.Create(literalExpression);

        if (info.QuoteCount == 0)
            return;

        string text = info.Text;

        if (ContainsBackSlashQuote(text, info.QuoteCount, text.Length - (info.QuoteCount * 2)))
            return;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.SimplifyRawStringLiteral,
            info.Token);
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
            DiagnosticRules.SimplifyRawStringLiteral,
            interpolatedString);
    }

    private static bool ContainsBackSlashQuote(
        string text,
        int start,
        int length)
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
