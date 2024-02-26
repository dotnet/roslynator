// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

internal static class UseStringInterpolationInsteadOfStringConcatAnalysis
{
    internal static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
    {
        ISymbol symbol = context.SemanticModel.GetSymbol(invocationInfo.InvocationExpression, context.CancellationToken);

        if (symbol?.Name == "Concat"
            && symbol.IsStatic
            && symbol.ContainingType.IsString())
        {
            bool? isVerbatim = null;
            var containsNonLiteral = false;

            foreach (ArgumentSyntax argument in invocationInfo.Arguments)
            {
                ExpressionSyntax expression = argument.Expression;

                if (expression.IsKind(SyntaxKind.InterpolatedStringExpression))
                    return;

                if (expression.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    var literalExpression = (LiteralExpressionSyntax)expression;

                    string text = literalExpression.Token.Text;

                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '{'
                            || text[i] == '}')
                        {
                            return;
                        }
                    }

                    if (text.StartsWith("@"))
                    {
                        if (isVerbatim is null)
                        {
                            isVerbatim = true;
                        }
                        else if (isVerbatim == false)
                        {
                            return;
                        }
                    }
                    else if (isVerbatim is null)
                    {
                        isVerbatim = false;
                    }
                    else if (isVerbatim == true)
                    {
                        return;
                    }
                }
                else
                {
                    containsNonLiteral = true;
                }
            }

            if (isVerbatim is not null
                && containsNonLiteral
                && invocationInfo.ArgumentList.IsSingleLine())
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseStringInterpolationInsteadOfStringConcat, invocationInfo.InvocationExpression);
            }
        }
    }
}
