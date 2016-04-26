// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(UncommentCodeRefactoringProvider))]
    public class UncommentCodeRefactoringProvider : CodeRefactoringProvider
    {
        private static readonly Regex _uncommentRegex = new Regex(@"
(?<=
    ^
    [\s-[\r\n]]*
)
//", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SyntaxTrivia trivia = root.FindTrivia(context.Span.Start);

            if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                return;

            context.RegisterRefactoring(
                "Uncomment",
                cancellationToken => UncommentAsync(context.Document, trivia, cancellationToken));
        }

        private static async Task<Document> UncommentAsync(
            Document document,
            SyntaxTrivia comment,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxToken token = comment.Token;

            int index = token.LeadingTrivia.IndexOf(comment);

            SyntaxTriviaList triviaList = (index != -1) ? token.LeadingTrivia : token.TrailingTrivia;

            int firstIndex = FindFirstTriviaToRemove(triviaList, index);
            int lastIndex = FindLastTriviaToRemove(triviaList, index);

            string text = _uncommentRegex.Replace(
                oldRoot.ToFullString(),
                string.Empty,
                GetSingleLineCommentCount(triviaList, firstIndex, lastIndex),
                triviaList[firstIndex].SpanStart);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);

            SyntaxNode newRoot = await tree.GetRootAsync();

            return document.WithSyntaxRoot(newRoot);
        }

        private static int GetSingleLineCommentCount(SyntaxTriviaList triviaList, int firstIndex, int lastIndex)
        {
            int count = 0;
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if (triviaList[i].IsKind(SyntaxKind.SingleLineCommentTrivia))
                    count++;
            }

            return count;
        }

        private static int FindFirstTriviaToRemove(SyntaxTriviaList triviaList, int index)
        {
            int firstIndex = index;

            while (index > 0)
            {
                if (IsAllowedTrivia(triviaList[index - 1]))
                {
                    index--;

                    if (triviaList[index].IsKind(SyntaxKind.SingleLineCommentTrivia))
                        firstIndex = index;
                }
                else
                {
                    break;
                }
            }

            return firstIndex;
        }

        private static int FindLastTriviaToRemove(SyntaxTriviaList triviaList, int index)
        {
            int lastIndex = index;

            while (index < triviaList.Count - 1)
            {
                if (IsAllowedTrivia(triviaList[index + 1]))
                {
                    index++;

                    if (triviaList[index].IsKind(SyntaxKind.SingleLineCommentTrivia))
                        lastIndex = index;
                }
                else
                {
                    break;
                }
            }

            return lastIndex;
        }

        private static bool IsAllowedTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.WhitespaceTrivia:
                    return true;
                case SyntaxKind.EndOfLineTrivia:
                    return true;
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }
    }
}
