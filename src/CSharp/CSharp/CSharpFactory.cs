// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

#pragma warning disable CS1591

namespace Roslynator.CSharp
{
    /// <summary>
    /// A factory for syntax nodes, tokens and trivia. This class is built on top of <see cref="SyntaxFactory"/> members.
    /// </summary>
    public static class CSharpFactory
    {
        #region Trivia
        public static SyntaxTrivia EmptyWhitespace()
        {
            return SyntaxTrivia(SyntaxKind.WhitespaceTrivia, "");
        }

        public static SyntaxTrivia NewLine()
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

        internal static SyntaxTriviaList IncreaseIndentation(SyntaxTrivia trivia)
        {
            return TriviaList(trivia, SingleIndentation(trivia));
        }

        internal static SyntaxTrivia SingleIndentation(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceTrivia())
            {
                string s = trivia.ToString();

                int length = s.Length;

                if (length > 0)
                {
                    if (s.All(f => f == '\t'))
                    {
                        return Tab;
                    }
                    else if (s.All(f => f == ' '))
                    {
                        if (length % 4 == 0)
                            return Whitespace("    ");

                        if (length % 3 == 0)
                            return Whitespace("   ");

                        if (length % 2 == 0)
                            return Whitespace("  ");
                    }
                }
            }

            return DefaultIndentation;
        }

        internal static SyntaxTrivia DefaultIndentation { get; } = Whitespace("    ");
        #endregion Trivia

        #region Token
        public static SyntaxToken TildeToken()
        {
            return Token(SyntaxKind.TildeToken);
        }

        public static SyntaxToken ExclamationToken()
        {
            return Token(SyntaxKind.ExclamationToken);
        }

        public static SyntaxToken DollarToken()
        {
            return Token(SyntaxKind.DollarToken);
        }

        public static SyntaxToken PercentToken()
        {
            return Token(SyntaxKind.PercentToken);
        }

        public static SyntaxToken CaretToken()
        {
            return Token(SyntaxKind.CaretToken);
        }

        public static SyntaxToken AmpersandToken()
        {
            return Token(SyntaxKind.AmpersandToken);
        }

        public static SyntaxToken AsteriskToken()
        {
            return Token(SyntaxKind.AsteriskToken);
        }

        public static SyntaxToken OpenParenToken()
        {
            return Token(SyntaxKind.OpenParenToken);
        }

        public static SyntaxToken CloseParenToken()
        {
            return Token(SyntaxKind.CloseParenToken);
        }

        public static SyntaxToken MinusToken()
        {
            return Token(SyntaxKind.MinusToken);
        }

        public static SyntaxToken PlusToken()
        {
            return Token(SyntaxKind.PlusToken);
        }

        public static SyntaxToken EqualsToken()
        {
            return Token(SyntaxKind.EqualsToken);
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

        public static SyntaxToken BarToken()
        {
            return Token(SyntaxKind.BarToken);
        }

        public static SyntaxToken BackslashToken()
        {
            return Token(SyntaxKind.BackslashToken);
        }

        public static SyntaxToken ColonToken()
        {
            return Token(SyntaxKind.ColonToken);
        }

        public static SyntaxToken SemicolonToken()
        {
            return Token(SyntaxKind.SemicolonToken);
        }

        public static SyntaxToken DoubleQuoteToken()
        {
            return Token(SyntaxKind.DoubleQuoteToken);
        }

        public static SyntaxToken SingleQuoteToken()
        {
            return Token(SyntaxKind.SingleQuoteToken);
        }

        public static SyntaxToken LessThanToken()
        {
            return Token(SyntaxKind.LessThanToken);
        }

        public static SyntaxToken CommaToken()
        {
            return Token(SyntaxKind.CommaToken);
        }

        public static SyntaxToken GreaterThanToken()
        {
            return Token(SyntaxKind.GreaterThanToken);
        }

        public static SyntaxToken DotToken()
        {
            return Token(SyntaxKind.DotToken);
        }

        public static SyntaxToken QuestionToken()
        {
            return Token(SyntaxKind.QuestionToken);
        }

        public static SyntaxToken HashToken()
        {
            return Token(SyntaxKind.HashToken);
        }

        public static SyntaxToken SlashToken()
        {
            return Token(SyntaxKind.SlashToken);
        }

        public static SyntaxToken SlashGreaterThanToken()
        {
            return Token(SyntaxKind.SlashGreaterThanToken);
        }

        public static SyntaxToken LessThanSlashToken()
        {
            return Token(SyntaxKind.LessThanSlashToken);
        }

        public static SyntaxToken XmlCommentStartToken()
        {
            return Token(SyntaxKind.XmlCommentStartToken);
        }

        public static SyntaxToken XmlCommentEndToken()
        {
            return Token(SyntaxKind.XmlCommentEndToken);
        }

        public static SyntaxToken XmlCDataStartToken()
        {
            return Token(SyntaxKind.XmlCDataStartToken);
        }

        public static SyntaxToken XmlCDataEndToken()
        {
            return Token(SyntaxKind.XmlCDataEndToken);
        }

        public static SyntaxToken XmlProcessingInstructionStartToken()
        {
            return Token(SyntaxKind.XmlProcessingInstructionStartToken);
        }

        public static SyntaxToken XmlProcessingInstructionEndToken()
        {
            return Token(SyntaxKind.XmlProcessingInstructionEndToken);
        }

        public static SyntaxToken BarBarToken()
        {
            return Token(SyntaxKind.BarBarToken);
        }

        public static SyntaxToken AmpersandAmpersandToken()
        {
            return Token(SyntaxKind.AmpersandAmpersandToken);
        }

        public static SyntaxToken MinusMinusToken()
        {
            return Token(SyntaxKind.MinusMinusToken);
        }

        public static SyntaxToken PlusPlusToken()
        {
            return Token(SyntaxKind.PlusPlusToken);
        }

        public static SyntaxToken ColonColonToken()
        {
            return Token(SyntaxKind.ColonColonToken);
        }

        public static SyntaxToken QuestionQuestionToken()
        {
            return Token(SyntaxKind.QuestionQuestionToken);
        }

        public static SyntaxToken MinusGreaterThanToken()
        {
            return Token(SyntaxKind.MinusGreaterThanToken);
        }

        public static SyntaxToken ExclamationEqualsToken()
        {
            return Token(SyntaxKind.ExclamationEqualsToken);
        }

        public static SyntaxToken EqualsEqualsToken()
        {
            return Token(SyntaxKind.EqualsEqualsToken);
        }

        public static SyntaxToken EqualsGreaterThanToken()
        {
            return Token(SyntaxKind.EqualsGreaterThanToken);
        }

        public static SyntaxToken LessThanEqualsToken()
        {
            return Token(SyntaxKind.LessThanEqualsToken);
        }

        public static SyntaxToken LessThanLessThanToken()
        {
            return Token(SyntaxKind.LessThanLessThanToken);
        }

        public static SyntaxToken LessThanLessThanEqualsToken()
        {
            return Token(SyntaxKind.LessThanLessThanEqualsToken);
        }

        public static SyntaxToken GreaterThanEqualsToken()
        {
            return Token(SyntaxKind.GreaterThanEqualsToken);
        }

        public static SyntaxToken GreaterThanGreaterThanToken()
        {
            return Token(SyntaxKind.GreaterThanGreaterThanToken);
        }

        public static SyntaxToken GreaterThanGreaterThanEqualsToken()
        {
            return Token(SyntaxKind.GreaterThanGreaterThanEqualsToken);
        }

        public static SyntaxToken SlashEqualsToken()
        {
            return Token(SyntaxKind.SlashEqualsToken);
        }

        public static SyntaxToken AsteriskEqualsToken()
        {
            return Token(SyntaxKind.AsteriskEqualsToken);
        }

        public static SyntaxToken BarEqualsToken()
        {
            return Token(SyntaxKind.BarEqualsToken);
        }

        public static SyntaxToken AmpersandEqualsToken()
        {
            return Token(SyntaxKind.AmpersandEqualsToken);
        }

        public static SyntaxToken PlusEqualsToken()
        {
            return Token(SyntaxKind.PlusEqualsToken);
        }

        public static SyntaxToken MinusEqualsToken()
        {
            return Token(SyntaxKind.MinusEqualsToken);
        }

        public static SyntaxToken CaretEqualsToken()
        {
            return Token(SyntaxKind.CaretEqualsToken);
        }

        public static SyntaxToken PercentEqualsToken()
        {
            return Token(SyntaxKind.PercentEqualsToken);
        }

        public static SyntaxToken BoolKeyword()
        {
            return Token(SyntaxKind.BoolKeyword);
        }

        public static SyntaxToken ByteKeyword()
        {
            return Token(SyntaxKind.ByteKeyword);
        }

        public static SyntaxToken SByteKeyword()
        {
            return Token(SyntaxKind.SByteKeyword);
        }

        public static SyntaxToken ShortKeyword()
        {
            return Token(SyntaxKind.ShortKeyword);
        }

        public static SyntaxToken UShortKeyword()
        {
            return Token(SyntaxKind.UShortKeyword);
        }

        public static SyntaxToken IntKeyword()
        {
            return Token(SyntaxKind.IntKeyword);
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

        public static SyntaxToken DoubleKeyword()
        {
            return Token(SyntaxKind.DoubleKeyword);
        }

        public static SyntaxToken FloatKeyword()
        {
            return Token(SyntaxKind.FloatKeyword);
        }

        public static SyntaxToken DecimalKeyword()
        {
            return Token(SyntaxKind.DecimalKeyword);
        }

        public static SyntaxToken StringKeyword()
        {
            return Token(SyntaxKind.StringKeyword);
        }

        public static SyntaxToken CharKeyword()
        {
            return Token(SyntaxKind.CharKeyword);
        }

        public static SyntaxToken VoidKeyword()
        {
            return Token(SyntaxKind.VoidKeyword);
        }

        public static SyntaxToken ObjectKeyword()
        {
            return Token(SyntaxKind.ObjectKeyword);
        }

        public static SyntaxToken TypeOfKeyword()
        {
            return Token(SyntaxKind.TypeOfKeyword);
        }

        public static SyntaxToken SizeOfKeyword()
        {
            return Token(SyntaxKind.SizeOfKeyword);
        }

        public static SyntaxToken NullKeyword()
        {
            return Token(SyntaxKind.NullKeyword);
        }

        public static SyntaxToken TrueKeyword()
        {
            return Token(SyntaxKind.TrueKeyword);
        }

        public static SyntaxToken FalseKeyword()
        {
            return Token(SyntaxKind.FalseKeyword);
        }

        public static SyntaxToken IfKeyword()
        {
            return Token(SyntaxKind.IfKeyword);
        }

        public static SyntaxToken ElseKeyword()
        {
            return Token(SyntaxKind.ElseKeyword);
        }

        public static SyntaxToken WhileKeyword()
        {
            return Token(SyntaxKind.WhileKeyword);
        }

        public static SyntaxToken ForKeyword()
        {
            return Token(SyntaxKind.ForKeyword);
        }

        public static SyntaxToken ForEachKeyword()
        {
            return Token(SyntaxKind.ForEachKeyword);
        }

        public static SyntaxToken DoKeyword()
        {
            return Token(SyntaxKind.DoKeyword);
        }

        public static SyntaxToken SwitchKeyword()
        {
            return Token(SyntaxKind.SwitchKeyword);
        }

        public static SyntaxToken CaseKeyword()
        {
            return Token(SyntaxKind.CaseKeyword);
        }

        public static SyntaxToken DefaultKeyword()
        {
            return Token(SyntaxKind.DefaultKeyword);
        }

        public static SyntaxToken TryKeyword()
        {
            return Token(SyntaxKind.TryKeyword);
        }

        public static SyntaxToken CatchKeyword()
        {
            return Token(SyntaxKind.CatchKeyword);
        }

        public static SyntaxToken FinallyKeyword()
        {
            return Token(SyntaxKind.FinallyKeyword);
        }

        public static SyntaxToken LockKeyword()
        {
            return Token(SyntaxKind.LockKeyword);
        }

        public static SyntaxToken GotoKeyword()
        {
            return Token(SyntaxKind.GotoKeyword);
        }

        public static SyntaxToken BreakKeyword()
        {
            return Token(SyntaxKind.BreakKeyword);
        }

        public static SyntaxToken ContinueKeyword()
        {
            return Token(SyntaxKind.ContinueKeyword);
        }

        public static SyntaxToken ReturnKeyword()
        {
            return Token(SyntaxKind.ReturnKeyword);
        }

        public static SyntaxToken ThrowKeyword()
        {
            return Token(SyntaxKind.ThrowKeyword);
        }

        public static SyntaxToken PublicKeyword()
        {
            return Token(SyntaxKind.PublicKeyword);
        }

        public static SyntaxToken PrivateKeyword()
        {
            return Token(SyntaxKind.PrivateKeyword);
        }

        public static SyntaxToken InternalKeyword()
        {
            return Token(SyntaxKind.InternalKeyword);
        }

        public static SyntaxToken ProtectedKeyword()
        {
            return Token(SyntaxKind.ProtectedKeyword);
        }

        public static SyntaxToken StaticKeyword()
        {
            return Token(SyntaxKind.StaticKeyword);
        }

        public static SyntaxToken ReadOnlyKeyword()
        {
            return Token(SyntaxKind.ReadOnlyKeyword);
        }

        public static SyntaxToken SealedKeyword()
        {
            return Token(SyntaxKind.SealedKeyword);
        }

        public static SyntaxToken ConstKeyword()
        {
            return Token(SyntaxKind.ConstKeyword);
        }

        public static SyntaxToken FixedKeyword()
        {
            return Token(SyntaxKind.FixedKeyword);
        }

        public static SyntaxToken StackAllocKeyword()
        {
            return Token(SyntaxKind.StackAllocKeyword);
        }

        public static SyntaxToken VolatileKeyword()
        {
            return Token(SyntaxKind.VolatileKeyword);
        }

        public static SyntaxToken NewKeyword()
        {
            return Token(SyntaxKind.NewKeyword);
        }

        public static SyntaxToken OverrideKeyword()
        {
            return Token(SyntaxKind.OverrideKeyword);
        }

        public static SyntaxToken AbstractKeyword()
        {
            return Token(SyntaxKind.AbstractKeyword);
        }

        public static SyntaxToken VirtualKeyword()
        {
            return Token(SyntaxKind.VirtualKeyword);
        }

        public static SyntaxToken EventKeyword()
        {
            return Token(SyntaxKind.EventKeyword);
        }

        public static SyntaxToken ExternKeyword()
        {
            return Token(SyntaxKind.ExternKeyword);
        }

        public static SyntaxToken RefKeyword()
        {
            return Token(SyntaxKind.RefKeyword);
        }

        public static SyntaxToken OutKeyword()
        {
            return Token(SyntaxKind.OutKeyword);
        }

        public static SyntaxToken InKeyword()
        {
            return Token(SyntaxKind.InKeyword);
        }

        public static SyntaxToken IsKeyword()
        {
            return Token(SyntaxKind.IsKeyword);
        }

        public static SyntaxToken AsKeyword()
        {
            return Token(SyntaxKind.AsKeyword);
        }

        public static SyntaxToken ParamsKeyword()
        {
            return Token(SyntaxKind.ParamsKeyword);
        }

        public static SyntaxToken ArgListKeyword()
        {
            return Token(SyntaxKind.ArgListKeyword);
        }

        public static SyntaxToken MakeRefKeyword()
        {
            return Token(SyntaxKind.MakeRefKeyword);
        }

        public static SyntaxToken RefTypeKeyword()
        {
            return Token(SyntaxKind.RefTypeKeyword);
        }

        public static SyntaxToken RefValueKeyword()
        {
            return Token(SyntaxKind.RefValueKeyword);
        }

        public static SyntaxToken ThisKeyword()
        {
            return Token(SyntaxKind.ThisKeyword);
        }

        public static SyntaxToken BaseKeyword()
        {
            return Token(SyntaxKind.BaseKeyword);
        }

        public static SyntaxToken NamespaceKeyword()
        {
            return Token(SyntaxKind.NamespaceKeyword);
        }

        public static SyntaxToken UsingKeyword()
        {
            return Token(SyntaxKind.UsingKeyword);
        }

        public static SyntaxToken ClassKeyword()
        {
            return Token(SyntaxKind.ClassKeyword);
        }

        public static SyntaxToken StructKeyword()
        {
            return Token(SyntaxKind.StructKeyword);
        }

        public static SyntaxToken InterfaceKeyword()
        {
            return Token(SyntaxKind.InterfaceKeyword);
        }

        public static SyntaxToken EnumKeyword()
        {
            return Token(SyntaxKind.EnumKeyword);
        }

        public static SyntaxToken DelegateKeyword()
        {
            return Token(SyntaxKind.DelegateKeyword);
        }

        public static SyntaxToken CheckedKeyword()
        {
            return Token(SyntaxKind.CheckedKeyword);
        }

        public static SyntaxToken UncheckedKeyword()
        {
            return Token(SyntaxKind.UncheckedKeyword);
        }

        public static SyntaxToken UnsafeKeyword()
        {
            return Token(SyntaxKind.UnsafeKeyword);
        }

        public static SyntaxToken OperatorKeyword()
        {
            return Token(SyntaxKind.OperatorKeyword);
        }

        public static SyntaxToken ExplicitKeyword()
        {
            return Token(SyntaxKind.ExplicitKeyword);
        }

        public static SyntaxToken ImplicitKeyword()
        {
            return Token(SyntaxKind.ImplicitKeyword);
        }

        public static SyntaxToken YieldKeyword()
        {
            return Token(SyntaxKind.YieldKeyword);
        }

        public static SyntaxToken PartialKeyword()
        {
            return Token(SyntaxKind.PartialKeyword);
        }

        public static SyntaxToken AliasKeyword()
        {
            return Token(SyntaxKind.AliasKeyword);
        }

        public static SyntaxToken GlobalKeyword()
        {
            return Token(SyntaxKind.GlobalKeyword);
        }

        public static SyntaxToken AssemblyKeyword()
        {
            return Token(SyntaxKind.AssemblyKeyword);
        }

        public static SyntaxToken ModuleKeyword()
        {
            return Token(SyntaxKind.ModuleKeyword);
        }

        public static SyntaxToken TypeKeyword()
        {
            return Token(SyntaxKind.TypeKeyword);
        }

        public static SyntaxToken FieldKeyword()
        {
            return Token(SyntaxKind.FieldKeyword);
        }

        public static SyntaxToken MethodKeyword()
        {
            return Token(SyntaxKind.MethodKeyword);
        }

        public static SyntaxToken ParamKeyword()
        {
            return Token(SyntaxKind.ParamKeyword);
        }

        public static SyntaxToken PropertyKeyword()
        {
            return Token(SyntaxKind.PropertyKeyword);
        }

        public static SyntaxToken TypeVarKeyword()
        {
            return Token(SyntaxKind.TypeVarKeyword);
        }

        public static SyntaxToken GetKeyword()
        {
            return Token(SyntaxKind.GetKeyword);
        }

        public static SyntaxToken SetKeyword()
        {
            return Token(SyntaxKind.SetKeyword);
        }

        public static SyntaxToken AddKeyword()
        {
            return Token(SyntaxKind.AddKeyword);
        }

        public static SyntaxToken RemoveKeyword()
        {
            return Token(SyntaxKind.RemoveKeyword);
        }

        public static SyntaxToken WhereKeyword()
        {
            return Token(SyntaxKind.WhereKeyword);
        }

        public static SyntaxToken FromKeyword()
        {
            return Token(SyntaxKind.FromKeyword);
        }

        public static SyntaxToken GroupKeyword()
        {
            return Token(SyntaxKind.GroupKeyword);
        }

        public static SyntaxToken JoinKeyword()
        {
            return Token(SyntaxKind.JoinKeyword);
        }

        public static SyntaxToken IntoKeyword()
        {
            return Token(SyntaxKind.IntoKeyword);
        }

        public static SyntaxToken LetKeyword()
        {
            return Token(SyntaxKind.LetKeyword);
        }

        public static SyntaxToken ByKeyword()
        {
            return Token(SyntaxKind.ByKeyword);
        }

        public static SyntaxToken SelectKeyword()
        {
            return Token(SyntaxKind.SelectKeyword);
        }

        public static SyntaxToken OrderByKeyword()
        {
            return Token(SyntaxKind.OrderByKeyword);
        }

        public static SyntaxToken OnKeyword()
        {
            return Token(SyntaxKind.OnKeyword);
        }

        public static SyntaxToken EqualsKeyword()
        {
            return Token(SyntaxKind.EqualsKeyword);
        }

        public static SyntaxToken AscendingKeyword()
        {
            return Token(SyntaxKind.AscendingKeyword);
        }

        public static SyntaxToken DescendingKeyword()
        {
            return Token(SyntaxKind.DescendingKeyword);
        }

        public static SyntaxToken NameOfKeyword()
        {
            return Token(SyntaxKind.NameOfKeyword);
        }

        public static SyntaxToken AsyncKeyword()
        {
            return Token(SyntaxKind.AsyncKeyword);
        }

        public static SyntaxToken AwaitKeyword()
        {
            return Token(SyntaxKind.AwaitKeyword);
        }

        public static SyntaxToken WhenKeyword()
        {
            return Token(SyntaxKind.WhenKeyword);
        }

        public static SyntaxToken ElifKeyword()
        {
            return Token(SyntaxKind.ElifKeyword);
        }

        public static SyntaxToken EndIfKeyword()
        {
            return Token(SyntaxKind.EndIfKeyword);
        }

        public static SyntaxToken RegionKeyword()
        {
            return Token(SyntaxKind.RegionKeyword);
        }

        public static SyntaxToken EndRegionKeyword()
        {
            return Token(SyntaxKind.EndRegionKeyword);
        }

        public static SyntaxToken DefineKeyword()
        {
            return Token(SyntaxKind.DefineKeyword);
        }

        public static SyntaxToken UndefKeyword()
        {
            return Token(SyntaxKind.UndefKeyword);
        }

        public static SyntaxToken WarningKeyword()
        {
            return Token(SyntaxKind.WarningKeyword);
        }

        public static SyntaxToken ErrorKeyword()
        {
            return Token(SyntaxKind.ErrorKeyword);
        }

        public static SyntaxToken LineKeyword()
        {
            return Token(SyntaxKind.LineKeyword);
        }

        public static SyntaxToken PragmaKeyword()
        {
            return Token(SyntaxKind.PragmaKeyword);
        }

        public static SyntaxToken HiddenKeyword()
        {
            return Token(SyntaxKind.HiddenKeyword);
        }

        public static SyntaxToken ChecksumKeyword()
        {
            return Token(SyntaxKind.ChecksumKeyword);
        }

        public static SyntaxToken DisableKeyword()
        {
            return Token(SyntaxKind.DisableKeyword);
        }

        public static SyntaxToken RestoreKeyword()
        {
            return Token(SyntaxKind.RestoreKeyword);
        }

        public static SyntaxToken ReferenceKeyword()
        {
            return Token(SyntaxKind.ReferenceKeyword);
        }

        public static SyntaxToken LoadKeyword()
        {
            return Token(SyntaxKind.LoadKeyword);
        }

        public static SyntaxToken InterpolatedStringStartToken()
        {
            return Token(SyntaxKind.InterpolatedStringStartToken);
        }

        public static SyntaxToken InterpolatedStringEndToken()
        {
            return Token(SyntaxKind.InterpolatedStringEndToken);
        }

        public static SyntaxToken InterpolatedVerbatimStringStartToken()
        {
            return Token(SyntaxKind.InterpolatedVerbatimStringStartToken);
        }

        public static SyntaxToken UnderscoreToken()
        {
            return Token(SyntaxKind.UnderscoreToken);
        }

        public static SyntaxToken OmittedTypeArgumentToken()
        {
            return Token(SyntaxKind.OmittedTypeArgumentToken);
        }

        public static SyntaxToken OmittedArraySizeExpressionToken()
        {
            return Token(SyntaxKind.OmittedArraySizeExpressionToken);
        }

        public static SyntaxToken EndOfDirectiveToken()
        {
            return Token(SyntaxKind.EndOfDirectiveToken);
        }

        public static SyntaxToken EndOfDocumentationCommentToken()
        {
            return Token(SyntaxKind.EndOfDocumentationCommentToken);
        }

        public static SyntaxToken EndOfFileToken()
        {
            return Token(SyntaxKind.EndOfFileToken);
        }

        private static SyntaxToken Token(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.Token(syntaxKind);
        }
        #endregion Token

        #region Type
        public static PredefinedTypeSyntax PredefinedBoolType()
        {
            return PredefinedType(SyntaxKind.BoolKeyword);
        }

        public static PredefinedTypeSyntax PredefinedByteType()
        {
            return PredefinedType(SyntaxKind.ByteKeyword);
        }

        public static PredefinedTypeSyntax PredefinedSByteType()
        {
            return PredefinedType(SyntaxKind.SByteKeyword);
        }

        public static PredefinedTypeSyntax PredefinedIntType()
        {
            return PredefinedType(SyntaxKind.IntKeyword);
        }

        public static PredefinedTypeSyntax PredefinedUIntType()
        {
            return PredefinedType(SyntaxKind.UIntKeyword);
        }

        public static PredefinedTypeSyntax PredefinedShortType()
        {
            return PredefinedType(SyntaxKind.ShortKeyword);
        }

        public static PredefinedTypeSyntax PredefinedUShortType()
        {
            return PredefinedType(SyntaxKind.UShortKeyword);
        }

        public static PredefinedTypeSyntax PredefinedLongType()
        {
            return PredefinedType(SyntaxKind.LongKeyword);
        }

        public static PredefinedTypeSyntax PredefinedULongType()
        {
            return PredefinedType(SyntaxKind.ULongKeyword);
        }

        public static PredefinedTypeSyntax PredefinedFloatType()
        {
            return PredefinedType(SyntaxKind.FloatKeyword);
        }

        public static PredefinedTypeSyntax PredefinedDoubleType()
        {
            return PredefinedType(SyntaxKind.DoubleKeyword);
        }

        public static PredefinedTypeSyntax PredefinedDecimalType()
        {
            return PredefinedType(SyntaxKind.DecimalKeyword);
        }

        public static PredefinedTypeSyntax PredefinedStringType()
        {
            return PredefinedType(SyntaxKind.StringKeyword);
        }

        public static PredefinedTypeSyntax PredefinedCharType()
        {
            return PredefinedType(SyntaxKind.CharKeyword);
        }

        public static PredefinedTypeSyntax PredefinedObjectType()
        {
            return PredefinedType(SyntaxKind.ObjectKeyword);
        }

        public static PredefinedTypeSyntax VoidType()
        {
            return PredefinedType(SyntaxKind.VoidKeyword);
        }

        private static PredefinedTypeSyntax PredefinedType(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.PredefinedType(Token(syntaxKind));
        }

        #endregion Type

        #region List
        public static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments)
        {
            return SyntaxFactory.ArgumentList(SeparatedList(arguments));
        }

        public static ArgumentListSyntax ArgumentList(ArgumentSyntax argument)
        {
            return SyntaxFactory.ArgumentList(SingletonSeparatedList(argument));
        }

        public static BracketedArgumentListSyntax BracketedArgumentList(params ArgumentSyntax[] arguments)
        {
            return SyntaxFactory.BracketedArgumentList(SeparatedList(arguments));
        }

        public static BracketedArgumentListSyntax BracketedArgumentList(ArgumentSyntax argument)
        {
            return SyntaxFactory.BracketedArgumentList(SingletonSeparatedList(argument));
        }

        public static AttributeListSyntax AttributeList(AttributeSyntax attribute)
        {
            return SyntaxFactory.AttributeList(SingletonSeparatedList(attribute));
        }

        public static AttributeListSyntax AttributeList(params AttributeSyntax[] attributes)
        {
            return SyntaxFactory.AttributeList(SeparatedList(attributes));
        }

        public static AttributeArgumentListSyntax AttributeArgumentList(params AttributeArgumentSyntax[] attributeArguments)
        {
            return SyntaxFactory.AttributeArgumentList(SeparatedList(attributeArguments));
        }

        public static AttributeArgumentListSyntax AttributeArgumentList(AttributeArgumentSyntax attributeArgument)
        {
            return SyntaxFactory.AttributeArgumentList(SingletonSeparatedList(attributeArgument));
        }

        public static AccessorListSyntax AccessorList(params AccessorDeclarationSyntax[] accessors)
        {
            return SyntaxFactory.AccessorList(List(accessors));
        }

        public static AccessorListSyntax AccessorList(AccessorDeclarationSyntax accessor)
        {
            return SyntaxFactory.AccessorList(SingletonList(accessor));
        }

        public static ParameterListSyntax ParameterList(ParameterSyntax parameter)
        {
            return SyntaxFactory.ParameterList(SingletonSeparatedList(parameter));
        }

        public static ParameterListSyntax ParameterList(params ParameterSyntax[] parameters)
        {
            return SyntaxFactory.ParameterList(SeparatedList(parameters));
        }

        public static BracketedParameterListSyntax BracketedParameterList(ParameterSyntax parameter)
        {
            return SyntaxFactory.BracketedParameterList(SingletonSeparatedList(parameter));
        }

        public static BracketedParameterListSyntax BracketedParameterList(params ParameterSyntax[] parameters)
        {
            return SyntaxFactory.BracketedParameterList(SeparatedList(parameters));
        }

        public static TypeArgumentListSyntax TypeArgumentList(TypeSyntax argument)
        {
            return SyntaxFactory.TypeArgumentList(SingletonSeparatedList(argument));
        }

        public static TypeArgumentListSyntax TypeArgumentList(params TypeSyntax[] arguments)
        {
            return SyntaxFactory.TypeArgumentList(SeparatedList(arguments));
        }

        public static TypeParameterListSyntax TypeParameterList(TypeParameterSyntax parameter)
        {
            return SyntaxFactory.TypeParameterList(SingletonSeparatedList(parameter));
        }

        public static TypeParameterListSyntax TypeParameterList(params TypeParameterSyntax[] parameters)
        {
            return SyntaxFactory.TypeParameterList(SeparatedList(parameters));
        }

        public static BaseListSyntax BaseList(BaseTypeSyntax type)
        {
            return SyntaxFactory.BaseList(SingletonSeparatedList(type));
        }

        public static BaseListSyntax BaseList(params BaseTypeSyntax[] types)
        {
            return SyntaxFactory.BaseList(SeparatedList(types));
        }

        public static BaseListSyntax BaseList(SyntaxToken colonToken, BaseTypeSyntax baseType)
        {
            return SyntaxFactory.BaseList(colonToken, SingletonSeparatedList(baseType));
        }

        public static BaseListSyntax BaseList(SyntaxToken colonToken, params BaseTypeSyntax[] types)
        {
            return SyntaxFactory.BaseList(colonToken, SeparatedList(types));
        }
        #endregion List

        #region MemberDeclaration

        internal static NamespaceDeclarationSyntax NamespaceDeclaration(string name, MemberDeclarationSyntax member)
        {
            return NamespaceDeclaration(ParseName(name), member);
        }

        public static NamespaceDeclarationSyntax NamespaceDeclaration(NameSyntax name, MemberDeclarationSyntax member)
        {
            return NamespaceDeclaration(name, SingletonList(member));
        }

        internal static NamespaceDeclarationSyntax NamespaceDeclaration(string name, SyntaxList<MemberDeclarationSyntax> members)
        {
            return NamespaceDeclaration(ParseName(name), members);
        }

        public static NamespaceDeclarationSyntax NamespaceDeclaration(NameSyntax name, SyntaxList<MemberDeclarationSyntax> members)
        {
            return SyntaxFactory.NamespaceDeclaration(
                name,
                default(SyntaxList<ExternAliasDirectiveSyntax>),
                default(SyntaxList<UsingDirectiveSyntax>),
                members);
        }

        public static ClassDeclarationSyntax ClassDeclaration(SyntaxTokenList modifiers, string identifier, SyntaxList<MemberDeclarationSyntax> members = default(SyntaxList<MemberDeclarationSyntax>))
        {
            return ClassDeclaration(modifiers, Identifier(identifier), members);
        }

        public static ClassDeclarationSyntax ClassDeclaration(SyntaxTokenList modifiers, SyntaxToken identifier, SyntaxList<MemberDeclarationSyntax> members = default(SyntaxList<MemberDeclarationSyntax>))
        {
            return SyntaxFactory.ClassDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                identifier,
                default(TypeParameterListSyntax),
                default(BaseListSyntax),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                members);
        }

        internal static ClassDeclarationSyntax ClassDeclaration(StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            SyntaxToken keyword = structDeclaration.Keyword;

            return SyntaxFactory.ClassDeclaration(
                structDeclaration.AttributeLists,
                structDeclaration.Modifiers,
                SyntaxFactory.Token(keyword.LeadingTrivia, SyntaxKind.ClassKeyword, keyword.TrailingTrivia),
                structDeclaration.Identifier,
                structDeclaration.TypeParameterList,
                structDeclaration.BaseList,
                structDeclaration.ConstraintClauses,
                structDeclaration.OpenBraceToken,
                structDeclaration.Members,
                structDeclaration.CloseBraceToken,
                structDeclaration.SemicolonToken);
        }

        public static StructDeclarationSyntax StructDeclaration(SyntaxTokenList modifiers, string identifier, SyntaxList<MemberDeclarationSyntax> members = default(SyntaxList<MemberDeclarationSyntax>))
        {
            return StructDeclaration(modifiers, Identifier(identifier), members);
        }

        public static StructDeclarationSyntax StructDeclaration(SyntaxTokenList modifiers, SyntaxToken identifier, SyntaxList<MemberDeclarationSyntax> members = default(SyntaxList<MemberDeclarationSyntax>))
        {
            return SyntaxFactory.StructDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                identifier,
                default(TypeParameterListSyntax),
                default(BaseListSyntax),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                members);
        }

        internal static StructDeclarationSyntax StructDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            SyntaxToken keyword = classDeclaration.Keyword;

            return SyntaxFactory.StructDeclaration(
                classDeclaration.AttributeLists,
                classDeclaration.Modifiers,
                SyntaxFactory.Token(keyword.LeadingTrivia, SyntaxKind.StructKeyword, keyword.TrailingTrivia),
                classDeclaration.Identifier,
                classDeclaration.TypeParameterList,
                classDeclaration.BaseList,
                classDeclaration.ConstraintClauses,
                classDeclaration.OpenBraceToken,
                classDeclaration.Members,
                classDeclaration.CloseBraceToken,
                classDeclaration.SemicolonToken);
        }

        public static InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxTokenList modifiers, string identifier, SyntaxList<MemberDeclarationSyntax> members = default(SyntaxList<MemberDeclarationSyntax>))
        {
            return InterfaceDeclaration(modifiers, Identifier(identifier), members);
        }

        public static InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxTokenList modifiers, SyntaxToken identifier, SyntaxList<MemberDeclarationSyntax> members = default(SyntaxList<MemberDeclarationSyntax>))
        {
            return SyntaxFactory.InterfaceDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                identifier,
                default(TypeParameterListSyntax),
                default(BaseListSyntax),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                members);
        }

        public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxTokenList modifiers, SyntaxToken identifier, ParameterListSyntax parameterList, BlockSyntax body)
        {
            return SyntaxFactory.ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                identifier,
                parameterList,
                default(ConstructorInitializerSyntax),
                body);
        }

        public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxTokenList modifiers, SyntaxToken identifier, ParameterListSyntax parameterList, ArrowExpressionClauseSyntax expressionBody)
        {
            return SyntaxFactory.ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                identifier,
                parameterList,
                default(ConstructorInitializerSyntax),
                expressionBody);
        }

        public static EnumDeclarationSyntax EnumDeclaration(SyntaxTokenList modifiers, SyntaxToken identifier, SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
        {
            return SyntaxFactory.EnumDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                identifier,
                default(BaseListSyntax),
                members);
        }

        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(string name, ExpressionSyntax value)
        {
            return EnumMemberDeclaration(Identifier(name), value);
        }

        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxToken identifier, ExpressionSyntax value)
        {
            return EnumMemberDeclaration(identifier, EqualsValueClause(value));
        }

        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(string name, EqualsValueClauseSyntax value)
        {
            return EnumMemberDeclaration(Identifier(name), value);
        }

        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxToken identifier, EqualsValueClauseSyntax value)
        {
            return SyntaxFactory.EnumMemberDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                identifier,
                value);
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, string identifier, ExpressionSyntax value = null)
        {
            return FieldDeclaration(
                modifiers,
                type,
                Identifier(identifier),
                value);
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, string identifier, EqualsValueClauseSyntax initializer)
        {
            return FieldDeclaration(
                modifiers,
                type,
                Identifier(identifier),
                initializer);
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, SyntaxToken identifier, ExpressionSyntax value = null)
        {
            return FieldDeclaration(
                modifiers,
                type,
                identifier,
                (value != null) ? EqualsValueClause(value) : default(EqualsValueClauseSyntax));
        }

        public static FieldDeclarationSyntax FieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax initializer)
        {
            return SyntaxFactory.FieldDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                VariableDeclaration(
                    type,
                    identifier,
                    initializer));
        }

        public static EventFieldDeclarationSyntax EventFieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, string identifier)
        {
            return EventFieldDeclaration(
                modifiers,
                type,
                Identifier(identifier));
        }

        public static EventFieldDeclarationSyntax EventFieldDeclaration(SyntaxTokenList modifiers, TypeSyntax type, SyntaxToken identifier)
        {
            return SyntaxFactory.EventFieldDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                VariableDeclaration(type, identifier));
        }

        public static EventDeclarationSyntax EventDeclaration(SyntaxTokenList modifiers, TypeSyntax type, string identifier, AccessorListSyntax accessorList)
        {
            return EventDeclaration(
                modifiers,
                type,
                Identifier(identifier),
                accessorList);
        }

        public static EventDeclarationSyntax EventDeclaration(SyntaxTokenList modifiers, TypeSyntax type, SyntaxToken identifier, AccessorListSyntax accessorList)
        {
            return SyntaxFactory.EventDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                type,
                default(ExplicitInterfaceSpecifierSyntax),
                identifier,
                accessorList);
        }

        public static DelegateDeclarationSyntax DelegateDeclaration(SyntaxTokenList modifiers, TypeSyntax returnType, string identifier, ParameterListSyntax parameterList)
        {
            return DelegateDeclaration(
                modifiers,
                returnType,
                Identifier(identifier),
                parameterList);
        }

        public static DelegateDeclarationSyntax DelegateDeclaration(SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken identifier, ParameterListSyntax parameterList)
        {
            return SyntaxFactory.DelegateDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                returnType,
                identifier,
                default(TypeParameterListSyntax),
                parameterList,
                default(SyntaxList<TypeParameterConstraintClauseSyntax>));
        }

        public static MethodDeclarationSyntax MethodDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax returnType,
    SyntaxToken identifier,
    ParameterListSyntax parameterList,
    BlockSyntax body)
        {
            return SyntaxFactory.MethodDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                returnType,
                default(ExplicitInterfaceSpecifierSyntax),
                identifier,
                default(TypeParameterListSyntax),
                parameterList,
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                body,
                default(ArrowExpressionClauseSyntax));
        }

        public static MethodDeclarationSyntax MethodDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax returnType,
    SyntaxToken identifier,
    ParameterListSyntax parameterList,
    ArrowExpressionClauseSyntax expressionBody)
        {
            return SyntaxFactory.MethodDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                returnType,
                default(ExplicitInterfaceSpecifierSyntax),
                identifier,
                default(TypeParameterListSyntax),
                parameterList,
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                default(BlockSyntax),
                expressionBody);
        }

        public static OperatorDeclarationSyntax OperatorDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax returnType,
    SyntaxToken operatorToken,
    ParameterListSyntax parameterList,
    BlockSyntax body)
        {
            return SyntaxFactory.OperatorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                returnType,
                operatorToken,
                parameterList,
                body,
                default(ArrowExpressionClauseSyntax));
        }

        public static OperatorDeclarationSyntax OperatorDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax returnType,
    SyntaxToken operatorToken,
    ParameterListSyntax parameterList,
    ArrowExpressionClauseSyntax expressionBody)
        {
            return SyntaxFactory.OperatorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                returnType,
                operatorToken,
                parameterList,
                default(BlockSyntax),
                expressionBody);
        }

        public static ConversionOperatorDeclarationSyntax ImplicitConversionOperatorDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    ParameterListSyntax parameterList,
    BlockSyntax body)
        {
            return SyntaxFactory.ConversionOperatorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                ImplicitKeyword(),
                type,
                parameterList,
                body,
                default(ArrowExpressionClauseSyntax));
        }

        public static ConversionOperatorDeclarationSyntax ImplicitConversionOperatorDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    ParameterListSyntax parameterList,
    ArrowExpressionClauseSyntax expressionBody)
        {
            return SyntaxFactory.ConversionOperatorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                ImplicitKeyword(),
                type,
                parameterList,
                default(BlockSyntax),
                expressionBody);
        }

        public static ConversionOperatorDeclarationSyntax ExplicitConversionOperatorDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    ParameterListSyntax parameterList,
    BlockSyntax body)
        {
            return SyntaxFactory.ConversionOperatorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                ExplicitKeyword(),
                type,
                parameterList,
                body,
                default(ArrowExpressionClauseSyntax));
        }

        public static ConversionOperatorDeclarationSyntax ExplicitConversionOperatorDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    ParameterListSyntax parameterList,
    ArrowExpressionClauseSyntax expressionBody)
        {
            return SyntaxFactory.ConversionOperatorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                ExplicitKeyword(),
                type,
                parameterList,
                default(BlockSyntax),
                expressionBody);
        }

        public static PropertyDeclarationSyntax PropertyDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    SyntaxToken identifier,
    AccessorListSyntax accessorList,
    ExpressionSyntax value = null)
        {
            return SyntaxFactory.PropertyDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                type,
                default(ExplicitInterfaceSpecifierSyntax),
                identifier,
                accessorList,
                default(ArrowExpressionClauseSyntax),
                (value != null) ? EqualsValueClause(value) : default(EqualsValueClauseSyntax),
                (value != null) ? SemicolonToken() : default(SyntaxToken));
        }

        public static PropertyDeclarationSyntax PropertyDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    SyntaxToken identifier,
    ArrowExpressionClauseSyntax expressionBody)
        {
            return SyntaxFactory.PropertyDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                type,
                default(ExplicitInterfaceSpecifierSyntax),
                identifier,
                default(AccessorListSyntax),
                expressionBody,
                default(EqualsValueClauseSyntax));
        }

        public static IndexerDeclarationSyntax IndexerDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    BracketedParameterListSyntax parameterList,
    AccessorListSyntax accessorList)
        {
            return SyntaxFactory.IndexerDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                type,
                default(ExplicitInterfaceSpecifierSyntax),
                parameterList,
                accessorList);
        }

        public static IndexerDeclarationSyntax IndexerDeclaration(
    SyntaxTokenList modifiers,
    TypeSyntax type,
    BracketedParameterListSyntax parameterList,
    ArrowExpressionClauseSyntax expressionBody)
        {
            return SyntaxFactory.IndexerDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                type,
                default(ExplicitInterfaceSpecifierSyntax),
                parameterList,
                default(AccessorListSyntax),
                expressionBody);
        }
        #endregion MemberDeclaration

        #region AccessorDeclaration
        public static AccessorDeclarationSyntax GetAccessorDeclaration(BlockSyntax body)
        {
            return GetAccessorDeclaration(default(SyntaxTokenList), body);
        }

        public static AccessorDeclarationSyntax GetAccessorDeclaration(SyntaxTokenList modifiers, BlockSyntax body)
        {
            return AccessorDeclaration(
                SyntaxKind.GetAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                body);
        }

        public static AccessorDeclarationSyntax GetAccessorDeclaration(ArrowExpressionClauseSyntax expressionBody)
        {
            return GetAccessorDeclaration(default(SyntaxTokenList), expressionBody);
        }

        public static AccessorDeclarationSyntax GetAccessorDeclaration(SyntaxTokenList modifiers, ArrowExpressionClauseSyntax expressionBody)
        {
            return AccessorDeclaration(
                SyntaxKind.GetAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                expressionBody);
        }

        public static AccessorDeclarationSyntax SetAccessorDeclaration(BlockSyntax body)
        {
            return SetAccessorDeclaration(default(SyntaxTokenList), body);
        }

        public static AccessorDeclarationSyntax SetAccessorDeclaration(SyntaxTokenList modifiers, BlockSyntax body)
        {
            return AccessorDeclaration(
                SyntaxKind.SetAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                body);
        }

        public static AccessorDeclarationSyntax SetAccessorDeclaration(ArrowExpressionClauseSyntax expressionBody)
        {
            return SetAccessorDeclaration(default(SyntaxTokenList), expressionBody);
        }

        public static AccessorDeclarationSyntax SetAccessorDeclaration(SyntaxTokenList modifiers, ArrowExpressionClauseSyntax expressionBody)
        {
            return AccessorDeclaration(
                SyntaxKind.SetAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                expressionBody);
        }

        public static AccessorDeclarationSyntax AddAccessorDeclaration(BlockSyntax body)
        {
            return AddAccessorDeclaration(default(SyntaxTokenList), body);
        }

        public static AccessorDeclarationSyntax AddAccessorDeclaration(SyntaxTokenList modifiers, BlockSyntax body)
        {
            return AccessorDeclaration(
                SyntaxKind.AddAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                body);
        }

        public static AccessorDeclarationSyntax AddAccessorDeclaration(ArrowExpressionClauseSyntax expressionBody)
        {
            return AddAccessorDeclaration(default(SyntaxTokenList), expressionBody);
        }

        public static AccessorDeclarationSyntax AddAccessorDeclaration(SyntaxTokenList modifiers, ArrowExpressionClauseSyntax expressionBody)
        {
            return AccessorDeclaration(
                SyntaxKind.AddAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                expressionBody);
        }

        public static AccessorDeclarationSyntax RemoveAccessorDeclaration(BlockSyntax body)
        {
            return RemoveAccessorDeclaration(default(SyntaxTokenList), body);
        }

        public static AccessorDeclarationSyntax RemoveAccessorDeclaration(SyntaxTokenList modifiers, BlockSyntax body)
        {
            return AccessorDeclaration(
                SyntaxKind.RemoveAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                body);
        }

        public static AccessorDeclarationSyntax RemoveAccessorDeclaration(ArrowExpressionClauseSyntax expressionBody)
        {
            return RemoveAccessorDeclaration(default(SyntaxTokenList), expressionBody);
        }

        public static AccessorDeclarationSyntax RemoveAccessorDeclaration(SyntaxTokenList modifiers, ArrowExpressionClauseSyntax expressionBody)
        {
            return AccessorDeclaration(
                SyntaxKind.RemoveAccessorDeclaration,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                expressionBody);
        }

        public static AccessorDeclarationSyntax AutoGetAccessorDeclaration(SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AutoAccessorDeclaration(SyntaxKind.GetAccessorDeclaration, modifiers);
        }

        public static AccessorDeclarationSyntax AutoSetAccessorDeclaration(SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AutoAccessorDeclaration(SyntaxKind.SetAccessorDeclaration, modifiers);
        }

        private static AccessorDeclarationSyntax AutoAccessorDeclaration(SyntaxKind kind, SyntaxTokenList modifiers = default(SyntaxTokenList))
        {
            return AccessorDeclaration(
                kind,
                default(SyntaxList<AttributeListSyntax>),
                modifiers,
                Token(AccessorDeclarationKeywordKind(kind)),
                default(BlockSyntax),
                SemicolonToken());
        }

        private static SyntaxKind AccessorDeclarationKeywordKind(SyntaxKind kind)
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
        #endregion AccessorDeclaration

        #region Statement
        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(TypeSyntax type, string identifier, ExpressionSyntax value = null)
        {
            return LocalDeclarationStatement(type, Identifier(identifier), value);
        }

        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(TypeSyntax type, SyntaxToken identifier, ExpressionSyntax value = null)
        {
            VariableDeclaratorSyntax variableDeclarator = (value != null)
                ? VariableDeclarator(identifier, EqualsValueClause(value))
                : SyntaxFactory.VariableDeclarator(identifier);

            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    type,
                    SingletonSeparatedList(variableDeclarator)));
        }

        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(TypeSyntax type, string identifier, EqualsValueClauseSyntax initializer)
        {
            return LocalDeclarationStatement(type, Identifier(identifier), initializer);
        }

        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax initializer)
        {
            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    type,
                    SingletonSeparatedList(VariableDeclarator(identifier, initializer))));
        }

        public static YieldStatementSyntax YieldReturnStatement(ExpressionSyntax expression)
        {
            return YieldStatement(SyntaxKind.YieldReturnStatement, expression);
        }

        public static YieldStatementSyntax YieldBreakStatement()
        {
            return YieldStatement(SyntaxKind.YieldBreakStatement);
        }

        public static TryStatementSyntax TryStatement(BlockSyntax block, CatchClauseSyntax @catch, FinallyClauseSyntax @finally = null)
        {
            return SyntaxFactory.TryStatement(block, SingletonList(@catch), @finally);
        }

        public static ExpressionStatementSyntax SimpleAssignmentStatement(ExpressionSyntax left, ExpressionSyntax right)
        {
            return ExpressionStatement(SimpleAssignmentExpression(left, right));
        }

        public static ExpressionStatementSyntax SimpleAssignmentStatement(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return ExpressionStatement(SimpleAssignmentExpression(left, operatorToken, right));
        }

        public static BlockSyntax Block(StatementSyntax statement)
        {
            return SyntaxFactory.Block(SingletonList(statement));
        }

        public static BlockSyntax Block(SyntaxToken openBrace, StatementSyntax statement, SyntaxToken closeBrace)
        {
            return SyntaxFactory.Block(openBrace, SingletonList(statement), closeBrace);
        }
        #endregion Statement

        #region BinaryExpression
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

        public static BinaryExpressionSyntax MultiplyExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.MultiplyExpression, left, right);
        }

        public static BinaryExpressionSyntax MultiplyExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.MultiplyExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax DivideExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.DivideExpression, left, right);
        }

        public static BinaryExpressionSyntax DivideExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.DivideExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax ModuloExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.ModuloExpression, left, right);
        }

        public static BinaryExpressionSyntax ModuloExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.ModuloExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax LeftShiftExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LeftShiftExpression, left, right);
        }

        public static BinaryExpressionSyntax LeftShiftExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LeftShiftExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax RightShiftExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.RightShiftExpression, left, right);
        }

        public static BinaryExpressionSyntax RightShiftExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.RightShiftExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax LogicalOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalOrExpression, left, right);
        }

        public static BinaryExpressionSyntax LogicalOrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalOrExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax LogicalAndExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);
        }

        public static BinaryExpressionSyntax LogicalAndExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalAndExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax BitwiseOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseOrExpression, left, right);
        }

        public static BinaryExpressionSyntax BitwiseOrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseOrExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax BitwiseAndExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseAndExpression, left, right);
        }

        public static BinaryExpressionSyntax BitwiseAndExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.BitwiseAndExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax ExclusiveOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.ExclusiveOrExpression, left, right);
        }

        public static BinaryExpressionSyntax ExclusiveOrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.ExclusiveOrExpression, left, operatorToken, right);
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

        public static BinaryExpressionSyntax IsExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.IsExpression, left, right);
        }

        public static BinaryExpressionSyntax IsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.IsExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax AsExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.AsExpression, left, right);
        }

        public static BinaryExpressionSyntax AsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.AsExpression, left, operatorToken, right);
        }

        public static BinaryExpressionSyntax CoalesceExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.CoalesceExpression, left, right);
        }

        public static BinaryExpressionSyntax CoalesceExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.CoalesceExpression, left, operatorToken, right);
        }
        #endregion BinaryExpression

        #region PrefixUnaryExpression
        public static PrefixUnaryExpressionSyntax UnaryPlusExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.UnaryPlusExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax UnaryPlusExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.UnaryPlusExpression, operatorToken, operand);
        }

        public static PrefixUnaryExpressionSyntax UnaryMinusExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.UnaryMinusExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax UnaryMinusExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.UnaryMinusExpression, operatorToken, operand);
        }

        public static PrefixUnaryExpressionSyntax BitwiseNotExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.BitwiseNotExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax BitwiseNotExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.BitwiseNotExpression, operatorToken, operand);
        }

        public static PrefixUnaryExpressionSyntax LogicalNotExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax LogicalNotExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, operatorToken, operand);
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

        public static PrefixUnaryExpressionSyntax AddressOfExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.AddressOfExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax AddressOfExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.AddressOfExpression, operatorToken, operand);
        }

        public static PrefixUnaryExpressionSyntax PointerIndirectionExpression(ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(SyntaxKind.PointerIndirectionExpression, operand);
        }

        public static PrefixUnaryExpressionSyntax PointerIndirectionExpression(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            return PrefixUnaryExpression(SyntaxKind.PointerIndirectionExpression, operatorToken, operand);
        }
        #endregion PrefixUnaryExpression

        #region PostfixUnaryExpression
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
        #endregion PostfixUnaryExpression

        #region AssignmentExpression
        public static AssignmentExpressionSyntax SimpleAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax SimpleAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax AddAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.AddAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax AddAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.AddAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax SubtractAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SubtractAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax SubtractAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SubtractAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax MultiplyAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.MultiplyAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax MultiplyAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.MultiplyAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax DivideAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.DivideAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax DivideAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.DivideAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax ModuloAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.ModuloAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax ModuloAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.ModuloAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax AndAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.AndAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax AndAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.AndAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax ExclusiveOrAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.ExclusiveOrAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax ExclusiveOrAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.ExclusiveOrAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax OrAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.OrAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax OrAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.OrAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax LeftShiftAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.LeftShiftAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax LeftShiftAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.LeftShiftAssignmentExpression, left, operatorToken, right);
        }

        public static AssignmentExpressionSyntax RightShiftAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.RightShiftAssignmentExpression, left, right);
        }

        public static AssignmentExpressionSyntax RightShiftAssignmentExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.RightShiftAssignmentExpression, left, operatorToken, right);
        }
        #endregion AssignmentExpression

        #region LiteralExpression
        public static LiteralExpressionSyntax StringLiteralExpression(string value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax CharacterLiteralExpression(char value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.CharacterLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(int value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(uint value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(sbyte value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(decimal value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(double value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(float value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(long value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(ulong value)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax TrueLiteralExpression()
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
        }

        public static LiteralExpressionSyntax FalseLiteralExpression()
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
        }

        public static LiteralExpressionSyntax BooleanLiteralExpression(bool value)
        {
            return (value) ? TrueLiteralExpression() : FalseLiteralExpression();
        }

        public static LiteralExpressionSyntax NullLiteralExpression()
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
        }

        public static LiteralExpressionSyntax LiteralExpression(object value)
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
        #endregion LiteralExpression

        #region Expression
        public static ObjectCreationExpressionSyntax ObjectCreationExpression(TypeSyntax type, ArgumentListSyntax argumentList)
        {
            return SyntaxFactory.ObjectCreationExpression(type, argumentList, default(InitializerExpressionSyntax));
        }

        public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
        }

        public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, operatorToken, name);
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return InvocationExpression(SimpleMemberAccessExpression(expression, name));
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name, ArgumentSyntax argument)
        {
            return SimpleMemberInvocationExpression(expression, name, ArgumentList(argument));
        }

        public static InvocationExpressionSyntax SimpleMemberInvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name, ArgumentListSyntax argumentList)
        {
            return InvocationExpression(SimpleMemberAccessExpression(expression, name), argumentList);
        }

        public static InvocationExpressionSyntax NameOfExpression(string identifier)
        {
            return NameOfExpression(IdentifierName(identifier));
        }

        public static InvocationExpressionSyntax NameOfExpression(ExpressionSyntax expression)
        {
            return InvocationExpression(
                IdentifierName("nameof"),
                ArgumentList(SyntaxFactory.Argument(expression)));
        }

        public static InitializerExpressionSyntax ArrayInitializerExpression(SeparatedSyntaxList<ExpressionSyntax> expressions = default(SeparatedSyntaxList<ExpressionSyntax>))
        {
            return InitializerExpression(SyntaxKind.ArrayInitializerExpression, expressions);
        }

        public static InitializerExpressionSyntax ArrayInitializerExpression(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            return InitializerExpression(SyntaxKind.ArrayInitializerExpression, openBraceToken, expressions, closeBraceToken);
        }

        public static InitializerExpressionSyntax CollectionInitializerExpression(SeparatedSyntaxList<ExpressionSyntax> expressions = default(SeparatedSyntaxList<ExpressionSyntax>))
        {
            return InitializerExpression(SyntaxKind.CollectionInitializerExpression, expressions);
        }

        public static InitializerExpressionSyntax CollectionInitializerExpression(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            return InitializerExpression(SyntaxKind.CollectionInitializerExpression, openBraceToken, expressions, closeBraceToken);
        }

        public static InitializerExpressionSyntax ComplexElementInitializerExpression(SeparatedSyntaxList<ExpressionSyntax> expressions = default(SeparatedSyntaxList<ExpressionSyntax>))
        {
            return InitializerExpression(SyntaxKind.ComplexElementInitializerExpression, expressions);
        }

        public static InitializerExpressionSyntax ComplexElementInitializerExpression(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            return InitializerExpression(SyntaxKind.ComplexElementInitializerExpression, openBraceToken, expressions, closeBraceToken);
        }

        public static InitializerExpressionSyntax ObjectInitializerExpression(SeparatedSyntaxList<ExpressionSyntax> expressions = default(SeparatedSyntaxList<ExpressionSyntax>))
        {
            return InitializerExpression(SyntaxKind.ObjectInitializerExpression, expressions);
        }

        public static InitializerExpressionSyntax ObjectInitializerExpression(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            return InitializerExpression(SyntaxKind.ObjectInitializerExpression, openBraceToken, expressions, closeBraceToken);
        }

        public static CheckedExpressionSyntax CheckedExpression(ExpressionSyntax expression)
        {
            return SyntaxFactory.CheckedExpression(SyntaxKind.CheckedExpression, expression);
        }

        public static CheckedExpressionSyntax CheckedExpression(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            return SyntaxFactory.CheckedExpression(SyntaxKind.CheckedExpression, CheckedKeyword(), openParenToken, expression, closeParenToken);
        }

        public static CheckedExpressionSyntax UncheckedExpression(ExpressionSyntax expression)
        {
            return SyntaxFactory.CheckedExpression(SyntaxKind.UncheckedExpression, expression);
        }

        public static CheckedExpressionSyntax UncheckedExpression(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            return SyntaxFactory.CheckedExpression(SyntaxKind.UncheckedExpression, UncheckedKeyword(), openParenToken, expression, closeParenToken);
        }
        #endregion Expression

        public static IdentifierNameSyntax VarType()
        {
            return IdentifierName("var");
        }

        public static GenericNameSyntax GenericName(string identifier, TypeSyntax typeArgument)
        {
            return GenericName(Identifier(identifier), typeArgument);
        }

        public static GenericNameSyntax GenericName(SyntaxToken identifier, TypeSyntax typeArgument)
        {
            return SyntaxFactory.GenericName(identifier, TypeArgumentList(typeArgument));
        }

        public static VariableDeclaratorSyntax VariableDeclarator(string identifier, EqualsValueClauseSyntax initializer)
        {
            return VariableDeclarator(Identifier(identifier), initializer);
        }

        public static VariableDeclaratorSyntax VariableDeclarator(SyntaxToken identifier, EqualsValueClauseSyntax initializer)
        {
            return SyntaxFactory.VariableDeclarator(identifier, default(BracketedArgumentListSyntax), initializer);
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, string identifier, ExpressionSyntax value = null)
        {
            return VariableDeclaration(type, Identifier(identifier), value);
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, SyntaxToken identifier, ExpressionSyntax value = null)
        {
            if (value != null)
            {
                return VariableDeclaration(type, identifier, EqualsValueClause(value));
            }
            else
            {
                return VariableDeclaration(type, SyntaxFactory.VariableDeclarator(identifier));
            }
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax initializer)
        {
            return VariableDeclaration(
                type,
                VariableDeclarator(identifier, initializer));
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, VariableDeclaratorSyntax variable)
        {
            return SyntaxFactory.VariableDeclaration(type, SingletonSeparatedList(variable));
        }

        public static UsingDirectiveSyntax UsingStaticDirective(NameSyntax name)
        {
            return UsingDirective(
                StaticKeyword(),
                default(NameEqualsSyntax),
                name);
        }

        public static UsingDirectiveSyntax UsingStaticDirective(SyntaxToken usingKeyword, SyntaxToken staticKeyword, NameSyntax name, SyntaxToken semicolonToken)
        {
            return UsingDirective(
                usingKeyword,
                staticKeyword,
                default(NameEqualsSyntax),
                name,
                semicolonToken);
        }

        public static AttributeSyntax Attribute(NameSyntax name, AttributeArgumentSyntax argument)
        {
            return SyntaxFactory.Attribute(
                name,
                AttributeArgumentList(argument));
        }

        public static AttributeArgumentSyntax AttributeArgument(NameEqualsSyntax nameEquals, ExpressionSyntax expression)
        {
            return SyntaxFactory.AttributeArgument(
                nameEquals: nameEquals,
                nameColon: default(NameColonSyntax),
                expression: expression);
        }

        public static AttributeArgumentSyntax AttributeArgument(NameColonSyntax nameColon, ExpressionSyntax expression)
        {
            return SyntaxFactory.AttributeArgument(
                nameEquals: default(NameEqualsSyntax),
                nameColon: nameColon,
                expression: expression);
        }

        public static ArgumentSyntax Argument(NameColonSyntax nameColon, ExpressionSyntax expression)
        {
            return SyntaxFactory.Argument(nameColon, default(SyntaxToken), expression);
        }

        public static ParameterSyntax Parameter(TypeSyntax type, string identifier, ExpressionSyntax @default = null)
        {
            return Parameter(type, Identifier(identifier), @default);
        }

        public static ParameterSyntax Parameter(TypeSyntax type, SyntaxToken identifier, ExpressionSyntax @default = null)
        {
            if (@default != null)
            {
                return Parameter(type, identifier, EqualsValueClause(@default));
            }
            else
            {
                return SyntaxFactory.Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    default(SyntaxTokenList),
                    type,
                    identifier,
                    default(EqualsValueClauseSyntax));
            }
        }

        public static ParameterSyntax Parameter(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax @default)
        {
            return SyntaxFactory.Parameter(
                default(SyntaxList<AttributeListSyntax>),
                default(SyntaxTokenList),
                type,
                identifier,
                @default);
        }

        public static TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(
    string name,
    TypeParameterConstraintSyntax typeParameterConstraint)
        {
            return TypeParameterConstraintClause(IdentifierName(name), typeParameterConstraint);
        }

        public static TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(
    IdentifierNameSyntax identifierName,
    TypeParameterConstraintSyntax typeParameterConstraint)
        {
            return SyntaxFactory.TypeParameterConstraintClause(identifierName, SingletonSeparatedList(typeParameterConstraint));
        }

        public static ClassOrStructConstraintSyntax ClassConstraint()
        {
            return ClassOrStructConstraint(SyntaxKind.ClassConstraint, ClassKeyword());
        }

        public static ClassOrStructConstraintSyntax StructConstraint()
        {
            return ClassOrStructConstraint(SyntaxKind.StructConstraint, StructKeyword());
        }

        public static ConstructorInitializerSyntax BaseConstructorInitializer(ArgumentListSyntax argumentList = null)
        {
            return ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, argumentList);
        }

        public static ConstructorInitializerSyntax BaseConstructorInitializer(SyntaxToken semicolonToken, ArgumentListSyntax argumentList)
        {
            return ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, semicolonToken, BaseKeyword(), argumentList);
        }

        public static ConstructorInitializerSyntax ThisConstructorInitializer(ArgumentListSyntax argumentList = null)
        {
            return ConstructorInitializer(SyntaxKind.ThisConstructorInitializer, argumentList);
        }

        public static ConstructorInitializerSyntax ThisConstructorInitializer(SyntaxToken semicolonToken, ArgumentListSyntax argumentList)
        {
            return ConstructorInitializer(SyntaxKind.ThisConstructorInitializer, semicolonToken, ThisKeyword(), argumentList);
        }

        public static SwitchSectionSyntax SwitchSection(SwitchLabelSyntax switchLabel, StatementSyntax statement)
        {
            return SwitchSection(switchLabel, SingletonList(statement));
        }

        public static SwitchSectionSyntax SwitchSection(SwitchLabelSyntax switchLabel, SyntaxList<StatementSyntax> statements)
        {
            return SyntaxFactory.SwitchSection(SingletonList(switchLabel), statements);
        }

        public static SwitchSectionSyntax SwitchSection(SyntaxList<SwitchLabelSyntax> switchLabels, StatementSyntax statement)
        {
            return SyntaxFactory.SwitchSection(switchLabels, SingletonList(statement));
        }

        public static SwitchSectionSyntax DefaultSwitchSection(StatementSyntax statement)
        {
            return DefaultSwitchSection(SingletonList(statement));
        }

        public static SwitchSectionSyntax DefaultSwitchSection(SyntaxList<StatementSyntax> statements)
        {
            return SwitchSection(DefaultSwitchLabel(), statements);
        }

        public static CompilationUnitSyntax CompilationUnit(MemberDeclarationSyntax member)
        {
            return CompilationUnit(
                default(SyntaxList<UsingDirectiveSyntax>),
                member);
        }

        public static CompilationUnitSyntax CompilationUnit(SyntaxList<UsingDirectiveSyntax> usings, MemberDeclarationSyntax member)
        {
            return CompilationUnit(
                usings,
                SingletonList(member));
        }

        public static CompilationUnitSyntax CompilationUnit(SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<MemberDeclarationSyntax> members)
        {
            return SyntaxFactory.CompilationUnit(
                default(SyntaxList<ExternAliasDirectiveSyntax>),
                usings,
                default(SyntaxList<AttributeListSyntax>),
                members);
        }

        internal static SyntaxList<UsingDirectiveSyntax> UsingDirectives(string name)
        {
            return SingletonList(UsingDirective(ParseName(name)));
        }

        internal static SyntaxList<UsingDirectiveSyntax> UsingDirectives(params string[] names)
        {
            return List(names.Select(f => UsingDirective(ParseName(f))));
        }

        internal static bool AreEquivalent(
            SyntaxNode node1,
            SyntaxNode node2,
            bool disregardTrivia = true,
            bool topLevel = false)
        {
            if (disregardTrivia)
                return SyntaxFactory.AreEquivalent(node1, node2, topLevel: topLevel);

            if (node1 == null)
                return node2 == null;

            return node1.IsEquivalentTo(node2, topLevel: topLevel);
        }

        internal static bool AreEquivalent(
            SyntaxNode node1,
            SyntaxNode node2,
            SyntaxNode node3,
            bool disregardTrivia = true,
            bool topLevel = false)
        {
            return AreEquivalent(node1, node2, disregardTrivia: disregardTrivia, topLevel: topLevel)
                && AreEquivalent(node1, node3, disregardTrivia: disregardTrivia, topLevel: topLevel);
        }

        internal static bool AreEquivalent<TNode>(
            IList<TNode> first,
            IList<TNode> second,
            bool disregardTrivia = true,
            bool topLevel = false) where TNode : SyntaxNode
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            int count = first.Count;

            if (count != second.Count)
                return false;

            for (int i = 0; i < count; i++)
            {
                if (!AreEquivalent(first[i], second[i], disregardTrivia: disregardTrivia, topLevel: topLevel))
                    return false;
            }

            return true;
        }
    }
}
