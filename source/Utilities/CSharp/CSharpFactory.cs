// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    public static class CSharpFactory
    {
        private static readonly SymbolDisplayFormat _typeSyntaxSymbolDisplayFormat = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SyntaxTrivia IndentTrivia()
        {
            return Whitespace("    ");
        }

        public static SyntaxTrivia EmptyWhitespaceTrivia()
        {
            return SyntaxTrivia(SyntaxKind.WhitespaceTrivia, string.Empty);
        }

        public static SyntaxTrivia NewLineTrivia()
        {
            switch (Environment.NewLine)
            {
                case "\r":
                    return CarriageReturn;
                case "\n":
                    return LineFeed;
                default:
                    return CarriageReturnLineFeed;
            }
        }

        public static TypeSyntax Type(ITypeSymbol typeSymbol)
        {
            return Type(typeSymbol, _typeSyntaxSymbolDisplayFormat);
        }

        public static TypeSyntax Type(ITypeSymbol typeSymbol, SymbolDisplayFormat symbolDisplayFormat)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (symbolDisplayFormat == null)
                throw new ArgumentNullException(nameof(symbolDisplayFormat));

            string s = typeSymbol.ToDisplayString(symbolDisplayFormat);

            return ParseTypeName(s);
        }

        internal static SyntaxTokenList TokenList(params SyntaxKind[] kinds)
        {
            var tokens = new SyntaxToken[kinds.Length];

            for (int i = 0; i < kinds.Length; i++)
                tokens[i] = Token(kinds[i]);

            return SyntaxFactory.TokenList(tokens);
        }

        internal static SyntaxTokenList TokenList(SyntaxKind kind)
        {
            return SyntaxFactory.TokenList(Token(kind));
        }

        public static AssignmentExpressionSyntax SimpleAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right);
        }

        public static ExpressionStatementSyntax SimpleAssignmentExpressionStatement(ExpressionSyntax left, ExpressionSyntax right)
        {
            return ExpressionStatement(SimpleAssignmentExpression(left, right));
        }

        public static AttributeListSyntax AttributeList(AttributeSyntax attribute)
        {
            return SyntaxFactory.AttributeList(SingletonSeparatedList(attribute));
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, string identifier, ExpressionSyntax value = null)
        {
            return FieldDeclaration(
                modifiers,
                type,
                Identifier(identifier),
                value);
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, SyntaxToken identifier, ExpressionSyntax value = null)
        {
            return FieldDeclaration(
                modifiers,
                type,
                identifier,
                (value != null) ? EqualsValueClause(value) : null);
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, string identifier, EqualsValueClauseSyntax initializer)
        {
            return FieldDeclaration(modifiers, type, Identifier(identifier), initializer);
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax initializer)
        {
            return SyntaxFactory.FieldDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                VariableDeclaration(
                    type,
                    VariableDeclarator(
                        identifier,
                        null,
                        initializer)));
        }

        public static ArgumentListSyntax ArgumentList(ArgumentSyntax argument)
        {
            return SyntaxFactory.ArgumentList(SingletonSeparatedList(argument));
        }

        public static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments)
        {
            return SyntaxFactory.ArgumentList(SeparatedList(arguments));
        }

        public static ArgumentSyntax Argument(string name)
        {
            return SyntaxFactory.Argument(IdentifierName(name));
        }

        public static AttributeSyntax Attribute(string name)
        {
            return SyntaxFactory.Attribute(IdentifierName(name));
        }

        public static NamespaceDeclarationSyntax NamespaceDeclaration(string name)
        {
            return SyntaxFactory.NamespaceDeclaration(IdentifierName(name));
        }

        public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
        }

        public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, operatorToken, name);
        }

        public static InvocationExpressionSyntax InvocationExpression(string name)
        {
            return SyntaxFactory.InvocationExpression(IdentifierName(name));
        }

        public static InvocationExpressionSyntax InvocationExpression(string name, ArgumentListSyntax argumentList)
        {
            return SyntaxFactory.InvocationExpression(IdentifierName(name), argumentList);
        }

        public static InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression, string name)
        {
            return InvocationExpression(expression, IdentifierName(name));
        }

        public static InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return SyntaxFactory.InvocationExpression(SimpleMemberAccessExpression(expression, name));
        }

        public static InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression, string name, ArgumentListSyntax argumentList)
        {
            return InvocationExpression(expression, IdentifierName(name), argumentList);
        }

        public static InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name, ArgumentListSyntax argumentList)
        {
            return SyntaxFactory.InvocationExpression(SimpleMemberAccessExpression(expression, name), argumentList);
        }

        public static AccessorDeclarationSyntax Getter(BlockSyntax body = null)
        {
            return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, body);
        }

        public static AccessorDeclarationSyntax AutoGetter(SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AutoAccessor(SyntaxKind.GetAccessorDeclaration, modifiers);
        }

        public static AccessorDeclarationSyntax Setter(BlockSyntax body = null)
        {
            return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, body);
        }

        public static AccessorDeclarationSyntax AutoSetter(SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AutoAccessor(SyntaxKind.SetAccessorDeclaration, modifiers);
        }

        private static AccessorDeclarationSyntax AutoAccessor(SyntaxKind kind, SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AccessorDeclaration(
                kind,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                Token(GetAccessorDeclarationKeywordKind(kind)),
                default(BlockSyntax),
                SemicolonToken());
        }

        private static SyntaxKind GetAccessorDeclarationKeywordKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.GetAccessorDeclaration:
                    return SyntaxKind.GetKeyword;
                case SyntaxKind.SetAccessorDeclaration:
                    return SyntaxKind.SetKeyword;
                case SyntaxKind.AddAccessorDeclaration:
                    return SyntaxKind.AddKeyword;
                case SyntaxKind.RemoveAccessorDeclaration:
                    return SyntaxKind.RemoveKeyword;
                case SyntaxKind.UnknownAccessorDeclaration:
                    return SyntaxKind.IdentifierToken;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, VariableDeclaratorSyntax variable)
        {
            return SyntaxFactory.VariableDeclaration(type, SingletonSeparatedList(variable));
        }

        public static SyntaxToken SemicolonToken()
        {
            return Token(SyntaxKind.SemicolonToken);
        }

        public static SyntaxToken CommaToken()
        {
            return Token(SyntaxKind.CommaToken);
        }

        public static SyntaxToken NoneToken()
        {
            return Token(SyntaxKind.None);
        }

        public static SyntaxToken DoToken()
        {
            return Token(SyntaxKind.DoKeyword);
        }

        public static SyntaxToken WhileToken()
        {
            return Token(SyntaxKind.WhileKeyword);
        }

        public static SyntaxToken UsingToken()
        {
            return Token(SyntaxKind.UsingKeyword);
        }

        public static SyntaxToken StaticToken()
        {
            return Token(SyntaxKind.StaticKeyword);
        }

        public static SyntaxToken ReadOnlyToken()
        {
            return Token(SyntaxKind.ReadOnlyKeyword);
        }

        public static SyntaxToken ConstToken()
        {
            return Token(SyntaxKind.ConstKeyword);
        }

        private static SyntaxToken Token(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.Token(syntaxKind);
        }

        public static PredefinedTypeSyntax StringType()
        {
            return PredefinedType(SyntaxKind.StringKeyword);
        }

        public static PredefinedTypeSyntax IntType()
        {
            return PredefinedType(SyntaxKind.IntKeyword);
        }

        public static PredefinedTypeSyntax BoolType()
        {
            return PredefinedType(SyntaxKind.BoolKeyword);
        }

        public static PredefinedTypeSyntax VoidType()
        {
            return PredefinedType(SyntaxKind.VoidKeyword);
        }

        public static PredefinedTypeSyntax ObjectType()
        {
            return PredefinedType(SyntaxKind.ObjectKeyword);
        }

        public static PredefinedTypeSyntax CharType()
        {
            return PredefinedType(SyntaxKind.CharKeyword);
        }

        public static PredefinedTypeSyntax SByteType()
        {
            return PredefinedType(SyntaxKind.SByteKeyword);
        }

        public static PredefinedTypeSyntax ByteType()
        {
            return PredefinedType(SyntaxKind.ByteKeyword);
        }

        public static PredefinedTypeSyntax ShortType()
        {
            return PredefinedType(SyntaxKind.ShortKeyword);
        }

        public static PredefinedTypeSyntax UShortType()
        {
            return PredefinedType(SyntaxKind.UShortKeyword);
        }

        public static PredefinedTypeSyntax UIntType()
        {
            return PredefinedType(SyntaxKind.UIntKeyword);
        }

        public static PredefinedTypeSyntax LongType()
        {
            return PredefinedType(SyntaxKind.LongKeyword);
        }

        public static PredefinedTypeSyntax ULongType()
        {
            return PredefinedType(SyntaxKind.ULongKeyword);
        }

        public static PredefinedTypeSyntax DecimalType()
        {
            return PredefinedType(SyntaxKind.DecimalKeyword);
        }

        public static PredefinedTypeSyntax SingleType()
        {
            return PredefinedType(SyntaxKind.FloatKeyword);
        }

        public static PredefinedTypeSyntax DoubleType()
        {
            return PredefinedType(SyntaxKind.DoubleKeyword);
        }

        private static PredefinedTypeSyntax PredefinedType(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.PredefinedType(Token(syntaxKind));
        }

        public static SyntaxToken PublicToken()
        {
            return Token(SyntaxKind.PublicKeyword);
        }

        public static SyntaxToken InternalToken()
        {
            return Token(SyntaxKind.InternalKeyword);
        }

        public static SyntaxToken ProtectedToken()
        {
            return Token(SyntaxKind.ProtectedKeyword);
        }

        public static SyntaxToken PrivateToken()
        {
            return Token(SyntaxKind.PrivateKeyword);
        }

        public static SyntaxToken PartialToken()
        {
            return Token(SyntaxKind.PartialKeyword);
        }

        public static SyntaxToken VirtualToken()
        {
            return Token(SyntaxKind.VirtualKeyword);
        }

        public static IdentifierNameSyntax Var()
        {
            return IdentifierName("var");
        }

        public static LiteralExpressionSyntax StringLiteralExpression(string value)
        {
            return LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax CharacterLiteralExpression(char value)
        {
            return LiteralExpression(
                SyntaxKind.CharacterLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(int value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(uint value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(sbyte value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(decimal value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(double value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(float value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(long value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(ulong value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax ZeroLiteralExpression()
        {
            return NumericLiteralExpression(0);
        }

        public static LiteralExpressionSyntax TrueLiteralExpression()
        {
            return LiteralExpression(SyntaxKind.TrueLiteralExpression);
        }

        public static LiteralExpressionSyntax FalseLiteralExpression()
        {
            return LiteralExpression(SyntaxKind.FalseLiteralExpression);
        }

        public static LiteralExpressionSyntax NullLiteralExpression()
        {
            return LiteralExpression(SyntaxKind.NullLiteralExpression);
        }

        public static BinaryExpressionSyntax BitwiseAndExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseAndExpression, left, right);
        }

        public static BinaryExpressionSyntax BitwiseOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseOrExpression, left, right);
        }

        public static BinaryExpressionSyntax LogicalAndExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);
        }

        public static BinaryExpressionSyntax LogicalOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalOrExpression, left, right);
        }

        public static BinaryExpressionSyntax EqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.EqualsExpression, left, right);
        }

        public static BinaryExpressionSyntax NotEqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);
        }

        public static PrefixUnaryExpressionSyntax LogicalNotExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, operand);
        }

        public static AttributeSyntax Attribute(string name, AttributeArgumentSyntax argument)
        {
            return Attribute(IdentifierName(name), argument);
        }

        public static AttributeSyntax Attribute(NameSyntax name, AttributeArgumentSyntax argument)
        {
            return SyntaxFactory.Attribute(
                name,
                AttributeArgumentList(
                    SingletonSeparatedList(argument)));
        }

        public static AttributeSyntax Attribute(string name, ExpressionSyntax expression)
        {
            return Attribute(IdentifierName(name), expression);
        }

        public static AttributeSyntax Attribute(NameSyntax name, ExpressionSyntax expression)
        {
            return SyntaxFactory.Attribute(
                name,
                AttributeArgumentList(
                    SingletonSeparatedList(AttributeArgument(expression))));
        }

        public static InvocationExpressionSyntax NameOf(string identifier)
        {
            return InvocationExpression(
                "nameof",
                ArgumentList(Argument(identifier)));
        }

        public static UsingDirectiveSyntax UsingStaticDirective(string name)
        {
            return SyntaxFactory.UsingDirective(StaticToken(), null, ParseName(name));
        }

        public static TryStatementSyntax TryStatement(BlockSyntax block, CatchClauseSyntax @catch, FinallyClauseSyntax @finally = null)
        {
            return SyntaxFactory.TryStatement(block, SingletonList(@catch), @finally);
        }

        public static PropertyDeclarationSyntax PropertyDeclaration(PropertyKind kind, TypeSyntax type, string name)
        {
            return PropertyDeclaration(
                kind,
                default(SyntaxTokenList),
                type,
                name);
        }

        public static PropertyDeclarationSyntax PropertyDeclaration(PropertyKind kind, SyntaxTokenList modifiers, TypeSyntax type, string name)
        {
            return PropertyDeclaration(
                kind,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                type,
                default(ExplicitInterfaceSpecifierSyntax),
                name);
        }

        public static PropertyDeclarationSyntax PropertyDeclaration(
            PropertyKind kind,
            SyntaxList<AttributeListSyntax> attributeLists,
            SyntaxTokenList modifiers,
            TypeSyntax type,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier,
            string name)
        {
            switch (kind)
            {
                case PropertyKind.AutoProperty:
                    {
                        return SyntaxFactory.PropertyDeclaration(
                            attributeLists,
                            modifiers,
                            type,
                            explicitInterfaceSpecifier,
                            Identifier(name),
                            AccessorList(
                                AutoGetter(),
                                AutoSetter()));
                    }
                case PropertyKind.AutoPropertyWithPrivateSet:
                    {
                        return SyntaxFactory.PropertyDeclaration(
                            attributeLists,
                            modifiers,
                            type,
                            explicitInterfaceSpecifier,
                            Identifier(name),
                            AccessorList(
                                AutoGetter(),
                                AutoSetter(TokenList(SyntaxKind.PrivateKeyword))));
                    }
                case PropertyKind.ReadOnlyAutoProperty:
                    {
                        return SyntaxFactory.PropertyDeclaration(
                            attributeLists,
                            modifiers,
                            type,
                            explicitInterfaceSpecifier,
                            Identifier(name),
                            AccessorList(AutoGetter()));
                    }
                default:
                    {
                        Debug.Assert(false, kind.ToString());
                        throw new ArgumentOutOfRangeException(nameof(kind));
                    }
            }
        }

        public static AccessorListSyntax AccessorList(params AccessorDeclarationSyntax[] accessors)
        {
            return SyntaxFactory.AccessorList(List(accessors));
        }

        public static YieldStatementSyntax YieldReturnStatement(ExpressionSyntax expression)
        {
            return YieldStatement(SyntaxKind.YieldReturnStatement, expression);
        }

        public static YieldStatementSyntax YieldBreakStatement()
        {
            return YieldStatement(SyntaxKind.YieldBreakStatement);
        }

        public static ObjectCreationExpressionSyntax ObjectCreationExpression(TypeSyntax type, ArgumentListSyntax argumentList)
        {
            return SyntaxFactory.ObjectCreationExpression(type, argumentList, default(InitializerExpressionSyntax));
        }

        public static ParameterSyntax Parameter(TypeSyntax type, SyntaxToken identifier)
        {
            return SyntaxFactory.Parameter(
                default(SyntaxList<AttributeListSyntax>),
                default(SyntaxTokenList),
                type,
                identifier,
                default(EqualsValueClauseSyntax));
        }

        public static PrefixUnaryExpressionSyntax PreIncrementExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax PreDecrementExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, operand);
        }

        public static PostfixUnaryExpressionSyntax PostIncrementExpression(ExpressionSyntax operand)
        {
            return PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, operand);
        }

        public static PostfixUnaryExpressionSyntax PostDecrementExpression(ExpressionSyntax operand)
        {
            return PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, operand);
        }

        public static ConstructorInitializerSyntax BaseConstructorInitializer(ArgumentListSyntax argumentList = null)
        {
            return ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, argumentList);
        }

        public static ConstructorInitializerSyntax ThisConstructorInitializer(ArgumentListSyntax argumentList = null)
        {
            return ConstructorInitializer(SyntaxKind.ThisConstructorInitializer, argumentList);
        }

        public static ParameterListSyntax ParameterList(IEnumerable<ParameterSyntax> parameters)
        {
            return SyntaxFactory.ParameterList(SeparatedList(parameters));
        }

        public static ParameterListSyntax ParameterList(ParameterSyntax parameter)
        {
            return SyntaxFactory.ParameterList(SingletonSeparatedList(parameter));
        }

        public static ParameterListSyntax ParameterList(params ParameterSyntax[] parameters)
        {
            return SyntaxFactory.ParameterList(SeparatedList(parameters));
        }

        public static SwitchSectionSyntax DefaultSwitchSection(StatementSyntax statement)
        {
            return DefaultSwitchSection(SingletonList(statement));
        }

        public static SwitchSectionSyntax DefaultSwitchSection(SyntaxList<StatementSyntax> statements)
        {
            return SwitchSection(
                SingletonList<SwitchLabelSyntax>(DefaultSwitchLabel()),
                statements);
        }
    }
}
