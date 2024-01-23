// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

        if (!s.StartsWith("@"))
            return;

        int i = 2;

        if (!ScanStart(s, ref i))
            return;

        if (!ScanEnd(s, ref i))
            return;

        i--;
        while (i >= 2)
        {
            if (s[i] == '\"')
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseRawStringLiteral, node);
                return;
            }

            i--;
        }

        static bool ScanStart(string s, ref int i)
        {
            while (i < s.Length - 1)
            {
                if (s[i] == '\r'
                    || s[i] == '\n')
                {
                    return true;
                }

                if (char.IsWhiteSpace(s[i]))
                    return false;

                i++;
            }

            Debug.Fail("");
            return false;
        }
    }

    private static bool ScanEnd(string s, ref int i)
    {
        i = s.Length - 2;
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
