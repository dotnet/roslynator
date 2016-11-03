// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    public static class ForEachStatementAnalysis
    {
        public static TypeAnalysisResult AnalyzeType(
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (forEachStatement.Type == null)
                return TypeAnalysisResult.None;

            if (!forEachStatement.Type.IsVar)
                return TypeAnalysisResult.Explicit;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(forEachStatement.Type, cancellationToken).Type;

            if (typeSymbol?.SupportsExplicitDeclaration() == true)
                return TypeAnalysisResult.ImplicitButShouldBeExplicit;

            return TypeAnalysisResult.Implicit;
        }

        public static bool HasParenthesesOnSameLine(ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            TextSpan textSpan = TextSpan.FromBounds(
                forEachStatement.OpenParenToken.Span.Start,
                forEachStatement.CloseParenToken.Span.End);

            return !forEachStatement
                .DescendantTrivia(textSpan)
                .ContainsEndOfLine();
        }
    }
}
