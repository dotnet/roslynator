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
    public class MakeMethodExtensionMethodAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.MakeMethodExtensionMethod); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (!classDeclaration.Identifier.ValueText.EndsWith("Extensions", StringComparison.Ordinal))
                return;

            if (!classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                return;

            if (!classDeclaration.IsParentKind(SyntaxKind.NamespaceDeclaration, SyntaxKind.CompilationUnit))
                return;

            if (!SyntaxAccessibility<ClassDeclarationSyntax>.Instance.GetAccessibility(classDeclaration).Is(Accessibility.Public, Accessibility.Internal))
                return;

            foreach (MemberDeclarationSyntax member in classDeclaration.Members)
            {
                if (!member.IsKind(SyntaxKind.MethodDeclaration))
                    continue;

                var methodDeclaration = (MethodDeclarationSyntax)member;

                if (!methodDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    continue;

                if (!SyntaxAccessibility<MethodDeclarationSyntax>.Instance.GetAccessibility(methodDeclaration).Is(Accessibility.Public, Accessibility.Internal))
                    continue;

                ParameterSyntax parameter = methodDeclaration.ParameterList?.Parameters.FirstOrDefault();

                if (parameter == null)
                    continue;

                if (parameter.Default != null)
                    continue;

                if (parameter.Type.IsKind(SyntaxKind.PointerType))
                    continue;

                if (parameter.Modifiers.Contains(SyntaxKind.ParamsKeyword))
                    continue;

                bool isThis = false;
                bool isIn = false;
                bool isRef = false;

                foreach (SyntaxToken modifier in parameter.Modifiers)
                {
                    SyntaxKind kind = modifier.Kind();

                    if (kind == SyntaxKind.ThisKeyword)
                    {
                        isThis = true;
                        break;
                    }
                    else if (kind == SyntaxKind.InKeyword)
                    {
                        isIn = true;
                    }
                    else if (kind == SyntaxKind.RefKeyword)
                    {
                        isRef = true;
                    }

                    if (isThis)
                        break;
                }

                if (isThis)
                    continue;

                if (isIn)
                {
                    IParameterSymbol parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

                    ITypeSymbol typeSymbol = parameterSymbol.Type;

                    if (!typeSymbol.IsValueType)
                        continue;

                    if (typeSymbol.Kind == SymbolKind.TypeParameter)
                        continue;
                }
                else if (isRef)
                {
                    IParameterSymbol parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

                    if (!parameterSymbol.Type.IsValueType)
                        continue;
                }

                context.ReportDiagnostic(DiagnosticDescriptors.MakeMethodExtensionMethod, methodDeclaration.Identifier);
            }
        }
    }
}
