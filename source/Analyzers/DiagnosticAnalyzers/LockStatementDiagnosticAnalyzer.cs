// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LockStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidLockingOnPubliclyAccessibleInstance); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeLockStatement(f), SyntaxKind.LockStatement);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lockStatement = (LockStatementSyntax)context.Node;

            ExpressionSyntax expression = lockStatement.Expression;

            if (expression?.IsKind(SyntaxKind.ThisExpression, SyntaxKind.TypeOfExpression) == true)
            {
                ITypeSymbol typeSymbol = context.SemanticModel
                    .GetTypeInfo(expression, context.CancellationToken)
                    .Type;

                if (typeSymbol?.IsPubliclyAccessible() == true)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AvoidLockingOnPubliclyAccessibleInstance,
                        expression.GetLocation(),
                        expression.ToString());
                }
            }
        }
    }
}
