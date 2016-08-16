// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.MissingNodeInList
{
    internal abstract class DuplicateArgumentOrParameterRefactoring<TSyntax, TSyntaxList> : NodeInListRefactoring<TSyntax, TSyntaxList>
        where TSyntax : SyntaxNode
        where TSyntaxList : SyntaxNode
    {
        public DuplicateArgumentOrParameterRefactoring(TSyntaxList nodeList)
            : base(nodeList)
        {
        }

        protected override int FindMissingNode(TSyntaxList nodeList, TextSpan span)
        {
            int index = base.FindMissingNode(nodeList, span);

            if (index != -1
                && !Nodes[index - 1].IsMissing)
            {
                return index;
            }

            return -1;
        }
    }
}
