// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class OptimizeStringBuilderAppendCallAnalysis
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            INamedTypeSymbol stringBuilderSymbol = context.GetTypeByMetadataName(MetadataNames.System_Text_StringBuilder);

            if (stringBuilderSymbol == null)
                return;

            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            IMethodSymbol methodSymbol = context.SemanticModel.GetMethodSymbol(invocationExpression, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (methodSymbol.IsExtensionMethod)
                return;

            if (methodSymbol.ContainingType?.Equals(stringBuilderSymbol) != true)
                return;

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            int parameterCount = parameters.Length;

            if (parameterCount == 0)
            {
                if (methodSymbol.IsName("AppendLine"))
                {
                    SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

                    if (invocationInfo2.Success
                        && invocationInfo2.NameText == "Append"
                        && invocationInfo2.Arguments.Count == 1)
                    {
                        IMethodSymbol methodInfo2 = context.SemanticModel.GetMethodSymbol(invocationInfo2.InvocationExpression, context.CancellationToken);

                        if (methodInfo2?.IsStatic == false
                            && methodInfo2.ContainingType?.Equals(stringBuilderSymbol) == true
                            && methodInfo2.HasSingleParameter(SpecialType.System_String))
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, invocationInfo.Name, methodSymbol.Name);
                        }
                    }
                }
            }
            else if (parameterCount == 1)
            {
                if (methodSymbol.IsName("Append", "AppendLine"))
                {
                    ArgumentSyntax argument = invocationInfo.Arguments.SingleOrDefault(shouldThrow: false);

                    if (argument != null)
                    {
                        ExpressionSyntax expression = argument.Expression;

                        SyntaxKind expressionKind = expression.Kind();

                        switch (expressionKind)
                        {
                            case SyntaxKind.InterpolatedStringExpression:
                                {
                                    context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument, methodSymbol.Name);
                                    return;
                                }
                            case SyntaxKind.AddExpression:
                                {
                                    BinaryExpressionInfo binaryExpressionInfo = SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)expression);

                                    if (binaryExpressionInfo.Success
                                        && binaryExpressionInfo.IsStringConcatenation(context.SemanticModel, context.CancellationToken))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument, methodSymbol.Name);
                                        return;
                                    }

                                    break;
                                }
                            default:
                                {
                                    if (expressionKind == SyntaxKind.InvocationExpression
                                        && IsFixable((InvocationExpressionSyntax)expression, context.SemanticModel, context.CancellationToken))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument, methodSymbol.Name);
                                        return;
                                    }

                                    if (methodSymbol.IsName("Append")
                                        && parameterCount == 1
                                        && parameters[0].Type.IsObject()
                                        && context.SemanticModel.GetTypeSymbol(argument.Expression, context.CancellationToken).IsValueType)
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, argument);
                                        return;
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
            else if (parameterCount == 2)
            {
                if (methodSymbol.IsName("Insert")
                    && parameters[0].Type.SpecialType == SpecialType.System_Int32
                    && parameters[1].Type.SpecialType == SpecialType.System_Object)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                    if (arguments.Count == 2
                        && context.SemanticModel
                            .GetTypeSymbol(arguments[1].Expression, context.CancellationToken)
                            .IsValueType)
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, arguments[1]);
                    }
                }
            }
        }

        private static bool IsFixable(InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            if (!invocationInfo.Success)
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocationInfo.InvocationExpression, cancellationToken);

            if (methodSymbol == null)
                return false;

            if (!methodSymbol.IsContainingType(SpecialType.System_String))
                return false;

            if (!methodSymbol.IsReturnType(SpecialType.System_String))
                return false;

            switch (methodSymbol.Name)
            {
                case "Substring":
                    {
                        if (methodSymbol.HasTwoParameters(SpecialType.System_Int32, SpecialType.System_Int32))
                            return true;

                        break;
                    }
                case "Remove":
                    {
                        if (methodSymbol.HasSingleParameter(SpecialType.System_Int32))
                            return true;

                        break;
                    }
                case "Format":
                    {
                        return true;
                    }
            }

            return false;
        }
    }
}