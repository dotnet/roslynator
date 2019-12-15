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
    public class RemovePartialModifierFromTypeWithSinglePartAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node);
        }

        private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node);
        }

        private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node);
        }

        private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
        {
            if (!typeDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
                return;

            INamedTypeSymbol symbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken);

            if (symbol?.DeclaringSyntaxReferences.SingleOrDefault(shouldThrow: false) == null)
                return;

            foreach (MemberDeclarationSyntax member in typeDeclaration.Members)
            {
                if (member.IsKind(SyntaxKind.MethodDeclaration))
                {
                    var method = (MethodDeclarationSyntax)member;

                    if (method.Modifiers.Contains(SyntaxKind.PartialKeyword)
                        && method.BodyOrExpressionBody() == null
                        && method.ContainsUnbalancedIfElseDirectives(method.Span))
                    {
                        return;
                    }
                }
            }

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart,
                typeDeclaration.Modifiers.Find(SyntaxKind.PartialKeyword));
        }
    }
}
