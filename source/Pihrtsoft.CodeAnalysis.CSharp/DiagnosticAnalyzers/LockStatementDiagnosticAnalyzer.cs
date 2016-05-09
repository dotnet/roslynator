// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LockStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.AvoidLockingOnPubliclyAccessibleInstance);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.LockStatement);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lockStatement = (LockStatementSyntax)context.Node;

            if (lockStatement.Expression != null
                && lockStatement.Expression.IsAnyKind(SyntaxKind.ThisExpression, SyntaxKind.TypeOfExpression))
            {
                ITypeSymbol typeSymbol = context.SemanticModel
                    .GetTypeInfo(lockStatement.Expression, context.CancellationToken).Type;

                if (typeSymbol != null && IsPubliclyAccessible(typeSymbol))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AvoidLockingOnPubliclyAccessibleInstance,
                        lockStatement.Expression.GetLocation());
                }
            }
        }

        private static bool IsPubliclyAccessible(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.DeclaredAccessibility)
            {
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.Public:
                    return true;
                default:
                    return false;
            }
        }
    }
}
