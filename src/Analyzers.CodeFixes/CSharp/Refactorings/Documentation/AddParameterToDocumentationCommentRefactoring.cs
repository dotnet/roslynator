// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.Documentation;

namespace Roslynator.CSharp.Refactorings.DocumentationComment
{
    internal class AddParameterToDocumentationCommentRefactoring : DocumentationCommentRefactoring<ParameterSyntax>
    {
        public override string ElementName
        {
            get { return "param"; }
        }

        public override string ElementNameUppercase
        {
            get { return "PARAM"; }
        }

        public override ImmutableArray<string> ElementNames { get; } = ImmutableArray.Create("param", "PARAM", "typeparam", "TYPEPARAM", "summary", "SUMMARY");

        public override SeparatedSyntaxList<ParameterSyntax> GetContainingList(ParameterSyntax node)
        {
            SyntaxNode parent = node.Parent;

            if (parent.IsKind(SyntaxKind.ParameterList))
            {
                return ((ParameterListSyntax)parent).Parameters;
            }
            else
            {
                return ((BracketedParameterListSyntax)parent).Parameters;
            }
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