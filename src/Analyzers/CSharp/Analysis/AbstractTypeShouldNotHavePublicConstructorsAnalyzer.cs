// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AbstractTypeShouldNotHavePublicConstructorsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AbstractTypeShouldNotHavePublicConstructors); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            if (!SyntaxAccessibility.GetExplicitAccessibility(constructorDeclaration).Is(Accessibility.Public, Accessibility.ProtectedOrInternal))
                return;

            if (!constructorDeclaration.IsParentKind(SyntaxKind.ClassDeclaration))
                return;

            var classDeclaration = (ClassDeclarationSyntax)constructorDeclaration.Parent;

            SyntaxTokenList modifiers = classDeclaration.Modifiers;

            bool isAbstract = modifiers.Contains(SyntaxKind.AbstractKeyword);

            if (!isAbstract
                && modifiers.Contains(SyntaxKind.PartialKeyword))
            {
                INamedTypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

                if (classSymbol != null)
                    isAbstract = classSymbol.IsAbstract;
            }

            if (!isAbstract)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AbstractTypeShouldNotHavePublicConstructors, constructorDeclaration.Identifier);
        }
    }
}
