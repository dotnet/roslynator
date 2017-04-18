// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsePredefinedTypeRefactoring
    {
        public static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            var identifierName = (IdentifierNameSyntax)context.Node;

            if (!identifierName.IsVar
                && !identifierName.IsParentKind(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.QualifiedName,
                    SyntaxKind.UsingDirective))
            {
                var typeSymbol = context.SemanticModel.GetSymbol(identifierName, context.CancellationToken) as ITypeSymbol;

                if (typeSymbol?.SupportsPredefinedType() == true)
                {
                    IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(identifierName, context.CancellationToken);

                    if (aliasSymbol == null)
                        ReportDiagnostic(context, identifierName);
                }
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, QualifiedNameSyntax qualifiedName)
        {
            if (!qualifiedName.IsParentKind(SyntaxKind.UsingDirective))
            {
                var typeSymbol = context.SemanticModel.GetSymbol(qualifiedName, context.CancellationToken) as ITypeSymbol;

                if (typeSymbol?.SupportsPredefinedType() == true)
                {
                    ReportDiagnostic(context, qualifiedName);
                }
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (!memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                ExpressionSyntax expression = memberAccess.Expression;

                if (expression?.IsKind(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.IdentifierName) == true)
                {
                    var typeSymbol = context.SemanticModel.GetSymbol(expression, context.CancellationToken) as ITypeSymbol;

                    if (typeSymbol?.SupportsPredefinedType() == true)
                    {
                        IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(expression, context.CancellationToken);

                        if (aliasSymbol == null)
                            ReportDiagnostic(context, expression);
                    }
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.UsePredefinedType, expression);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax newType = typeSymbol.ToTypeSyntax()
                .WithTriviaFrom(node)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newType, cancellationToken);
        }
    }
}
