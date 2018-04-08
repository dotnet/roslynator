// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantDefaultSwitchSectionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SwitchStatementSyntax newSwitchStatement = GetNewSwitchStatement(switchSection, switchStatement);

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }

        private static SwitchStatementSyntax GetNewSwitchStatement(SwitchSectionSyntax switchSection, SwitchStatementSyntax switchStatement)
        {
            if (switchSection.GetLeadingTrivia().IsEmptyOrWhitespace())
            {
                int index = switchStatement.Sections.IndexOf(switchSection);

                if (index > 0)
                {
                    SwitchSectionSyntax previousSection = switchStatement.Sections[index - 1];

                    if (previousSection.GetTrailingTrivia().IsEmptyOrWhitespace())
                    {
                        SwitchStatementSyntax newSwitchStatement = switchStatement.RemoveNode(
                            switchSection,
                            SyntaxRemoveOptions.KeepNoTrivia);

                        previousSection = newSwitchStatement.Sections[index - 1];

                        return newSwitchStatement.ReplaceNode(
                            previousSection,
                            previousSection.WithTrailingTrivia(switchSection.GetTrailingTrivia()));
                    }
                }
                else
                {
                    SyntaxToken openBrace = switchStatement.OpenBraceToken;

                    if (!openBrace.IsMissing
                        && openBrace.TrailingTrivia.IsEmptyOrWhitespace())
                    {
                        return switchStatement
                            .RemoveNode(switchSection, SyntaxRemoveOptions.KeepNoTrivia)
                            .WithOpenBraceToken(openBrace.WithTrailingTrivia(switchSection.GetTrailingTrivia()));
                    }
                }
            }

            return switchStatement.RemoveNode(switchSection, SyntaxRemoveOptions.KeepExteriorTrivia);
        }
    }
}
