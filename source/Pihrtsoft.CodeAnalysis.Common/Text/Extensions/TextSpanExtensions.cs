using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.Text
{
    public static class TextSpanExtensions
    {
        public static bool IsMultiline(this TextSpan span, SyntaxTree tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            return !IsSingleline(span, tree);
        }

        public static bool IsSingleline(this TextSpan span, SyntaxTree tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            FileLinePositionSpan positionSpan = tree.GetLineSpan(span);

            return positionSpan.StartLinePosition.Line == positionSpan.EndLinePosition.Line;
        }
    }
}
