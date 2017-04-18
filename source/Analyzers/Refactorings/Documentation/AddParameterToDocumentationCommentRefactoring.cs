// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

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
                    ImmutableArray<string> values = DocumentationCommentRefactoring.GetAttributeValues(comment, "param", "name");

                    if (!values.IsDefault)
                    {
                        foreach (ParameterSyntax parameter in parameters)
                        {
                            if (!parameter.IsMissing
                                && !values.Contains(parameter.Identifier.ValueText))
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