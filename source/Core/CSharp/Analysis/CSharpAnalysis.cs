// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class CSharpAnalysis
    {
        public static TypeAnalysisFlags AnalyzeType(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (variableDeclaration == null)
                throw new ArgumentNullException(nameof(variableDeclaration));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = variableDeclaration.Type;

            if (type != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

                if (variables.Count > 0
                    && !variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration))
                {
                    ExpressionSyntax expression = variables[0].Initializer?.Value;

                    if (expression != null)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                        if (typeSymbol?.IsErrorType() == false)
                        {
                            TypeAnalysisFlags flags;

                            if (typeSymbol.IsDynamicType())
                            {
                                flags = TypeAnalysisFlags.Dynamic;
                            }
                            else
                            {
                                flags = TypeAnalysisFlags.ValidSymbol;

                                if (type.IsVar)
                                {
                                    flags |= TypeAnalysisFlags.Implicit;

                                    if (typeSymbol.SupportsExplicitDeclaration())
                                        flags |= TypeAnalysisFlags.SupportsExplicit;
                                }
                                else
                                {
                                    flags |= TypeAnalysisFlags.Explicit;

                                    if (variables.Count == 1
                                        && !IsLocalConstDeclaration(variableDeclaration)
                                        && !expression.IsKind(SyntaxKind.NullLiteralExpression)
                                        && typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken)))
                                    {
                                        flags |= TypeAnalysisFlags.SupportsImplicit;
                                    }
                                }

                                if (IsTypeObvious(expression, semanticModel, cancellationToken))
                                    flags |= TypeAnalysisFlags.TypeObvious;
                            }

                            return flags;
                        }
                    }
                }
            }

            return TypeAnalysisFlags.None;
        }

        public static TypeAnalysisFlags AnalyzeType(
            DeclarationExpressionSyntax declarationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (declarationExpression == null)
                throw new ArgumentNullException(nameof(declarationExpression));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = declarationExpression.Type;

            if (type != null)
            {
                VariableDesignationSyntax designation = declarationExpression.Designation;

                if (designation?.IsKind(SyntaxKind.SingleVariableDesignation) == true)
                {
                    var symbol = semanticModel.GetDeclaredSymbol((SingleVariableDesignationSyntax)designation, cancellationToken) as ILocalSymbol;

                    if (symbol?.IsErrorType() == false)
                    {
                        ITypeSymbol typeSymbol = symbol.Type;

                        if (typeSymbol?.IsErrorType() == false)
                        {
                            TypeAnalysisFlags flags;

                            if (typeSymbol.IsDynamicType())
                            {
                                flags = TypeAnalysisFlags.Dynamic;
                            }
                            else
                            {
                                flags = TypeAnalysisFlags.ValidSymbol;

                                if (type.IsVar)
                                {
                                    flags |= TypeAnalysisFlags.Implicit;

                                    if (symbol.Type.SupportsExplicitDeclaration())
                                        flags |= TypeAnalysisFlags.SupportsExplicit;
                                }
                                else
                                {
                                    flags |= TypeAnalysisFlags.Explicit;
                                    flags |= TypeAnalysisFlags.SupportsImplicit;
                                }
                            }

                            return flags;
                        }
                    }
                }
            }

            return TypeAnalysisFlags.None;
        }

        private static bool IsLocalConstDeclaration(VariableDeclarationSyntax variableDeclaration)
        {
            SyntaxNode parent = variableDeclaration.Parent;

            return parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true
                && ((LocalDeclarationStatementSyntax)parent).IsConst;
        }

        private static bool IsTypeObvious(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.DefaultExpression:
                    {
                        return true;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        return semanticModel.GetSymbol(expression, cancellationToken)?.IsEnumField() == true;
                    }
            }

            return false;
        }

        public static TypeAnalysisFlags AnalyzeType(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = forEachStatement.Type;

            if (type != null)
            {
                ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

                ITypeSymbol typeSymbol = info.ElementType;

                if (typeSymbol?.IsErrorType() == false)
                {
                    TypeAnalysisFlags flags;

                    if (typeSymbol.IsDynamicType())
                    {
                        flags = TypeAnalysisFlags.Dynamic;
                    }
                    else
                    {
                        flags = TypeAnalysisFlags.ValidSymbol;

                        if (type.IsVar)
                        {
                            flags |= TypeAnalysisFlags.Implicit;

                            if (typeSymbol.SupportsExplicitDeclaration())
                                flags |= TypeAnalysisFlags.SupportsExplicit;
                        }
                        else
                        {
                            flags |= TypeAnalysisFlags.Explicit;

                            if (info.ElementConversion.IsIdentity)
                                flags |= TypeAnalysisFlags.SupportsImplicit;
                        }
                    }

                    return flags;
                }
            }

            return TypeAnalysisFlags.None;
        }

        public static BracesAnalysisResult AnalyzeBraces(SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count > 1)
            {
                return BracesAnalysisResult.AddBraces;
            }
            else if (statements.Count == 1)
            {
                if (statements[0].IsKind(SyntaxKind.Block))
                {
                    return BracesAnalysisResult.RemoveBraces;
                }
                else
                {
                    return BracesAnalysisResult.AddBraces;
                }
            }

            return BracesAnalysisResult.None;
        }

        public static BracesAnalysisResult AnalyzeBraces(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            bool anyHasEmbedded = false;
            bool anyHasBlock = false;
            bool allSupportsEmbedded = true;

            int cnt = 0;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.GetChain())
            {
                cnt++;

                StatementSyntax statement = ifOrElse.Statement;

                if (!anyHasEmbedded && !statement.IsKind(SyntaxKind.Block))
                    anyHasEmbedded = true;

                if (!anyHasBlock && statement.IsKind(SyntaxKind.Block))
                    anyHasBlock = true;

                if (allSupportsEmbedded && !SupportsEmbedded(statement))
                    allSupportsEmbedded = false;

                if (cnt > 1 && anyHasEmbedded && !allSupportsEmbedded)
                {
                    return BracesAnalysisResult.AddBraces;
                }
            }

            if (cnt > 1
                && allSupportsEmbedded
                && anyHasBlock)
            {
                if (anyHasEmbedded)
                {
                    return BracesAnalysisResult.AddBraces | BracesAnalysisResult.RemoveBraces;
                }
                else
                {
                    return BracesAnalysisResult.RemoveBraces;
                }
            }

            return BracesAnalysisResult.None;
        }

        private static bool SupportsEmbedded(StatementSyntax statement)
        {
            if (statement.IsParentKind(SyntaxKind.IfStatement)
                && ((IfStatementSyntax)statement.Parent).Condition?.IsMultiLine() == true)
            {
                return false;
            }

            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                if (block.Statements.Count != 1)
                    return false;

                statement = block.Statements[0];
            }

            return !statement.IsKind(SyntaxKind.LocalDeclarationStatement)
                && !statement.IsKind(SyntaxKind.LabeledStatement)
                && statement.IsSingleLine();
        }
    }
}
