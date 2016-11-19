// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.ReplaceCountMethod;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyLinqMethodChain,
                    DiagnosticDescriptors.ReplaceAnyMethodWithCountOrLengthProperty,
                    DiagnosticDescriptors.ReplaceCountMethodWithCountOrLengthProperty,
                    DiagnosticDescriptors.ReplaceCountMethodWithAnyMethod,
                    DiagnosticDescriptors.UseBitwiseOperationInsteadOfHasFlagMethod,
                    DiagnosticDescriptors.RemoveRedundantToStringCall,
                    DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall,
                    DiagnosticDescriptors.UseCastMethodInsteadOfSelectMethod);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeInvocationExpression(f), SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var invocation = (InvocationExpressionSyntax)context.Node;

            ExpressionSyntax expression = invocation.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                string methodName = memberAccess.Name?.Identifier.ValueText;

                switch (methodName)
                {
                    case "Select":
                        {
                            ReplaceSelectWithCastRefactoring.Analyze(context, invocation, memberAccess);
                            break;
                        }
                }

                if (invocation.ArgumentList?.Arguments.Count == 0)
                {
                    switch (methodName)
                    {
                        case "Any":
                            {
                                SimplifyLinqMethodChainRefactoring.Analyze(context, invocation, memberAccess, methodName);
                                ReplaceAnyMethodWithCountOrLengthPropertyRefactoring.Analyze(context, invocation, memberAccess);
                                break;
                            }
                        case "Cast":
                            {
                                ReplaceWhereAndCastWithOfTypeRefactoring.Analyze(context, invocation);
                                break;
                            }
                        case "Count":
                            {
                                SimplifyLinqMethodChainRefactoring.Analyze(context, invocation, memberAccess, methodName);
                                ReplaceCountMethodRefactoring.Analyze(context, invocation, memberAccess);
                                break;
                            }
                        case "First":
                        case "FirstOrDefault":
                        case "Last":
                        case "LastOrDefault":
                        case "LongCount":
                        case "Single":
                        case "SingleOrDefault":
                            {
                                SimplifyLinqMethodChainRefactoring.Analyze(context, invocation, memberAccess, methodName);
                                break;
                            }
                    }
                }
            }

            if (ReplaceHasFlagWithBitwiseOperationRefactoring.CanRefactor(invocation, context.SemanticModel, context.CancellationToken))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseBitwiseOperationInsteadOfHasFlagMethod,
                    invocation.GetLocation());
            }

            if (RemoveRedundantToStringCallRefactoring.CanRefactor(invocation, context.SemanticModel, context.CancellationToken))
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                TextSpan span = TextSpan.FromBounds(memberAccess.OperatorToken.Span.Start, invocation.Span.End);

                if (!invocation.ContainsDirectives(span))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantToStringCall,
                        Location.Create(invocation.SyntaxTree, span));
                }
            }

            if (RemoveRedundantStringToCharArrayCallRefactoring.CanRefactor(invocation, context.SemanticModel, context.CancellationToken))
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                TextSpan span = TextSpan.FromBounds(memberAccess.OperatorToken.Span.Start, invocation.Span.End);

                if (!invocation.ContainsDirectives(span))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall,
                        Location.Create(invocation.SyntaxTree, span));
                }
            }
        }
    }
}
