using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Analysis;

internal static class ConvertExpressionBodyAnalysis
{
    public static bool BreakExpressionOnNewLine(SyntaxKind syntaxKind)
    {
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
