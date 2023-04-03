using System.Collections.Generic;
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
        if (!TryGetOuterScope(ifStatement, out var outerScope))
            return true;
        
        var ifVariablesDeclared = semanticModel.AnalyzeDataFlow(ifStatement)!
            .VariablesDeclared;
        
        if (ifVariablesDeclared.IsEmpty)
            return false;

        IEnumerable<ISymbol> parentVariablesDeclared;
        if (outerScope is SwitchSectionSyntax switchSectionSyntax)
        {
            var allDeclaredVariables = new List<ISymbol>();
            foreach (var statement in switchSectionSyntax.Statements)
            {
                allDeclaredVariables.AddRange(semanticModel.AnalyzeDataFlow(statement)!.VariablesDeclared);
            }

            parentVariablesDeclared = allDeclaredVariables;
        }
        else
        {
            parentVariablesDeclared = semanticModel.AnalyzeDataFlow(outerScope)!
                .VariablesDeclared;
        }

        // The parent's declared variables will include those from the if and so we have to check for any symbols occurring twice.
        foreach (var variable in ifVariablesDeclared)
        {
            if (parentVariablesDeclared.Count(s => s.Name == variable.Name) > 1)
                return true;
        }

        return false;
    }

    private static bool TryGetOuterScope(IfStatementSyntax ifStatement, out SyntaxNode outerScope)
    {
        switch (ifStatement.Parent)
        {
            case BlockSyntax blockSyntax:
                outerScope = blockSyntax;
                return true;
            case ForStatementSyntax forStatementSyntax:
                outerScope = forStatementSyntax.Statement;
                return true;
            case ForEachStatementSyntax forEachStatementSyntax:
                outerScope = forEachStatementSyntax.Statement;
                return true;
            case DoStatementSyntax doStatementSyntax:
                outerScope = doStatementSyntax.Statement;
                return true;
            case WhileStatementSyntax whileStatementSyntax:
                outerScope = whileStatementSyntax.Statement;
                return true;
            case SwitchSectionSyntax switchSectionSyntax:
                outerScope = switchSectionSyntax;
                return true;
            case ConstructorDeclarationSyntax constructorDeclarationSyntax:
                outerScope = constructorDeclarationSyntax.Body;
                return true;
            case DestructorDeclarationSyntax destructorDeclarationSyntax:
                outerScope = destructorDeclarationSyntax.Body;
                return true;
            case AccessorDeclarationSyntax accessorDeclarationSyntax:
                outerScope = accessorDeclarationSyntax.Body;
                return true;
            case OperatorDeclarationSyntax operatorDeclarationSyntax:
                outerScope = operatorDeclarationSyntax.Body;
                return true;
            case ConversionOperatorDeclarationSyntax conversionOperatorDeclarationSyntax:
                outerScope = conversionOperatorDeclarationSyntax.Body;
                return true;
            case MethodDeclarationSyntax methodDeclarationSyntax:
                outerScope = methodDeclarationSyntax.Body;
                return true;
            case LocalFunctionStatementSyntax localFunctionStatementSyntax:
                outerScope = localFunctionStatementSyntax.Body;
                return true;
            case AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax:
                outerScope = anonymousFunctionExpressionSyntax.Block;
                return true;
            default:
                outerScope = null;
                return false;
        }

    }
}