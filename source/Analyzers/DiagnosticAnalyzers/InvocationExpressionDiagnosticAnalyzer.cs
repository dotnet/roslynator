// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.ReplaceCountMethod;
using Roslynator.Extensions;

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
                    DiagnosticDescriptors.UseCastMethodInsteadOfSelectMethod,
                    DiagnosticDescriptors.CombineEnumerableWhereMethodChain,
                    DiagnosticDescriptors.CombineEnumerableWhereMethodChainFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeInvocationExpression(f), SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            ExpressionSyntax expression = invocation.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList?.IsMissing == false)
                {
                    int argumentCount = argumentList.Arguments.Count;

                    string methodName = memberAccess.Name?.Identifier.ValueText;

                    if (argumentCount == 0)
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
                    else if (argumentCount == 1)
                    {
                        switch (methodName)
                        {
                            case "Select":
                                {
                                    ReplaceSelectWithCastRefactoring.Analyze(context, invocation, memberAccess);
                                    break;
                                }
                            case "Where":
                                {
                                    CombineEnumerableWhereMethodChainRefactoring.Analyze(context, invocation, memberAccess);
                                    break;
                                }
                        }
                    }
                }
            }

            if (ReplaceHasFlagWithBitwiseOperationRefactoring.CanRefactor(invocation, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.UseBitwiseOperationInsteadOfHasFlagMethod, invocation.GetLocation());

            RemoveRedundantToStringCallRefactoring.Analyze(context, invocation);

            RemoveRedundantStringToCharArrayCallRefactoring.Analyze(context, invocation);
        }
    }
}
