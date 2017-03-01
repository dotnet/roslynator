// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    public static class CSharpFactory
    {
        public static SyntaxTrivia IndentTrivia(int indentSize = 4)
        {
            if (indentSize == 4)
            {
                return Whitespace("    ");
            }
            else
            {
                return Whitespace(new string(' ', indentSize));
            }
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

        public static LiteralExpressionSyntax ConstantExpression(object value)
        {
            if (value == null)
                return NullLiteralExpression();

            if (value is bool)
                return ((bool)value) ? TrueLiteralExpression() : FalseLiteralExpression();

            if (value is char)
                return CharacterLiteralExpression((char)value);

            if (value is sbyte)
                return NumericLiteralExpression((sbyte)value);

            if (value is byte)
                return NumericLiteralExpression((byte)value);

            if (value is short)
                return NumericLiteralExpression((short)value);

            if (value is ushort)
                return NumericLiteralExpression((ushort)value);

            if (value is int)
                return NumericLiteralExpression((int)value);

            if (value is uint)
                return NumericLiteralExpression((uint)value);

            if (value is long)
                return NumericLiteralExpression((long)value);

            if (value is ulong)
                return NumericLiteralExpression((ulong)value);

            if (value is decimal)
                return NumericLiteralExpression((decimal)value);

            if (value is float)
                return NumericLiteralExpression((float)value);

            if (value is double)
                return NumericLiteralExpression((double)value);

            return StringLiteralExpression(value.ToString());
        }

        public static AssignmentExpressionSyntax SimpleAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax SimpleAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, operatorToken, right);
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
            return SyntaxFactory.FieldDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                VariableDeclaration(
                    type,
                    identifier,
                    (value != null) ? EqualsValueClause(value) : null));
        }

        public static ArgumentListSyntax ArgumentList(ArgumentSyntax argument)
        {
            return SyntaxFactory.ArgumentList(SingletonSeparatedList(argument));
        }

        public static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments)
        {
            return SyntaxFactory.ArgumentList(SeparatedList(arguments));
        }

        public static AttributeArgumentListSyntax AttributeArgumentList(AttributeArgumentSyntax attributeArgument)
        {
            return SyntaxFactory.AttributeArgumentList(SingletonSeparatedList(attributeArgument));
        }

        public static AttributeArgumentListSyntax AttributeArgumentList(params AttributeArgumentSyntax[] attributeArguments)
        {
            return SyntaxFactory.AttributeArgumentList(SeparatedList(attributeArguments));
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
            return InvocationExpression(name, ArgumentList(argument));
        }

        public static InvocationExpressionSyntax InvocationExpression(string name, ArgumentListSyntax argumentList)
        {
            return SyntaxFactory.InvocationExpression(IdentifierName(name), argumentList);
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, string name)
        {
            return SimpleMemberInvocationExpression(expression, IdentifierName(name));
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return SyntaxFactory.InvocationExpression(SimpleMemberAccessExpression(expression, name));
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, string name, ArgumentSyntax argument)
        {
            return SimpleMemberInvocationExpression(expression, name, ArgumentList(argument));
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, string name, ArgumentListSyntax argumentList)
        {
            return SimpleMemberInvocationExpression(expression, IdentifierName(name), argumentList);
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name, ArgumentSyntax argument)
        {
            return SimpleMemberInvocationExpression(expression, name, ArgumentList(argument));
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name, ArgumentListSyntax argumentList)
        {
            return SyntaxFactory.InvocationExpression(SimpleMemberAccessExpression(expression, name), argumentList);
        }

        public static AccessorDeclarationSyntax Getter(BlockSyntax body = null)
        {
            return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, body);
        }

        public static AccessorDeclarationSyntax AutoImplementedGetter(SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AutoImplementedAccessor(SyntaxKind.GetAccessorDeclaration, modifiers);
        }

        public static AccessorDeclarationSyntax Setter(BlockSyntax body = null)
        {
            return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, body);
        }

        public static AccessorDeclarationSyntax AutoImplementedSetter(SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AutoImplementedAccessor(SyntaxKind.SetAccessorDeclaration, modifiers);
        }

        private static AccessorDeclarationSyntax AutoImplementedAccessor(SyntaxKind kind, SyntaxTokenList modifiers = default(SyntaxTokenList))
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

        public static VariableDeclaratorSyntax VariableDeclarator(string identifier, EqualsValueClauseSyntax initializer)
        {
            return VariableDeclarator(Identifier(identifier), initializer);
        }

        public static VariableDeclaratorSyntax VariableDeclarator(SyntaxToken identifier, EqualsValueClauseSyntax initializer)
        {
            return SyntaxFactory.VariableDeclarator(identifier, default(BracketedArgumentListSyntax), initializer);
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, string identifier, EqualsValueClauseSyntax initializer)
        {
            return VariableDeclaration(type, Identifier(identifier), initializer);
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax initializer)
        {
            return VariableDeclaration(type, VariableDeclarator(identifier, initializer));
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

        public static SyntaxToken DoKeyword()
        {
            return Token(SyntaxKind.DoKeyword);
        }

        public static SyntaxToken WhileKeyword()
        {
            return Token(SyntaxKind.WhileKeyword);
        }

        public static SyntaxToken UsingKeyword()
        {
            return Token(SyntaxKind.UsingKeyword);
        }

        public static SyntaxToken StaticKeyword()
        {
            return Token(SyntaxKind.StaticKeyword);
        }

        public static SyntaxToken ReadOnlyKeyword()
        {
            return Token(SyntaxKind.ReadOnlyKeyword);
        }

        public static SyntaxToken ConstKeyword()
        {
            return Token(SyntaxKind.ConstKeyword);
        }

        public static SyntaxToken EqualsToken()
        {
            return Token(SyntaxKind.EqualsToken);
        }

        public static SyntaxToken GreaterThanToken()
        {
            return Token(SyntaxKind.GreaterThanToken);
        }

        public static SyntaxToken GreaterThanEqualsToken()
        {
            return Token(SyntaxKind.GreaterThanEqualsToken);
        }

        public static SyntaxToken LessThanToken()
        {
            return Token(SyntaxKind.LessThanToken);
        }

        public static SyntaxToken LessThanEqualsToken()
        {
            return Token(SyntaxKind.LessThanEqualsToken);
        }

        public static SyntaxToken EqualsGreaterThanToken()
        {
            return Token(SyntaxKind.EqualsGreaterThanToken);
        }

        public static SyntaxToken ExclamationEqualsToken()
        {
            return Token(SyntaxKind.ExclamationEqualsToken);
        }

        public static SyntaxToken OpenParenToken()
        {
            return Token(SyntaxKind.OpenParenToken);
        }

        public static SyntaxToken CloseParenToken()
        {
            return Token(SyntaxKind.CloseParenToken);
        }

        public static SyntaxToken OpenBraceToken()
        {
            return Token(SyntaxKind.OpenBraceToken);
        }

        public static SyntaxToken CloseBraceToken()
        {
            return Token(SyntaxKind.CloseBraceToken);
        }

        public static SyntaxToken OpenBracketToken()
        {
            return Token(SyntaxKind.OpenBracketToken);
        }

        public static SyntaxToken CloseBracketToken()
        {
            return Token(SyntaxKind.CloseBracketToken);
        }

        public static SyntaxToken AsyncKeyword()
        {
            return Token(SyntaxKind.AsyncKeyword);
        }

        public static SyntaxToken BarBarToken()
        {
            return Token(SyntaxKind.BarBarToken);
        }

        public static SyntaxToken AmpersandAmpersandToken()
        {
            return Token(SyntaxKind.AmpersandAmpersandToken);
        }

        public static SyntaxToken QuestionToken()
        {
            return Token(SyntaxKind.QuestionToken);
        }

        public static SyntaxToken ColonToken()
        {
            return Token(SyntaxKind.ColonToken);
        }

        public static SyntaxToken MinusMinusToken()
        {
            return Token(SyntaxKind.MinusMinusToken);
        }

        public static SyntaxToken PlusPlusToken()
        {
            return Token(SyntaxKind.PlusPlusToken);
        }

        public static SyntaxToken ReturnKeyword()
        {
            return Token(SyntaxKind.ReturnKeyword);
        }

        public static SyntaxToken ForKeyword()
        {
            return Token(SyntaxKind.ForKeyword);
        }

        public static SyntaxToken ForEachKeyword()
        {
            return Token(SyntaxKind.ForEachKeyword);
        }

        public static SyntaxToken ThrowKeyword()
        {
            return Token(SyntaxKind.ThrowKeyword);
        }

        private static SyntaxToken Token(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.Token(syntaxKind);
        }

        public static SyntaxToken StringKeyword()
        {
            return Token(SyntaxKind.StringKeyword);
        }

        public static SyntaxToken IntKeyword()
        {
            return Token(SyntaxKind.IntKeyword);
        }

        public static SyntaxToken BoolKeyword()
        {
            return Token(SyntaxKind.BoolKeyword);
        }

        public static SyntaxToken VoidKeyword()
        {
            return Token(SyntaxKind.VoidKeyword);
        }

        public static SyntaxToken ObjectKeyword()
        {
            return Token(SyntaxKind.ObjectKeyword);
        }

        public static SyntaxToken CharKeyword()
        {
            return Token(SyntaxKind.CharKeyword);
        }

        public static SyntaxToken SByteKeyword()
        {
            return Token(SyntaxKind.SByteKeyword);
        }

        public static SyntaxToken ByteKeyword()
        {
            return Token(SyntaxKind.ByteKeyword);
        }

        public static SyntaxToken ShortKeyword()
        {
            return Token(SyntaxKind.ShortKeyword);
        }

        public static SyntaxToken UShortKeyword()
        {
            return Token(SyntaxKind.UShortKeyword);
        }

        public static SyntaxToken UIntKeyword()
        {
            return Token(SyntaxKind.UIntKeyword);
        }

        public static SyntaxToken LongKeyword()
        {
            return Token(SyntaxKind.LongKeyword);
        }

        public static SyntaxToken ULongKeyword()
        {
            return Token(SyntaxKind.ULongKeyword);
        }

        public static SyntaxToken DecimalKeyword()
        {
            return Token(SyntaxKind.DecimalKeyword);
        }

        public static SyntaxToken FloatKeyword()
        {
            return Token(SyntaxKind.FloatKeyword);
        }

        public static SyntaxToken DoubleKeyword()
        {
            return Token(SyntaxKind.DoubleKeyword);
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

        public static SyntaxToken PublicKeyword()
        {
            return Token(SyntaxKind.PublicKeyword);
        }

        public static SyntaxToken InternalKeyword()
        {
            return Token(SyntaxKind.InternalKeyword);
        }

        public static SyntaxToken ProtectedKeyword()
        {
            return Token(SyntaxKind.ProtectedKeyword);
        }

        public static SyntaxToken PrivateKeyword()
        {
            return Token(SyntaxKind.PrivateKeyword);
        }

        public static SyntaxToken PartialKeyword()
        {
            return Token(SyntaxKind.PartialKeyword);
        }

        public static SyntaxToken VirtualKeyword()
        {
            return Token(SyntaxKind.VirtualKeyword);
        }

        public static SyntaxToken AbstractKeyword()
        {
            return Token(SyntaxKind.AbstractKeyword);
        }

        public static SyntaxToken YieldKeyword()
        {
            return Token(SyntaxKind.YieldKeyword);
        }

        public static SyntaxToken BreakKeyword()
        {
            return Token(SyntaxKind.BreakKeyword);
        }

        public static IdentifierNameSyntax VarType()
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

        public static BinaryExpressionSyntax BitwiseAndExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseAndExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax BitwiseOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseOrExpression, left, right);
        }

        public static BinaryExpressionSyntax BitwiseOrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseOrExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax LogicalAndExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);
        }

        public static BinaryExpressionSyntax LogicalAndExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalAndExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax LogicalOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalOrExpression, left, right);
        }

        public static BinaryExpressionSyntax LogicalOrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalOrExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax EqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.EqualsExpression, left, right);
        }

        public static BinaryExpressionSyntax EqualsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.EqualsExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax NotEqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);
        }

        public static BinaryExpressionSyntax NotEqualsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.NotEqualsExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax AsExpression(ExpressionSyntax expression, TypeSyntax type)
        {
            return BinaryExpression(SyntaxKind.AsExpression, expression, type);
        }

        public static BinaryExpressionSyntax AsExpression(ExpressionSyntax expression, SyntaxToken operatorToken, TypeSyntax type)
        {
            return BinaryExpression(SyntaxKind.AsExpression, expression, operatorToken, type);
        }

        public static BinaryExpressionSyntax IsExpression(ExpressionSyntax expression, TypeSyntax type)
        {
            return BinaryExpression(SyntaxKind.IsExpression, expression, type);
        }

        public static BinaryExpressionSyntax IsExpression(ExpressionSyntax expression, SyntaxToken operatorToken, TypeSyntax type)
        {
            return BinaryExpression(SyntaxKind.IsExpression, expression, operatorToken, type);
        }

        public static BinaryExpressionSyntax AddExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.AddExpression, left, right);
        }

        public static BinaryExpressionSyntax AddExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.AddExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax SubtractExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.SubtractExpression, left, right);
        }

        public static BinaryExpressionSyntax SubtractExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.SubtractExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax LessThanExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LessThanExpression, left, right);
        }

        public static BinaryExpressionSyntax LessThanExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LessThanExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax LessThanOrEqualExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LessThanOrEqualExpression, left, right);
        }

        public static BinaryExpressionSyntax LessThanOrEqualExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LessThanOrEqualExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax GreaterThanExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.GreaterThanExpression, left, right);
        }

        public static BinaryExpressionSyntax GreaterThanExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.GreaterThanExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax GreaterThanOrEqualExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.GreaterThanOrEqualExpression, left, right);
        }

        public static BinaryExpressionSyntax GreaterThanOrEqualExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.GreaterThanOrEqualExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax CoalesceExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.CoalesceExpression, left, right);
        }

        public static BinaryExpressionSyntax CoalesceExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.CoalesceExpression, left, operatorToken, right);
        }

        public static PrefixUnaryExpressionSyntax LogicalNotExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax LogicalNotExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, operatorToken, operand);
        }

        public static AttributeSyntax Attribute(string name, ExpressionSyntax argumentExpression)
        {
            return Attribute(IdentifierName(name), argumentExpression);
        }

        public static AttributeSyntax Attribute(NameSyntax name, ExpressionSyntax argumentExpression)
        {
            return SyntaxFactory.Attribute(
                name,
                AttributeArgumentList(AttributeArgument(argumentExpression)));
        }

        public static InvocationExpressionSyntax NameOf(string identifier)
        {
            return InvocationExpression(
                "nameof",
                ArgumentList(
                    Argument(IdentifierName(identifier))));
        }

        public static UsingDirectiveSyntax UsingStaticDirective(string name)
        {
            return UsingDirective(
                StaticKeyword(),
                default(NameEqualsSyntax),
                ParseName(name));
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
                                AutoImplementedGetter(),
                                AutoImplementedSetter()));
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
                                AutoImplementedGetter(),
                                AutoImplementedSetter(TokenList(PrivateKeyword()))));
                    }
                case PropertyKind.ReadOnlyAutoProperty:
                    {
                        return SyntaxFactory.PropertyDeclaration(
                            attributeLists,
                            modifiers,
                            type,
                            explicitInterfaceSpecifier,
                            Identifier(name),
                            AccessorList(AutoImplementedGetter()));
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

        public static ParameterSyntax Parameter(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax initializer = null)
        {
            return SyntaxFactory.Parameter(
                default(SyntaxList<AttributeListSyntax>),
                default(SyntaxTokenList),
                type,
                identifier,
                initializer);
        }

        public static PrefixUnaryExpressionSyntax PreIncrementExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax PreIncrementExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, operatorToken, operand);
        }

        public static PrefixUnaryExpressionSyntax PreDecrementExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax PreDecrementExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, operatorToken, operand);
        }

        public static PostfixUnaryExpressionSyntax PostIncrementExpression(ExpressionSyntax operand)
        {
            return PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, operand);
        }

        public static PostfixUnaryExpressionSyntax PostIncrementExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, operand, operatorToken);
        }

        public static PostfixUnaryExpressionSyntax PostDecrementExpression(ExpressionSyntax operand)
        {
            return PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, operand);
        }

        public static PostfixUnaryExpressionSyntax PostDecrementExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, operand, operatorToken);
        }

        public static ConstructorInitializerSyntax BaseConstructorInitializer(ArgumentListSyntax argumentList = null)
        {
            return ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, argumentList);
        }

        public static ConstructorInitializerSyntax ThisConstructorInitializer(ArgumentListSyntax argumentList = null)
        {
            return ConstructorInitializer(SyntaxKind.ThisConstructorInitializer, argumentList);
        }

        public static ParameterListSyntax ParameterList(ParameterSyntax parameter)
        {
            return SyntaxFactory.ParameterList(SingletonSeparatedList(parameter));
        }

        public static ParameterListSyntax ParameterList(params ParameterSyntax[] parameters)
        {
            return SyntaxFactory.ParameterList(SeparatedList(parameters));
        }

        public static SwitchSectionSyntax SwitchSection(SwitchLabelSyntax switchLabel, StatementSyntax statement)
        {
            return SwitchSection(switchLabel, SingletonList(statement));
        }

        public static SwitchSectionSyntax SwitchSection(SwitchLabelSyntax switchLabel, SyntaxList<StatementSyntax> statements)
        {
            return SyntaxFactory.SwitchSection(SingletonList(switchLabel), statements);
        }

        public static SwitchSectionSyntax DefaultSwitchSection(StatementSyntax statement)
        {
            return DefaultSwitchSection(SingletonList(statement));
        }

        public static SwitchSectionSyntax DefaultSwitchSection(SyntaxList<StatementSyntax> statements)
        {
            return SwitchSection(DefaultSwitchLabel(), statements);
        }

        public static ImplicitElementAccessSyntax ImplicitElementAccess(ExpressionSyntax expression)
        {
            return SyntaxFactory.ImplicitElementAccess(
                BracketedArgumentList(
                    SingletonSeparatedList(
                        Argument(expression))));
        }

        public static GenericNameSyntax GenericName(SyntaxToken identifier, TypeSyntax typeArgument)
        {
            return SyntaxFactory.GenericName(identifier, TypeArgumentList(typeArgument));
        }

        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(TypeSyntax type, string identifier, ExpressionSyntax value = null)
        {
            return LocalDeclarationStatement(type, Identifier(identifier), value);
        }

        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(TypeSyntax type, SyntaxToken identifier, ExpressionSyntax value = null)
        {
            VariableDeclaratorSyntax declarator = (value != null)
                ? VariableDeclarator(identifier, EqualsValueClause(value))
                : SyntaxFactory.VariableDeclarator(identifier);

            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    type,
                    SingletonSeparatedList(declarator)));
        }

        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(string name, ExpressionSyntax value)
        {
            return EnumMemberDeclaration(Identifier(name), value);
        }

        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxToken identifier, ExpressionSyntax value)
        {
            return SyntaxFactory.EnumMemberDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                identifier,
                EqualsValueClause(value));
        }

        public static TypeArgumentListSyntax TypeArgumentList(TypeSyntax argument)
        {
            return SyntaxFactory.TypeArgumentList(SingletonSeparatedList(argument));
        }
    }
}
