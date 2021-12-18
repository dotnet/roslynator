// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings.NodeInList
{
    internal abstract class CopyArgumentOrParameterRefactoring<TSyntax, TListSyntax> : NodeInListRefactoring<TSyntax, TListSyntax>
        where TSyntax : SyntaxNode
        where TListSyntax : SyntaxNode
    {
        protected CopyArgumentOrParameterRefactoring(TListSyntax listSyntax, SeparatedSyntaxList<TSyntax> list)
            : base(listSyntax, list)
        {
        }

        protected abstract string GetTitle(params string[] args);

        public void ComputeRefactoring(RefactoringContext context, RefactoringDescriptor descriptor)
        {
            int index = FindNode(context.Span);

            if (index > 0
                && !List[index - 1].IsMissing)
            {
                context.RegisterRefactoring(
                    GetTitle(),
                    ct => RefactorAsync(context.Document, index, ct),
                    descriptor);
            }
        }

        protected Task<Document> RefactorAsync(
            Document document,
            int nodeIndex,
            CancellationToken cancellationToken = default)
        {
            var info = new RewriterInfo<TSyntax>(
                List[nodeIndex],
                List[nodeIndex - 1],
                GetTokenBefore(nodeIndex),
                GetTokenAfter(nodeIndex));

            return document.ReplaceNodeAsync(ListSyntax, Rewrite(info), cancellationToken);
        }
    }
}
