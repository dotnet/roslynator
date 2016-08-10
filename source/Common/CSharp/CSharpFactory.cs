// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class CSharpFactory
    {
        public static SyntaxTrivia IndentTrivia { get; } = Whitespace("    ");

        public static SyntaxTrivia EmptyWhitespaceTrivia { get; } = SyntaxTrivia(SyntaxKind.WhitespaceTrivia, string.Empty);

        public static SyntaxTrivia NewLine { get; } = CreateNewLine();

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

        public static AttributeListSyntax AttributeList(AttributeSyntax attribute)
        {
            return SyntaxFactory.AttributeList(SingletonSeparatedList(attribute));
        }

        public static FieldDeclarationSyntax FieldDeclaration(TypeSyntax type, string identifier, ExpressionSyntax value = null)
        {
            return FieldDeclaration(
                type,
                identifier,
                (value != null) ? EqualsValueClause(value) : null);
        }

        public static FieldDeclarationSyntax FieldDeclaration(TypeSyntax type, string identifier, EqualsValueClauseSyntax initializer = null)
        {
            return SyntaxFactory.FieldDeclaration(
                VariableDeclaration(
                    type,
                    VariableDeclarator(
                        Identifier(identifier),
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

        public static InvocationExpressionSyntax InvocationExpression(string name, ArgumentSyntax argument)
        {
            return SyntaxFactory.InvocationExpression(
                IdentifierName(name),
                ArgumentList(argument));
        }

        public static InvocationExpressionSyntax InvocationExpression(string name, params ArgumentSyntax[] arguments)
        {
            return SyntaxFactory.InvocationExpression(
                IdentifierName(name),
                ArgumentList(arguments));
        }

        public static AccessorDeclarationSyntax Getter()
        {
            return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax AutoGetter()
        {
            return Getter().WithSemicolonToken(SemicolonToken());
        }

        public static AccessorDeclarationSyntax Setter()
        {
            return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax AutoSetter()
        {
            return Setter().WithSemicolonToken(SemicolonToken());
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

        private static SyntaxToken Token(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.Token(syntaxKind);
        }

        public static PredefinedTypeSyntax StringType()
        {
            return PredefinedType((SyntaxKind.StringKeyword));
        }

        public static PredefinedTypeSyntax IntType()
        {
            return PredefinedType((SyntaxKind.IntKeyword));
        }

        public static PredefinedTypeSyntax BoolType()
        {
            return PredefinedType((SyntaxKind.BoolKeyword));
        }

        public static PredefinedTypeSyntax VoidType()
        {
            return PredefinedType((SyntaxKind.VoidKeyword));
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
                Argument(identifier));
        }

        public static UsingDirectiveSyntax UsingStaticDirective(string name)
        {
            return SyntaxFactory.UsingDirective(StaticToken(), null, ParseName(name));
        }

        public static TryStatementSyntax TryStatement(BlockSyntax block, CatchClauseSyntax @catch, FinallyClauseSyntax @finally = null)
        {
            return SyntaxFactory.TryStatement(block, SingletonList(@catch), @finally);
        }

        private static SyntaxTrivia CreateNewLine()
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
    }
}
