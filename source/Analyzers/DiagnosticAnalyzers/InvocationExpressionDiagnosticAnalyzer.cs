// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.UseInsteadOfCountMethod;
using Roslynator.CSharp.Syntax;

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
                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod,
                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfCountMethod,
                    DiagnosticDescriptors.UseAnyMethodInsteadOfCountMethod,
                    DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag,
                    DiagnosticDescriptors.RemoveRedundantToStringCall,
                    DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall,
                    DiagnosticDescriptors.CallCastInsteadOfSelect,
                    DiagnosticDescriptors.CombineEnumerableWhereMethodChain,
                    DiagnosticDescriptors.CombineEnumerableWhereMethodChainFadeOut,
                    DiagnosticDescriptors.CallFindInsteadOfFirstOrDefault,
                    DiagnosticDescriptors.UseElementAccessInsteadOfElementAt,
                    DiagnosticDescriptors.UseElementAccessInsteadOfFirst,
                    DiagnosticDescriptors.UseRegexInstanceInsteadOfStaticMethod,
                    DiagnosticDescriptors.CallExtensionMethodAsInstanceMethod,
                    DiagnosticDescriptors.OptimizeStringBuilderAppendCall,
                    DiagnosticDescriptors.AvoidBoxingOfValueType,
                    DiagnosticDescriptors.CallThenByInsteadOfOrderBy,
                    DiagnosticDescriptors.UseMethodChaining,
                    DiagnosticDescriptors.AvoidNullReferenceException);
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
                                    UseCountOrLengthPropertyInsteadOfAnyMethodRefactoring.Analyze(context, invocation, memberAccess);
                                    break;
                                }
                            case "Cast":
                                {
                                    CallOfTypeInsteadOfWhereAndCastRefactoring.Analyze(context, invocation);
                                    break;
                                }
                            case "Count":
                                {
                                    SimplifyLinqMethodChainRefactoring.Analyze(context, invocation, memberAccess, methodName);
                                    UseInsteadOfCountMethodRefactoring.Analyze(context, invocation, memberAccess);
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
                            case "FirstOrDefault":
                                {
                                    CallFindInsteadOfFirstOrDefaultRefactoring.Analyze(context, invocation, memberAccess);
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

            if (UseBitwiseOperationInsteadOfCallingHasFlagRefactoring.CanRefactor(invocation, context.SemanticModel, context.CancellationToken)
                && !invocation.SpanContainsDirectives())
            {
                context.ReportDiagnostic(DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag, invocation);
            }

            RemoveRedundantStringToCharArrayCallRefactoring.Analyze(context, invocation);

            CombineEnumerableWhereAndAnyRefactoring.AnalyzeInvocationExpression(context);

            if (!invocation.ContainsDiagnostics)
            {
                if (!invocation.SpanContainsDirectives())
                {
                    CallExtensionMethodAsInstanceMethodRefactoring.AnalysisResult result =
                        CallExtensionMethodAsInstanceMethodRefactoring.Analyze(invocation, context.SemanticModel, context.CancellationToken);

                    if (result.Success
                        && context.SemanticModel
                            .GetEnclosingNamedType(result.InvocationExpression.SpanStart, context.CancellationToken)?
                            .Equals(result.MethodSymbol.ContainingType) == false)
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.CallExtensionMethodAsInstanceMethod, invocation);
                    }
                }

                MemberInvocationExpression memberInvocation;
                if (MemberInvocationExpression.TryCreate(invocation, out memberInvocation))
                {
                    if (!invocation.SpanContainsDirectives())
                        UseRegexInstanceInsteadOfStaticMethodRefactoring.Analyze(context, memberInvocation);

                    string methodName = memberInvocation.NameText;

                    AvoidNullReferenceExceptionRefactoring.Analyze(context, memberInvocation);

                    int argumentCount = memberInvocation.ArgumentList.Arguments.Count;

                    switch (argumentCount)
                    {
                        case 0:
                            {
                                switch (methodName)
                                {
                                    case "First":
                                        {
                                            if (!memberInvocation.Expression.IsKind(SyntaxKind.InvocationExpression)
                                                && UseElementAccessInsteadOfFirstRefactoring.CanRefactor(memberInvocation, context.SemanticModel, context.CancellationToken))
                                            {
                                                context.ReportDiagnostic(DiagnosticDescriptors.UseElementAccessInsteadOfFirst, memberInvocation.Name);
                                            }

                                            break;
                                        }
                                    case "ToString":
                                        {
                                            RemoveRedundantToStringCallRefactoring.Analyze(context, memberInvocation);
                                            break;
                                        }
                                }

                                break;
                            }
                        case 1:
                            {
                                switch (methodName)
                                {
                                    case "ElementAt":
                                        {
                                            if (!memberInvocation.Expression.IsKind(SyntaxKind.InvocationExpression)
                                                && UseElementAccessInsteadOfElementAtRefactoring.CanRefactor(memberInvocation, context.SemanticModel, context.CancellationToken))
                                            {
                                                context.ReportDiagnostic(DiagnosticDescriptors.UseElementAccessInsteadOfElementAt, memberInvocation.Name);
                                            }

                                            break;
                                        }
                                }

                                break;
                            }
                    }

                    switch (methodName)
                    {
                        case "Append":
                        case "AppendLine":
                        case "AppendFormat":
                        case "Insert":
                            {
                                OptimizeStringBuilderAppendCallRefactoring.Analyze(context, memberInvocation);
                                break;
                            }
                        case "Select":
                            {
                                if (argumentCount == 1
                                    || argumentCount == 2)
                                {
                                    CallCastInsteadOfSelectRefactoring.Analyze(context, memberInvocation);
                                }

                                break;
                            }
                        case "OrderBy":
                        case "OrderByDescending":
                            {
                                if (argumentCount == 1
                                    || argumentCount == 2
                                    || argumentCount == 3)
                                {
                                    CallThenByInsteadOfOrderByRefactoring.Analyze(context, memberInvocation);
                                }

                                break;
                            }
                    }

                    UseMethodChainingRefactoring.Analyze(context, memberInvocation);
                }
            }
        }
    }
}
