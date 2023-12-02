// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddOrRemoveNewLineBeforeWhileInDoStatementAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
    }

    private static void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
    {
        var doStatement = (DoStatementSyntax)context.Node;

        StatementSyntax statement = doStatement.Statement;

        if (!statement.IsKind(SyntaxKind.Block))
            return;

        NewLineStyle newLineStyle = context.GetNewLineBeforeWhileInDoStatement();

        if (newLineStyle == NewLineStyle.None)
            return;

        TriviaBetweenAnalysis analysis = TriviaBetweenAnalysis.Create(doStatement.Statement, doStatement.WhileKeyword);

        if (!analysis.Success)
            return;

        if (analysis.Kind == TriviaBetweenKind.NoNewLine)
        {
            if (newLineStyle == NewLineStyle.Add)
            {
                context.ReportDiagnostic(
                    DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement,
                    analysis.GetLocation(),
                    "Add");
            }
        }
        else if (newLineStyle == NewLineStyle.Remove)
        {
            context.ReportDiagnostic(
                DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement,
                analysis.GetLocation(),
                "Remove");
        }
    }
}
