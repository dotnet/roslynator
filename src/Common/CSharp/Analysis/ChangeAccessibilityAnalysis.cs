// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class ChangeAccessibilityAnalysis
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

        public static Accessibilities GetValidAccessibilities(MemberDeclarationListSelection selectedMembers, bool allowOverride = false)
        {
            if (selectedMembers.Count < 2)
                return Accessibilities.None;

            var all = Accessibilities.None;

            Accessibilities valid = Accessibilities.Public
                | Accessibilities.Internal
                | Accessibilities.Protected
                | Accessibilities.Private;

            foreach (MemberDeclarationSyntax member in selectedMembers)
            {
                Accessibility accessibility = SyntaxAccessibility.GetExplicitAccessibility(member);

                if (accessibility == Accessibility.NotApplicable)
                {
                    accessibility = SyntaxAccessibility.GetDefaultExplicitAccessibility(member);

                    if (accessibility == Accessibility.NotApplicable)
                        return Accessibilities.None;
                }

                Accessibilities accessibilities = accessibility.GetAccessibilities();

                switch (accessibility)
                {
                    case Accessibility.Private:
                    case Accessibility.Protected:
                    case Accessibility.ProtectedAndInternal:
                    case Accessibility.ProtectedOrInternal:
                    case Accessibility.Internal:
                    case Accessibility.Public:
                        {
                            all |= accessibilities;
                            break;
                        }
                    default:
                        {
                            Debug.Fail(accessibility.ToString());
                            return Accessibilities.None;
                        }
                }

                foreach (Accessibility accessibility2 in AvailableAccessibilities)
                {
                    if (accessibility != accessibility2
                        && !SyntaxAccessibility.IsValidAccessibility(member, accessibility2, ignoreOverride: allowOverride))
                    {
                        valid &= ~accessibility2.GetAccessibilities();
                    }
                }
            }

            switch (all)
            {
                case Accessibilities.Private:
                case Accessibilities.Protected:
                case Accessibilities.Internal:
                case Accessibilities.Public:
                    {
                        valid &= ~all;
                        break;
                    }
            }

            return valid;
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
    }
}
