// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.Comparers;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModifiersDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.ReorderModifiers,
                    DiagnosticDescriptors.AddAccessModifier);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeDeclaration(f),
                SyntaxKind.ClassDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.StructDeclaration);
        }

        private void AnalyzeDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            SyntaxTokenList modifiers = context.Node.GetDeclarationModifiers();

            ReorderModifiers(context, modifiers);

            AddDefaultAccessModifier(context, modifiers);
        }

        private static void ReorderModifiers(SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers)
        {
            if (modifiers.Count <= 1)
                return;

            if (AreModifiersSorted(modifiers))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.ReorderModifiers,
                Location.Create(context.Node.SyntaxTree, modifiers.Span));
        }

        private static void AddDefaultAccessModifier(SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers)
        {
            if (context.Node.IsKind(SyntaxKind.OperatorDeclaration))
                return;

            //TODO: check access modifier for partial class, struct, interface
            if (modifiers.Any(SyntaxKind.PartialKeyword))
                return;

            if (modifiers.ContainsAccessModifier())
                return;

            if (!IsAccessModifierRequired(context))
                return;

            Location location = GetLocation(context.Node);

            if (location == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddAccessModifier, location);
        }

        private static bool IsAccessModifierRequired(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Kind() == SyntaxKind.FieldDeclaration)
                return true;

            if (context.Node.HasExplicitInterfaceSpecifier())
                return false;

            SyntaxNode declaration = GetDeclarationNode(context.Node);

            if (declaration == null)
                return false;

            ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

            if (symbol == null || symbol.IsKind(SymbolKind.ErrorType))
                return false;

            if (context.Node.Kind() == SyntaxKind.ConstructorDeclaration && symbol.IsStatic)
                return false;

            if (symbol.ContainingType?.TypeKind == TypeKind.Interface)
                return false;

            return true;
        }

        private static SyntaxNode GetDeclarationNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode.Kind() != SyntaxKind.EventFieldDeclaration)
                return syntaxNode;

            var eventFieldDeclaration = (EventFieldDeclarationSyntax)syntaxNode;

            if (eventFieldDeclaration.Declaration?.Variables.Count > 0)
                return eventFieldDeclaration.Declaration?.Variables[0];

            return null;
        }

        private static Location GetLocation(SyntaxNode syntaxNode)
        {
            if (syntaxNode.Kind() == SyntaxKind.ConversionOperatorDeclaration)
                return ((ConversionOperatorDeclarationSyntax)syntaxNode).Type?.GetLocation();

            SyntaxToken? syntaxToken = GetToken(syntaxNode);

            if (syntaxToken == null)
                return null;

            if (syntaxToken.Value.Kind() == SyntaxKind.None)
                return null;

            return syntaxToken.Value.GetLocation();
        }

        private static SyntaxToken? GetToken(SyntaxNode declaration)
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
                    {
                        var fieldDeclaration = (FieldDeclarationSyntax)declaration;

                        if (fieldDeclaration.Declaration?.Variables.Count > 0)
                            return fieldDeclaration.Declaration.Variables[0].Identifier;

                        break;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        var eventFieldDeclaration = (EventFieldDeclarationSyntax)declaration;

                        if (eventFieldDeclaration.Declaration?.Variables.Count > 0)
                            return eventFieldDeclaration.Declaration.Variables[0].Identifier;

                        break;
                    }
            }

            return null;
        }

        private static bool AreModifiersSorted(SyntaxTokenList list)
        {
            return AreModifiersSorted(list, ModifierSorter.Instance);
        }

        private static bool AreModifiersSorted(SyntaxTokenList list, IComparer<SyntaxToken> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            for (int i = 0; i < list.Count - 1; i++)
            {
                if (comparer.Compare(list[i], list[i + 1]) >= 0)
                    return false;
            }

            return true;
        }
    }
}
