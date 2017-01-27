// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

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
            if (typeParameters.Any(f => !f.IsMissing))
            {
                SyntaxTrivia trivia = memberDeclaration.GetSingleLineDocumentationComment();

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                    if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true)
                    {
                        var names = new HashSet<string>(DocumentationCommentRefactoring.GetNameAttributeValues(comment, "typeparam"));

                        foreach (TypeParameterSyntax typeParameter in typeParameters)
                        {
                            if (!typeParameter.IsMissing
                                && !names.Contains(typeParameter.Identifier.ValueText))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.AddTypeParameterToDocumentationComment,
                                    typeParameter.GetLocation());
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