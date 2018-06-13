// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal class AddParameterToDocumentationCommentAnalysis : DocumentationCommentAnalysis<ParameterSyntax>
    {
        public override XmlElementKind ElementKind
        {
            get { return XmlElementKind.Param; }
        }

        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax memberDeclaration,
            SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            if (!parameters.Any())
                return;

            DocumentationCommentTriviaSyntax comment = memberDeclaration.GetSingleLineDocumentationComment();

            if (comment == null)
                return;

            ImmutableArray<string> values = DocumentationCommentAnalysis.GetAttributeValues(comment, XmlElementKind.Param, "name");

            if (values.IsDefault)
                return;

            foreach (ParameterSyntax parameter in parameters)
            {
                if (!parameter.IsMissing
                    && !values.Contains(parameter.Identifier.ValueText))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AddParameterToDocumentationComment, parameter.Identifier);
                }
            }
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