// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeAccessibilityRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            Accessibility newAccessibility,
            CancellationToken cancellationToken)
        {
            SyntaxNode newNode = node.WithAccessibility(newAccessibility, ModifierComparer.Instance);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        public static Task<Solution> RefactorAsync(
            Solution solution,
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations,
            Accessibility newAccessibility,
            CancellationToken cancellationToken)
        {
            return solution.ReplaceNodesAsync(
                memberDeclarations,
                (node, rewrittenNode) =>
                {
                    AccessibilityInfo info = SyntaxInfo.AccessibilityInfo(node);

                    if (info.Accessibility == Accessibility.NotApplicable)
                        return node;

                    AccessibilityInfo newInfo = info.WithAccessibility(newAccessibility, ModifierComparer.Instance);

                    return newInfo.Node;
                },
                cancellationToken);
        }
    }
}
