// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public static bool AreParenthesesRedundantOrInvalid(ExpressionSyntax expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            switch (expression.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                    return true;
            }

            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.QualifiedName:
                case SyntaxKind.SimpleMemberAccessExpression:
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

                        return expression == forEachStatement.Expression
                            || expression == forEachStatement.Type;
                    }
                case SyntaxKind.WhileStatement:
                    return expression == ((WhileStatementSyntax)parent).Condition;
                case SyntaxKind.DoStatement:
                    return expression == ((DoStatementSyntax)parent).Condition;
                case SyntaxKind.UsingStatement:
                    return expression == ((UsingStatementSyntax)parent).Expression;
                case SyntaxKind.LockStatement:
                    return expression == ((LockStatementSyntax)parent).Expression;
                case SyntaxKind.IfStatement:
                    return expression == ((IfStatementSyntax)parent).Condition;
                case SyntaxKind.SwitchStatement:
                    return expression == ((SwitchStatementSyntax)parent).Expression;
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        return expression == conditionalExpression.WhenTrue
                            || expression == conditionalExpression.WhenFalse;
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
            }

            return false;
        }

        public static string GetUniqueName(string name, SemanticModel semanticModel, int position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ImmutableArray<ISymbol> symbols = semanticModel.LookupSymbols(position);

            int suffix = 2;

            string newName = name;

            while (symbols.Any(f => string.Equals(f.Name, newName, StringComparison.Ordinal)))
            {
                newName = name + suffix.ToString();
                suffix++;
            }

            return newName;
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

        public static string CreateIdentifier(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return null;

            ITypeSymbol typeSymbol2 = ExtractFromNullableType(typeSymbol);

            ITypeSymbol typeSymbol3 = ExtractFromArrayOrGenericCollection(typeSymbol2);

            string name = GetName(typeSymbol3);

            if (string.IsNullOrEmpty(name))
                return null;

            if (typeSymbol3.TypeKind == TypeKind.Interface
                && name.Length > 1
                && name[0] == 'I')
            {
                name = name.Substring(1);
            }

            if (name.Length > 1
                && UsePlural(typeSymbol2))
            {
                name = TextUtility.RemoveSuffix(name, "Collection");

                if (name.EndsWith("s", StringComparison.Ordinal) || name.EndsWith("x", StringComparison.Ordinal))
                    name += "es";
                else if (name.EndsWith("y", StringComparison.Ordinal))
                    name = name.Remove(name.Length - 1) + "ies";
                else
                    name += "s";
            }

            if (firstCharToLower)
                name = TextUtility.FirstCharToLowerInvariant(name);

            return name;
        }

        private static ITypeSymbol ExtractFromNullableType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsNamedType())
            {
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                    return namedTypeSymbol.TypeArguments[0];
            }

            return typeSymbol;
        }

        private static ITypeSymbol ExtractFromArrayOrGenericCollection(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        return ((IArrayTypeSymbol)typeSymbol).ElementType;
                    }
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                        if (namedTypeSymbol.TypeArguments.Length == 1
                            && namedTypeSymbol.Implements(SpecialType.System_Collections_IEnumerable))
                        {
                            return namedTypeSymbol.TypeArguments[0];
                        }

                        break;
                    }
            }

            return typeSymbol;
        }

        private static bool UsePlural(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        return true;
                    }
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                        if (namedTypeSymbol.TypeArguments.Length <= 1)
                        {
                            ImmutableArray<INamedTypeSymbol> allInterfaces = typeSymbol.AllInterfaces;

                            return allInterfaces.Any(f => f.SpecialType == SpecialType.System_Collections_IEnumerable)
                                && !allInterfaces.Any(f => ImplementsIDictionary(f));
                        }

                        break;
                    }
            }

            return false;
        }

        private static bool ImplementsIDictionary(INamedTypeSymbol namedTypeSymbol)
        {
            return string.Equals(namedTypeSymbol.ContainingNamespace?.ToString(), "System.Collections", StringComparison.Ordinal)
                && string.Equals(namedTypeSymbol.MetadataName, "IDictionary", StringComparison.Ordinal);
        }

        private static string GetName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsTypeParameter())
            {
                if (typeSymbol.Name.Length > 1
                    && typeSymbol.Name[0] == 'T')
                {
                    return typeSymbol.Name.Substring(1);
                }
            }
            else if (typeSymbol.IsAnonymousType)
            {
                return null;
            }
            else if (typeSymbol.IsPredefinedType())
            {
                return null;
            }

            return typeSymbol.Name;
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
    }
}
