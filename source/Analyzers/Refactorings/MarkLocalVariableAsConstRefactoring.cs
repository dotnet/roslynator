// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkLocalVariableAsConstRefactoring
    {
        public static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            if (!localDeclaration.IsConst)
            {
                StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(localDeclaration);
                if (statementsInfo.Success)
                {
                    SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

                    if (statements.Count > 1)
                    {
                        int index = statements.IndexOf(localDeclaration);

                        if (index < statements.Count - 1)
                        {
                            VariableDeclarationSyntax variableDeclaration = localDeclaration.Declaration;

                            if (variableDeclaration?.IsMissing == false)
                            {
                                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

                                if (variables.Any())
                                {
                                    TypeSyntax type = variableDeclaration.Type;
                                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                                    if (typeSymbol?.SupportsConstantValue() == true
                                        && variables.All(variable => HasConstantValue(variable.Initializer?.Value, typeSymbol, semanticModel, cancellationToken)))
                                    {
                                        TextSpan span = TextSpan.FromBounds(statements[index + 1].Span.Start, statements.Last().Span.End);
                                        IEnumerable<SyntaxNode> nodes = statementsInfo.Node.DescendantNodes(span);

                                        if (!IsAnyVariableInvalidOrAssigned(
                                            variables,
                                            nodes,
                                            semanticModel,
                                            cancellationToken))
                                        {
                                            context.ReportDiagnostic(
                                                DiagnosticDescriptors.MarkLocalVariableAsConst,
                                                type);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsAnyVariableInvalidOrAssigned(SeparatedSyntaxList<VariableDeclaratorSyntax> variables, IEnumerable<SyntaxNode> nodes, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            List<ISymbol> localSymbols = null;

            foreach (SyntaxNode node in nodes)
            {
                if (node.IsKind(SyntaxKind.IdentifierName))
                {
                    ISymbol symbol = semanticModel.GetSymbol(node, cancellationToken);

                    if (symbol?.IsLocal() == true)
                    {
                        if (localSymbols == null)
                        {
                            localSymbols = new List<ISymbol>();

                            foreach (VariableDeclaratorSyntax variable in variables)
                            {
                                ISymbol localSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

                                if (localSymbol?.IsLocal() == true)
                                {
                                    localSymbols.Add(localSymbol);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }

                        if (localSymbols.Contains(symbol))
                        {
                            ExpressionSyntax expression = GetAssignedExpression(node);

                            if (expression != null)
                            {
                                ISymbol expressionSymbol = semanticModel.GetSymbol(expression, cancellationToken);

                                if (localSymbols.Any(localSymbol => localSymbol.Equals(expressionSymbol)))
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool HasConstantValue(
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression?.IsMissing == false)
            {
                switch (typeSymbol.SpecialType)
                {
                    case SpecialType.System_Boolean:
                        {
                            if (expression.Kind().IsBooleanLiteralExpression())
                                return true;

                            break;
                        }
                    case SpecialType.System_Char:
                        {
                            if (expression.IsKind(SyntaxKind.CharacterLiteralExpression))
                                return true;

                            break;
                        }
                    case SpecialType.System_SByte:
                    case SpecialType.System_Byte:
                    case SpecialType.System_Int16:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Decimal:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                        {
                            if (expression.IsKind(SyntaxKind.NumericLiteralExpression))
                                return true;

                            break;
                        }
                    case SpecialType.System_String:
                        {
                            if (expression.IsKind(SyntaxKind.StringLiteralExpression))
                                return true;

                            break;
                        }
                }

                return semanticModel.GetConstantValue(expression, cancellationToken).HasValue;
            }

            return false;
        }

        private static ExpressionSyntax GetAssignedExpression(SyntaxNode node)
        {
            for (node = node.Parent; node != null; node = node.Parent)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.SimpleAssignmentExpression:
                    case SyntaxKind.AddAssignmentExpression:
                    case SyntaxKind.SubtractAssignmentExpression:
                    case SyntaxKind.MultiplyAssignmentExpression:
                    case SyntaxKind.DivideAssignmentExpression:
                    case SyntaxKind.ModuloAssignmentExpression:
                    case SyntaxKind.AndAssignmentExpression:
                    case SyntaxKind.ExclusiveOrAssignmentExpression:
                    case SyntaxKind.OrAssignmentExpression:
                    case SyntaxKind.LeftShiftAssignmentExpression:
                    case SyntaxKind.RightShiftAssignmentExpression:
                        return ((AssignmentExpressionSyntax)node).Left;
                    case SyntaxKind.PreIncrementExpression:
                    case SyntaxKind.PreDecrementExpression:
                        return ((PrefixUnaryExpressionSyntax)node).Operand;
                    case SyntaxKind.PostIncrementExpression:
                    case SyntaxKind.PostDecrementExpression:
                        return ((PostfixUnaryExpressionSyntax)node).Operand;
                    case SyntaxKind.Argument:
                        {
                            var argument = (ArgumentSyntax)node;

                            if (argument.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword, SyntaxKind.OutKeyword))
                                return argument.Expression;

                            break;
                        }
                    case SyntaxKind.Block:
                        return null;
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclaration.Declaration;
            TypeSyntax type = variableDeclaration.Type;

            LocalDeclarationStatementSyntax newNode = localDeclaration;

            if (type.IsVar)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, localDeclaration.SpanStart);

                newNode = newNode.ReplaceNode(type, newType.WithTriviaFrom(type));
            }

            Debug.Assert(!newNode.Modifiers.Any(), newNode.Modifiers.ToString());

            if (newNode.Modifiers.Any())
            {
                newNode = newNode.InsertModifier(SyntaxKind.ConstKeyword, ModifierComparer.Instance);
            }
            else
            {
                newNode = newNode
                    .WithoutLeadingTrivia()
                    .WithModifiers(TokenList(ConstKeyword().WithLeadingTrivia(newNode.GetLeadingTrivia())));
            }

            return await document.ReplaceNodeAsync(localDeclaration, newNode).ConfigureAwait(false);
        }
    }
}
