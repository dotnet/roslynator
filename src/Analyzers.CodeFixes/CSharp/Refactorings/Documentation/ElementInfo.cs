// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal abstract class ElementInfo<TNode> where TNode : SyntaxNode
    {
        protected ElementInfo(TNode node, int insertIndex, NewLinePosition newLinePosition)
        {
            Node = node;
            InsertIndex = insertIndex;
            NewLinePosition = newLinePosition;
        }

        public abstract string Name { get; }

        public TNode Node { get; }

        public int InsertIndex { get; }

        public NewLinePosition NewLinePosition { get; }
    }
}