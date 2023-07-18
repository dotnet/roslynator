using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

public static class SwitchLocallyDeclaredVariablesHelper
{
    internal static bool BlockDeclaredVariablesOverlapWithOtherSwitchSections(BlockSyntax block, SwitchStatementSyntax switchStatement, SemanticModel semanticModel)
    {
        ImmutableArray<ISymbol> sectionVariablesDeclared = semanticModel.AnalyzeDataFlow(block)!
            .VariablesDeclared;

        if (sectionVariablesDeclared.IsEmpty)
            return false;

        ImmutableHashSet<string> sectionDeclaredVariablesNames = sectionVariablesDeclared
            .Select(s => s.Name)
            .ToImmutableHashSet();

        foreach (SwitchSectionSyntax otherSection in switchStatement.Sections)
        {
            if (otherSection.Span.Contains(block.Span))
                continue;

            foreach (SwitchLabelSyntax label in otherSection.Labels)
            {
                if (label is not CasePatternSwitchLabelSyntax casePatternSwitchLabel)
                    continue;

                if (PatternMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(casePatternSwitchLabel.Pattern, sectionDeclaredVariablesNames))
                    return true;
            }

            foreach (StatementSyntax statement in otherSection.Statements)
            {
                foreach (ISymbol symbol in semanticModel.AnalyzeDataFlow(statement)!.VariablesDeclared)
                {
                    if (sectionDeclaredVariablesNames.Contains(symbol.Name))
                        return true;
                }
            }
        }
        return false;
    }
}
