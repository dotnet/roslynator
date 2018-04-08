// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal class AddTypeParameterToDocumentationCommentAnalysis : DocumentationCommentAnalysis<TypeParameterSyntax>
    {
        public override string ElementName
        {
            get { return "typeparam"; }
        }

        public override string ElementNameUppercase
        {
            get { return "TYPEPARAM"; }
        }

        public override ImmutableArray<string> ElementNames { get; } = ImmutableArray.Create("typeparam", "TYPEPARAM", "summary", "SUMMARY");

        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax memberDeclaration,
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters)
        {
            if (typeParameters.Any())
            {
                DocumentationCommentTriviaSyntax comment = memberDeclaration.GetSingleLineDocumentationComment();

                if (comment != null)
                {
                    ImmutableArray<string> values = DocumentationCommentAnalysis.GetAttributeValues(comment, "typeparam", "TYPEPARAM", "name");

                    if (!values.IsDefault)
                    {
                        foreach (TypeParameterSyntax typeParameter in typeParameters)
                        {
                            if (!typeParameter.IsMissing
                                && !values.Contains(typeParameter.Identifier.ValueText))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.AddTypeParameterToDocumentationComment,
                                    typeParameter.Identifier);
                            }
                        }
                    }
                }
            }
        }

        public override SeparatedSyntaxList<TypeParameterSyntax> GetContainingList(TypeParameterSyntax node)
        {
            var typeParameterList = (TypeParameterListSyntax)node.Parent;

            return typeParameterList.Parameters;
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