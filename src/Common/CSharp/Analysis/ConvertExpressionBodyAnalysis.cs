using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Analysis;

internal static class ConvertExpressionBodyAnalysis
{
    public static bool AllowPutExpressionBodyOnItsOwnLine(SyntaxKind syntaxKind)
    {
        // allow putting expression on new line for all method-like declarations, except for accessors.
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
