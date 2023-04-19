// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.ReduceIfNesting;

internal static class IfStatementLocalVariableAnalysis
{
    public static bool DoDeclaredVariablesOverlapWithOuterScope(
        IfStatementSyntax ifStatement,
        SemanticModel semanticModel
    )
    {
        var ifVariablesDeclared = semanticModel.AnalyzeDataFlow(ifStatement)!
            .VariablesDeclared;

        if (ifVariablesDeclared.IsEmpty)
            return false;
        
        foreach (StatementSyntax statement in SyntaxInfo.StatementListInfo(ifStatement).Statements)
        {
            if (statement == ifStatement)
                continue;

            foreach (ISymbol parentVariable in semanticModel.AnalyzeDataFlow(statement)!.VariablesDeclared)
            {
                foreach (ISymbol ifVariable in ifVariablesDeclared)
                {
                    if (ifVariable.Name == parentVariable.Name)
                        return true;
                }
            }
        }

        return false;
    }
}