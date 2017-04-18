// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Comparers;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class OverridingMemberCannotChangeAccessModifiersRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                SyntaxToken identifier = methodDeclaration.Identifier;

                if (ContainsDiagnostic(identifier, context.SemanticModel, context.CancellationToken))
                    ReportDiagnostic(context, methodDeclaration, modifiers, identifier);
            }
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                SyntaxToken identifier = propertyDeclaration.Identifier;

                if (ContainsDiagnostic(identifier, context.SemanticModel, context.CancellationToken))
                    ReportDiagnostic(context, propertyDeclaration, modifiers, identifier);
            }
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                SyntaxToken thisKeyword = indexerDeclaration.ThisKeyword;

                if (ContainsDiagnostic(thisKeyword, context.SemanticModel, context.CancellationToken))
                    ReportDiagnostic(context, indexerDeclaration, modifiers, thisKeyword);
            }
        }

        public static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = eventDeclaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                SyntaxToken identifier = eventDeclaration.Identifier;

                if (ContainsDiagnostic(identifier, context.SemanticModel, context.CancellationToken))
                    ReportDiagnostic(context, eventDeclaration, modifiers, identifier);
            }
        }

        public static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = eventFieldDeclaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                VariableDeclarationSyntax declaration = eventFieldDeclaration.Declaration;

                if (declaration != null)
                {
                    SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = declaration.Variables;

                    for (int i = 0; i < declarators.Count; i++)
                    {
                        SyntaxToken identifier = declarators[i].Identifier;

                        if (ContainsDiagnostic(identifier, context.SemanticModel, context.CancellationToken))
                        {
                            if (declarators.Count == 1
                                || CheckOtherEvents(declarators, i, context.SemanticModel, context.CancellationToken))
                            {
                                ReportDiagnostic(context, eventFieldDeclaration, modifiers, identifier);
                            }

                            break;
                        }
                    }
                }
            }
        }

        private static bool ContainsDiagnostic(SyntaxToken identifier, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return semanticModel.ContainsCompilerDiagnostic(
                CSharpErrorCodes.CannotChangeAccessModifiersWhenOverridingInheritedMember,
                identifier.Span,
                cancellationToken);
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTokenList modifiers,
            SyntaxToken token)
        {
            if (!memberDeclaration.ContainsDirectives(modifiers.Span))
            {
                if (!modifiers.ContainsAccessModifier()
                    || memberDeclaration.GetDeclaredAccessibility() != Accessibility.ProtectedOrInternal
                    || AreTokensNextToEachOther(modifiers, SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.OverridingMemberCannotChangeAccessModifiers,
                        token);
                }
            }
        }

        private static bool CheckOtherEvents(SeparatedSyntaxList<VariableDeclaratorSyntax> declarators, int i, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var eventSymbol = semanticModel.GetDeclaredSymbol(declarators[i], cancellationToken) as IEventSymbol;

            IEventSymbol overriddenEvent = eventSymbol?.OverriddenEvent;

            if (overriddenEvent == null)
                return false;

            for (int j = 0; j < declarators.Count; j++)
            {
                if (j != i)
                {
                    var eventSymbol2 = semanticModel.GetDeclaredSymbol(declarators[j], cancellationToken) as IEventSymbol;

                    if (!eventSymbol2.IsOverride
                        || eventSymbol2.DeclaredAccessibility != overriddenEvent.DeclaredAccessibility)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            OverrideInfo overrideInfo,
            CancellationToken cancellationToken)
        {
            SyntaxTokenList modifiers = memberDeclaration.GetModifiers();

            Accessibility accessibility = overrideInfo.Symbol.DeclaredAccessibility;

            Accessibility newAccessibility = overrideInfo.OverridenSymbol.DeclaredAccessibility;

            int index = IndexOfFirstAccessModifier(modifiers);

            SyntaxTokenList newModifiers = modifiers;

            if (index == -1)
            {
                foreach (SyntaxToken modifier in Modifiers.FromAccessibility(newAccessibility))
                    newModifiers = newModifiers.InsertModifier(modifier, ModifierComparer.Instance);
            }
            else
            {
                SyntaxToken modifier = modifiers[index];

                if (accessibility == Accessibility.ProtectedOrInternal)
                {
                    SyntaxToken newModifier = CreateAccessModifier(newAccessibility)
                        .WithLeadingTrivia(modifier.LeadingTrivia)
                        .WithTrailingTrivia(modifiers[index + 1].TrailingTrivia);

                    newModifiers = newModifiers
                        .ReplaceAt(index, newModifier)
                        .RemoveAt(index + 1);
                }
                else if (newAccessibility == Accessibility.ProtectedOrInternal)
                {
                    newModifiers = newModifiers
                        .ReplaceAt(index, ProtectedKeyword().WithLeadingTrivia(modifier.LeadingTrivia))
                        .Insert(index + 1, InternalKeyword().WithTrailingTrivia(modifier.TrailingTrivia));
                }
                else
                {
                    SyntaxToken newModifier = CreateAccessModifier(newAccessibility).WithTriviaFrom(modifier);

                    newModifiers = newModifiers.ReplaceAt(index, newModifier);
                }
            }

            return document.ReplaceNodeAsync(memberDeclaration, memberDeclaration.WithModifiers(newModifiers), cancellationToken);
        }

        private static SyntaxToken CreateAccessModifier(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return PrivateKeyword();
                case Accessibility.Protected:
                    return ProtectedKeyword();
                case Accessibility.Internal:
                    return InternalKeyword();
                case Accessibility.Public:
                    return PublicKeyword();
                default:
                    return default(SyntaxToken);
            }
        }

        private static int IndexOfFirstAccessModifier(SyntaxTokenList modifiers)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].IsAccessModifier())
                    return i;
            }

            return -1;
        }

        private static bool AreTokensNextToEachOther(SyntaxTokenList modifiers, SyntaxKind kind1, SyntaxKind kind2)
        {
            if (modifiers.Count > 1)
            {
                int index1 = modifiers.IndexOf(kind1);

                if (index1 != -1)
                {
                    int index2 = modifiers.IndexOf(kind2);

                    if (index2 != -1)
                    {
                        return Math.Abs(index1 - index2) == 1;
                    }
                }
            }

            return false;
        }

        internal static OverrideInfo GetOverrideInfo(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);

                        return new OverrideInfo(methodSymbol, methodSymbol.OverriddenMethod);
                    }
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                    {
                        var propertySymbol = (IPropertySymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);

                        return new OverrideInfo(propertySymbol, propertySymbol.OverriddenProperty);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventSymbol = (IEventSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);

                        return new OverrideInfo(eventSymbol, eventSymbol.OverriddenEvent);
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        VariableDeclaratorSyntax declarator = ((EventFieldDeclarationSyntax)memberDeclaration).Declaration.Variables.First();

                        var eventSymbol = (IEventSymbol)semanticModel.GetDeclaredSymbol(declarator, cancellationToken);

                        return new OverrideInfo(eventSymbol, eventSymbol.OverriddenEvent);
                    }
                default:
                    {
                        Debug.Assert(false, memberDeclaration.Kind().ToString());

                        return null;
                    }
            }
        }

        internal static string GetAccessibilityText(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return "private";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.ProtectedOrInternal:
                    return "protected internal";
                case Accessibility.Public:
                    return "public";
                default:
                    {
                        Debug.Assert(false, accessibility.ToString());
                        return "";
                    }
            }
        }
    }
}
