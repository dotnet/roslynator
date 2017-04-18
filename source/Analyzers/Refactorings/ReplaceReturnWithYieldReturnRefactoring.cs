// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceReturnWithYieldReturnRefactoring
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, ReturnStatementSyntax returnStatement)
        {
            ExpressionSyntax expression = returnStatement.Expression;

            if (expression != null)
            {
                ISymbol containingSymbol = context.ContainingSymbol;

                if (containingSymbol?.IsKind(SymbolKind.Method) == true)
                {
                    var methodSymbol = (IMethodSymbol)containingSymbol;

                    ITypeSymbol returnType = methodSymbol.ReturnType;

                    if (returnType?.IsIEnumerableOrConstructedFromIEnumerableOfT() == true)
                    {
                        switch (GetReplacementKind(returnStatement, containingSymbol, context.SemanticModel, context.CancellationToken))
                        {
                            case SyntaxKind.YieldReturnStatement:
                                {
                                    context.ReportDiagnostic(DiagnosticDescriptors.ReplaceReturnWithYieldReturn, returnStatement);
                                    break;
                                }
                            case SyntaxKind.ForEachStatement:
                                {
                                    if (!returnStatement.ContainsDirectives)
                                        context.ReportDiagnostic(DiagnosticDescriptors.ReplaceReturnWithYieldReturn, returnStatement);

                                    break;
                                }
                        }
                    }
                }
            }
        }

        private static SyntaxKind GetReplacementKind(
            ReturnStatementSyntax returnStatement,
            ISymbol containingSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxToken returnKeyword = returnStatement.ReturnKeyword;
            ExpressionSyntax expression = returnStatement.Expression;

            if (semanticModel.ContainsCompilerDiagnostic(CSharpErrorCodes.CannotImplicitlyConvertType, expression.Span, cancellationToken))
            {
                return SyntaxKind.YieldReturnStatement;
            }
            else if (semanticModel.ContainsCompilerDiagnostic(CSharpErrorCodes.CannotReturnValueFromIterator, returnKeyword.Span, cancellationToken))
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

                containingSymbol = containingSymbol ?? semanticModel.GetEnclosingSymbol(returnStatement.SpanStart, cancellationToken);

                if (containingSymbol?.IsKind(SymbolKind.Method) == true)
                {
                    var methodSymbol = (IMethodSymbol)containingSymbol;

                    ITypeSymbol returnType = methodSymbol.ReturnType;

                    if (returnType.SpecialType == SpecialType.System_Collections_IEnumerable)
                    {
                        if (typeSymbol.IsIEnumerableOrConstructedFromIEnumerableOfT())
                        {
                            return SyntaxKind.ForEachStatement;
                        }
                        else
                        {
                            return SyntaxKind.YieldReturnStatement;
                        }
                    }
                    else if (returnType.IsNamedType())
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)returnType;

                        if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                        {
                            if (semanticModel
                                .ClassifyConversion(expression, namedTypeSymbol.TypeArguments[0])
                                .IsImplicit)
                            {
                                return SyntaxKind.YieldReturnStatement;
                            }
                            else
                            {
                                return SyntaxKind.ForEachStatement;
                            }
                        }
                    }
                }
            }

            return SyntaxKind.None;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            SyntaxToken returnKeyword = returnStatement.ReturnKeyword;
            ExpressionSyntax expression = returnStatement.Expression;

            switch (GetReplacementKind(returnStatement, null, semanticModel, cancellationToken))
            {
                case SyntaxKind.YieldReturnStatement:
                    {
                        YieldStatementSyntax yieldReturnStatement = YieldStatement(
                            SyntaxKind.YieldReturnStatement,
                            Token(returnKeyword.LeadingTrivia, SyntaxKind.YieldKeyword, TriviaList(Space)),
                            returnKeyword.WithoutLeadingTrivia(),
                            expression,
                            returnStatement.SemicolonToken);

                        return await document.ReplaceNodeAsync(returnStatement, yieldReturnStatement, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        string name = NameGenerator.Default.EnsureUniqueLocalName(
                            DefaultNames.ForEachVariable,
                            semanticModel,
                            returnStatement.SpanStart,
                            cancellationToken: cancellationToken);

                        YieldStatementSyntax yieldReturnStatement = YieldStatement(
                            SyntaxKind.YieldReturnStatement,
                            Token(default(SyntaxTriviaList), SyntaxKind.YieldKeyword, TriviaList(Space)),
                            returnKeyword.WithoutLeadingTrivia(),
                            IdentifierName(name),
                            returnStatement.SemicolonToken.WithoutTrailingTrivia());

                        StatementSyntax newNode = ForEachStatement(
                            VarType(),
                            name,
                            expression,
                            Block(yieldReturnStatement));

                        if (EmbeddedStatementHelper.IsEmbeddedStatement(returnStatement))
                            newNode = Block(newNode);

                        newNode = newNode.WithTriviaFrom(returnStatement);

                        return await document.ReplaceNodeAsync(returnStatement, newNode, cancellationToken).ConfigureAwait(false);
                    }
                default:
                    {
                        Debug.Assert(false, "");
                        return document;
                    }
            }
        }
    }
}
