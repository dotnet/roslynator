// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDefaultAccessModifierRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration)
        {
            SyntaxTokenList modifiers = declaration.GetModifiers();

            AccessModifier accessModifier = GetAccessModifier(context, declaration, modifiers);

            if (accessModifier != AccessModifier.None)
            {
                Location location = GetLocation(declaration);

                if (location != null)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddDefaultAccessModifier,
                        location,
                        ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(nameof(AccessModifier), accessModifier.ToString()) }));
                }
            }
        }

        private static AccessModifier GetAccessModifier(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration, SyntaxTokenList modifiers)
        {
            if (!ModifierUtility.ContainsAccessModifier(modifiers))
            {
                if (modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    if (!declaration.IsKind(SyntaxKind.MethodDeclaration))
                    {
                        AccessModifier? accessModifier = GetPartialAccessModifier(context, declaration);

                        if (accessModifier != null)
                        {
                            if (accessModifier == AccessModifier.None)
                            {
                                return ModifierUtility.GetDefaultAccessModifier(declaration);
                            }
                            else
                            {
                                return accessModifier.Value;
                            }
                        }
                    }
                }
                else
                {
                    return ModifierUtility.GetDefaultAccessModifier(declaration);
                }
            }

            return AccessModifier.None;
        }

        private static AccessModifier? GetPartialAccessModifier(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax declaration)
        {
            var accessModifier = AccessModifier.None;

            ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

            if (symbol != null)
            {
                foreach (SyntaxReference syntaxReference in symbol.DeclaringSyntaxReferences)
                {
                    var declaration2 = syntaxReference.GetSyntax(context.CancellationToken) as MemberDeclarationSyntax;

                    if (declaration2 != null)
                    {
                        SyntaxTokenList modifiers = declaration2.GetModifiers();

                        AccessModifier accessModifier2 = ModifierUtility.GetAccessModifier(modifiers);

                        if (accessModifier2 != AccessModifier.None)
                        {
                            if (accessModifier == AccessModifier.None || accessModifier == accessModifier2)
                            {
                                accessModifier = accessModifier2;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            return accessModifier;
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

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            AccessModifier accessModifier,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxToken[] accessModifiers = CreateModifiers(accessModifier);

            List<SyntaxToken> modifiers = declaration.GetModifiers().ToList();

            MemberDeclarationSyntax newDeclaration = declaration;

            if (modifiers.Count > 0)
            {
                accessModifiers[0] = accessModifiers[0].WithLeadingTrivia(modifiers[0].LeadingTrivia);

                modifiers[0] = modifiers[0].WithoutLeadingTrivia();

                modifiers.InsertRange(0, accessModifiers);
            }
            else
            {
                SyntaxToken token = declaration.GetFirstToken();

                accessModifiers[0] = accessModifiers[0].WithLeadingTrivia(token.LeadingTrivia);

                modifiers = accessModifiers.ToList();

                newDeclaration = declaration.ReplaceToken(
                    token,
                    token.WithoutLeadingTrivia());
            }

            newDeclaration = newDeclaration.SetModifiers(TokenList(modifiers));

            SyntaxNode newRoot = root.ReplaceNode(declaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxToken[] CreateModifiers(AccessModifier accessModifier)
        {
            switch (accessModifier)
            {
                case AccessModifier.Public:
                    return new SyntaxToken[] { PublicToken() };
                case AccessModifier.Internal:
                    return new SyntaxToken[] { InternalToken() };
                case AccessModifier.Protected:
                    return new SyntaxToken[] { ProtectedToken() };
                case AccessModifier.ProtectedInternal:
                    return new SyntaxToken[] { ProtectedToken(), InternalToken() };
                case AccessModifier.Private:
                    return new SyntaxToken[] { PrivateToken() };
                default:
                    return new SyntaxToken[0];
            }
        }
    }
}
