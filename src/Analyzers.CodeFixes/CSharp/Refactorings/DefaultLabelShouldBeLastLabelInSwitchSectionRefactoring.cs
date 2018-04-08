// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DefaultLabelShouldBeLastLabelInSwitchSectionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

            SwitchLabelSyntax defaultLabel = labels.First(f => f.IsKind(SyntaxKind.DefaultSwitchLabel));

            int index = labels.IndexOf(defaultLabel);

            SwitchLabelSyntax lastLabel = labels.Last();

            labels = labels.Replace(lastLabel, defaultLabel.WithTriviaFrom(lastLabel));

            labels = labels.Replace(labels[index], lastLabel.WithTriviaFrom(defaultLabel));

            SwitchSectionSyntax newSwitchSection = switchSection.WithLabels(labels);

            return document.ReplaceNodeAsync(switchSection, newSwitchSection, cancellationToken);
        }
    }
}
