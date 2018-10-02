// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal class AddParamElementToDocumentationCommentAnalysis : DocumentationCommentAnalysis<ParameterSyntax>
    {
        public override XmlElementKind ElementKind
        {
            get { return XmlElementKind.Param; }
        }

        public override SeparatedSyntaxList<ParameterSyntax> GetContainingList(ParameterSyntax node)
        {
            return ((BaseParameterListSyntax)node.Parent).Parameters;
        }

        public override string GetName(ParameterSyntax node)
        {
            return node.Identifier.ValueText;
        }

        public override ElementInfo<ParameterSyntax> CreateInfo(ParameterSyntax node, int insertIndex, NewLinePosition newLinePosition)
        {
            return new ParamElementInfo(node, insertIndex, newLinePosition);
        }
    }
}