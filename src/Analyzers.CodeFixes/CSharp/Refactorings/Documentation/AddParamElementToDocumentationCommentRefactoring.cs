// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.Documentation;

namespace Roslynator.CSharp.Refactorings.Documentation
{
    internal class AddParamElementToDocumentationCommentRefactoring : DocumentationCommentRefactoring<ParameterSyntax>
    {
        public override XmlTag Tag
        {
            get { return XmlTag.Param; }
        }

        public override bool ShouldBeBefore(XmlTag tag)
        {
            return tag == XmlTag.TypeParam
                || tag == XmlTag.Summary;
        }

        public override string GetName(ParameterSyntax node)
        {
            return node.Identifier.ValueText;
        }

        public override ElementInfo<ParameterSyntax> CreateInfo(ParameterSyntax node, int insertIndex, NewLinePosition newLinePosition)
        {
            return new ParamElementInfo(node, insertIndex, newLinePosition);
        }

        protected override SeparatedSyntaxList<ParameterSyntax> GetSyntaxList(SyntaxNode node)
        {
            return CSharpUtility.GetParameters(node);
        }
    }
}