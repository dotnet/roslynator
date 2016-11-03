// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimpleMemberAccessExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UsePredefinedType,
                    DiagnosticDescriptors.ReplaceStringEmptyWithEmptyStringLiteral);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleMemberAccessExpression(f), SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            AnalyzePredefinedType(context, memberAccess);

            if (ReplaceStringEmptyWithEmptyStringLiteralRefactoring.CanRefactor(memberAccess, context.SemanticModel, context.CancellationToken))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.ReplaceStringEmptyWithEmptyStringLiteral,
                    memberAccess.GetLocation());
            }
        }

        private static void AnalyzePredefinedType(
            SyntaxNodeAnalysisContext context,
            MemberAccessExpressionSyntax memberAccess)
        {
            if (!memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                ExpressionSyntax expression = memberAccess.Expression;

                if (expression?.IsKind(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.IdentifierName) == true)
                {
                    var namedTypeSymbol = context.SemanticModel
                        .GetSymbolInfo(expression, context.CancellationToken)
                        .Symbol as INamedTypeSymbol;

                    if (namedTypeSymbol?.IsPredefinedType() == true)
                    {
                        IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(expression, context.CancellationToken);

                        if (aliasSymbol == null)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.UsePredefinedType,
                                expression.GetLocation());
                        }
                    }
                }
            }
        }
    }
}
