// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FormatTypeDeclarationBracesAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, FormattingDiagnosticRules.FormatTypeDeclarationBraces);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            f => AnalyzeTypeDeclaration(f),
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
#if ROSLYN_4_0
            SyntaxKind.RecordStructDeclaration,
#endif
            SyntaxKind.InterfaceDeclaration);
    }

    private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;

        SyntaxToken openBrace = typeDeclaration.OpenBraceToken;

        if (openBrace.IsKind(SyntaxKind.None)
            || openBrace.IsMissing)
        {
            return;
        }

        SyntaxToken closeBrace = typeDeclaration.CloseBraceToken;

        if (closeBrace.IsKind(SyntaxKind.None)
            || closeBrace.IsMissing)
        {
            return;
        }

        if (typeDeclaration.SyntaxTree.IsSingleLineSpan(TextSpan.FromBounds(openBrace.SpanStart, closeBrace.SpanStart)))
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                FormattingDiagnosticRules.FormatTypeDeclarationBraces,
                openBrace);
        }
    }
}
