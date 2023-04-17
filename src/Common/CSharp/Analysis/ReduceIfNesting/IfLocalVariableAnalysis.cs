// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        
        var outerScope = GetOuterScope(ifStatement);
        
        if (outerScope == null)
            return true;

        IList<ISymbol> parentVariablesDeclared;
        if (outerScope is SwitchSectionSyntax switchSection)
        {
            parentVariablesDeclared = switchSection.Statements.SelectMany(s => semanticModel.AnalyzeDataFlow(s)!.VariablesDeclared).ToList();
        }
        else
        {
            parentVariablesDeclared = semanticModel.AnalyzeDataFlow(outerScope)!.VariablesDeclared;
        }

        Debug.Assert(parentVariablesDeclared.Count >= ifVariablesDeclared.Length);
        // The parent's declared variables will include those from the if and so we have to check for any symbols occurring twice.
        foreach (var variable in ifVariablesDeclared)
        {
            if (parentVariablesDeclared.Count(s => s.Name == variable.Name) > 1)
                return true;
        }

        return false;
    }

    private static SyntaxNode GetOuterScope(IfStatementSyntax ifStatement)
    {
        return ifStatement.Parent switch
        {
            BlockSyntax block => block,
            ForStatementSyntax forStatement => forStatement.Statement,
            CommonForEachStatementSyntax forEachStatement => forEachStatement.Statement,
            DoStatementSyntax doStatement => doStatement.Statement,
            WhileStatementSyntax whileStatement => whileStatement.Statement,
            SwitchSectionSyntax switchSection => switchSection,
            ConstructorDeclarationSyntax constructorDeclaration => constructorDeclaration.Body,
            DestructorDeclarationSyntax destructorDeclaration => destructorDeclaration.Body,
            AccessorDeclarationSyntax accessorDeclaration => accessorDeclaration.Body,
            OperatorDeclarationSyntax operatorDeclaration => operatorDeclaration.Body,
            ConversionOperatorDeclarationSyntax conversionOperatorDeclaration => conversionOperatorDeclaration.Body,
            MethodDeclarationSyntax methodDeclaration => methodDeclaration.Body,
            LocalFunctionStatementSyntax localFunctionStatement => localFunctionStatement.Body,
            AnonymousFunctionExpressionSyntax anonymousFunctionExpression => anonymousFunctionExpression.Block,
            _ => null
        };
    }
}