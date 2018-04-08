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
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeAccessibilityRefactoring
    {
        public static ImmutableArray<Accessibility> AvailableAccessibilities { get; } = ImmutableArray.Create(
            Accessibility.Public,
            Accessibility.Internal,
            Accessibility.Protected,
            Accessibility.Private);

        public static string GetTitle(Accessibility accessibility)
        {
            return $"Change accessibility to '{SyntaxFacts.GetText(accessibility)}'";
        }

        public static ISymbol GetBaseSymbolOrDefault(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ISymbol symbol = GetDeclaredSymbol();

            if (symbol != null)
            {
                if (!symbol.IsOverride)
                    return symbol;

                symbol = symbol.BaseOverriddenSymbol();

                if (symbol != null)
                {
                    SyntaxNode syntax = symbol.GetSyntaxOrDefault(cancellationToken);

                    if (syntax != null)
                    {
                        if (syntax is MemberDeclarationSyntax
                            || syntax.Kind() == SyntaxKind.VariableDeclarator)
                        {
                            return symbol;
                        }
                    }
                }
            }

            return null;

            ISymbol GetDeclaredSymbol()
            {
                if (node is EventFieldDeclarationSyntax eventFieldDeclaration)
                {
                    VariableDeclaratorSyntax declarator = eventFieldDeclaration.Declaration?.Variables.SingleOrDefault(shouldThrow: false);

                    if (declarator != null)
                        return semanticModel.GetDeclaredSymbol(declarator, cancellationToken);

                    return null;
                }
                else
                {
                    return semanticModel.GetDeclaredSymbol(node, cancellationToken);
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationListSelection selectedMembers,
            Accessibility newAccessibility,
            CancellationToken cancellationToken)
        {
            SyntaxList<MemberDeclarationSyntax> newMembers = selectedMembers
                .UnderlyingList
                .ReplaceRangeAt(selectedMembers.FirstIndex, selectedMembers.Count, selectedMembers.Select(f => SyntaxAccessibility.WithExplicitAccessibility(f, newAccessibility)));

            MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(selectedMembers);

            return document.ReplaceMembersAsync(info, newMembers, cancellationToken);
        }

        public static async Task<Solution> RefactorAsync(
            Solution solution,
            MemberDeclarationListSelection selectedMembers,
            Accessibility newAccessibility,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var members = new HashSet<MemberDeclarationSyntax>();

            foreach (MemberDeclarationSyntax member in selectedMembers)
            {
                ModifierKinds kinds = SyntaxInfo.ModifierListInfo(member).GetKinds();

                if (kinds.Any(ModifierKinds.Partial))
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);

                    foreach (SyntaxReference reference in symbol.DeclaringSyntaxReferences)
                        members.Add((MemberDeclarationSyntax)reference.GetSyntax(cancellationToken));
                }
                else if (kinds.Any(ModifierKinds.AbstractVirtualOverride))
                {
                    ISymbol symbol = GetBaseSymbolOrDefault(member, semanticModel, cancellationToken);

                    if (symbol != null)
                    {
                        foreach (MemberDeclarationSyntax member2 in GetMemberDeclarations(symbol, cancellationToken))
                            members.Add(member2);

                        foreach (MemberDeclarationSyntax member2 in await FindOverridingMemberDeclarationsAsync(symbol, solution, cancellationToken).ConfigureAwait(false))
                            members.Add(member2);
                    }
                    else
                    {
                        members.Add(member);
                    }
                }
                else
                {
                    members.Add(member);
                }
            }

            return await solution.ReplaceNodesAsync(
                members,
                (node, _) => SyntaxAccessibility.WithExplicitAccessibility(node, newAccessibility),
                cancellationToken).ConfigureAwait(false);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            Accessibility newAccessibility,
            CancellationToken cancellationToken)
        {
            SyntaxNode newNode = SyntaxAccessibility.WithExplicitAccessibility(node, newAccessibility);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        public static async Task<Solution> RefactorAsync(
            Solution solution,
            ISymbol symbol,
            Accessibility newAccessibility,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<ISymbol> symbols = await SymbolFinder.FindOverridesAsync(symbol, solution, cancellationToken: cancellationToken).ConfigureAwait(false);

            ImmutableArray<MemberDeclarationSyntax> memberDeclarations = GetMemberDeclarations(symbol, cancellationToken)
                .Concat(await FindOverridingMemberDeclarationsAsync(symbol, solution, cancellationToken).ConfigureAwait(false))
                .ToImmutableArray();

            return await RefactorAsync(solution, memberDeclarations, newAccessibility, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IEnumerable<MemberDeclarationSyntax>> FindOverridingMemberDeclarationsAsync(
            ISymbol symbol,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<ISymbol> symbols = await SymbolFinder.FindOverridesAsync(symbol, solution, cancellationToken: cancellationToken).ConfigureAwait(false);

            return symbols.SelectMany(f => GetMemberDeclarations(f, cancellationToken));
        }

        private static IEnumerable<MemberDeclarationSyntax> GetMemberDeclarations(
            ISymbol symbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (SyntaxReference syntaxReference in symbol.DeclaringSyntaxReferences)
            {
                switch (syntaxReference.GetSyntax(cancellationToken))
                {
                    case MemberDeclarationSyntax memberDeclaration:
                        {
                            yield return memberDeclaration;
                            break;
                        }
                    case VariableDeclaratorSyntax variableDeclarator:
                        {
                            if (variableDeclarator.Parent.Parent is MemberDeclarationSyntax memberDeclaration2)
                                yield return memberDeclaration2;

                            break;
                        }
                    default:
                        {
                            Debug.Fail(syntaxReference.GetSyntax(cancellationToken).Kind().ToString());
                            break;
                        }
                }
            }
        }

        public static Task<Solution> RefactorAsync(
            Solution solution,
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations,
            Accessibility newAccessibility,
            CancellationToken cancellationToken)
        {
            return solution.ReplaceNodesAsync(
                memberDeclarations,
                (node, _) => WithAccessibility(node, newAccessibility),
                cancellationToken);
        }

        private static SyntaxNode WithAccessibility(MemberDeclarationSyntax node, Accessibility newAccessibility)
        {
            ModifierListInfo info = SyntaxInfo.ModifierListInfo(node);

            if (info.ExplicitAccessibility == Accessibility.NotApplicable)
                return node;

            ModifierListInfo newInfo = info.WithExplicitAccessibility(newAccessibility, ModifierKindComparer.Default);

            return newInfo.Parent;
        }
    }
}
