// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal abstract class DocumentationCommentAnalysis<TNode> where TNode : SyntaxNode
    {
        public abstract XmlElementKind ElementKind { get; }

        public abstract SeparatedSyntaxList<TNode> GetContainingList(TNode node);

        public abstract string GetName(TNode node);

        public abstract ElementInfo<TNode> CreateInfo(TNode node, int insertIndex, NewLinePosition newLinePosition);

        public virtual MemberDeclarationSyntax GetMemberDeclaration(TNode node)
        {
            return node.FirstAncestor<MemberDeclarationSyntax>();
        }
    }
}