// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RemovePartialModifierFromTypeWithSinglePartAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;
        private static readonly MetadataName _componentBaseName = MetadataName.Parse("Microsoft.AspNetCore.Components.ComponentBase");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemovePartialModifierFromTypeWithSinglePart);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                f => AnalyzeTypeDeclaration(f),
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.RecordDeclaration,
                SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            if (!typeDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
                return;

            INamedTypeSymbol symbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken);

            if (symbol?.DeclaringSyntaxReferences.SingleOrDefault(shouldThrow: false) == null)
                return;

            if (symbol.InheritsFrom(_componentBaseName))
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
                DiagnosticRules.RemovePartialModifierFromTypeWithSinglePart,
                typeDeclaration.Modifiers.Find(SyntaxKind.PartialKeyword));
        }
    }
}
