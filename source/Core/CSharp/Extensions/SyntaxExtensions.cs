// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Extensions
{
    public static class SyntaxExtensions
    {
        public static bool IsAutoImplementedGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return IsAutoImplemented(accessorDeclaration, SyntaxKind.GetAccessorDeclaration);
        }

        public static bool IsAutoImplementedSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return IsAutoImplemented(accessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        }

        private static bool IsAutoImplemented(this AccessorDeclarationSyntax accessorDeclaration, SyntaxKind kind)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.IsKind(kind)
                && IsAutoImplemented(accessorDeclaration);
        }

        private static bool IsAutoImplemented(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return accessorDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && accessorDeclaration.Body == null;
        }

        public static AccessorDeclarationSyntax WithoutSemicolonToken(
            this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.WithSemicolonToken(default(SyntaxToken));
        }

        public static AccessorDeclarationSyntax Getter(this AccessorListSyntax accessorList)
        {
            return Accessor(accessorList, SyntaxKind.GetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax Setter(this AccessorListSyntax accessorList)
        {
            return Accessor(accessorList, SyntaxKind.SetAccessorDeclaration);
        }

        private static AccessorDeclarationSyntax Accessor(this AccessorListSyntax accessorList, SyntaxKind kind)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            return accessorList
                .Accessors
                .FirstOrDefault(accessor => accessor.IsKind(kind));
        }

        public static IEnumerable<SyntaxToken> CommaTokens(this AttributeListSyntax attributeList)
        {
            if (attributeList == null)
                throw new ArgumentNullException(nameof(attributeList));

            return attributeList
                .ChildTokens()
                .Where(token => token.IsKind(SyntaxKind.CommaToken));
        }

        public static BlockSyntax WithoutStatements(this BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.WithStatements(default(SyntaxList<StatementSyntax>));
        }

        public static StatementSyntax SingleStatementOrDefault(this BlockSyntax body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            SyntaxList<StatementSyntax> statements = body.Statements;

            return (statements.Count == 1)
                ? statements.First()
                : null;
        }

        public static TextSpan BracesSpan(this CastExpressionSyntax castExpression)
        {
            if (castExpression == null)
                throw new ArgumentNullException(nameof(castExpression));

            return TextSpan.FromBounds(
                castExpression.OpenParenToken.Span.Start,
                castExpression.CloseParenToken.Span.End);
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

        public static bool IsStatic(this ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static TextSpan BracesSpan(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return TextSpan.FromBounds(
                classDeclaration.OpenBraceToken.Span.Start,
                classDeclaration.CloseBraceToken.Span.End);
        }

        public static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax memberDeclaration)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(SingletonList(memberDeclaration));
        }

        public static CompilationUnitSyntax AddUsings(this CompilationUnitSyntax compilationUnit, bool keepSingleLineCommentsOnTop, params UsingDirectiveSyntax[] usings)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (usings == null)
                throw new ArgumentNullException(nameof(usings));

            if (keepSingleLineCommentsOnTop
                && usings.Length > 0
                && !compilationUnit.Usings.Any())
            {
                SyntaxTriviaList leadingTrivia = compilationUnit.GetLeadingTrivia();

                SyntaxTrivia[] topTrivia = GetTopSingleLineComments(leadingTrivia).ToArray();

                if (topTrivia.Length > 0)
                {
                    compilationUnit = compilationUnit.WithoutLeadingTrivia();

                    usings[0] = usings[0].WithLeadingTrivia(topTrivia);

                    usings[usings.Length - 1] = usings[usings.Length - 1].WithTrailingTrivia(leadingTrivia.Skip(topTrivia.Length));
                }
            }

            return compilationUnit.AddUsings(usings);
        }

        private static IEnumerable<SyntaxTrivia> GetTopSingleLineComments(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    SyntaxTrivia trivia = en.Current;

                    if (en.MoveNext() && en.Current.IsEndOfLineTrivia())
                    {
                        yield return trivia;
                        yield return en.Current;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public static TextSpan HeaderSpan(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.Span.Start,
                constructorDeclaration.ParameterList?.Span.End ?? constructorDeclaration.Identifier.Span.End);
        }

        public static TextSpan HeaderSpanIncludingInitializer(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.Span.Start,
                constructorDeclaration.Initializer?.Span.End
                    ?? constructorDeclaration.ParameterList?.Span.End
                    ?? constructorDeclaration.Identifier.Span.End);
        }

        public static bool IsStatic(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            return constructorDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
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

        public static CSharpSyntaxNode BodyOrExpressionBody(this ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            BlockSyntax body = conversionOperatorDeclaration.Body;

            if (body != null)
            {
                return body;
            }
            else
            {
                return conversionOperatorDeclaration.ExpressionBody;
            }
        }

        public static XmlElementSyntax SummaryElement(this DocumentationCommentTriviaSyntax documentationComment)
        {
            if (documentationComment == null)
                throw new ArgumentNullException(nameof(documentationComment));

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var element = (XmlElementSyntax)node;

                    string name = element.StartTag?.Name?.LocalName.ValueText;

                    if (string.Equals(name, "summary", StringComparison.Ordinal))
                        return element;
                }
            }

            return null;
        }

        public static IEnumerable<XmlElementSyntax> ExceptionElements(this DocumentationCommentTriviaSyntax documentationComment)
        {
            return Elements(documentationComment, "exception");
        }

        public static IEnumerable<XmlElementSyntax> ParamElements(this DocumentationCommentTriviaSyntax documentationComment)
        {
            return Elements(documentationComment, "param");
        }

        public static IEnumerable<XmlElementSyntax> TypeParamElements(this DocumentationCommentTriviaSyntax documentationComment)
        {
            return Elements(documentationComment, "typeparam");
        }

        public static IEnumerable<XmlElementSyntax> Elements(this DocumentationCommentTriviaSyntax documentationComment, string localName)
        {
            if (documentationComment == null)
                throw new ArgumentNullException(nameof(documentationComment));

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var xmlElement = (XmlElementSyntax)node;

                    XmlNameSyntax xmlName = xmlElement.StartTag?.Name;

                    if (xmlName != null
                        && string.Equals(xmlName.LocalName.ValueText, localName, StringComparison.Ordinal))
                    {
                        yield return xmlElement;
                    }
                }
            }
        }

        public static TextSpan BracesSpan(this EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return TextSpan.FromBounds(
                enumDeclaration.OpenBraceToken.Span.Start,
                enumDeclaration.CloseBraceToken.Span.End);
        }

        public static TextSpan HeaderSpan(this EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return TextSpan.FromBounds(
                eventDeclaration.Span.Start,
                eventDeclaration.Identifier.Span.End);
        }

        public static bool IsStatic(this EventDeclarationSyntax eventDeclaration)
        {
            return eventDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static bool IsStatic(this EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return eventFieldDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static ParenthesizedExpressionSyntax Parenthesize(this ExpressionSyntax expression, bool moveTrivia = false)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (moveTrivia)
            {
                return ParenthesizedExpression(expression.WithoutTrivia())
                    .WithTriviaFrom(expression);
            }
            else
            {
                return ParenthesizedExpression(expression);
            }
        }

        public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
        {
            while (expression?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;

            return expression;
        }

        public static bool IsIncrementOrDecrementExpression(this ExpressionSyntax expression)
        {
            return expression?.IsKind(
                SyntaxKind.PreIncrementExpression,
                SyntaxKind.PreDecrementExpression,
                SyntaxKind.PostIncrementExpression,
                SyntaxKind.PostDecrementExpression) == true;
        }

        public static bool SupportsCompoundAssignment(this ExpressionSyntax expression)
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

        public static bool IsConst(this FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration?.Modifiers.Contains(SyntaxKind.ConstKeyword) == true;
        }

        public static bool IsStatic(this FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static TextSpan ParenthesesSpan(this ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            return TextSpan.FromBounds(forEachStatement.OpenParenToken.Span.Start, forEachStatement.CloseParenToken.Span.End);
        }

        public static TextSpan ParenthesesSpan(this ForStatementSyntax forStatement)
        {
            if (forStatement == null)
                throw new ArgumentNullException(nameof(forStatement));

            return TextSpan.FromBounds(forStatement.OpenParenToken.Span.Start, forStatement.CloseParenToken.Span.End);
        }

        public static TextSpan HeaderSpan(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return TextSpan.FromBounds(
                indexerDeclaration.Span.Start,
                indexerDeclaration.ParameterList?.Span.End ?? indexerDeclaration.ThisKeyword.Span.End);
        }

        public static AccessorDeclarationSyntax Getter(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration
                .AccessorList?
                .Getter();
        }

        public static AccessorDeclarationSyntax Setter(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration
                .AccessorList?
                .Setter();
        }

        public static TextSpan HeaderSpan(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return TextSpan.FromBounds(
                interfaceDeclaration.Span.Start,
                interfaceDeclaration.Identifier.Span.End);
        }

        public static TextSpan BracesSpan(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return TextSpan.FromBounds(
                interfaceDeclaration.OpenBraceToken.Span.Start,
                interfaceDeclaration.CloseBraceToken.Span.End);
        }

        public static bool IsVerbatim(this InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            return interpolatedString.StringStartToken.ValueText.Contains("@");
        }

        public static InvocationExpressionSyntax WithArgumentList(this InvocationExpressionSyntax invocationExpression)
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

        public static bool IsZeroNumericLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.NumericLiteralExpression)
                && string.Equals(literalExpression.Token.ValueText, "0", StringComparison.Ordinal);
        }

        public static string GetStringLiteralInnerText(this LiteralExpressionSyntax literalExpression)
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

        public static bool IsHexadecimalNumericLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.NumericLiteralExpression)
                && literalExpression.Token.Text.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        }

        public static SyntaxTrivia GetSingleLineDocumentationComment(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return memberDeclaration
                .GetLeadingTrivia()
                .FirstOrDefault(f => f.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        }

        public static bool HasSingleLineDocumentationComment(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return memberDeclaration
                .GetLeadingTrivia()
                .Any(f => f.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        }

        public static SyntaxTokenList GetModifiers(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).Modifiers;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).Modifiers;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).Modifiers;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).Modifiers;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).Modifiers;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).Modifiers;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).Modifiers;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).Modifiers;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).Modifiers;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).Modifiers;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).Modifiers;
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)node).Modifiers;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.LocalDeclarationStatement:
                    return ((LocalDeclarationStatementSyntax)node).Modifiers;
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).Modifiers;
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return default(SyntaxTokenList);
                    }
            }
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
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)declaration).Modifiers;
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return default(SyntaxTokenList);
                    }
            }
        }

        public static SyntaxNode SetModifiers(this SyntaxNode node, SyntaxTokenList modifiers)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.LocalDeclarationStatement:
                    return ((LocalDeclarationStatementSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).WithModifiers(modifiers);
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return node;
                    }
            }
        }

        public static MemberDeclarationSyntax SetModifiers(this MemberDeclarationSyntax declaration, SyntaxTokenList modifiers)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

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
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)declaration).WithModifiers(modifiers);
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return declaration;
                    }
            }
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
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return default(SyntaxList<MemberDeclarationSyntax>);
                    }
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
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return declaration;
                    }
            }
        }

        public static bool SupportsExpressionBody(this MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration?.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                    return true;
                default:
                    return false;
            }
        }

        public static Accessibility GetDefaultExplicitAccessibility(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        if (((ConstructorDeclarationSyntax)memberDeclaration).IsStatic())
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        return Accessibility.NotApplicable;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                        if (methodDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword)
                            || methodDeclaration.ExplicitInterfaceSpecifier != null
                            || methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                        if (propertyDeclaration.ExplicitInterfaceSpecifier != null
                            || propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                        if (indexerDeclaration.ExplicitInterfaceSpecifier != null
                            || indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)memberDeclaration;

                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        if (memberDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        return Accessibility.Private;
                    }
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return Accessibility.Public;
                    }
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    {
                        if (memberDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                        {
                            return Accessibility.Private;
                        }
                        else
                        {
                            return Accessibility.Internal;
                        }
                    }
            }

            return Accessibility.NotApplicable;
        }

        public static Accessibility GetDeclaredAccessibility(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)memberDeclaration;

                        if (constructorDeclaration.IsStatic())
                        {
                            return Accessibility.Private;
                        }
                        else
                        {
                            return AccessibilityOrDefault(constructorDeclaration, constructorDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                        SyntaxTokenList modifiers = methodDeclaration.Modifiers;

                        if (modifiers.Contains(SyntaxKind.PartialKeyword))
                        {
                            return Accessibility.Private;
                        }
                        else if (methodDeclaration.ExplicitInterfaceSpecifier != null
                            || methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(methodDeclaration, modifiers);
                        }
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                        if (propertyDeclaration.ExplicitInterfaceSpecifier != null
                            || propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(propertyDeclaration, propertyDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                        if (indexerDeclaration.ExplicitInterfaceSpecifier != null
                            || indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(indexerDeclaration, indexerDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)memberDeclaration;

                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(eventDeclaration, eventDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        if (memberDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            var eventFieldDeclaration = (EventFieldDeclarationSyntax)memberDeclaration;

                            return AccessibilityOrDefault(eventFieldDeclaration, eventFieldDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    {
                        return AccessibilityOrDefault(memberDeclaration, memberDeclaration.GetModifiers());
                    }
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.EnumMemberDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    {
                        return Accessibility.Public;
                    }
            }

            return Accessibility.NotApplicable;
        }

        private static Accessibility AccessibilityOrDefault(MemberDeclarationSyntax memberDeclaration, SyntaxTokenList modifiers)
        {
            Accessibility accessibility = modifiers.GetAccessibility();

            if (accessibility != Accessibility.NotApplicable)
            {
                return accessibility;
            }
            else
            {
                return GetDefaultExplicitAccessibility(memberDeclaration);
            }
        }

        public static bool IsPubliclyVisible(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            do
            {
                if (memberDeclaration.IsKind(SyntaxKind.NamespaceDeclaration))
                    return true;

                Accessibility accessibility = memberDeclaration.GetDeclaredAccessibility();

                if (accessibility == Accessibility.Public
                    || accessibility == Accessibility.Protected
                    || accessibility == Accessibility.ProtectedOrInternal)
                {
                    SyntaxNode parent = memberDeclaration.Parent;

                    if (parent != null)
                    {
                        if (parent.IsKind(SyntaxKind.CompilationUnit))
                            return true;

                        memberDeclaration = parent as MemberDeclarationSyntax;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            } while (memberDeclaration != null);

            return false;
        }

        public static bool IsIterator(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                    .DescendantNodes(node => !node.IsNestedMethod())
                    .Any(f => f.IsKind(SyntaxKind.YieldReturnStatement, SyntaxKind.YieldBreakStatement));
        }

        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration?.ReturnType?.IsVoid() == true;
        }

        public static TextSpan HeaderSpan(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return TextSpan.FromBounds(
                methodDeclaration.Span.Start,
                methodDeclaration.ParameterList?.Span.End ?? methodDeclaration.Identifier.Span.End);
        }

        public static bool IsStatic(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(SingletonList(member));
        }

        public static bool ContainsAwait(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                .DescendantNodes(node => !node.IsNestedMethod())
                .Any(f => f.IsKind(SyntaxKind.AwaitExpression));
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            BlockSyntax body = methodDeclaration.Body;

            if (body != null)
            {
                return body;
            }
            else
            {
                return methodDeclaration.ExpressionBody;
            }
        }

        public static TextSpan HeaderSpan(this NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return TextSpan.FromBounds(
                namespaceDeclaration.Span.Start,
                namespaceDeclaration.Name?.Span.End ?? namespaceDeclaration.NamespaceKeyword.Span.End);
        }

        public static TextSpan BracesSpan(this NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return TextSpan.FromBounds(
                namespaceDeclaration.OpenBraceToken.Span.Start,
                namespaceDeclaration.CloseBraceToken.Span.End);
        }

        public static TextSpan HeaderSpan(this OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return TextSpan.FromBounds(
                operatorDeclaration.Span.Start,
                operatorDeclaration.ParameterList?.Span.End ?? operatorDeclaration.OperatorToken.Span.End);
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            BlockSyntax body = operatorDeclaration.Body;

            if (body != null)
            {
                return body;
            }
            else
            {
                return operatorDeclaration.ExpressionBody;
            }
        }

        public static bool IsThis(this ParameterSyntax parameter)
        {
            return parameter?.Modifiers.Contains(SyntaxKind.ThisKeyword) == true;
        }

        public static PropertyDeclarationSyntax WithAttributeLists(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AttributeListSyntax[] attributeLists)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithAttributeLists(List(attributeLists));
        }

        public static PropertyDeclarationSyntax WithoutSemicolonToken(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithSemicolonToken(default(SyntaxToken));
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

        public static bool IsStatic(this PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static StatementSyntax PreviousStatement(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxList<StatementSyntax> statements = StatementContainer.GetStatements(statement);

            if (statements.Any())
            {
                int index = statements.IndexOf(statement);

                if (index > 0)
                {
                    return statements[index - 1];
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public static StatementSyntax NextStatement(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxList<StatementSyntax> statements = StatementContainer.GetStatements(statement);

            if (statements.Any())
            {
                int index = statements.IndexOf(statement);

                if (index < statements.Count - 1)
                {
                    return statements[index + 1];
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public static TextSpan HeaderSpan(this StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return TextSpan.FromBounds(
                structDeclaration.Span.Start,
                structDeclaration.Identifier.Span.End);
        }

        public static TextSpan BracesSpan(this StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return TextSpan.FromBounds(
                structDeclaration.OpenBraceToken.Span.Start,
                structDeclaration.CloseBraceToken.Span.End);
        }

        public static SwitchSectionSyntax WithoutStatements(this SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            return switchSection.WithStatements(default(SyntaxList<StatementSyntax>));
        }

        public static int LastIndexOf<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        public static int LastIndexOf<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        public static bool Contains<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        public static bool Contains<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        public static SyntaxList<TNode> ReplaceAt<TNode>(this SyntaxList<TNode> list, int index, TNode newNode) where TNode : SyntaxNode
        {
            return list.Replace(list[index], newNode);
        }

        public static bool IsFirst<TNode>(this SyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.IndexOf(node) == 0;
        }

        public static bool IsLast<TNode>(this SyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.Any()
                && list.IndexOf(node) == list.Count - 1;
        }

        public static bool IsFirst<TNode>(this SeparatedSyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.IndexOf(node) == 0;
        }

        public static bool IsLast<TNode>(this SeparatedSyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.Any()
                && list.IndexOf(node) == list.Count - 1;
        }

        public static SeparatedSyntaxList<TNode> ReplaceAt<TNode>(this SeparatedSyntaxList<TNode> list, int index, TNode newNode) where TNode : SyntaxNode
        {
            return list.Replace(list[index], newNode);
        }

        public static bool IsVoid(this TypeSyntax type)
        {
            return type?.IsKind(SyntaxKind.PredefinedType) == true
                && ((PredefinedTypeSyntax)type).Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        public static CSharpSyntaxNode DeclarationOrExpression(this UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            CSharpSyntaxNode declaration = usingStatement.Declaration;

            return declaration ?? usingStatement.Expression;
        }

        public static VariableDeclaratorSyntax SingleVariableOrDefault(this VariableDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            return (variables.Count == 1)
                ? variables.First()
                : null;
        }

        public static bool IsYieldReturn(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement?.ReturnOrBreakKeyword.IsKind(SyntaxKind.ReturnKeyword) == true;
        }

        public static bool IsYieldBreak(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement?.ReturnOrBreakKeyword.IsKind(SyntaxKind.BreakKeyword) == true;
        }
    }
}
