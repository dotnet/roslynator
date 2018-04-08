// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesFromSwitchSectionsRefactoring
    {
        public const string Title = "Remove braces from sections";

        public static Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            SwitchSectionSyntax[] sections,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<SwitchSectionSyntax> newSections = switchStatement
                .Sections
                .Select(section =>
                {
                    if ((sections == null || Array.IndexOf(sections, section) != -1)
                        && RemoveBracesFromSwitchSectionRefactoring.CanRemoveBraces(section))
                    {
                        var block = (BlockSyntax)section.Statements[0];
                        return section.WithStatements(block.Statements);
                    }
                    else
                    {
                        return section;
                    }
                });

            SwitchStatementSyntax newSwitchStatement = switchStatement
                .WithSections(List(newSections))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }
    }
}
