// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis.UseMethodChaining;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyLinqMethodChain,
                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod,
                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfCountMethod,
                    DiagnosticDescriptors.CallAnyInsteadOfCount,
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
                    DiagnosticDescriptors.AvoidNullReferenceException,
                    DiagnosticDescriptors.UseStringComparison,
                    DiagnosticDescriptors.UseNameOfOperator,
                    DiagnosticDescriptors.RemoveRedundantCast,
                    DiagnosticDescriptors.SimplifyLogicalNegation,
                    DiagnosticDescriptors.CallStringConcatInsteadOfStringJoin);
           }
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
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (UseBitwiseOperationInsteadOfCallingHasFlagAnalysis.IsFixable(invocation, context.SemanticModel, context.CancellationToken)
                && !invocation.SpanContainsDirectives())
            {
                context.ReportDiagnostic(DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag, invocation);
            }

            RemoveRedundantStringToCharArrayCallAnalysis.Analyze(context, invocation);

            CombineEnumerableWhereAndAnyAnalysis.AnalyzeInvocationExpression(context);

            if (!invocation.ContainsDiagnostics)
            {
                if (!invocation.SpanContainsDirectives())
                {
                    CallExtensionMethodAsInstanceMethodAnalysisResult analysis = CallExtensionMethodAsInstanceMethodAnalysis.Analyze(invocation, context.SemanticModel, allowAnyExpression: false, cancellationToken: context.CancellationToken);

                    if (analysis.Success
                        && context.SemanticModel
                            .GetEnclosingNamedType(analysis.InvocationExpression.SpanStart, context.CancellationToken)?
                            .Equals(analysis.MethodSymbol.ContainingType) == false)
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.CallExtensionMethodAsInstanceMethod, invocation);
                    }
                }

                SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

                if (invocationInfo.Success)
                {
                    if (!invocation.SpanContainsDirectives())
                        UseRegexInstanceInsteadOfStaticMethodAnalysis.Analyze(context, invocationInfo);

                    string methodName = invocationInfo.NameText;

                    AvoidNullReferenceExceptionAnalyzer.Analyze(context, invocationInfo);

                    CallStringConcatInsteadOfStringJoinAnalysis.Analyze(context, invocationInfo);

                    int argumentCount = invocationInfo.Arguments.Count;

                    switch (argumentCount)
                    {
                        case 0:
                            {
                                switch (methodName)
                                {
                                    case "Any":
                                        {
                                            UseCountOrLengthPropertyInsteadOfAnyMethodAnalysis.Analyze(context, invocationInfo);

                                            SimplifyLinqMethodChainAnalysis.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "Cast":
                                        {
                                            CallOfTypeInsteadOfWhereAndCastAnalysis.Analyze(context, invocationInfo);
                                            RemoveRedundantCastAnalyzer.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "Count":
                                        {
                                            UseInsteadOfCountMethodAnalysis.Analyze(context, invocationInfo);
                                            SimplifyLinqMethodChainAnalysis.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "First":
                                        {
                                            if (!invocationInfo.Expression.IsKind(SyntaxKind.InvocationExpression)
                                                && UseElementAccessInsteadOfFirstAnalysis.IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
                                            {
                                                context.ReportDiagnostic(DiagnosticDescriptors.UseElementAccessInsteadOfFirst, invocationInfo.Name);
                                            }

                                            SimplifyLinqMethodChainAnalysis.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "ToString":
                                        {
                                            RemoveRedundantToStringCallAnalysis.Analyze(context, invocationInfo);
                                            UseNameOfOperatorAnalyzer.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "ToLower":
                                    case "ToLowerInvariant":
                                    case "ToUpper":
                                    case "ToUpperInvariant":
                                        {
                                            UseStringComparisonAnalysis.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "FirstOrDefault":
                                    case "Last":
                                    case "LastOrDefault":
                                    case "LongCount":
                                    case "Single":
                                    case "SingleOrDefault":
                                        {
                                            SimplifyLinqMethodChainAnalysis.Analyze(context, invocationInfo);
                                            break;
                                        }
                                }

                                break;
                            }
                        case 1:
                            {
                                switch (methodName)
                                {
                                    case "All":
                                    case "Any":
                                        {
                                            SimplifyLogicalNegationAnalyzer.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "ElementAt":
                                        {
                                            if (!invocationInfo.Expression.IsKind(SyntaxKind.InvocationExpression)
                                                && UseElementAccessInsteadOfElementAtAnalysis.IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
                                            {
                                                context.ReportDiagnostic(DiagnosticDescriptors.UseElementAccessInsteadOfElementAt, invocationInfo.Name);
                                            }

                                            break;
                                        }
                                    case "FirstOrDefault":
                                        {
                                            CallFindInsteadOfFirstOrDefaultAnalysis.Analyze(context, invocationInfo);
                                            break;
                                        }
                                    case "Where":
                                        {
                                            CombineEnumerableWhereMethodChainAnalysis.Analyze(context, invocationInfo);
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
                                OptimizeStringBuilderAppendCallAnalysis.Analyze(context, invocationInfo);
                                break;
                            }
                        case "Select":
                            {
                                if (argumentCount == 1
                                    || argumentCount == 2)
                                {
                                    CallCastInsteadOfSelectAnalysis.Analyze(context, invocationInfo);
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
                                    CallThenByInsteadOfOrderByAnalysis.Analyze(context, invocationInfo);
                                }

                                break;
                            }
                    }

                    if (UseMethodChainingAnalysis.IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
                        context.ReportDiagnostic(DiagnosticDescriptors.UseMethodChaining, invocationInfo.InvocationExpression);
                }
            }
        }
    }
}
