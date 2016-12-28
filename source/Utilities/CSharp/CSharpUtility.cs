// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Internal;

namespace Roslynator.CSharp
{
    public static class CSharpUtility
    {
        public static TypeDeclarationSyntax GetEnclosingTypeDeclaration(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.StructDeclaration:
                        return (TypeDeclarationSyntax)ancestor;
                }
            }

            return null;
        }

        public static bool IsIncrementOrDecrementExpression(ExpressionSyntax expression)
        {
            return expression?.IsKind(
                SyntaxKind.PreIncrementExpression,
                SyntaxKind.PreDecrementExpression,
                SyntaxKind.PostIncrementExpression,
                SyntaxKind.PostDecrementExpression) == true;
        }

        public static bool SupportsCompoundAssignment(ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return true;
                default:
                    return false;
            }
        }

        public static ExpressionSyntax LogicallyNegate(ExpressionSyntax booleanExpression)
        {
            if (booleanExpression == null)
                throw new ArgumentNullException(nameof(booleanExpression));

            return LogicalNegationHelper.LogicallyNegate(booleanExpression);
        }

        public static bool IsMethodInsideMethod(SyntaxNode node)
        {
            return node?.IsKind(
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression) == true;
        }

        public static TypeAnalysisResult AnalyzeType(
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
                    && !variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                {
                    ExpressionSyntax expression = variables[0].Initializer?.Value;

                    if (expression != null)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                        if (typeSymbol != null
                            && Symbol.SupportsExplicitDeclaration(typeSymbol))
                        {
                            if (variables.Count > 1
                                || IsLocalConstDeclaration(variableDeclaration.Parent))
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.ImplicitButShouldBeExplicit
                                    : TypeAnalysisResult.None;
                            }
                            else if (IsImplicitTypeAllowed(typeSymbol, expression, semanticModel, cancellationToken))
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.Implicit
                                    : TypeAnalysisResult.ExplicitButShouldBeImplicit;
                            }
                            else
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.ImplicitButShouldBeExplicit
                                    : TypeAnalysisResult.Explicit;
                            }
                        }
                    }
                }
            }

            return TypeAnalysisResult.None;
        }

        private static bool IsLocalConstDeclaration(SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.LocalDeclarationStatement)
                && ((LocalDeclarationStatementSyntax)node).IsConst;
        }

        private static bool IsImplicitTypeAllowed(
            ITypeSymbol typeSymbol,
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
                        return typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken));
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        if (typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken)))
                        {
                            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                            return Symbol.IsEnumField(symbol);
                        }

                        break;
                    }
            }

            return false;
        }

        public static TypeAnalysisResult AnalyzeType(
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = forEachStatement.Type;

            if (type == null)
                return TypeAnalysisResult.None;

            if (!type.IsVar)
                return TypeAnalysisResult.Explicit;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol != null
                && Symbol.SupportsExplicitDeclaration(typeSymbol))
            {
                return TypeAnalysisResult.ImplicitButShouldBeExplicit;
            }

            return TypeAnalysisResult.Implicit;
        }

        public static string GetStringLiteralInnerText(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            string s = literalExpression.Token.Text;

            if (s.StartsWith("@", StringComparison.Ordinal))
            {
                if (s.StartsWith("@\"", StringComparison.Ordinal))
                    s = s.Substring(2);

                if (s.EndsWith("\"", StringComparison.Ordinal))
                    s = s.Remove(s.Length - 1);
            }
            else
            {
                if (s.StartsWith("\"", StringComparison.Ordinal))
                    s = s.Substring(1);

                if (s.EndsWith("\"", StringComparison.Ordinal))
                    s = s.Remove(s.Length - 1);
            }

            return s;
        }

        public static IParameterSymbol DetermineParameter(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return DetermineParameterHelper.DetermineParameter(argument, semanticModel, allowParams, allowParams, cancellationToken);
        }

        public static ImmutableArray<ITypeSymbol> DetermineParameterTypes(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return DetermineParameterHelper.DetermineParameterTypes(argument, semanticModel, cancellationToken);
        }

        public static IParameterSymbol DetermineParameter(
            AttributeArgumentSyntax attributeArgument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return DetermineParameterHelper.DetermineParameter(attributeArgument, semanticModel, allowParams, allowCandidate, cancellationToken);
        }

        public static bool IsNamespaceInScope(
            SyntaxNode node,
            INamespaceSymbol namespaceSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (namespaceSymbol == null)
                throw new ArgumentNullException(nameof(namespaceSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            foreach (NameSyntax name in NamespacesInScope(node))
            {
                ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                if (symbol?.IsNamespace() == true)
                {
                    string namespaceText = namespaceSymbol.ToString();

                    if (string.Equals(namespaceText, symbol.ToString(), StringComparison.Ordinal))
                    {
                        return true;
                    }
                    else if (name.IsParentKind(SyntaxKind.NamespaceDeclaration))
                    {
                        foreach (INamespaceSymbol containingNamespace in symbol.ContainingNamespaces())
                        {
                            if (string.Equals(namespaceText, containingNamespace.ToString(), StringComparison.Ordinal))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static IEnumerable<NameSyntax> NamespacesInScope(SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)ancestor;

                    NameSyntax namespaceName = namespaceDeclaration.Name;

                    if (namespaceName != null)
                        yield return namespaceName;

                    foreach (NameSyntax name in namespaceDeclaration.Usings.Namespaces())
                        yield return name;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    var compilationUnit = (CompilationUnitSyntax)ancestor;

                    foreach (NameSyntax name in compilationUnit.Usings.Namespaces())
                        yield return name;
                }
            }
        }

        public static bool IsStaticClassInScope(
            SyntaxNode node,
            INamedTypeSymbol staticClassSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (staticClassSymbol == null)
                throw new ArgumentNullException(nameof(staticClassSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            foreach (NameSyntax name in StaticClassesInScope(node))
            {
                ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                if (symbol?.Equals(staticClassSymbol) == true)
                    return true;
            }

            return false;
        }

        private static IEnumerable<NameSyntax> StaticClassesInScope(SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)ancestor;

                    foreach (NameSyntax name in namespaceDeclaration.Usings.StaticClasses())
                        yield return name;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    var compilationUnit = (CompilationUnitSyntax)ancestor;

                    foreach (NameSyntax name in compilationUnit.Usings.StaticClasses())
                        yield return name;
                }
            }
        }

        private static IEnumerable<UsingDirectiveSyntax> RegularUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    && usingDirective.Alias == null)
                {
                    yield return usingDirective;
                }
            }
        }

        private static IEnumerable<NameSyntax> Namespaces(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings.RegularUsings())
            {
                NameSyntax name = usingDirective.Name;

                if (name != null)
                    yield return name;
            }
        }

        private static IEnumerable<UsingDirectiveSyntax> StaticUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    yield return usingDirective;
            }
        }

        private static IEnumerable<NameSyntax> StaticClasses(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings.StaticUsings())
            {
                NameSyntax name = usingDirective.Name;

                if (name != null)
                    yield return name;
            }
        }

        private static IEnumerable<UsingDirectiveSyntax> AliasUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (usingDirective.Alias != null)
                    yield return usingDirective;
            }
        }

        public static BaseParameterListSyntax GetParameterList(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).ParameterList;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).ParameterList;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).ParameterList;
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return ((ParenthesizedLambdaExpressionSyntax)node).ParameterList;
                case SyntaxKind.AnonymousMethodExpression:
                    return ((AnonymousMethodExpressionSyntax)node).ParameterList;
                default:
                    return null;
            }
        }

        public static bool IsValidSwitchExpression(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ITypeSymbol typeSymbol = semanticModel.GetConvertedTypeSymbol(expression, cancellationToken);

            if (typeSymbol.IsEnum())
                return true;

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
            }

            if (typeSymbol.IsNamedType())
            {
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                {
                    switch (namedTypeSymbol.ConstructedFrom.TypeArguments.First().SpecialType)
                    {
                        case SpecialType.System_Boolean:
                        case SpecialType.System_Char:
                        case SpecialType.System_SByte:
                        case SpecialType.System_Byte:
                        case SpecialType.System_Int16:
                        case SpecialType.System_UInt16:
                        case SpecialType.System_Int32:
                        case SpecialType.System_UInt32:
                        case SpecialType.System_Int64:
                        case SpecialType.System_UInt64:
                        case SpecialType.System_Single:
                        case SpecialType.System_Double:
                            return true;
                    }
                }
            }

            return false;
        }

        public static int GetOperatorPriority(ExpressionSyntax expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return GetOperatorPriority(expression.Kind());
        }

        public static int GetOperatorPriority(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.TypeOfExpression:
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                    return 1;
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.CastExpression:
                    return 2;
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                    return 3;
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                    return 4;
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return 5;
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                    return 6;
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    return 7;
                case SyntaxKind.BitwiseAndExpression:
                    return 8;
                case SyntaxKind.ExclusiveOrExpression:
                    return 9;
                case SyntaxKind.BitwiseOrExpression:
                    return 10;
                case SyntaxKind.LogicalAndExpression:
                    return 11;
                case SyntaxKind.LogicalOrExpression:
                    return 12;
                case SyntaxKind.CoalesceExpression:
                    return 13;
                case SyntaxKind.ConditionalExpression:
                    return 14;
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
                    return 15;
                default:
                    return 0;
            }
        }

        public static bool AreParenthesesRedundantOrInvalid(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return AreParenthesesRedundantOrInvalidPrivate(node, node.Kind());
        }

        public static bool AreParenthesesRedundantOrInvalid(SyntaxNode node, SyntaxKind replacementKind)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return AreParenthesesRedundantOrInvalidPrivate(node, replacementKind);
        }

        private static bool AreParenthesesRedundantOrInvalidPrivate(SyntaxNode node, SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                    return true;
            }

            SyntaxNode parent = node.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.QualifiedName:
                //case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.TypeArgumentList:
                case SyntaxKind.VariableDeclaration:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.Interpolation:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrowExpressionClause:
                    return true;
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)parent;

                        return node == forEachStatement.Expression
                            || node == forEachStatement.Type;
                    }
                case SyntaxKind.WhileStatement:
                    return node == ((WhileStatementSyntax)parent).Condition;
                case SyntaxKind.DoStatement:
                    return node == ((DoStatementSyntax)parent).Condition;
                case SyntaxKind.UsingStatement:
                    return node == ((UsingStatementSyntax)parent).Expression;
                case SyntaxKind.LockStatement:
                    return node == ((LockStatementSyntax)parent).Expression;
                case SyntaxKind.IfStatement:
                    return node == ((IfStatementSyntax)parent).Condition;
                case SyntaxKind.SwitchStatement:
                    return node == ((SwitchStatementSyntax)parent).Expression;
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        return node == conditionalExpression.WhenTrue
                            || node == conditionalExpression.WhenFalse;
                    }
            }

            if (parent is AssignmentExpressionSyntax)
                return true;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                        return true;
                }
                else if (parent?.IsKind(SyntaxKind.Parameter) == true)
                {
                    return true;
                }
            }

            return false;
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

            foreach (SyntaxNode node in IfElseChain.GetChain(ifStatement))
            {
                cnt++;

                StatementSyntax statement = GetStatement(node);

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
            if (statement.Parent.IsKind(SyntaxKind.IfStatement)
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

        private static StatementSyntax GetStatement(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.IfStatement))
            {
                return ((IfStatementSyntax)node).Statement;
            }
            else
            {
                return ((ElseClauseSyntax)node).Statement;
            }
        }

        public static StatementSyntax GetEmbeddedStatement(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            StatementSyntax statement = GetBlockOrEmbeddedStatement(node);

            if (statement?.IsKind(SyntaxKind.Block) == false)
            {
                return statement;
            }
            else
            {
                return null;
            }
        }

        public static bool IsEmbeddableBlock(BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            if (CanContainEmbeddedStatement(block.Parent))
            {
                SyntaxList<StatementSyntax> statements = block.Statements;

                return statements.Count == 1
                    && !statements[0].IsKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.LabeledStatement);
            }
            else
            {
                return false;
            }
        }

        public static bool IsEmbeddedStatement(StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (!statement.IsKind(SyntaxKind.Block))
            {
                SyntaxNode parent = statement.Parent;

                return CanContainEmbeddedStatement(parent)
                    && (!parent.IsKind(SyntaxKind.ElseClause) || !statement.IsKind(SyntaxKind.IfStatement));
            }
            else
            {
                return false;
            }
        }

        public static bool CanContainEmbeddedStatement(SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.IfStatement:
                case SyntaxKind.ElseClause:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.FixedStatement:
                    return true;
                default:
                    return false;
            }
        }

        public static StatementSyntax GetBlockOrEmbeddedStatement(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)node).Statement;
                case SyntaxKind.ForEachStatement:
                    return ((ForEachStatementSyntax)node).Statement;
                case SyntaxKind.ForStatement:
                    return ((ForStatementSyntax)node).Statement;
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)node).Statement;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)node).Statement;
                case SyntaxKind.LockStatement:
                    return ((LockStatementSyntax)node).Statement;
                case SyntaxKind.FixedStatement:
                    return ((FixedStatementSyntax)node).Statement;
                case SyntaxKind.UsingStatement:
                    {
                        StatementSyntax statement = ((UsingStatementSyntax)node).Statement;
                        if (statement?.IsKind(SyntaxKind.UsingStatement) != true)
                            return statement;

                        break;
                    }
                case SyntaxKind.ElseClause:
                    {
                        var elseClause = (ElseClauseSyntax)node;

                        if (IfElseChain.IsEndOfChain(elseClause))
                            return elseClause.Statement;

                        break;
                    }
            }

            return null;
        }

        public static bool FormatSupportsEmbeddedStatement(SyntaxNode containingNode)
        {
            switch (containingNode.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        return ((IfStatementSyntax)containingNode).Condition?.IsMultiLine() != true;
                    }
                case SyntaxKind.ElseClause:
                    {
                        return true;
                    }
                case SyntaxKind.DoStatement:
                    {
                        return ((DoStatementSyntax)containingNode).Condition?.IsMultiLine() != true;
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)containingNode;

                        return forEachStatement.SyntaxTree.IsSingleLineSpan(forEachStatement.ParenthesesSpan());
                    }
                case SyntaxKind.ForStatement:
                    {
                        var forStatement = (ForStatementSyntax)containingNode;

                        return forStatement.Statement?.IsKind(SyntaxKind.EmptyStatement) == true
                            || forStatement.SyntaxTree.IsSingleLineSpan(forStatement.ParenthesesSpan());
                    }
                case SyntaxKind.UsingStatement:
                    {
                        return ((UsingStatementSyntax)containingNode).DeclarationOrExpression()?.IsMultiLine() != true;
                    }
                case SyntaxKind.WhileStatement:
                    {
                        var whileStatement = (WhileStatementSyntax)containingNode;

                        return whileStatement.Condition?.IsMultiLine() != true
                            || whileStatement.Statement?.IsKind(SyntaxKind.EmptyStatement) == true;
                    }
                case SyntaxKind.LockStatement:
                    {
                        return ((LockStatementSyntax)containingNode).Expression?.IsMultiLine() != true;
                    }
                case SyntaxKind.FixedStatement:
                    {
                        return ((FixedStatementSyntax)containingNode).Declaration?.IsMultiLine() != true;
                    }
                default:
                    {
                        Debug.Assert(false, containingNode.Kind().ToString());
                        return false;
                    }
            }
        }
    }
}
