// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal class AddTypeParamElementToDocumentationCommentAnalysis : DocumentationCommentAnalysis<TypeParameterSyntax>
    {
        public override XmlElementKind ElementKind
        {
            get { return XmlElementKind.TypeParam; }
        }

        public override SeparatedSyntaxList<TypeParameterSyntax> GetContainingList(TypeParameterSyntax node)
        {
            return ((TypeParameterListSyntax)node.Parent).Parameters;
        }

        public override string GetName(TypeParameterSyntax node)
        {
            return node.Identifier.ValueText;
        }

        public override ElementInfo<TypeParameterSyntax> CreateInfo(TypeParameterSyntax node, int insertIndex, NewLinePosition newLinePosition)
        {
            return new TypeParamElementInfo(node, insertIndex, newLinePosition);
        }
    }
}