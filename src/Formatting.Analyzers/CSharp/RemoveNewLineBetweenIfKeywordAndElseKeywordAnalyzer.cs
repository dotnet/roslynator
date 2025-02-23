﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveNewLineBetweenIfKeywordAndElseKeywordAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveNewLineBetweenIfKeywordAndElseKeyword);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeElseClause(f), SyntaxKind.ElseClause);
    }

    private static void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
    {
        var elseClause = (ElseClauseSyntax)context.Node;

        StatementSyntax statement = elseClause.Statement;

        if (!statement.IsKind(SyntaxKind.IfStatement))
            return;

        TriviaBlock block = TriviaBlock.FromBetween(elseClause.ElseKeyword, statement);

        if (!block.Success)
            return;

        if (block.Kind != TriviaBlockKind.NoNewLine
            && !block.ContainsComment)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.RemoveNewLineBetweenIfKeywordAndElseKeyword,
                block.GetLocation());
        }
    }
}
