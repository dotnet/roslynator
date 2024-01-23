#if ROSLYN_4_2
// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseRawStringLiteralAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseRawStringLiteral);

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
            }
        });
    }

    private static void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context)
    {
        var node = (LiteralExpressionSyntax)context.Node;

        SyntaxToken token = node.Token;

        if (!token.IsKind(SyntaxKind.StringLiteralToken))
            return;

        string s = token.Text;

        if (s.StartsWith("@")
            && ScanStart(s)
            && ScanEnd(s)
            && s.IndexOf("\"", 2, s.Length - 3) >= 0)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseRawStringLiteral, Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart, 2)));
        }
    }

    private static bool ScanStart(string s)
    {
        for (int i = 2; i < s.Length - 1; i++)
        {
            if (s[i] == '\r'
                || s[i] == '\n')
            {
                return true;
            }

            if (char.IsWhiteSpace(s[i]))
                return false;
        }

        Debug.Fail("");
        return false;
    }

    private static bool ScanEnd(string s)
    {
        int i = s.Length - 2;
        while (i >= 0)
        {
            if (s[i] == '\r'
                || s[i] == '\n')
            {
                return true;
            }

            if (char.IsWhiteSpace(s[i]))
                return false;

            i--;
        }

        Debug.Fail("");
        return false;
    }
}
#endif
