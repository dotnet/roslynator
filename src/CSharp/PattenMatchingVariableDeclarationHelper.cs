using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

public static class PattenMatchingVariableDeclarationHelper
{
    public static IEnumerable<string> GetVariablesDeclared(PatternSyntax patternSyntax)
    {
        switch (patternSyntax)
        {
            case DeclarationPatternSyntax { Designation: var variableDesignationSyntax}:
                return GetVariablesDeclared(variableDesignationSyntax);
            case RecursivePatternSyntax { PositionalPatternClause: var positionalPatternClause, PropertyPatternClause: var propertyPatternClause, Designation: var designation }:
                 var designationVars = designation != null ? GetVariablesDeclared(designation):Array.Empty<string>();
                 var propertyVars = propertyPatternClause?.Subpatterns.SelectMany(p=>GetVariablesDeclared(p.Pattern)) ?? Array.Empty<string>();
                 var positionalVars = positionalPatternClause?.Subpatterns.SelectMany(p => GetVariablesDeclared(p.Pattern)) ?? Array.Empty<string>();
                 return designationVars.Concat(propertyVars).Concat(positionalVars);
            case VarPatternSyntax { Designation: var variableDesignationSyntax}:
                return GetVariablesDeclared(variableDesignationSyntax);
            case BinaryPatternSyntax binaryPatternSyntax:
                return GetVariablesDeclared(binaryPatternSyntax.Left)
                    .Concat(GetVariablesDeclared(binaryPatternSyntax.Right));
            case ParenthesizedPatternSyntax parenthesizedPatternSyntax:
                return GetVariablesDeclared(parenthesizedPatternSyntax.Pattern);
        }
        return Array.Empty<string>();
    }
    
    public static IEnumerable<string> GetVariablesDeclared(VariableDesignationSyntax? designationSyntax)
    {
        switch (designationSyntax)
        {
            case SingleVariableDesignationSyntax singleVariableDesignationSyntax:
                yield return singleVariableDesignationSyntax.Identifier.ValueText;
                break;
            case ParenthesizedVariableDesignationSyntax parenthesizedVariableDesignationSyntax:
                foreach (var variable in parenthesizedVariableDesignationSyntax.Variables)
                {
                    foreach (var v in GetVariablesDeclared(variable))
                    {
                        yield return v;
                    }
                }
                break;
            case DiscardDesignationSyntax _:
                yield break;
        }
    }

}