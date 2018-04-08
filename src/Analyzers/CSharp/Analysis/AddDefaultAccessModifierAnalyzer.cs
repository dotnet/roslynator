// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddDefaultAccessModifierAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddDefaultAccessModifier); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            Analyze(context, classDeclaration, classDeclaration.Modifiers);
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            Analyze(context, constructorDeclaration, constructorDeclaration.Modifiers);
        }

        public static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            Analyze(context, conversionOperatorDeclaration, conversionOperatorDeclaration.Modifiers);
        }

        public static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            Analyze(context, delegateDeclaration, delegateDeclaration.Modifiers);
        }

        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            Analyze(context, enumDeclaration, enumDeclaration.Modifiers);
        }

        public static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            Analyze(context, eventDeclaration, eventDeclaration.Modifiers);
        }

        public static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            Analyze(context, eventFieldDeclaration, eventFieldDeclaration.Modifiers);
        }

        public static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            Analyze(context, fieldDeclaration, fieldDeclaration.Modifiers);
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            Analyze(context, indexerDeclaration, indexerDeclaration.Modifiers);
        }

        public static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            Analyze(context, interfaceDeclaration, interfaceDeclaration.Modifiers);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            Analyze(context, methodDeclaration, methodDeclaration.Modifiers);
        }

        public static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            Analyze(context, operatorDeclaration, operatorDeclaration.Modifiers);
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            Analyze(context, propertyDeclaration, propertyDeclaration.Modifiers);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            Analyze(context, structDeclaration, structDeclaration.Modifiers);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration, SyntaxTokenList modifiers)
        {
            Accessibility accessibility = GetAccessibility(context, declaration, modifiers);

            if (accessibility == Accessibility.NotApplicable)
                return;

            Location location = GetLocation(declaration);

            if (location == null)
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.AddDefaultAccessModifier,
                location,
                ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(nameof(Accessibility), accessibility.ToString()) }));
        }

        private static Accessibility GetAccessibility(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration, SyntaxTokenList modifiers)
        {
            if (!modifiers.Any(f => SyntaxFacts.IsAccessibilityModifier(f.Kind())))
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
                                return SyntaxAccessibility.GetDefaultExplicitAccessibility(declaration);
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
                    return SyntaxAccessibility.GetDefaultExplicitAccessibility(declaration);
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
                    if (syntaxReference.GetSyntax(context.CancellationToken) is MemberDeclarationSyntax declaration2)
                    {
                        Accessibility accessibility2 = SyntaxAccessibility.GetExplicitAccessibility(declaration2);

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

        private static Location GetLocation(SyntaxNode node)
        {
            SyntaxKind kind = node.Kind();

            if (kind == SyntaxKind.OperatorDeclaration)
                return ((OperatorDeclarationSyntax)node).OperatorToken.GetLocation();

            if (kind == SyntaxKind.ConversionOperatorDeclaration)
                return ((ConversionOperatorDeclarationSyntax)node).Type?.GetLocation();

            SyntaxToken token = CSharpUtility.GetIdentifier(node);

            if (!token.IsKind(SyntaxKind.None))
                return token.GetLocation();

            return null;
        }
    }
}
