// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
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
                    DiagnosticDescriptors.OptimizeLinqMethodCall,
                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod,
                    DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag,
                    DiagnosticDescriptors.RemoveRedundantToStringCall,
                    DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall,
                    DiagnosticDescriptors.CombineEnumerableWhereMethodChain,
                    DiagnosticDescriptors.CombineEnumerableWhereMethodChainFadeOut,
                    DiagnosticDescriptors.UseRegexInstanceInsteadOfStaticMethod,
                    DiagnosticDescriptors.OptimizeStringBuilderAppendCall,
                    DiagnosticDescriptors.AvoidBoxingOfValueType,
                    DiagnosticDescriptors.CallThenByInsteadOfOrderBy,
                    DiagnosticDescriptors.UseMethodChaining,
                    DiagnosticDescriptors.AvoidNullReferenceException,
                    DiagnosticDescriptors.UseStringComparison,
                    DiagnosticDescriptors.UseNameOfOperator,
                    DiagnosticDescriptors.RemoveRedundantCast,
                    DiagnosticDescriptors.SimplifyLogicalNegation,
                    DiagnosticDescriptors.UseCoalesceExpression,
                    DiagnosticDescriptors.OptimizeMethodCall);
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

            if (invocation.ContainsDiagnostics)
                return;

            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

            if (!invocationInfo.Success)
                return;

            string methodName = invocationInfo.NameText;

            int argumentCount = invocationInfo.Arguments.Count;

            switch (argumentCount)
            {
                case 0:
                    {
                        switch (methodName)
                        {
                            case "Any":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod))
                                        UseCountOrLengthPropertyInsteadOfAnyMethodAnalysis.Analyze(context, invocationInfo);

                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                    {
                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                        OptimizeLinqMethodCallAnalysis.AnalyzeAny(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "Cast":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhereAndCast(context, invocationInfo);

                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveRedundantCast))
                                        RemoveRedundantCastAnalyzer.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "Count":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                    {
                                        OptimizeLinqMethodCallAnalysis.AnalyzeCount(context, invocationInfo);
                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "First":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                    {
                                        if (!invocationInfo.Expression.IsKind(SyntaxKind.InvocationExpression)
                                            && UseElementAccessAnalysis.IsFixableFirst(invocationInfo, context.SemanticModel, context.CancellationToken))
                                        {
                                            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.OptimizeLinqMethodCall, Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationInfo.ArgumentList.Span.End)));
                                        }

                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                        OptimizeLinqMethodCallAnalysis.AnalyzeFirst(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "Max":
                            case "Min":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                        OptimizeLinqMethodCallAnalysis.AnalyzeSelectAndMinOrMax(context, invocationInfo);

                                    break;
                                }
                            case "ToString":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveRedundantToStringCall))
                                        RemoveRedundantToStringCallAnalysis.Analyze(context, invocationInfo);

                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseNameOfOperator)
                                        && ((CSharpCompilation)context.Compilation).LanguageVersion >= LanguageVersion.CSharp6)
                                    {
                                        UseNameOfOperatorAnalyzer.Analyze(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "ToLower":
                            case "ToLowerInvariant":
                            case "ToUpper":
                            case "ToUpperInvariant":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseStringComparison))
                                        UseStringComparisonAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "FirstOrDefault":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                    {
                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                        OptimizeLinqMethodCallAnalysis.AnalyzeFirstOrDefault(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "Last":
                            case "LastOrDefault":
                            case "LongCount":
                            case "Single":
                            case "SingleOrDefault":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);

                                    break;
                                }
                            case "OfType":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall)
                                        && !invocation.SpanContainsDirectives())
                                    {
                                        OptimizeLinqMethodCallAnalysis.AnalyzeOfType(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "ToCharArray":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall))
                                        RemoveRedundantStringToCharArrayCallAnalysis.Analyze(context, invocationInfo);

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
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.SimplifyLogicalNegation))
                                        SimplifyLogicalNegationAnalyzer.Analyze(context, invocationInfo);

                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall)
                                        && !invocation.SpanContainsDirectives())
                                    {
                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhereAndAny(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "ContainsKey":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeMethodCall))
                                        OptimizeMethodCallAnalysis.OptimizeDictionaryContainsKey(context, invocationInfo);

                                    break;
                                }
                            case "ElementAt":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall)
                                        && !invocationInfo.Expression.IsKind(SyntaxKind.InvocationExpression)
                                        && UseElementAccessAnalysis.IsFixableElementAt(invocationInfo, context.SemanticModel, context.CancellationToken))
                                    {
                                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.OptimizeLinqMethodCall, Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationInfo.ArgumentList.Span.End)));
                                    }

                                    break;
                                }
                            case "FirstOrDefault":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                        OptimizeLinqMethodCallAnalysis.AnalyzeFirstOrDefault(context, invocationInfo);

                                    break;
                                }
                            case "GetValueOrDefault":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseCoalesceExpression)
                                        && invocationInfo.Name.IsKind(SyntaxKind.IdentifierName)
                                        && !invocation.IsParentKind(SyntaxKind.InvocationExpression, SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression)
                                        && context.SemanticModel
                                            .GetMethodSymbol(invocationInfo.InvocationExpression, context.CancellationToken)?
                                            .ContainingType
                                            .OriginalDefinition
                                            .SpecialType == SpecialType.System_Nullable_T)
                                    {
                                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseCoalesceExpression, invocationInfo.Name);
                                    }

                                    break;
                                }
                            case "Where":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.CombineEnumerableWhereMethodChain))
                                        CombineEnumerableWhereMethodChainAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "HasFlag":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag)
                                        && !invocation.SpanContainsDirectives()
                                        && UseBitwiseOperationInsteadOfCallingHasFlagAnalysis.IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
                                    {
                                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag, invocation);
                                    }

                                    break;
                                }
                            case "Select":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                        CallCastInsteadOfSelectAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "OrderBy":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.CallThenByInsteadOfOrderBy))
                                        CallThenByInsteadOfOrderByAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                        }

                        break;
                    }
                case 2:
                    {
                        switch (invocationInfo.NameText)
                        {
                            case "IsMatch":
                            case "Match":
                            case "Matches":
                            case "Split":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseRegexInstanceInsteadOfStaticMethod)
                                        && !invocation.SpanContainsDirectives())
                                    {
                                        UseRegexInstanceInsteadOfStaticMethodAnalysis.Analyze(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "Select":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeLinqMethodCall))
                                        CallCastInsteadOfSelectAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "OrderBy":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.CallThenByInsteadOfOrderBy))
                                        CallThenByInsteadOfOrderByAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                        }

                        break;
                    }
                case 3:
                    {
                        switch (invocationInfo.NameText)
                        {
                            case "IsMatch":
                            case "Match":
                            case "Matches":
                            case "Split":
                            case "Replace":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseRegexInstanceInsteadOfStaticMethod)
                                        && !invocation.SpanContainsDirectives())
                                    {
                                        UseRegexInstanceInsteadOfStaticMethodAnalysis.Analyze(context, invocationInfo);
                                    }

                                    break;
                                }
                            case "OrderBy":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.CallThenByInsteadOfOrderBy))
                                        CallThenByInsteadOfOrderByAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "Compare":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeMethodCall))
                                        OptimizeMethodCallAnalysis.OptimizeStringCompare(context, invocationInfo);

                                    break;
                                }
                        }

                        break;
                    }
                case 4:
                    {
                        switch (invocationInfo.NameText)
                        {
                            case "Replace":
                                {
                                    if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseRegexInstanceInsteadOfStaticMethod)
                                        && !invocation.SpanContainsDirectives())
                                    {
                                        UseRegexInstanceInsteadOfStaticMethodAnalysis.Analyze(context, invocationInfo);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
            }

            switch (methodName)
            {
                case "ElementAtOrDefault":
                case "FirstOrDefault":
                case "LastOrDefault":
                case "SingleOrDefault":
                    {
                        if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.AvoidNullReferenceException))
                        {
                            if (argumentCount == 0
                                || argumentCount == 1
                                || argumentCount == 2)
                            {
                                AvoidNullReferenceExceptionAnalyzer.Analyze(context, invocationInfo);
                            }
                        }

                        break;
                    }
                case "Append":
                case "AppendLine":
                case "AppendFormat":
                case "Insert":
                    {
                        if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeStringBuilderAppendCall))
                            OptimizeStringBuilderAppendCallAnalysis.Analyze(context, invocationInfo);

                        if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.AvoidBoxingOfValueType))
                            AvoidBoxingOfValueTypeAnalysis.Analyze(context, invocationInfo);

                        break;
                    }
                case "Assert":
                    {
                        if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeMethodCall)
                            && (argumentCount >= 1 && argumentCount <= 3))
                        {
                            OptimizeMethodCallAnalysis.OptimizeDebugAssert(context, invocationInfo);
                        }

                        break;
                    }
                case "Join":
                    {
                        if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.OptimizeMethodCall)
                            && argumentCount >= 2)
                        {
                            OptimizeMethodCallAnalysis.OptimizeStringJoin(context, invocationInfo);
                        }

                        break;
                    }
            }

            if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseMethodChaining)
                && UseMethodChainingAnalysis.IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseMethodChaining, invocationInfo.InvocationExpression);
            }
        }
    }
}
