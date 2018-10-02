// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.Documentation;

namespace Roslynator.CSharp.Refactorings.DocumentationComment
{
    internal class AddParamElementToDocumentationCommentRefactoring : DocumentationCommentRefactoring<ParameterSyntax>
    {
        public override XmlElementKind ElementKind
        {
            get { return XmlElementKind.Param; }
        }

        public override bool ShouldBeBefore(XmlElementKind elementKind)
        {
            return elementKind == XmlElementKind.TypeParam
                || elementKind == XmlElementKind.Summary;
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
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).ParameterList.Parameters;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).ParameterList.Parameters;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).ParameterList.Parameters;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).ParameterList.Parameters;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).ParameterList.Parameters;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).ParameterList.Parameters;
            }

            return default;
        }
    }
}