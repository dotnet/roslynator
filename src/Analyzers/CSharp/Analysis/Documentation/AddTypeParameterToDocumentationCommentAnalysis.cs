// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal class AddTypeParameterToDocumentationCommentAnalysis : DocumentationCommentAnalysis<TypeParameterSyntax>
    {
        public override XmlElementKind ElementKind
        {
            get { return XmlElementKind.TypeParam; }
        }

        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax memberDeclaration,
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters)
        {
            if (!typeParameters.Any())
                return;

            DocumentationCommentTriviaSyntax comment = memberDeclaration.GetSingleLineDocumentationComment();

            if (comment == null)
                return;

            ImmutableArray<string> values = DocumentationCommentAnalysis.GetAttributeValues(comment, XmlElementKind.TypeParam, "name");

            if (values.IsDefault)
                return;

            foreach (TypeParameterSyntax typeParameter in typeParameters)
            {
                if (!typeParameter.IsMissing
                    && !values.Contains(typeParameter.Identifier.ValueText))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AddTypeParameterToDocumentationComment, typeParameter.Identifier);
                }
            }
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