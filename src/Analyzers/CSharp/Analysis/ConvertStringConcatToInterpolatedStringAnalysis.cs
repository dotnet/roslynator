// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

internal static class ConvertStringConcatToInterpolatedStringAnalysis
{
    internal static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
    {
        ISymbol symbol = context.SemanticModel.GetSymbol(invocationInfo.InvocationExpression, context.CancellationToken);

        if (symbol?.Name == "Concat"
            && symbol.IsStatic
            && symbol.ContainingType.IsString())
        {
            var containsLiteral = false;
            foreach (ArgumentSyntax argument in invocationInfo.Arguments)
            {
                ExpressionSyntax expression = argument.Expression;

                if (expression.IsKind(SyntaxKind.InterpolatedStringExpression))
                    return;

                if (expression.IsKind(SyntaxKind.StringLiteralExpression))
                    containsLiteral = true;
            }

            if (containsLiteral
                && invocationInfo.ArgumentList.IsSingleLine())
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.ConvertStringConcatToInterpolatedString, invocationInfo.InvocationExpression);
            }
        }
    }
}
