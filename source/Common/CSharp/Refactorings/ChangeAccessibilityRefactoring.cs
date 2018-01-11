// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeAccessibilityRefactoring
    {
        public static ImmutableArray<Accessibility> Accessibilities { get; } = ImmutableArray.Create(
            Accessibility.Public,
            Accessibility.Internal,
            Accessibility.Protected,
            Accessibility.Private);

        public static string GetTitle(Accessibility accessibility)
        {
            return $"Change accessibility to '{accessibility.GetName()}'";
        }

        public static AccessibilityFlags GetAccessibilityFlags(MemberDeclarationSelection selectedMembers)
        {
            if (selectedMembers.Count < 2)
                return AccessibilityFlags.None;

            var allFlags = AccessibilityFlags.None;

            AccessibilityFlags fixableFlags = AccessibilityFlags.Public
                | AccessibilityFlags.Internal
                | AccessibilityFlags.Protected
                | AccessibilityFlags.Private;

            foreach (MemberDeclarationSyntax member in selectedMembers)
            {
                Accessibility accessibility = SyntaxInfo.AccessibilityInfo(member).Accessibility;

                if (accessibility == Accessibility.NotApplicable)
                    accessibility = member.GetDefaultExplicitAccessibility();

                Debug.Assert(accessibility != Accessibility.NotApplicable, member.Kind().ToString());

                AccessibilityFlags flag = accessibility.GetAccessibilityFlag();

                switch (accessibility)
                {
                    case Accessibility.Private:
                    case Accessibility.Protected:
                    case Accessibility.ProtectedAndInternal:
                    case Accessibility.ProtectedOrInternal:
                    case Accessibility.Internal:
                    case Accessibility.Public:
                        {
                            allFlags |= flag;
                            break;
                        }
                    default:
                        {
                            Debug.Fail(accessibility.ToString());
                            return AccessibilityFlags.None;
                        }
                }

                foreach (Accessibility accessibility2 in Accessibilities)
                {
                    if (accessibility != accessibility2
                        && !CSharpUtility.IsAllowedAccessibility(member, accessibility2))
                    {
                        fixableFlags &= ~accessibility2.GetAccessibilityFlag();
                    }
                }
            }

            switch (allFlags)
            {
                case AccessibilityFlags.Private:
                case AccessibilityFlags.Protected:
                case AccessibilityFlags.Internal:
                case AccessibilityFlags.Public:
                    {
                        fixableFlags &= ~allFlags;
                        break;
                    }
            }

            return fixableFlags;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSelection selectedMembers,
            Accessibility newAccessibility,
            CancellationToken cancellationToken)
        {
            var members = (SyntaxList<MemberDeclarationSyntax>)selectedMembers.Items;

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Take(selectedMembers.StartIndex)
                .Concat(selectedMembers.Select(f => f.WithAccessibility(newAccessibility)))
                .Concat(members.Skip(selectedMembers.EndIndex + 1))
                .ToSyntaxList();

            MemberDeclarationSyntax containingMember = selectedMembers.ContainingMember;

            MemberDeclarationSyntax newNode = containingMember.WithMembers(newMembers);

            return document.ReplaceNodeAsync(containingMember, newNode, cancellationToken);
        }

        public static Task<Solution> RefactorAsync(
            Solution solution,
            MemberDeclarationSelection selectedMembers,
            Accessibility newAccessibility,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var members = new HashSet<MemberDeclarationSyntax>();

            foreach (MemberDeclarationSyntax member in selectedMembers)
            {
                if (member.GetModifiers().Contains(SyntaxKind.PartialKeyword))
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);

                    foreach (SyntaxReference reference in symbol.DeclaringSyntaxReferences)
                        members.Add((MemberDeclarationSyntax)reference.GetSyntax(cancellationToken));
                }
                else
                {
                    members.Add(member);
                }
            }

            return solution.ReplaceNodesAsync(
                members,
                (node, rewrittenNode) => node.WithAccessibility(newAccessibility),
                cancellationToken);
        }

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
                (node, rewrittenNode) => WithAccessibility(node, newAccessibility),
                cancellationToken);
        }

        private static SyntaxNode WithAccessibility(MemberDeclarationSyntax node, Accessibility newAccessibility)
        {
            AccessibilityInfo info = SyntaxInfo.AccessibilityInfo(node);

            if (info.Accessibility == Accessibility.NotApplicable)
                return node;

            AccessibilityInfo newInfo = info.WithAccessibility(newAccessibility, ModifierComparer.Instance);

            return newInfo.Node;
        }
    }
}
