// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CallSkipAndAnyInsteadOfCountAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.CallSkipAndAnyInsteadOfCount); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (invocationExpression.ContainsDiagnostics)
                return;

            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            if (!invocationInfo.Success)
                return;

            if (invocationInfo.Arguments.Count != 0)
                return;

            if (invocationInfo.NameText != "Count")
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, "Count"))
                return;

            SyntaxNode parent = invocationExpression.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)parent;

                        if (binaryExpression.Left == invocationExpression)
                        {
                            if (!binaryExpression.Right.IsNumericLiteralExpression("0"))
                                ReportDiagnostic(parent);
                        }
                        else if (!binaryExpression.Left.IsNumericLiteralExpression("1"))
                        {
                            ReportDiagnostic(parent);
                        }

                        break;
                    }
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.LessThanExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)parent;

                        if (binaryExpression.Left == invocationExpression)
                        {
                            if (!binaryExpression.Right.IsNumericLiteralExpression("1"))
                                ReportDiagnostic(parent);
                        }
                        else if (!binaryExpression.Left.IsNumericLiteralExpression("0"))
                        {
                            ReportDiagnostic(parent);
                        }

                        break;
                    }
            }

            void ReportDiagnostic(SyntaxNode node)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.CallSkipAndAnyInsteadOfCount, node);
            }
        }
    }
}
