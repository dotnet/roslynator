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
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class co
    {
        public static bool IsGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.Keyword.IsKind(SyntaxKind.GetKeyword);
        }

        public static bool IsSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.Keyword.IsKind(SyntaxKind.SetKeyword);
        }

        public static bool IsAutoGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return IsAutoAccessor(accessorDeclaration, SyntaxKind.GetKeyword);
        }

        public static bool IsAutoSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return IsAutoAccessor(accessorDeclaration, SyntaxKind.SetKeyword);
        }

        private static bool IsAutoAccessor(this AccessorDeclarationSyntax accessorDeclaration, SyntaxKind kind)
        {
            return accessorDeclaration.Keyword.IsKind(kind)
                && accessorDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && accessorDeclaration.Body == null;
        }

        public static AccessorDeclarationSyntax WithoutSemicolonToken(
            this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.WithSemicolonToken(NoneToken());
        }

        public static AccessorDeclarationSyntax Getter(this AccessorListSyntax accessorList)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            return Accessor(accessorList, SyntaxKind.GetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax Setter(this AccessorListSyntax accessorList)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            return Accessor(accessorList, SyntaxKind.SetAccessorDeclaration);
        }

        public static bool ContainsGetter(this AccessorListSyntax accessorList)
        {
            return Getter(accessorList) != null;
        }

        public static bool ContainsSetter(this AccessorListSyntax accessorList)
        {
            return Setter(accessorList) != null;
        }

        private static AccessorDeclarationSyntax Accessor(this AccessorListSyntax accessorList, SyntaxKind kind)
        {
            return accessorList
                .Accessors
                .FirstOrDefault(accessor => accessor.IsKind(kind));
        }

        public static IParameterSymbol DetermineParameter(
            this ArgumentSyntax argument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return DetermineParameterHelper.DetermineParameter(argument, semanticModel, allowParams, allowParams, cancellationToken);
        }

        public static ImmutableArray<ITypeSymbol> DetermineParameterTypes(
            this ArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return DetermineParameterHelper.DetermineParameterTypes(argument, semanticModel, cancellationToken);
        }

        public static ArgumentSyntax WithoutNameColon(this ArgumentSyntax argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            return argument.WithNameColon(null);
        }

        public static IParameterSymbol DetermineParameter(
            this AttributeArgumentSyntax argument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (argument.NameEquals != null)
                return null;

            if (argument.Parent?.IsKind(SyntaxKind.AttributeArgumentList) != true)
                return null;

            var argumentList = (AttributeArgumentListSyntax)argument.Parent;

            if (argumentList.Parent?.IsKind(SyntaxKind.Attribute) != true)
                return null;

            var attribute = (AttributeSyntax)argument.Parent.Parent;

            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(attribute, cancellationToken);

            ISymbol symbol = symbolInfo.Symbol;

            if (symbol == null
                && allowCandidate
                && symbolInfo.CandidateSymbols.Length > 0)
            {
                symbol = symbolInfo.CandidateSymbols[0];
            }

            if (symbol == null)
                return null;

            ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

            if (argument.NameColon != null && !argument.NameColon.IsMissing)
            {
                string name = argument.NameColon.Name.Identifier.ValueText;

                return parameters.FirstOrDefault(p => p.Name == name);
            }

            int index = argumentList.Arguments.IndexOf(argument);

            if (index < 0)
                return null;

            if (index < parameters.Length)
                return parameters[index];

            if (allowParams)
            {
                IParameterSymbol lastParameter = parameters.LastOrDefault();

                if (lastParameter == null)
                    return null;

                if (lastParameter.IsParams)
                    return lastParameter;
            }

            return null;
        }

        public static AttributeArgumentSyntax WithoutNameColon(this AttributeArgumentSyntax attributeArgument)
        {
            if (attributeArgument == null)
                throw new ArgumentNullException(nameof(attributeArgument));

            return attributeArgument.WithNameColon(null);
        }

        public static IEnumerable<SyntaxToken> CommaTokens(this AttributeListSyntax attributeList)
        {
            if (attributeList == null)
                throw new ArgumentNullException(nameof(attributeList));

            return attributeList
                .ChildTokens()
                .Where(token => token.IsKind(SyntaxKind.CommaToken));
        }

        public static AttributeSyntax WithArgumentList(
            this AttributeSyntax attribute,
            params AttributeArgumentSyntax[] arguments)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            return attribute
                .WithArgumentList(
                    AttributeArgumentList(
                        SeparatedList(arguments)));
        }

        public static AttributeSyntax WithArgumentList(
            this AttributeSyntax attribute,
            AttributeArgumentSyntax argument)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            return attribute
                .WithArgumentList(
                    AttributeArgumentList(
                        SingletonSeparatedList(argument)));
        }

        public static BlockSyntax WithoutStatements(this BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.WithStatements(List<StatementSyntax>());
        }

        public static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(List(memberDeclarations));
        }

        public static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax memberDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(SingletonList(memberDeclaration));
        }

        public static TextSpan HeaderSpan(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return TextSpan.FromBounds(
                classDeclaration.Span.Start,
                classDeclaration.Identifier.Span.End);
        }

        public static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax memberDeclaration)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(SingletonList(memberDeclaration));
        }

        public static CompilationUnitSyntax WithUsings(
            this CompilationUnitSyntax compilationUnit,
            params UsingDirectiveSyntax[] usingDirectives)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithUsings(List(usingDirectives));
        }

        public static CompilationUnitSyntax WithUsings(
            this CompilationUnitSyntax compilationUnit,
            IEnumerable<UsingDirectiveSyntax> usingDirectives)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithUsings(List(usingDirectives));
        }

        public static CompilationUnitSyntax WithNamespace(
            this CompilationUnitSyntax compilationUnit,
            NameSyntax name)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(NamespaceDeclaration(name));
        }

        public static CompilationUnitSyntax WithNamespace(
            this CompilationUnitSyntax compilationUnit,
            string name)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(NamespaceDeclaration(name));
        }

        internal static SyntaxNode AddUsingDirective(this CompilationUnitSyntax compilationUnit, ITypeSymbol typeSymbol)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (typeSymbol.IsNamedType())
            {
                foreach (ITypeSymbol typeSymbol2 in ((INamedTypeSymbol)typeSymbol).GetAllTypeArgumentsAndSelf())
                    compilationUnit = AddUsingDirectivePrivate(compilationUnit, typeSymbol2);

                return compilationUnit;
            }
            else
            {
                return AddUsingDirectivePrivate(compilationUnit, typeSymbol);
            }
        }

        private static CompilationUnitSyntax AddUsingDirectivePrivate(this CompilationUnitSyntax compilationUnit, ITypeSymbol type)
        {
            if (type.ContainingNamespace == null)
                return compilationUnit;

            return AddUsingDirective(compilationUnit, type.ContainingNamespace);
        }

        internal static CompilationUnitSyntax AddUsingDirective(this CompilationUnitSyntax compilationUnit, INamespaceSymbol namespaceSymbol)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (namespaceSymbol == null)
                throw new ArgumentNullException(nameof(namespaceSymbol));

            if (namespaceSymbol.IsGlobalNamespace)
                return compilationUnit;

            UsingDirectiveSyntax usingDirective = UsingDirective(ParseName(namespaceSymbol.ToString()));

            return compilationUnit.AddUsings(usingDirective);
        }

        public static CompilationUnitSyntax AddUsings(this CompilationUnitSyntax compilationUnit, IEnumerable<UsingDirectiveSyntax> usings)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (usings == null)
                throw new ArgumentNullException(nameof(usings));

            return compilationUnit.WithUsings(compilationUnit.Usings.AddRange(usings));
        }

        public static ConstructorDeclarationSyntax WithBody(
            this ConstructorDeclarationSyntax constructorDeclaration,
            IEnumerable<StatementSyntax> statements)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return constructorDeclaration.WithBody(Block(statements));
        }

        public static TextSpan HeaderSpan(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.Span.Start,
                constructorDeclaration.ParameterList?.Span.End ?? constructorDeclaration.Identifier.Span.End);
        }

        public static TextSpan HeaderSpan(this ConversionOperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return TextSpan.FromBounds(
                operatorDeclaration.Span.Start,
                operatorDeclaration.ParameterList?.Span.End
                    ?? operatorDeclaration.Type?.Span.End
                    ?? operatorDeclaration.OperatorKeyword.Span.End);
        }

        public static SyntaxList<AttributeListSyntax> GetAttributeLists(this CSharpSyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.TypeParameter:
                    return ((TypeParameterSyntax)node).AttributeLists;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).AttributeLists;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EnumMemberDeclaration:
                    return ((EnumMemberDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).AttributeLists;
            }

            return default(SyntaxList<AttributeListSyntax>);
        }

        public static CSharpSyntaxNode SetAttributeLists(this CSharpSyntaxNode node, SyntaxList<AttributeListSyntax> attributeLists)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.TypeParameter:
                    return ((TypeParameterSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EnumMemberDeclaration:
                    return ((EnumMemberDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).WithAttributeLists(attributeLists);
            }

            return node;
        }

        public static TextSpan HeaderSpan(this EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return TextSpan.FromBounds(
                eventDeclaration.Span.Start,
                eventDeclaration.Identifier.Span.End);
        }

        public static ParenthesizedExpressionSyntax Parenthesize(this ExpressionSyntax expression, bool cutCopyTrivia = false)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (cutCopyTrivia)
            {
                return ParenthesizedExpression(expression.WithoutTrivia())
                    .WithTriviaFrom(expression);
            }
            else
            {
                return ParenthesizedExpression(expression);
            }
        }

        [DebuggerStepThrough]
        public static ExpressionSyntax UnwrapParentheses(this ExpressionSyntax expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            while (expression.IsKind(SyntaxKind.ParenthesizedExpression))
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;

            return expression;
        }

        public static ExpressionSyntax Negate(this ExpressionSyntax expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return NegateInternal(expression)
                .WithTriviaFrom(expression);
        }

        private static ExpressionSyntax NegateInternal(this ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                    {
                        var parenthesizedExpression = (ParenthesizedExpressionSyntax)expression;

                        return parenthesizedExpression
                            .WithExpression(Negate(parenthesizedExpression.Expression));
                    }
                case SyntaxKind.TrueLiteralExpression:
                    {
                        return LiteralExpression(SyntaxKind.FalseLiteralExpression);
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        return LiteralExpression(SyntaxKind.TrueLiteralExpression);
                    }
                case SyntaxKind.IdentifierName:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, expression);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        return ((PrefixUnaryExpressionSyntax)expression).Operand;
                    }
                case SyntaxKind.LogicalAndExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        return BinaryExpression(
                            SyntaxKind.LogicalOrExpression,
                            binaryExpression.Left?.Negate().WithTriviaFrom(binaryExpression.Left),
                            Token(SyntaxKind.BarBarToken).WithTriviaFrom(binaryExpression.OperatorToken),
                            binaryExpression.Right?.Negate().WithTriviaFrom(binaryExpression.Right));
                    }
                case SyntaxKind.LogicalOrExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        return BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            binaryExpression.Left?.Negate().WithTriviaFrom(binaryExpression.Left),
                            Token(SyntaxKind.AmpersandAmpersandToken).WithTriviaFrom(binaryExpression.OperatorToken),
                            binaryExpression.Right?.Negate().WithTriviaFrom(binaryExpression.Right));
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        return BinaryExpression(
                            NegateExpressionKind(expression.Kind()),
                            binaryExpression.Left,
                            Token(NegateOperatorTokenKind(binaryExpression.OperatorToken.Kind()))
                                .WithTriviaFrom(binaryExpression.OperatorToken),
                            binaryExpression.Right
                        ).WithTriviaFrom(binaryExpression);
                    }
            }

            return PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                ParenthesizedExpression(expression));
        }

        private static SyntaxKind NegateOperatorTokenKind(this SyntaxKind operatorKind)
        {
            switch (operatorKind)
            {
                case SyntaxKind.EqualsEqualsToken:
                    return SyntaxKind.ExclamationEqualsToken;
                case SyntaxKind.ExclamationEqualsToken:
                    return SyntaxKind.EqualsEqualsToken;
                case SyntaxKind.GreaterThanToken:
                    return SyntaxKind.LessThanEqualsToken;
                case SyntaxKind.GreaterThanEqualsToken:
                    return SyntaxKind.LessThanToken;
                case SyntaxKind.LessThanToken:
                    return SyntaxKind.GreaterThanEqualsToken;
                case SyntaxKind.LessThanEqualsToken:
                    return SyntaxKind.GreaterThanToken;
            }

            Debug.Assert(false, operatorKind.ToString());
            return SyntaxKind.None;
        }

        private static SyntaxKind NegateExpressionKind(this SyntaxKind expressionKind)
        {
            switch (expressionKind)
            {
                case SyntaxKind.EqualsExpression:
                    return SyntaxKind.NotEqualsExpression;
                case SyntaxKind.NotEqualsExpression:
                    return SyntaxKind.EqualsExpression;
                case SyntaxKind.GreaterThanExpression:
                    return SyntaxKind.LessThanOrEqualExpression;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return SyntaxKind.LessThanExpression;
                case SyntaxKind.LessThanExpression:
                    return SyntaxKind.GreaterThanOrEqualExpression;
                case SyntaxKind.LessThanOrEqualExpression:
                    return SyntaxKind.GreaterThanExpression;
            }

            Debug.Assert(false, expressionKind.ToString());
            return SyntaxKind.None;
        }

        public static TextSpan HeaderSpan(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return TextSpan.FromBounds(
                indexerDeclaration.Span.Start,
                indexerDeclaration.ParameterList?.Span.End ?? indexerDeclaration.ThisKeyword.Span.End);
        }

        public static AccessorDeclarationSyntax Getter(this IndexerDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration
                .AccessorList?
                .Accessors
                .FirstOrDefault(f => f.IsKind(SyntaxKind.GetAccessorDeclaration));
        }

        public static AccessorDeclarationSyntax Setter(this IndexerDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration
                .AccessorList?
                .Accessors
                .FirstOrDefault(f => f.IsKind(SyntaxKind.SetAccessorDeclaration));
        }

        public static TextSpan HeaderSpan(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return TextSpan.FromBounds(
                interfaceDeclaration.Span.Start,
                interfaceDeclaration.Identifier.Span.End);
        }

        public static InvocationExpressionSyntax WithArgumentList(
            this InvocationExpressionSyntax invocationExpression,
            params ArgumentSyntax[] arguments)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            return invocationExpression.WithArgumentList(ArgumentList(arguments));
        }

        public static InvocationExpressionSyntax WithEmptyArgumentList(this InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            return invocationExpression.WithArgumentList(ArgumentList());
        }

        public static bool IsVerbatimStringLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.Text.StartsWith("@", StringComparison.Ordinal);
        }

        //TODO: 
        public static bool IsTypeDeclaration(this MemberDeclarationSyntax memberDeclaration)
        {
            return SyntaxNodeExtensions.IsKind(memberDeclaration,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.DelegateDeclaration);
        }

        public static SyntaxTokenList GetModifiers(this MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Modifiers;
                default:
                    return SyntaxFactory.TokenList();
            }
        }

        public static MemberDeclarationSyntax SetModifiers(this MemberDeclarationSyntax declaration, SyntaxTokenList modifiers)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithModifiers(modifiers);
            }

            Debug.Assert(false, declaration.Kind().ToString());

            return declaration;
        }

        public static MemberDeclarationSyntax GetParentMember(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member.Parent as MemberDeclarationSyntax;
        }

        public static MemberDeclarationSyntax GetMemberAt(this MemberDeclarationSyntax declaration, int index)
        {
            SyntaxList<MemberDeclarationSyntax> members = GetMembers(declaration);

            return members[index];
        }

        public static SyntaxList<MemberDeclarationSyntax> GetMembers(this MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).Members;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Members;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Members;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Members;
                default:
                    return default(SyntaxList<MemberDeclarationSyntax>);
            }
        }

        public static MemberDeclarationSyntax SetMembers(this MemberDeclarationSyntax declaration, SyntaxList<MemberDeclarationSyntax> newMembers)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithMembers(newMembers);
                default:
                    return declaration;
            }
        }

        public static bool IsIterator(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                    .DescendantNodes(node => !node.IsKind(
                        SyntaxKind.SimpleLambdaExpression,
                        SyntaxKind.ParenthesizedLambdaExpression,
                        SyntaxKind.AnonymousMethodExpression))
                    .Any(f => f.IsYieldStatement());
        }

        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.ReturnType?.IsVoid() == true;
        }

        public static TextSpan HeaderSpan(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return TextSpan.FromBounds(
                methodDeclaration.Span.Start,
                methodDeclaration.ParameterList?.Span.End ?? methodDeclaration.Identifier.Span.End);
        }

        public static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(SingletonList(member));
        }

        public static TextSpan HeaderSpan(this NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return TextSpan.FromBounds(
                namespaceDeclaration.Span.Start,
                namespaceDeclaration.Name?.Span.End ?? namespaceDeclaration.NamespaceKeyword.Span.End);
        }

        public static TextSpan HeaderSpan(this OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return TextSpan.FromBounds(
                operatorDeclaration.Span.Start,
                operatorDeclaration.ParameterList?.Span.End ?? operatorDeclaration.OperatorToken.Span.End);
        }

        public static bool IsReadOnlyAutoProperty(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            return getter != null
                && getter.Body == null
                && getter.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && !propertyDeclaration.HasSetter();
        }

        public static PropertyDeclarationSyntax WithAttributeLists(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AttributeListSyntax[] attributeLists)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithAttributeLists(List(attributeLists));
        }

        public static PropertyDeclarationSyntax WithAccessorList(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AccessorDeclarationSyntax[] accessors)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration
                .WithAccessorList(
                    AccessorList(
                        List(accessors)));
        }

        public static PropertyDeclarationSyntax WithoutSemicolonToken(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithSemicolonToken(NoneToken());
        }

        public static PropertyDeclarationSyntax WithoutInitializer(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithInitializer(null);
        }

        public static TextSpan HeaderSpan(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return TextSpan.FromBounds(
                propertyDeclaration.Span.Start,
                propertyDeclaration.Identifier.Span.End);
        }

        public static AccessorDeclarationSyntax Getter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Getter();
        }

        public static AccessorDeclarationSyntax Setter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Setter();
        }

        public static bool HasGetter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.ContainsGetter() == true;
        }

        public static bool HasSetter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.ContainsSetter() == true;
        }

        public static bool IsQualified(this SimpleNameSyntax identifierName)
        {
            if (identifierName == null)
                throw new ArgumentNullException(nameof(identifierName));

            return identifierName.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true;
        }

        public static bool IsQualifiedWithThis(this SimpleNameSyntax identifierName)
        {
            if (identifierName == null)
                throw new ArgumentNullException(nameof(identifierName));

            if (IsQualified(identifierName))
            {
                var memberAccess = (MemberAccessExpressionSyntax)identifierName.Parent;

                return memberAccess.Expression?.IsKind(SyntaxKind.ThisExpression) == true;
            }

            return false;
        }

        public static TextSpan HeaderSpan(this StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return TextSpan.FromBounds(
                structDeclaration.Span.Start,
                structDeclaration.Identifier.Span.End);
        }

        public static int LastIndexOf<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        public static bool Contains<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        public static bool IsVoid(this TypeSyntax type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsKind(SyntaxKind.PredefinedType)
                && ((PredefinedTypeSyntax)type).Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        public static SyntaxNode DeclarationOrExpression(this UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            if (usingStatement.Declaration != null)
                return usingStatement.Declaration;

            return usingStatement.Expression;
        }

        public static VariableDeclaratorSyntax SingleVariableOrDefault(this VariableDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            if (variables.Count == 1)
                return variables[0];

            return null;
        }

        public static VariableDeclaratorSyntax FirstVariableOrDefault(this VariableDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            if (variables.Any())
                return variables[0];

            return null;
        }

        public static bool IsYieldReturn(this YieldStatementSyntax yieldStatement)
        {
            if (yieldStatement == null)
                throw new ArgumentNullException(nameof(yieldStatement));

            return yieldStatement.ReturnOrBreakKeyword.IsKind(SyntaxKind.ReturnKeyword);
        }

        public static bool IsYieldBreak(this YieldStatementSyntax yieldStatement)
        {
            if (yieldStatement == null)
                throw new ArgumentNullException(nameof(yieldStatement));

            return yieldStatement.ReturnOrBreakKeyword.IsKind(SyntaxKind.BreakKeyword);
        }
    }
}
