// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.Documentation;

namespace Roslynator.CSharp.Refactorings.Documentation
{
    internal class AddTypeParamElementToDocumentationCommentRefactoring : DocumentationCommentRefactoring<TypeParameterSyntax>
    {
        public override XmlTag Tag
        {
            get { return XmlTag.TypeParam; }
        }

        public override bool ShouldBeBefore(XmlTag tag)
        {
            return tag == XmlTag.Summary;
        }

        public override string GetName(TypeParameterSyntax node)
        {
            return node.Identifier.ValueText;
        }

        public override ElementInfo<TypeParameterSyntax> CreateInfo(TypeParameterSyntax node, int insertIndex, NewLinePosition newLinePosition)
        {
            return new TypeParamElementInfo(node, insertIndex, newLinePosition);
        }

        protected override SeparatedSyntaxList<TypeParameterSyntax> GetSyntaxList(SyntaxNode node)
        {
            return CSharpUtility.GetTypeParameters(node);
        }
    }
}