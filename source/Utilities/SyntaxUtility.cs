// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator
{
    public static class SyntaxUtility
    {
        public static SymbolDisplayFormat DefaultSymbolDisplayFormat { get; } = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static bool AreParenthesesRedundantOrInvalid(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
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

        public static bool IsUsingStaticDirectiveInScope(
            SyntaxNode node,
            INamedTypeSymbol namedTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return UsingsInScope(node)
                .Any(usings => ContainsUsingStatic(usings, namedTypeSymbol, semanticModel, cancellationToken));
        }

        public static bool IsUsingDirectiveInScope(
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

            return UsingsInScope(node)
                .Any(usings => ContainsUsing(usings, namespaceSymbol, semanticModel, cancellationToken));
        }

        private static IEnumerable<SyntaxList<UsingDirectiveSyntax>> UsingsInScope(SyntaxNode node)
        {
            while (node != null)
            {
                if (node.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    yield return ((NamespaceDeclarationSyntax)node).Usings;
                }
                else if (node.IsKind(SyntaxKind.CompilationUnit))
                {
                    yield return ((CompilationUnitSyntax)node).Usings;
                }

                node = node.Parent;
            }
        }

        private static bool ContainsUsingStatic(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            INamedTypeSymbol namedTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (UsingDirectiveSyntax usingDirective in usingDirectives)
            {
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                {
                    ISymbol symbol = semanticModel
                        .GetSymbolInfo(usingDirective.Name, cancellationToken)
                        .Symbol;

                    if (symbol?.IsErrorType() == false
                        && namedTypeSymbol.Equals(symbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool ContainsUsing(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            INamespaceSymbol namespaceSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (UsingDirectiveSyntax usingDirective in usingDirectives)
            {
                if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    && usingDirective.Alias == null)
                {
                    ISymbol symbol = semanticModel
                        .GetSymbolInfo(usingDirective.Name, cancellationToken)
                        .Symbol;

                    if (symbol?.IsNamespace() == true
                        && string.Equals(namespaceSymbol.ToString(), symbol.ToString(), StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static SyntaxTriviaList GetIndentTrivia(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList triviaList = GetNodeForLeadingTrivia(node).GetLeadingTrivia();

            return SyntaxFactory.TriviaList(
                triviaList
                    .Reverse()
                    .TakeWhile(f => f.IsKind(SyntaxKind.WhitespaceTrivia)));
        }

        private static SyntaxNode GetNodeForLeadingTrivia(this SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.AncestorsAndSelf())
            {
                if (ancestor.IsKind(SyntaxKind.IfStatement))
                {
                    var parentElse = ancestor.Parent as ElseClauseSyntax;

                    return parentElse ?? ancestor;
                }
                else if (ancestor.IsMemberDeclaration())
                {
                    return ancestor;
                }
                else if (ancestor.IsStatement())
                {
                    return ancestor;
                }
            }

            return node;
        }

        public static IEnumerable<TNode> FindNodes<TNode>(
            SyntaxNode root,
            IEnumerable<ReferencedSymbol> referencedSymbols) where TNode : SyntaxNode
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            if (referencedSymbols == null)
                throw new ArgumentNullException(nameof(referencedSymbols));

            foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (referenceLocation.IsCandidateLocation)
                        continue;

                    TNode identifierName = root
                        .FindNode(referenceLocation.Location.SourceSpan, getInnermostNodeForTie: true)
                        .FirstAncestorOrSelf<TNode>();

                    if (identifierName != null)
                        yield return identifierName;
                }
            }
        }

        public static ExpressionSyntax CreateDefaultValue(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return CreateDefaultValue(typeSymbol, default(TypeSyntax), f =>
            {
                return Type(typeSymbol)
                    .WithSimplifierAnnotation();
            });
        }

        public static ExpressionSyntax CreateDefaultValue(ITypeSymbol typeSymbol, TypeSyntax type)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return CreateDefaultValue(typeSymbol, type, null);
        }

        private static ExpressionSyntax CreateDefaultValue(ITypeSymbol typeSymbol, TypeSyntax type, Func<ITypeSymbol, TypeSyntax> typeFactory)
        {
            if (typeSymbol.IsErrorType())
                return null;

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return FalseLiteralExpression();
                case SpecialType.System_Char:
                    return CharacterLiteralExpression('\0');
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
                    return ZeroLiteralExpression();
            }

            if (typeSymbol.Kind == SymbolKind.NamedType
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
            {
                return NullLiteralExpression();
            }

            if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Enum)
            {
                IFieldSymbol fieldSymbol = GetDefaultEnumMember(typeSymbol);

                if (fieldSymbol != null)
                {
                    if (type == null)
                    {
                        type = typeFactory(typeSymbol);

                        if (type == null)
                            return null;
                    }

                    return SimpleMemberAccessExpression(type, IdentifierName(fieldSymbol.Name));
                }
                else
                {
                    return ZeroLiteralExpression();
                }
            }

            if (typeSymbol.IsValueType)
            {
                if (type == null)
                {
                    type = typeFactory(typeSymbol);

                    if (type == null)
                        return null;
                }

                return DefaultExpression(type);
            }

            return NullLiteralExpression();
        }

        private static IFieldSymbol GetDefaultEnumMember(ITypeSymbol typeSymbol)
        {
            foreach (ISymbol member in typeSymbol.GetMembers())
            {
                if (member.IsField())
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue
                        && fieldSymbol.ConstantValue is int
                        && (int)fieldSymbol.ConstantValue == 0)
                    {
                        return fieldSymbol;
                    }
                }
            }

            return null;
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

        public static bool ContainsEmbeddableUsingStatement(UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            StatementSyntax statement = usingStatement.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;
                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1
                    && statements[0].IsKind(SyntaxKind.UsingStatement))
                {
                    var usingStatement2 = (UsingStatementSyntax)statements[0];

                    return block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && usingStatement2.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && usingStatement2.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia());
                }
            }

            return false;
        }
    }
}
