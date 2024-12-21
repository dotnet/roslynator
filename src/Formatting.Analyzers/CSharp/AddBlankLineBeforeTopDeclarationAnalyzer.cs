// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddBlankLineBeforeTopDeclarationAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddBlankLineBeforeTopDeclaration);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var compilationUnit = (CompilationUnitSyntax)context.Node;

        MemberDeclarationSyntax declaration = compilationUnit.Members.FirstOrDefault();

        if (declaration is null)
            return;

        SyntaxNode node = compilationUnit.AttributeLists.LastOrDefault()
            ?? (SyntaxNode)compilationUnit.Usings.LastOrDefault()
            ?? compilationUnit.Externs.LastOrDefault();

        if (node is null)
            return;

        TriviaBlock block = TriviaBlock.FromBetween(node, declaration);

        if (block.Kind == TriviaBlockKind.NoNewLine
            || block.Kind == TriviaBlockKind.NewLine)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.AddBlankLineBeforeTopDeclaration,
                block.GetLocation());
        }
    }
}
