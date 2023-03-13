// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveUnnecessaryElseAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveUnnecessaryElse);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => Analyze(f), SyntaxKind.ElseClause);
    }

    public static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var elseClause = (ElseClauseSyntax)context.Node;

        if (elseClause.ContainsDiagnostics)
            return;

        if (!IsFixable(elseClause, context.SemanticModel))
            return;

        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveUnnecessaryElse, elseClause.ElseKeyword);
    }

    private static bool IsFixable(ElseClauseSyntax elseClause, SemanticModel semanticModel)
    {
        if (elseClause.Statement?.IsKind(SyntaxKind.IfStatement) != false)
            return false;

        if (elseClause.Parent is not IfStatementSyntax ifStatement)
            return false;

        if (!ifStatement.IsTopmostIf())
            return false;

        StatementSyntax ifStatementStatement = ifStatement.Statement;

        if (ifStatementStatement is not BlockSyntax ifBlock)
            return CSharpFacts.IsJumpStatement(ifStatementStatement.Kind());
        
        if (elseClause.Statement is BlockSyntax elseBlock && LocalDeclaredVariablesOverlap(elseBlock, ifBlock, semanticModel))
            return false;

        var lastStatementInIf = ifBlock.Statements.LastOrDefault();

        return lastStatementInIf is not null
               && CSharpFacts.IsJumpStatement(lastStatementInIf.Kind());
    }

    private static bool LocalDeclaredVariablesOverlap(BlockSyntax elseBlock, BlockSyntax ifBlock, SemanticModel semanticModel)
    {        
        var elseVariablesDeclared = semanticModel.AnalyzeDataFlow(elseBlock)!
            .VariablesDeclared;
        
        if (elseVariablesDeclared.IsEmpty)
            return false;

        var ifVariablesDeclared = semanticModel.AnalyzeDataFlow(ifBlock)!
            .VariablesDeclared;
        
        if (ifVariablesDeclared.IsEmpty)
            return false;

        var elseVariableNames = elseVariablesDeclared
            .Select(s => s.Name)
            .ToImmutableHashSet();

        foreach (var v in ifVariablesDeclared)
        {
            if (elseVariableNames.Contains(v.Name))
                return true;
        }

        return false;
    }
}
