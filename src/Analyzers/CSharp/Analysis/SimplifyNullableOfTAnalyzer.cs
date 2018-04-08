// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifyNullableOfTAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SimplifyNullableOfT); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeQualifiedName, SyntaxKind.QualifiedName);
            context.RegisterSyntaxNodeAction(AnalyzeGenericName, SyntaxKind.GenericName);
        }

        public static void AnalyzeGenericName(SyntaxNodeAnalysisContext context)
        {
            var genericName = (GenericNameSyntax)context.Node;

            if (genericName.IsParentKind(
                SyntaxKind.QualifiedName,
                SyntaxKind.UsingDirective,
                SyntaxKind.NameMemberCref,
                SyntaxKind.QualifiedCref))
            {
                return;
            }

            if (IsWithinNameOfExpression(genericName, context.SemanticModel, context.CancellationToken))
                return;

            if (genericName
                .TypeArgumentList?
                .Arguments
                .SingleOrDefault(shouldThrow: false)?
                .IsKind(SyntaxKind.OmittedTypeArgument) != false)
            {
                return;
            }

            if (!(context.SemanticModel.GetSymbol(genericName, context.CancellationToken) is INamedTypeSymbol namedTypeSymbol))
                return;

            if (!namedTypeSymbol.IsNullableType())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.SimplifyNullableOfT, genericName);
        }

        public static void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context)
        {
            var qualifiedName = (QualifiedNameSyntax)context.Node;

            if (qualifiedName.IsParentKind(SyntaxKind.UsingDirective, SyntaxKind.QualifiedCref))
                return;

            if (IsWithinNameOfExpression(qualifiedName, context.SemanticModel, context.CancellationToken))
                return;

            if (!(context.SemanticModel.GetSymbol(qualifiedName, context.CancellationToken) is INamedTypeSymbol typeSymbol))
                return;

            if (CSharpFacts.IsPredefinedType(typeSymbol.SpecialType))
                return;

            if (!typeSymbol.IsNullableType())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.SimplifyNullableOfT, qualifiedName);
        }

        private static bool IsWithinNameOfExpression(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            for (node = node.Parent; node != null; node = node.Parent)
            {
                SyntaxKind kind = node.Kind();

                if (kind == SyntaxKind.InvocationExpression)
                {
                    if (CSharpUtility.IsNameOfExpression((InvocationExpressionSyntax)node, semanticModel, cancellationToken))
                        return true;
                }
                else if (kind == SyntaxKind.TypeArgumentList)
                {
                    break;
                }

                if (node is StatementSyntax
                    || node is MemberDeclarationSyntax)
                {
                    break;
                }
            }

            return false;
        }
    }
}
