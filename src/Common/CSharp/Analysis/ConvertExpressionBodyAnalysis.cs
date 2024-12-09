using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;
internal static class ConvertExpressionBodyAnalysis
{
    public static bool BreakExpressionOnNewLine(SyntaxKind syntaxKind, AnalyzerConfigOptions configOptions)
    {
        if (!configOptions.TryGetValueAsBool(ConfigOptions.ExpressionBodyStyleOnNextLine, out bool breakOnNewLine)
            || !breakOnNewLine)
        {
            return false;
        }

        switch (syntaxKind)
        {
            case SyntaxKind.MethodDeclaration:
            case SyntaxKind.ConstructorDeclaration:
            case SyntaxKind.DestructorDeclaration:
            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.IndexerDeclaration:
            case SyntaxKind.OperatorDeclaration:
            case SyntaxKind.ConversionOperatorDeclaration:
                return true;

            default:
                return false;
        }
    }
}
