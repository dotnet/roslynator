// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Roslynator.CSharp.Refactorings.DocumentationComment
{
    internal class AddTypeParameterToDocumentationCommentRefactoring : DocumentationCommentRefactoring<TypeParameterSyntax>
    {
        public override string ElementName
        {
            get { return "typeparam"; }
        }

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
                    ImmutableArray<string> values = DocumentationCommentRefactoring.GetAttributeValues(comment, "typeparam", "name");

                    if (!values.IsDefault)
                    {
                        foreach (TypeParameterSyntax typeParameter in typeParameters)
                        {
                            if (!typeParameter.IsMissing
                                && !values.Contains(typeParameter.Identifier.ValueText))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.AddTypeParameterToDocumentationComment,
                                    typeParameter);
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

        public override string[] GetElementNames()
        {
            return new string[] { "typeparam", "summary" };
        }

        public override ElementInfo<TypeParameterSyntax> CreateInfo(TypeParameterSyntax node, int insertIndex, NewLinePosition newLinePosition)
        {
            return new TypeParamElementInfo(node, insertIndex, newLinePosition);
        }
    }
}