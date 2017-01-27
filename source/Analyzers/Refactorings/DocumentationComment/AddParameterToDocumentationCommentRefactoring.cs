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
    internal class AddParameterToDocumentationCommentRefactoring : DocumentationCommentRefactoring<ParameterSyntax>
    {
        public override string ElementName
        {
            get { return "param"; }
        }

        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax memberDeclaration,
            SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            if (parameters.Any(f => !f.IsMissing))
            {
                SyntaxTrivia trivia = memberDeclaration.GetSingleLineDocumentationComment();

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                    if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true)
                    {
                        var names = new HashSet<string>(DocumentationCommentRefactoring.GetNameAttributeValues(comment, "param"));

                        foreach (ParameterSyntax parameter in parameters)
                        {
                            if (!parameter.IsMissing
                                && !names.Contains(parameter.Identifier.ValueText))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.AddParameterToDocumentationComment,
                                    parameter.GetLocation());
                            }
                        }
                    }
                }
            }
        }

        public override string[] GetElementNames()
        {
            return new string[] { "param", "typeparam", "summary" };
        }

        public override SeparatedSyntaxList<ParameterSyntax> GetContainingList(ParameterSyntax node)
        {
            var parameterList = (ParameterListSyntax)node.Parent;

            return parameterList.Parameters;
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