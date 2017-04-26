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
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDefaultAccessModifierRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration)
        {
            SyntaxTokenList modifiers = declaration.GetModifiers();

            Accessibility accessibility = GetAccessModifier(context, declaration, modifiers);

            if (accessibility != Accessibility.NotApplicable)
            {
                Location location = GetLocation(declaration);

                if (location != null)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddDefaultAccessModifier,
                        location,
                        ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(nameof(Accessibility), accessibility.ToString()) }));
                }
            }
        }

        private static Accessibility GetAccessModifier(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration, SyntaxTokenList modifiers)
        {
            if (!modifiers.ContainsAccessModifier())
            {
                if (modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    if (!declaration.IsKind(SyntaxKind.MethodDeclaration))
                    {
                        Accessibility? accessibility = GetPartialAccessModifier(context, declaration);

                        if (accessibility != null)
                        {
                            if (accessibility == Accessibility.NotApplicable)
                            {
                                return declaration.GetDefaultExplicitAccessibility();
                            }
                            else
                            {
                                return accessibility.Value;
                            }
                        }
                    }
                }
                else
                {
                    return declaration.GetDefaultExplicitAccessibility();
                }
            }

            return Accessibility.NotApplicable;
        }

        private static Accessibility? GetPartialAccessModifier(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax declaration)
        {
            var accessibility = Accessibility.NotApplicable;

            ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

            if (symbol != null)
            {
                foreach (SyntaxReference syntaxReference in symbol.DeclaringSyntaxReferences)
                {
                    var declaration2 = syntaxReference.GetSyntax(context.CancellationToken) as MemberDeclarationSyntax;

                    if (declaration2 != null)
                    {
                        SyntaxTokenList modifiers = declaration2.GetModifiers();

                        Accessibility accessibility2 = DetermineAccessibility(modifiers);

                        if (accessibility2 != Accessibility.NotApplicable)
                        {
                            if (accessibility == Accessibility.NotApplicable || accessibility == accessibility2)
                            {
                                accessibility = accessibility2;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            return accessibility;
        }

        private static Accessibility DetermineAccessibility(SyntaxTokenList tokenList)
        {
            int count = tokenList.Count;

            for (int i = 0; i < count; i++)
            {
                switch (tokenList[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        return Accessibility.Public;
                    case SyntaxKind.PrivateKeyword:
                        return Accessibility.Private;
                    case SyntaxKind.InternalKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.ProtectedKeyword, Accessibility.Internal);
                    case SyntaxKind.ProtectedKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.InternalKeyword, Accessibility.Protected);
                }
            }

            return Accessibility.NotApplicable;
        }

        private static Accessibility GetAccessModifier(
            SyntaxTokenList tokenList,
            int startIndex,
            int count,
            SyntaxKind kind,
            Accessibility accessibility)
        {
            for (int i = startIndex; i < count; i++)
            {
                if (tokenList[i].Kind() == kind)
                    return Accessibility.ProtectedOrInternal;
            }

            return accessibility;
        }

        private static Location GetLocation(SyntaxNode node)
        {
            SyntaxKind kind = node.Kind();

            if (kind == SyntaxKind.OperatorDeclaration)
                return ((OperatorDeclarationSyntax)node).OperatorToken.GetLocation();

            if (kind == SyntaxKind.ConversionOperatorDeclaration)
                return ((ConversionOperatorDeclarationSyntax)node).Type?.GetLocation();

            SyntaxToken token = GetToken(node);

            if (!token.IsKind(SyntaxKind.None))
                return token.GetLocation();

            return null;
        }

        private static SyntaxToken GetToken(SyntaxNode declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).ThisKeyword;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.FieldDeclaration:
                    return GetToken(((FieldDeclarationSyntax)declaration).Declaration);
                case SyntaxKind.EventFieldDeclaration:
                    return GetToken(((EventFieldDeclarationSyntax)declaration).Declaration);
            }

            return default(SyntaxToken);
        }

        private static SyntaxToken GetToken(VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

                if (variables.Any())
                    return variables[0].Identifier;
            }

            return default(SyntaxToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            Accessibility accessibility,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newNode = GetNewMemberDeclaration(member, accessibility, ModifierComparer.Instance);

            return document.ReplaceNodeAsync(member, newNode, cancellationToken);
        }

        private static MemberDeclarationSyntax GetNewMemberDeclaration(MemberDeclarationSyntax member, Accessibility accessibility, IModifierComparer comparer)
        {
            SyntaxKind modifierKind = GetFirstModifierKind(accessibility);

            switch (member.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        ClassDeclarationSyntax declaration = ((ClassDeclarationSyntax)member)
                            .InsertModifier(modifierKind, comparer);

                        if (accessibility == Accessibility.ProtectedOrInternal)
                            return declaration.InsertModifier(SyntaxKind.InternalKeyword, comparer);

                        return declaration;
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        return ((ConstructorDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return ((ConversionOperatorDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        return ((DelegateDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        return ((EnumDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        return ((EventDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        return ((EventFieldDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        return ((FieldDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return ((IndexerDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        InterfaceDeclarationSyntax declaration = ((InterfaceDeclarationSyntax)member)
                            .InsertModifier(modifierKind, comparer);

                        if (accessibility == Accessibility.ProtectedOrInternal)
                            return declaration.InsertModifier(SyntaxKind.InternalKeyword, comparer);

                        return declaration;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        return ((MethodDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        return ((OperatorDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        return ((PropertyDeclarationSyntax)member).InsertModifier(modifierKind, comparer);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        StructDeclarationSyntax declaration = ((StructDeclarationSyntax)member)
                            .InsertModifier(modifierKind, comparer);

                        if (accessibility == Accessibility.ProtectedOrInternal)
                            return declaration.InsertModifier(SyntaxKind.InternalKeyword, comparer);

                        return declaration;
                    }
                default:
                    {
                        Debug.Assert(false, member.Kind().ToString());
                        return member;
                    }
            }
        }

        private static SyntaxKind GetFirstModifierKind(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Public:
                    {
                        return SyntaxKind.PublicKeyword;
                    }
                case Accessibility.Internal:
                    {
                        return SyntaxKind.InternalKeyword;
                    }
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                    {
                        return SyntaxKind.ProtectedKeyword;
                    }
                case Accessibility.Private:
                    {
                        return SyntaxKind.PrivateKeyword;
                    }
                default:
                    {
                        Debug.Assert(false, accessibility.ToString());
                        return SyntaxKind.None;
                    }
            }
        }
    }
}
