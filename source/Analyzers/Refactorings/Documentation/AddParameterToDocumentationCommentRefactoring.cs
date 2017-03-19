// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Diagnostics.Extensions;
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
            if (parameters.Any())
            {
                DocumentationCommentTriviaSyntax comment = memberDeclaration.GetSingleLineDocumentationComment();

                if (comment != null)
                {
                    bool containsInheritDoc = false;
                    HashSet<string> names = DocumentationCommentRefactoring.GetAttributeValues(comment, "param", "name", out containsInheritDoc);

                    if (!containsInheritDoc)
                    {
                        foreach (ParameterSyntax parameter in parameters)
                        {
                            if (!parameter.IsMissing
                                && names?.Contains(parameter.Identifier.ValueText) != true)
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.AddParameterToDocumentationComment,
                                    parameter);
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