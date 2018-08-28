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
                    DiagnosticDescriptors.CallStringConcatInsteadOfStringJoin,
                    DiagnosticDescriptors.UseCoalesceExpression);
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
                                    UseCountOrLengthPropertyInsteadOfAnyMethodAnalysis.Analyze(context, invocationInfo);
                                    OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                    break;
                                }
                            case "Cast":
                                {
                                    OptimizeLinqMethodCallAnalysis.AnalyzeWhereAndCast(context, invocationInfo);
                                    RemoveRedundantCastAnalyzer.Analyze(context, invocationInfo);
                                    break;
                                }
                            case "Count":
                                {
                                    OptimizeLinqMethodCallAnalysis.AnalyzeCount(context, invocationInfo);
                                    OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                    break;
                                }
                            case "First":
                                {
                                    if (!invocationInfo.Expression.IsKind(SyntaxKind.InvocationExpression)
                                        && UseElementAccessAnalysis.IsFixableFirst(invocationInfo, context.SemanticModel, context.CancellationToken))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeLinqMethodCall, Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationInfo.ArgumentList.Span.End)));
                                    }

                                    OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                    OptimizeLinqMethodCallAnalysis.AnalyzeFirst(context, invocationInfo);
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
                                {
                                    OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                    OptimizeLinqMethodCallAnalysis.AnalyzeFirstOrDefault(context, invocationInfo);
                                    break;
                                }
                            case "Last":
                            case "LastOrDefault":
                            case "LongCount":
                            case "Single":
                            case "SingleOrDefault":
                                {
                                    OptimizeLinqMethodCallAnalysis.AnalyzeWhere(context, invocationInfo);
                                    break;
                                }
                            case "OfType":
                                {
                                    if (!invocation.SpanContainsDirectives())
                                        OptimizeLinqMethodCallAnalysis.AnalyzeOfType(context, invocationInfo);

                                    break;
                                }
                            case "ToCharArray":
                                {
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
                                    SimplifyLogicalNegationAnalyzer.Analyze(context, invocationInfo);

                                    if (!invocation.SpanContainsDirectives())
                                        OptimizeLinqMethodCallAnalysis.AnalyzeWhereAndAny(context, invocationInfo);

                                    break;
                                }
                            case "ElementAt":
                                {
                                    if (!invocationInfo.Expression.IsKind(SyntaxKind.InvocationExpression)
                                        && UseElementAccessAnalysis.IsFixableElementAt(invocationInfo, context.SemanticModel, context.CancellationToken))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeLinqMethodCall, Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationInfo.ArgumentList.Span.End)));
                                    }

                                    break;
                                }
                            case "FirstOrDefault":
                                {
                                    OptimizeLinqMethodCallAnalysis.AnalyzeFirstOrDefault(context, invocationInfo);
                                    break;
                                }
                            case "GetValueOrDefault":
                                {
                                    if (invocationInfo.Name.IsKind(SyntaxKind.IdentifierName)
                                        && !invocation.IsParentKind(SyntaxKind.InvocationExpression, SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression)
                                        && context.SemanticModel
                                            .GetMethodSymbol(invocationInfo.InvocationExpression, context.CancellationToken)?
                                            .ContainingType
                                            .OriginalDefinition
                                            .SpecialType == SpecialType.System_Nullable_T)
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.UseCoalesceExpression, invocationInfo.Name);
                                    }

                                    break;
                                }
                            case "Where":
                                {
                                    CombineEnumerableWhereMethodChainAnalysis.Analyze(context, invocationInfo);
                                    break;
                                }
                            case "HasFlag":
                                {
                                    if (!invocation.SpanContainsDirectives()
                                        && UseBitwiseOperationInsteadOfCallingHasFlagAnalysis.IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag, invocation);
                                    }

                                    break;
                                }
                            case "Select":
                                {
                                    CallCastInsteadOfSelectAnalysis.Analyze(context, invocationInfo);
                                    break;
                                }
                            case "OrderBy":
                                {
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
                                    if (!invocation.SpanContainsDirectives())
                                        UseRegexInstanceInsteadOfStaticMethodAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "Select":
                                {
                                    CallCastInsteadOfSelectAnalysis.Analyze(context, invocationInfo);
                                    break;
                                }
                            case "OrderBy":
                                {
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
                                    if (!invocation.SpanContainsDirectives())
                                        UseRegexInstanceInsteadOfStaticMethodAnalysis.Analyze(context, invocationInfo);

                                    break;
                                }
                            case "OrderBy":
                                {
                                    CallThenByInsteadOfOrderByAnalysis.Analyze(context, invocationInfo);
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
                                    if (!invocation.SpanContainsDirectives())
                                        UseRegexInstanceInsteadOfStaticMethodAnalysis.Analyze(context, invocationInfo);

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
                        if (argumentCount == 0
                            || argumentCount == 1
                            || argumentCount == 2)
                        {
                            AvoidNullReferenceExceptionAnalyzer.Analyze(context, invocationInfo);
                        }

                        break;
                    }
                case "Append":
                case "AppendLine":
                case "AppendFormat":
                case "Insert":
                    {
                        OptimizeStringBuilderAppendCallAnalysis.Analyze(context, invocationInfo);
                        break;
                    }
                case "Join":
                    {
                        if (argumentCount >= 2)
                            CallStringConcatInsteadOfStringJoinAnalysis.Analyze(context, invocationInfo);

                        break;
                    }
            }

            if (UseMethodChainingAnalysis.IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.UseMethodChaining, invocationInfo.InvocationExpression);
        }
    }
}
