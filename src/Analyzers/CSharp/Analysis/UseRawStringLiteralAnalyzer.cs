#if ROSLYN_4_2
// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
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
            && s.IndexOf("\n", 2, s.Length - 3) >= 0
            && s.IndexOf("\"", 2, s.Length - 3) >= 0)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseRawStringLiteral, Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart, 2)));
        }
    }
}
#endif
