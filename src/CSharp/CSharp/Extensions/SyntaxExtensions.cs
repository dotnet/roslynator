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
using Roslynator.CSharp.Documentation;
using Roslynator.CSharp.Syntax;
using Roslynator.CSharp.SyntaxRewriters;
using Roslynator.CSharp.SyntaxWalkers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    /// <summary>
    /// A set of extension methods for syntax (types derived from <see cref="CSharpSyntaxNode"/>).
    /// </summary>
    public static class SyntaxExtensions
    {
        #region AccessorDeclarationSyntax
        /// <summary>
        /// Returns true is the specified accessor is auto-implemented accessor.
        /// </summary>
        /// <param name="accessorDeclaration"></param>
        /// <returns></returns>
        public static bool IsAutoImplemented(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return accessorDeclaration?.SemicolonToken.Kind() == SyntaxKind.SemicolonToken
                && accessorDeclaration.BodyOrExpressionBody() == null;
        }

        /// <summary>
        /// Returns accessor body or an expression body if the body is null.
        /// </summary>
        /// <param name="accessorDeclaration"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode BodyOrExpressionBody(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.Body ?? (CSharpSyntaxNode)accessorDeclaration.ExpressionBody;
        }
        #endregion AccessorDeclarationSyntax

        #region AccessorListSyntax
        /// <summary>
        /// Returns a get accessor contained in the specified list.
        /// </summary>
        /// <param name="accessorList"></param>
        /// <returns></returns>
        public static AccessorDeclarationSyntax Getter(this AccessorListSyntax accessorList)
        {
            return Accessor(accessorList, SyntaxKind.GetAccessorDeclaration);
        }

        /// <summary>
        /// Returns a set accessor contained in the specified list.
        /// </summary>
        /// <param name="accessorList"></param>
        /// <returns></returns>
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
        #endregion AccessorListSyntax

        #region BlockSyntax
        internal static StatementSyntax SingleNonBlockStatementOrDefault(this BlockSyntax body, bool recursive = false)
        {
            if (recursive)
            {
                StatementSyntax statement = null;

                do
                {
                    statement = body.Statements.SingleOrDefault(shouldThrow: false);

                    body = statement as BlockSyntax;
                }
                while (body != null);

                return statement;
            }
            else
            {
                StatementSyntax statement = body.Statements.SingleOrDefault(shouldThrow: false);

                if (statement != null
                    && statement.Kind() != SyntaxKind.Block)
                {
                    return statement;
                }

                return null;
            }
        }

        internal static bool ContainsYieldReturn(this BlockSyntax block, TextSpan? span = null)
        {
            return ContainsYieldWalker.ContainsYieldReturn(block, span);
        }

        internal static bool ContainsYieldBreak(this BlockSyntax block, TextSpan? span = null)
        {
            return ContainsYieldWalker.ContainsYieldBreak(block, span);
        }

        internal static bool ContainsYield(this BlockSyntax block, TextSpan? span = null, bool yieldReturn = true, bool yieldBreak = true)
        {
            return ContainsYieldWalker.ContainsYield(block, span, yieldReturn, yieldBreak);
        }

        internal static StatementSyntax LastStatementOrDefault(this BlockSyntax block, bool skipLocalFunction = false)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            SyntaxList<StatementSyntax> statements = block.Statements;

            if (!statements.Any())
                return null;

            if (!skipLocalFunction)
                return statements.Last();

            int i = statements.Count - 1;

            while (i >= 0)
            {
                StatementSyntax statement = statements[i];

                if (statement.Kind() != SyntaxKind.LocalFunctionStatement)
                    return statement;

                i--;
            }

            return null;
        }
        #endregion BlockSyntax

        #region BaseArgumentListSyntax
        internal static BaseArgumentListSyntax WithArguments(this BaseArgumentListSyntax baseArgumentList, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            switch (baseArgumentList.Kind())
            {
                case SyntaxKind.ArgumentList:
                    return ((ArgumentListSyntax)baseArgumentList).WithArguments(arguments);
                case SyntaxKind.BracketedArgumentList:
                    return ((BracketedArgumentListSyntax)baseArgumentList).WithArguments(arguments);
            }

            Debug.Fail(baseArgumentList?.Kind().ToString());

            return null;
        }
        #endregion BaseArgumentListSyntax

        #region CastExpressionSyntax
        /// <summary>
        /// The absolute span of the parentheses, not including its leading and trailing trivia.
        /// </summary>
        /// <param name="castExpression"></param>
        /// <returns></returns>
        public static TextSpan ParenthesesSpan(this CastExpressionSyntax castExpression)
        {
            if (castExpression == null)
                throw new ArgumentNullException(nameof(castExpression));

            return TextSpan.FromBounds(
                castExpression.OpenParenToken.SpanStart,
                castExpression.CloseParenToken.Span.End);
        }
        #endregion CastExpressionSyntax

        #region ClassDeclarationSyntax
        /// <summary>
        /// Creates a new <see cref="ClassDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(SingletonList(member));
        }

        /// <summary>
        /// Creates a new <see cref="ClassDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            IEnumerable<MemberDeclarationSyntax> members)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(List(members));
        }

        /// <summary>
        /// The absolute span of the braces, not including its leading and trailing trivia.
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <returns></returns>
        public static TextSpan BracesSpan(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return TextSpan.FromBounds(
                classDeclaration.OpenBraceToken.SpanStart,
                classDeclaration.CloseBraceToken.Span.End);
        }
        #endregion ClassDeclarationSyntax

        #region CommonForEachStatementSyntax
        /// <summary>
        /// The absolute span of the parentheses, not including its leading and trailing trivia.
        /// </summary>
        /// <param name="forEachStatement"></param>
        /// <returns></returns>
        public static TextSpan ParenthesesSpan(this CommonForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            return TextSpan.FromBounds(forEachStatement.OpenParenToken.SpanStart, forEachStatement.CloseParenToken.Span.End);
        }

        internal static StatementSyntax EmbeddedStatement(this CommonForEachStatementSyntax forEachStatement)
        {
            StatementSyntax statement = forEachStatement.Statement;

            return (statement?.Kind() == SyntaxKind.Block) ? null : statement;
        }
        #endregion CommonForEachStatementSyntax

        #region CompilationUnitSyntax
        /// <summary>
        /// Creates a new <see cref="CompilationUnitSyntax"/> with the members updated.
        /// </summary>
        /// <param name="compilationUnit"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(SingletonList(member));
        }

        /// <summary>
        /// Creates a new <see cref="CompilationUnitSyntax"/> with the members updated.
        /// </summary>
        /// <param name="compilationUnit"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            IEnumerable<MemberDeclarationSyntax> members)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(List(members));
        }

        /// <summary>
        /// Creates a new <see cref="CompilationUnitSyntax"/> with the specified using directives added.
        /// </summary>
        /// <param name="compilationUnit"></param>
        /// <param name="keepSingleLineCommentsOnTop"></param>
        /// <param name="usings"></param>
        /// <returns></returns>
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
                List<SyntaxTrivia> topTrivia = null;

                SyntaxTriviaList leadingTrivia = compilationUnit.GetLeadingTrivia();

                SyntaxTriviaList.Enumerator en = leadingTrivia.GetEnumerator();

                while (en.MoveNext())
                {
                    if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    {
                        SyntaxTrivia trivia = en.Current;

                        if (en.MoveNext()
                            && en.Current.IsEndOfLineTrivia())
                        {
                            (topTrivia ?? (topTrivia = new List<SyntaxTrivia>())).Add(trivia);
                            topTrivia.Add(en.Current);
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

                if (topTrivia?.Count > 0)
                {
                    compilationUnit = compilationUnit.WithoutLeadingTrivia();

                    usings[0] = usings[0].WithLeadingTrivia(topTrivia);

                    usings[usings.Length - 1] = usings[usings.Length - 1].WithTrailingTrivia(leadingTrivia.Skip(topTrivia.Count));
                }
            }

            return compilationUnit.AddUsings(usings);
        }
        #endregion CompilationUnitSyntax

        internal static ExpressionSyntax RemoveOperatorToken(this ConditionalAccessExpressionSyntax conditionalAccessExpression)
        {
            SyntaxToken operatorToken = conditionalAccessExpression.OperatorToken;

            string text = conditionalAccessExpression
                .ToFullString()
                .Remove(operatorToken.FullSpan.Start - conditionalAccessExpression.FullSpan.Start, operatorToken.FullSpan.Length);

            return ParseExpression(text);
        }

        #region ConstructorDeclarationSyntax
        internal static TextSpan HeaderSpan(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.SpanStart,
                constructorDeclaration.Initializer?.Span.End
                    ?? constructorDeclaration.ParameterList?.Span.End
                    ?? constructorDeclaration.Identifier.Span.End);
        }

        /// <summary>
        /// Returns constructor body or an expression body if the body is null.
        /// </summary>
        /// <param name="constructorDeclaration"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode BodyOrExpressionBody(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return constructorDeclaration.Body ?? (CSharpSyntaxNode)constructorDeclaration.ExpressionBody;
        }
        #endregion ConstructorDeclarationSyntax

        #region ConversionOperatorDeclarationSyntax
        /// <summary>
        /// Returns conversion operator body or an expression body if the body is null.
        /// </summary>
        /// <param name="conversionOperatorDeclaration"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode BodyOrExpressionBody(this ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return conversionOperatorDeclaration.Body ?? (CSharpSyntaxNode)conversionOperatorDeclaration.ExpressionBody;
        }
        #endregion ConversionOperatorDeclarationSyntax

        #region DelegateDeclarationSyntax
        /// <summary>
        /// Returns true the specified delegate return type is <see cref="void"/>.
        /// </summary>
        /// <param name="delegateDeclaration"></param>
        /// <returns></returns>
        public static bool ReturnsVoid(this DelegateDeclarationSyntax delegateDeclaration)
        {
            return delegateDeclaration?.ReturnType?.IsVoid() == true;
        }
        #endregion DelegateDeclarationSyntax

        #region DestructorDeclarationSyntax
        /// <summary>
        /// Returns destructor body or an expression body if the body is null.
        /// </summary>
        /// <param name="destructorDeclaration"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode BodyOrExpressionBody(this DestructorDeclarationSyntax destructorDeclaration)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return destructorDeclaration.Body ?? (CSharpSyntaxNode)destructorDeclaration.ExpressionBody;
        }
        #endregion DestructorDeclarationSyntax

        #region DocumentationCommentTriviaSyntax
        internal static XmlElementSyntax SummaryElement(this DocumentationCommentTriviaSyntax documentationComment)
        {
            if (documentationComment == null)
                throw new ArgumentNullException(nameof(documentationComment));

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var element = (XmlElementSyntax)node;

                    if (element.IsLocalName("summary", StringComparison.OrdinalIgnoreCase))
                        return element;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of xml elements with the specified local name.
        /// </summary>
        /// <param name="documentationComment"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        public static IEnumerable<XmlElementSyntax> Elements(this DocumentationCommentTriviaSyntax documentationComment, string localName)
        {
            if (documentationComment == null)
                throw new ArgumentNullException(nameof(documentationComment));

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var xmlElement = (XmlElementSyntax)node;

                    if (xmlElement.IsLocalName(localName))
                        yield return xmlElement;
                }
            }
        }

        internal static IEnumerable<XmlElementSyntax> Elements(this DocumentationCommentTriviaSyntax documentationComment, string localName1, string localName2)
        {
            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var xmlElement = (XmlElementSyntax)node;

                    if (xmlElement.IsLocalName(localName1, localName2))
                        yield return xmlElement;
                }
            }
        }
        #endregion DocumentationCommentTriviaSyntax

        #region DoStatementSyntax
        internal static StatementSyntax EmbeddedStatement(this DoStatementSyntax doStatement)
        {
            StatementSyntax statement = doStatement.Statement;

            return (statement?.Kind() == SyntaxKind.Block) ? null : statement;
        }
        #endregion DoStatementSyntax

        #region ElseClauseSyntax
        internal static StatementSyntax SingleNonBlockStatementOrDefault(this ElseClauseSyntax elseClause)
        {
            return SingleNonBlockStatementOrDefault(elseClause.Statement);
        }

        /// <summary>
        /// Returns topmost if statement of the if-else cascade the specified else clause is part of.
        /// </summary>
        /// <param name="elseClause"></param>
        /// <returns></returns>
        public static IfStatementSyntax GetTopmostIf(this ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            if (elseClause.Parent is IfStatementSyntax ifStatement)
                return ifStatement.GetTopmostIf();

            return null;
        }

        internal static StatementSyntax EmbeddedStatement(this ElseClauseSyntax elseClause, bool allowIfStatement = true)
        {
            StatementSyntax statement = elseClause.Statement;

            if (statement == null)
                return null;

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.Block)
                return null;

            if (!allowIfStatement
                && kind == SyntaxKind.IfStatement)
            {
                return null;
            }

            return statement;
        }
        #endregion ElseClauseSyntax

        #region EndRegionDirectiveTriviaSyntax
        /// <summary>
        /// Returns region directive that is related to the specified endregion directive. Returns null if no matching region directive is found.
        /// </summary>
        /// <param name="endRegionDirective"></param>
        /// <returns></returns>
        public static RegionDirectiveTriviaSyntax GetRegionDirective(this EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (endRegionDirective == null)
                throw new ArgumentNullException(nameof(endRegionDirective));

            RegionInfo region = SyntaxInfo.RegionInfo(endRegionDirective);

            return (region.Success) ? region.Directive : null;
        }

        /// <summary>
        /// Gets preprocessing message for the specified endregion directive if such message exists.
        /// </summary>
        /// <param name="endRegionDirective"></param>
        /// <returns></returns>
        public static SyntaxTrivia GetPreprocessingMessageTrivia(this EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (endRegionDirective == null)
                throw new ArgumentNullException(nameof(endRegionDirective));

            SyntaxToken endOfDirective = endRegionDirective.EndOfDirectiveToken;

            SyntaxTriviaList leading = endOfDirective.LeadingTrivia;

            if (leading.Count == 1)
            {
                SyntaxTrivia trivia = leading[0];

                if (trivia.IsKind(SyntaxKind.PreprocessingMessageTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        /// <summary>
        /// Returns true the specified endregion directive has preprocessing message trivia.
        /// </summary>
        /// <param name="endRegionDirective"></param>
        /// <returns></returns>
        internal static bool HasPreprocessingMessageTrivia(this EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            return GetPreprocessingMessageTrivia(endRegionDirective).Kind() == SyntaxKind.PreprocessingMessageTrivia;
        }
        #endregion EndRegionDirectiveTriviaSyntax

        #region EnumDeclarationSyntax
        /// <summary>
        /// The absolute span of the braces, not including its leading and trailing trivia.
        /// </summary>
        /// <param name="enumDeclaration"></param>
        /// <returns></returns>
        public static TextSpan BracesSpan(this EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return TextSpan.FromBounds(
                enumDeclaration.OpenBraceToken.SpanStart,
                enumDeclaration.CloseBraceToken.Span.End);
        }
        #endregion EnumDeclarationSyntax

        #region EventDeclarationSyntax
        internal static TextSpan HeaderSpan(this EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return TextSpan.FromBounds(
                eventDeclaration.SpanStart,
                eventDeclaration.Identifier.Span.End);
        }
        #endregion EventDeclarationSyntax

        #region ExpressionSyntax
        /// <summary>
        /// Returns topmost parenthesized expression or self if the expression if not parenthesized.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static ExpressionSyntax WalkUpParentheses(this ExpressionSyntax expression)
        {
            while (expression.Parent?.Kind() == SyntaxKind.ParenthesizedExpression)
                expression = (ExpressionSyntax)expression.Parent;

            return expression;
        }

        /// <summary>
        /// Returns lowest expression in parentheses or self if the expression is not parenthesized.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            while (expression.Kind() == SyntaxKind.ParenthesizedExpression)
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;

            return expression;
        }

        internal static ExpressionSyntax WalkDownParenthesesIf(this ExpressionSyntax expression, bool condition)
        {
            return (condition) ? WalkDownParentheses(expression) : expression;
        }

        internal static bool IsNumericLiteralExpression(this ExpressionSyntax expression, string valueText)
        {
            return expression.IsKind(SyntaxKind.NumericLiteralExpression)
                && string.Equals(((LiteralExpressionSyntax)expression).Token.ValueText, valueText, StringComparison.Ordinal);
        }
        #endregion ExpressionSyntax

        #region FixedStatementSyntax
        internal static StatementSyntax EmbeddedStatement(this FixedStatementSyntax fixedStatement)
        {
            StatementSyntax statement = fixedStatement.Statement;

            return (statement?.Kind() == SyntaxKind.Block) ? null : statement;
        }
        #endregion FixedStatementSyntax

        #region ForStatementSyntax
        /// <summary>
        /// Absolute span of the parentheses, not including the leading and trailing trivia.
        /// </summary>
        /// <param name="forStatement"></param>
        /// <returns></returns>
        public static TextSpan ParenthesesSpan(this ForStatementSyntax forStatement)
        {
            if (forStatement == null)
                throw new ArgumentNullException(nameof(forStatement));

            return TextSpan.FromBounds(forStatement.OpenParenToken.SpanStart, forStatement.CloseParenToken.Span.End);
        }

        internal static StatementSyntax EmbeddedStatement(this ForStatementSyntax forStatement)
        {
            StatementSyntax statement = forStatement.Statement;

            return (statement?.Kind() == SyntaxKind.Block) ? null : statement;
        }
        #endregion ForStatementSyntax

        #region IfStatementSyntax
        internal static StatementSyntax SingleNonBlockStatementOrDefault(this IfStatementSyntax ifStatement)
        {
            return SingleNonBlockStatementOrDefault(ifStatement.Statement);
        }

        /// <summary>
        /// Returns true if the specified if statement is a simple if statement.
        /// Simple if statement is defined as follows: it is not a child of an else clause and it has no else clause.
        /// </summary>
        /// <param name="ifStatement"></param>
        /// <returns></returns>
        public static bool IsSimpleIf(this IfStatementSyntax ifStatement)
        {
            return ifStatement?.IsParentKind(SyntaxKind.ElseClause) == false
                && ifStatement.Else == null;
        }

        /// <summary>
        /// Returns topmost if statement of the if-else cascade the specified if statement is part of.
        /// </summary>
        /// <param name="ifStatement"></param>
        /// <returns></returns>
        public static IfStatementSyntax GetTopmostIf(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            while (true)
            {
                IfStatementSyntax parentIf = GetPreviousIf(ifStatement);

                if (parentIf != null)
                {
                    ifStatement = parentIf;
                }
                else
                {
                    break;
                }
            }

            return ifStatement;
        }

        /// <summary>
        /// Returns true if the specified if statement is not a child of an else clause.
        /// </summary>
        /// <param name="ifStatement"></param>
        /// <returns></returns>
        public static bool IsTopmostIf(this IfStatementSyntax ifStatement)
        {
            return ifStatement?.IsParentKind(SyntaxKind.ElseClause) == false;
        }

        internal static IfStatementSyntax GetNextIf(this IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Else?.Statement;

            if (statement?.Kind() == SyntaxKind.IfStatement)
                return (IfStatementSyntax)statement;

            return null;
        }

        internal static IfStatementSyntax GetPreviousIf(this IfStatementSyntax ifStatement)
        {
            SyntaxNode parent = ifStatement.Parent;

            if (parent?.Kind() == SyntaxKind.ElseClause)
            {
                parent = parent.Parent;

                if (parent?.Kind() == SyntaxKind.IfStatement)
                    return (IfStatementSyntax)parent;
            }

            return null;
        }

        internal static StatementSyntax EmbeddedStatement(this IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            return (statement?.Kind() == SyntaxKind.Block) ? null : statement;
        }

        /// <summary>
        /// Returns <see cref="IfStatementCascade"/> that enables to enumerate if-else cascade.
        /// </summary>
        /// <param name="ifStatement"></param>
        /// <returns></returns>
        public static IfStatementCascade AsCascade(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return new IfStatementCascade(ifStatement);
        }
        #endregion IfStatementSyntax

        #region IEnumerable<T>
        /// <summary>
        /// Creates a list of syntax nodes from a sequence of nodes.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static SyntaxList<TNode> ToSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : SyntaxNode
        {
            return List(nodes);
        }

        /// <summary>
        /// Creates a separated list of syntax nodes from a sequence of nodes.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : SyntaxNode
        {
            return SeparatedList(nodes);
        }

        /// <summary>
        /// Creates a separated list of syntax nodes from a sequence of nodes and tokens.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="nodesAndTokens"></param>
        /// <returns></returns>
        public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<SyntaxNodeOrToken> nodesAndTokens) where TNode : SyntaxNode
        {
            return SeparatedList<TNode>(nodesAndTokens);
        }
        #endregion IEnumerable<T>

        #region IndexerDeclarationSyntax
        internal static TextSpan HeaderSpan(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return TextSpan.FromBounds(
                indexerDeclaration.SpanStart,
                indexerDeclaration.ParameterList?.Span.End ?? indexerDeclaration.ThisKeyword.Span.End);
        }

        /// <summary>
        /// Returns a get accessor that is contained in the specified indexer declaration.
        /// </summary>
        /// <param name="indexerDeclaration"></param>
        /// <returns></returns>
        public static AccessorDeclarationSyntax Getter(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration
                .AccessorList?
                .Getter();
        }

        /// <summary>
        /// Returns a set accessor that is contained in the specified indexer declaration.
        /// </summary>
        /// <param name="indexerDeclaration"></param>
        /// <returns></returns>
        public static AccessorDeclarationSyntax Setter(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration
                .AccessorList?
                .Setter();
        }
        #endregion IndexerDeclarationSyntax

        #region InterfaceDeclarationSyntax
        /// <summary>
        /// The absolute span of the braces, not including it leading and trailing trivia.
        /// </summary>
        /// <param name="interfaceDeclaration"></param>
        /// <returns></returns>
        public static TextSpan BracesSpan(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return TextSpan.FromBounds(
                interfaceDeclaration.OpenBraceToken.SpanStart,
                interfaceDeclaration.CloseBraceToken.Span.End);
        }

        /// <summary>
        /// Creates a new <see cref="InterfaceDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="interfaceDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static InterfaceDeclarationSyntax WithMembers(
            this InterfaceDeclarationSyntax interfaceDeclaration,
            MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithMembers(SingletonList(member));
        }

        /// <summary>
        /// Creates a new <see cref="InterfaceDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="interfaceDeclaration"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static InterfaceDeclarationSyntax WithMembers(
            this InterfaceDeclarationSyntax interfaceDeclaration,
            IEnumerable<MemberDeclarationSyntax> members)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithMembers(List(members));
        }
        #endregion InterfaceDeclarationSyntax

        #region InterpolatedStringExpressionSyntax
        /// <summary>
        /// Returns true if the specified interpolated string is a verbatim.
        /// </summary>
        /// <param name="interpolatedString"></param>
        /// <returns></returns>
        public static bool IsVerbatim(this InterpolatedStringExpressionSyntax interpolatedString)
        {
            return interpolatedString?.StringStartToken.ValueText.Contains("@") == true;
        }
        #endregion InterpolatedStringExpressionSyntax

        #region InvocationExpressionSyntax
        internal static ExpressionSyntax WalkDownMethodChain(
            this InvocationExpressionSyntax invocationExpression,
            bool walkInvocation = true,
            bool walkElementAccess = true)
        {
            ExpressionSyntax expression = invocationExpression;
            ExpressionSyntax current = invocationExpression.Expression;

            while (current.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)current;

                current = memberAccessExpression.Expression;

                SyntaxKind kind = current.Kind();

                if (kind == SyntaxKind.InvocationExpression)
                {
                    if (walkInvocation)
                    {
                        invocationExpression = (InvocationExpressionSyntax)current;
                        expression = invocationExpression;
                        current = invocationExpression.Expression;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (kind == SyntaxKind.ElementAccessExpression)
                {
                    if (walkElementAccess)
                    {
                        var elementAccess = (ElementAccessExpressionSyntax)current;
                        expression = elementAccess;
                        current = elementAccess.Expression;
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

            return expression;
        }
        #endregion InvocationExpressionSyntax

        #region LiteralExpressionSyntax
        /// <summary>
        /// Returns true if the specified literal expression is a hexadecimal numeric literal expression.
        /// </summary>
        /// <param name="literalExpression"></param>
        /// <returns></returns>
        public static bool IsHexNumericLiteral(this LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.NumericLiteralExpression)
                && literalExpression.Token.Text.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        }
        #endregion LiteralExpressionSyntax

        #region LocalFunctionStatementSyntax
        /// <summary>
        /// Returns local function body or an expression body if the body is null.
        /// </summary>
        /// <param name="localFunctionStatement"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode BodyOrExpressionBody(this LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                throw new ArgumentNullException(nameof(localFunctionStatement));

            return localFunctionStatement.Body ?? (CSharpSyntaxNode)localFunctionStatement.ExpressionBody;
        }

        /// <summary>
        /// Returns true if the specified local function' return type is <see cref="void"/>.
        /// </summary>
        /// <param name="localFunctionStatement"></param>
        /// <returns></returns>
        public static bool ReturnsVoid(this LocalFunctionStatementSyntax localFunctionStatement)
        {
            return localFunctionStatement?.ReturnType?.IsVoid() == true;
        }

        /// <summary>
        /// Returns true if the specified local function contains yield statement. Nested local functions are excluded.
        /// </summary>
        /// <param name="localFunctionStatement"></param>
        /// <returns></returns>
        public static bool ContainsYield(this LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                throw new ArgumentNullException(nameof(localFunctionStatement));

            return localFunctionStatement.Body?.ContainsYield() == true;
        }
        #endregion LocalFunctionStatementSyntax

        #region LockStatementSyntax
        internal static StatementSyntax EmbeddedStatement(this LockStatementSyntax lockStatement)
        {
            StatementSyntax statement = lockStatement.Statement;

            return (statement?.Kind() == SyntaxKind.Block) ? null : statement;
        }
        #endregion LockStatementSyntax

        #region MemberDeclarationSyntax
        /// <summary>
        /// Returns single-line documentation comment that is part of the specified declaration.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static SyntaxTrivia GetSingleLineDocumentationCommentTrivia(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            foreach (SyntaxTrivia trivia in member.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        /// <summary>
        /// Returns documentation comment that is part of the specified declaration.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static SyntaxTrivia GetDocumentationCommentTrivia(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            foreach (SyntaxTrivia trivia in member.GetLeadingTrivia())
            {
                if (trivia.IsDocumentationCommentTrivia())
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        /// <summary>
        /// Returns single-line documentation comment syntax that is part of the specified declaration.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static DocumentationCommentTriviaSyntax GetSingleLineDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode structure = member.GetSingleLineDocumentationCommentTrivia().GetStructure();

            if (structure?.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
                return (DocumentationCommentTriviaSyntax)structure;

            return null;
        }

        /// <summary>
        /// Returns documentation comment syntax that is part of the specified declaration.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static DocumentationCommentTriviaSyntax GetDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxTrivia trivia = member.GetDocumentationCommentTrivia();

            if (trivia.IsDocumentationCommentTrivia()
                && trivia.GetStructure() is DocumentationCommentTriviaSyntax comment
                && SyntaxFacts.IsDocumentationCommentTrivia(comment.Kind()))
            {
                return comment;
            }

            return null;
        }

        /// <summary>
        /// Returns true if the specified declaration has a single-line documentation comment.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool HasSingleLineDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member
                .GetLeadingTrivia()
                .Any(f => f.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        }

        /// <summary>
        /// Returns true if the specified declaration has a documentation comment.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool HasDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member
                .GetLeadingTrivia()
                .Any(IsDocumentationCommentTrivia);
        }

        internal static TMember WithNewSingleLineDocumentationComment<TMember>(
            this TMember member,
            DocumentationCommentGeneratorSettings settings = null) where TMember : MemberDeclarationSyntax
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            DocumentationCommentInserter inserter = DocumentationCommentInserter.Create(member);

            settings = settings ?? DocumentationCommentGeneratorSettings.Default;

            settings = settings.WithIndentation(inserter.Indent);

            SyntaxTriviaList comment = DocumentationCommentGenerator.Generate(member, settings);

            SyntaxTriviaList newLeadingTrivia = inserter.InsertRange(comment);

            return member.WithLeadingTrivia(newLeadingTrivia);
        }

        internal static TMember WithBaseOrNewSingleLineDocumentationComment<TMember>(
            this TMember member,
            SemanticModel semanticModel,
            DocumentationCommentGeneratorSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken)) where TMember : MemberDeclarationSyntax
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (DocumentationCommentGenerator.CanGenerateFromBase(member.Kind()))
            {
                DocumentationCommentData info = DocumentationCommentGenerator.GenerateFromBase(member, semanticModel, cancellationToken);

                if (info.Success)
                    return member.WithDocumentationComment(info.Comment, indent: true);
            }

            return WithNewSingleLineDocumentationComment(member, settings);
        }

        internal static TMember WithDocumentationComment<TMember>(
            this TMember member,
            SyntaxTrivia comment,
            bool indent = false) where TMember : MemberDeclarationSyntax
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            DocumentationCommentInserter inserter = DocumentationCommentInserter.Create(member);

            SyntaxTriviaList newLeadingTrivia = inserter.Insert(comment, indent: indent);

            return member.WithLeadingTrivia(newLeadingTrivia);
        }

        /// <summary>
        /// Returns true if the specified method contains yield statement. Nested local functions are excluded.
        /// </summary>
        /// <param name="methodDeclaration"></param>
        /// <returns></returns>
        public static bool ContainsYield(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.Body?.ContainsYield() == true;
        }
        #endregion MemberDeclarationSyntax

        #region MethodDeclarationSyntax
        /// <summary>
        /// Returns true if the specified method return type is <see cref="void"/>.
        /// </summary>
        /// <param name="methodDeclaration"></param>
        /// <returns></returns>
        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration?.ReturnType?.IsVoid() == true;
        }

        internal static TextSpan HeaderSpan(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return TextSpan.FromBounds(
                methodDeclaration.SpanStart,
                methodDeclaration.ParameterList?.Span.End ?? methodDeclaration.Identifier.Span.End);
        }

        /// <summary>
        /// Returns method body or an expression body if the body is null.
        /// </summary>
        /// <param name="methodDeclaration"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode BodyOrExpressionBody(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.Body ?? (CSharpSyntaxNode)methodDeclaration.ExpressionBody;
        }
        #endregion MethodDeclarationSyntax

        #region NamespaceDeclarationSyntax
        /// <summary>
        /// Creates a new <see cref="NamespaceDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="namespaceDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(SingletonList(member));
        }

        /// <summary>
        /// Creates a new <see cref="NamespaceDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="namespaceDeclaration"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            IEnumerable<MemberDeclarationSyntax> members)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(List(members));
        }

        /// <summary>
        /// The absolute span of the braces, not including leading and trailing trivia.
        /// </summary>
        /// <param name="namespaceDeclaration"></param>
        /// <returns></returns>
        public static TextSpan BracesSpan(this NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return TextSpan.FromBounds(
                namespaceDeclaration.OpenBraceToken.SpanStart,
                namespaceDeclaration.CloseBraceToken.Span.End);
        }
        #endregion NamespaceDeclarationSyntax

        #region OperatorDeclarationSyntax
        /// <summary>
        /// Returns operator body or an expression body if the body is null.
        /// </summary>
        /// <param name="operatorDeclaration"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode BodyOrExpressionBody(this OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return operatorDeclaration.Body ?? (CSharpSyntaxNode)operatorDeclaration.ExpressionBody;
        }
        #endregion OperatorDeclarationSyntax

        #region ParameterSyntax
        /// <summary>
        /// Returns true if the specified parameter has "params" modifier.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool IsParams(this ParameterSyntax parameter)
        {
            return parameter?.Modifiers.Contains(SyntaxKind.ParamsKeyword) == true;
        }

        internal static SeparatedSyntaxList<ParameterSyntax> GetContainingList(this ParameterSyntax parameter)
        {
            if (parameter?.Parent is ParameterListSyntax parameterList)
            {
                return parameterList.Parameters;
            }
            else
            {
                return default(SeparatedSyntaxList<ParameterSyntax>);
            }
        }
        #endregion ParameterSyntax

        #region PropertyDeclarationSyntax
        internal static TextSpan HeaderSpan(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return TextSpan.FromBounds(
                propertyDeclaration.SpanStart,
                propertyDeclaration.Identifier.Span.End);
        }

        /// <summary>
        /// Returns property get accessor, if any.
        /// </summary>
        /// <param name="propertyDeclaration"></param>
        /// <returns></returns>
        public static AccessorDeclarationSyntax Getter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Getter();
        }

        /// <summary>
        /// Returns property set accessor, if any.
        /// </summary>
        /// <param name="propertyDeclaration"></param>
        /// <returns></returns>
        public static AccessorDeclarationSyntax Setter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Setter();
        }
        #endregion PropertyDeclarationSyntax

        #region RegionDirectiveTriviaSyntax
        /// <summary>
        /// Returns endregion directive that is related to the specified region directive. Returns null if no matching endregion directive is found.
        /// </summary>
        /// <param name="regionDirective"></param>
        /// <returns></returns>
        public static EndRegionDirectiveTriviaSyntax GetEndRegionDirective(this RegionDirectiveTriviaSyntax regionDirective)
        {
            if (regionDirective == null)
                throw new ArgumentNullException(nameof(regionDirective));

            RegionInfo region = SyntaxInfo.RegionInfo(regionDirective);

            return (region.Success) ? region.EndDirective : null;
        }

        /// <summary>
        /// Gets preprocessing message for the specified region directive if such message exists.
        /// </summary>
        /// <param name="regionDirective"></param>
        /// <returns></returns>
        public static SyntaxTrivia GetPreprocessingMessageTrivia(this RegionDirectiveTriviaSyntax regionDirective)
        {
            if (regionDirective == null)
                throw new ArgumentNullException(nameof(regionDirective));

            SyntaxToken endOfDirective = regionDirective.EndOfDirectiveToken;

            SyntaxTriviaList leading = endOfDirective.LeadingTrivia;

            if (leading.Count == 1)
            {
                SyntaxTrivia trivia = leading[0];

                if (trivia.IsKind(SyntaxKind.PreprocessingMessageTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        /// <summary>
        /// Returns true the specified region directive has preprocessing message trivia.
        /// </summary>
        /// <param name="regionDirective"></param>
        /// <returns></returns>
        internal static bool HasPreprocessingMessageTrivia(this RegionDirectiveTriviaSyntax regionDirective)
        {
            return GetPreprocessingMessageTrivia(regionDirective).Kind() == SyntaxKind.PreprocessingMessageTrivia;
        }
        #endregion RegionDirectiveTriviaSyntax

        #region SeparatedSyntaxList<T>
        /// <summary>
        /// Searches for a node of the specified kind and returns the zero-based index of the last occurrence within the entire <see cref="SeparatedSyntaxList{TNode}"/>.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="list"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static int LastIndexOf<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        /// <summary>
        /// Searches for a node of the specified kind and returns the zero-based index of the first occurrence within the entire <see cref="SeparatedSyntaxList{TNode}"/>.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="list"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static bool Contains<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        /// <summary>
        /// Searches for a node of the specified kind and returns the first occurrence within the entire <see cref="SeparatedSyntaxList{TNode}"/>.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="list"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static TNode Find<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            int index = list.IndexOf(kind);

            if (index != -1)
                return list[index];

            return default(TNode);
        }

        internal static bool IsSingleLine<TNode>(
            this SeparatedSyntaxList<TNode> list,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            int count = list.Count;

            if (count == 0)
                return false;

            SyntaxNode firstNode = list.First();

            if (count == 1)
                return IsSingleLine(firstNode, includeExteriorTrivia, trim, cancellationToken);

            SyntaxTree tree = firstNode.SyntaxTree;

            if (tree == null)
                return false;

            TextSpan span = TextSpan.FromBounds(
                GetStartIndex(firstNode, includeExteriorTrivia, trim),
                GetEndIndex(list.Last(), includeExteriorTrivia, trim));

            return tree.IsSingleLineSpan(span, cancellationToken);
        }

        internal static bool IsMultiLine<TNode>(
            this SeparatedSyntaxList<TNode> list,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            int count = list.Count;

            if (count == 0)
                return false;

            SyntaxNode firstNode = list.First();

            if (count == 1)
                return IsMultiLine(firstNode, includeExteriorTrivia, trim, cancellationToken);

            SyntaxTree tree = firstNode.SyntaxTree;

            if (tree == null)
                return false;

            TextSpan span = TextSpan.FromBounds(
                GetStartIndex(firstNode, includeExteriorTrivia, trim),
                GetEndIndex(list.Last(), includeExteriorTrivia, trim));

            return tree.IsMultiLineSpan(span, cancellationToken);
        }

        internal static SeparatedSyntaxList<TNode> ReplaceRangeAt<TNode>(
            this SeparatedSyntaxList<TNode> list,
            int startIndex,
            int count,
            IEnumerable<TNode> newNodes) where TNode : SyntaxNode
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "");

            if (count < 0
                || startIndex + count > list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "");
            }

            return SeparatedList(ReplaceRange());

            IEnumerable<TNode> ReplaceRange()
            {
                SeparatedSyntaxList<TNode>.Enumerator en = list.GetEnumerator();

                int i = 0;

                while (i < startIndex
                    && en.MoveNext())
                {
                    yield return en.Current;
                    i++;
                }

                int endIndex = startIndex + count;

                while (i < endIndex
                    && en.MoveNext())
                {
                    i++;
                }

                foreach (TNode newNode in newNodes)
                    yield return newNode;

                while (en.MoveNext())
                    yield return en.Current;
            }
        }
        #endregion SeparatedSyntaxList<T>

        #region StatementSyntax
        /// <summary>
        /// Gets the previous statement of the specified statement.
        /// If the specified statement is not contained in the list, or if there is no previous statement, then this method returns null.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public static StatementSyntax PreviousStatement(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxList<StatementSyntax> statements = SyntaxInfo.StatementListInfo(statement).Statements;

            if (statements.Any())
            {
                int index = statements.IndexOf(statement);

                if (index > 0)
                    return statements[index - 1];
            }

            return null;
        }

        /// <summary>
        /// Gets the next statement of the specified statement.
        /// If the specified statement is not contained in the list, or if there is no next statement, then this method returns null.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public static StatementSyntax NextStatement(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxList<StatementSyntax> statements = SyntaxInfo.StatementListInfo(statement).Statements;

            if (statements.Any())
            {
                int index = statements.IndexOf(statement);

                if (index < statements.Count - 1)
                    return statements[index + 1];
            }

            return null;
        }

        /// <summary>
        /// Gets a list the specified statement is contained in.
        /// This method succeeds if the statement is in a block's statements or a switch section's statements.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="statements"></param>
        /// <returns>True if the statement is contained in the list; otherwise, false.</returns>
        public static bool TryGetContainingList(this StatementSyntax statement, out SyntaxList<StatementSyntax> statements)
        {
            statements = SyntaxInfo.StatementListInfo(statement).Statements;

            return statements.Any();
        }

        private static StatementSyntax SingleNonBlockStatementOrDefault(this StatementSyntax statement, bool recursive = false)
        {
            return (statement.Kind() == SyntaxKind.Block)
                ? SingleNonBlockStatementOrDefault((BlockSyntax)statement, recursive)
                : statement;
        }

        /// <summary>
        /// Returns true if the specified statement is an embedded statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="canBeBlock">Block can be considered as embedded statement</param>
        /// <param name="canBeIfInsideElse">If statement that is a child of an else statement can be considered as an embedded statement.</param>
        /// <param name="canBeUsingInsideUsing">Using statement that is a child of an using statement can be considered as en embedded statement.</param>
        /// <returns></returns>
        public static bool IsEmbedded(
            this StatementSyntax statement,
            bool canBeBlock = false,
            bool canBeIfInsideElse = true,
            bool canBeUsingInsideUsing = true)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxKind kind = statement.Kind();

            if (!canBeBlock
                && kind == SyntaxKind.Block)
            {
                return false;
            }

            if (!CSharpFacts.CanBeEmbeddedStatement(kind))
                return false;

            SyntaxNode parent = statement.Parent;

            if (parent == null)
                return false;

            SyntaxKind parentKind = parent.Kind();

            return CSharpFacts.CanHaveEmbeddedStatement(parentKind)
                && (canBeIfInsideElse
                    || kind != SyntaxKind.IfStatement
                    || parentKind != SyntaxKind.ElseClause)
                && (canBeUsingInsideUsing
                    || kind != SyntaxKind.UsingStatement
                    || parentKind != SyntaxKind.UsingStatement);
        }
        #endregion StatementSyntax

        #region StructDeclarationSyntax
        /// <summary>
        /// Creates a new <see cref="StructDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="structDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static StructDeclarationSyntax WithMembers(
            this StructDeclarationSyntax structDeclaration,
            MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return structDeclaration.WithMembers(SingletonList(member));
        }

        /// <summary>
        /// Creates a new <see cref="StructDeclarationSyntax"/> with the members updated.
        /// </summary>
        /// <param name="structDeclaration"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static StructDeclarationSyntax WithMembers(
            this StructDeclarationSyntax structDeclaration,
            IEnumerable<MemberDeclarationSyntax> members)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return structDeclaration.WithMembers(List(members));
        }

        /// <summary>
        /// The absolute span of the braces, not including its leading and trailing trivia.
        /// </summary>
        /// <param name="structDeclaration"></param>
        /// <returns></returns>
        public static TextSpan BracesSpan(this StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return TextSpan.FromBounds(
                structDeclaration.OpenBraceToken.SpanStart,
                structDeclaration.CloseBraceToken.Span.End);
        }
        #endregion StructDeclarationSyntax

        #region SwitchSectionSyntax
        /// <summary>
        /// Returns true if the specified switch section contains default switch label.
        /// </summary>
        /// <param name="switchSection"></param>
        /// <returns></returns>
        public static bool ContainsDefaultLabel(this SwitchSectionSyntax switchSection)
        {
            return switchSection?.Labels.Any(f => f.IsKind(SyntaxKind.DefaultSwitchLabel)) == true;
        }
        #endregion SwitchSectionSyntax

        #region SwitchStatementSyntax
        /// <summary>
        /// Returns a section that contains default label, or null if the specified swtich statement does not contains section with default label.
        /// </summary>
        /// <param name="switchStatement"></param>
        /// <returns></returns>
        public static SwitchSectionSyntax DefaultSection(this SwitchStatementSyntax switchStatement)
        {
            if (switchStatement == null)
                throw new ArgumentNullException(nameof(switchStatement));

            foreach (SwitchSectionSyntax section in switchStatement.Sections)
            {
                if (section.Labels.Any(SyntaxKind.DefaultSwitchLabel))
                    return section;
            }

            return null;
        }
        #endregion SwitchStatementSyntax

        #region SyntaxList<T>
        /// <summary>
        /// Searches for a node of the specified kind and returns the zero-based index of the last occurrence within the entire <see cref="SyntaxList{TNode}"/>.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="list"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static int LastIndexOf<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        /// <summary>
        /// Returns true if a node of the specified kind is in the <see cref="SyntaxList{TNode}"/>.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="list"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static bool Contains<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        /// <summary>
        /// Searches for a node of the specified kind and returns the first occurrence within the entire <see cref="SyntaxList{TNode}"/>.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="list"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static TNode Find<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            int index = list.IndexOf(kind);

            if (index != -1)
                return list[index];

            return default(TNode);
        }

        internal static bool IsSingleLine<TNode>(
            this SyntaxList<TNode> list,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            int count = list.Count;

            if (count == 0)
                return false;

            SyntaxNode firstNode = list.First();

            if (count == 1)
                return IsSingleLine(firstNode, includeExteriorTrivia, trim, cancellationToken);

            SyntaxTree tree = firstNode.SyntaxTree;

            if (tree == null)
                return false;

            TextSpan span = TextSpan.FromBounds(
                GetStartIndex(firstNode, includeExteriorTrivia, trim),
                GetEndIndex(list.Last(), includeExteriorTrivia, trim));

            return tree.IsSingleLineSpan(span, cancellationToken);
        }

        internal static bool IsMultiLine<TNode>(
            this SyntaxList<TNode> list,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            int count = list.Count;

            if (count == 0)
                return false;

            SyntaxNode firstNode = list.First();

            if (count == 1)
                return IsMultiLine(firstNode, includeExteriorTrivia, trim, cancellationToken);

            SyntaxTree tree = firstNode.SyntaxTree;

            if (tree == null)
                return false;

            TextSpan span = TextSpan.FromBounds(
                GetStartIndex(firstNode, includeExteriorTrivia, trim),
                GetEndIndex(list.Last(), includeExteriorTrivia, trim));

            return tree.IsMultiLineSpan(span, cancellationToken);
        }

        internal static StatementSyntax SingleOrDefault(this SyntaxList<StatementSyntax> statements, bool ignoreLocalFunctions, bool shouldThrow)
        {
            return (ignoreLocalFunctions)
                ? statements.SingleOrDefault(statement => statement.Kind() != SyntaxKind.LocalFunctionStatement, shouldThrow: shouldThrow)
                : statements.SingleOrDefault(shouldThrow: shouldThrow);
        }

        /// <summary>
        /// Returns true if the specified statement is a last statement in the list.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="statement"></param>
        /// <param name="ignoreLocalFunctions">Ignore local function statements at the end of the list.</param>
        /// <returns></returns>
        public static bool IsLast(
            this SyntaxList<StatementSyntax> statements,
            StatementSyntax statement,
            bool ignoreLocalFunctions)
        {
            if (!ignoreLocalFunctions)
                return statements.IsLast(statement);

            for (int i = statements.Count - 1; i >= 0; i--)
            {
                StatementSyntax s = statements[i];

                if (!s.IsKind(SyntaxKind.LocalFunctionStatement))
                    return s == statement;
            }

            return false;
        }

        /// <summary>
        /// Creates a new list with the specified node added or inserted.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="statement"></param>
        /// <param name="ignoreLocalFunctions">Insert statement before local function statements at the end of the list.</param>
        /// <returns></returns>
        public static SyntaxList<StatementSyntax> Add(
            this SyntaxList<StatementSyntax> statements,
            StatementSyntax statement,
            bool ignoreLocalFunctions)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (!ignoreLocalFunctions)
                return statements.Add(statement);

            int count = statements.Count;

            int index = count;

            for (int i = count - 1; i >= 0; i--)
            {
                if (statements[i].IsKind(SyntaxKind.LocalFunctionStatement))
                    index--;
            }

            return statements.Insert(index, statement);
        }

        internal static SyntaxList<TNode> ReplaceRangeAt<TNode>(
            this SyntaxList<TNode> list,
            int startIndex,
            int count,
            IEnumerable<TNode> newNodes) where TNode : SyntaxNode
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "");

            if (count < 0
                || startIndex + count > list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "");
            }

            return List(ReplaceRange());

            IEnumerable<TNode> ReplaceRange()
            {
                SyntaxList<TNode>.Enumerator en = list.GetEnumerator();

                int i = 0;

                while (i < startIndex
                    && en.MoveNext())
                {
                    yield return en.Current;
                    i++;
                }

                int endIndex = startIndex + count;

                while (i < endIndex
                    && en.MoveNext())
                {
                    i++;
                }

                foreach (TNode newNode in newNodes)
                    yield return newNode;

                while (en.MoveNext())
                    yield return en.Current;
            }
        }
        #endregion SyntaxList<T>

        #region SyntaxNode
        internal static IEnumerable<DirectiveTriviaSyntax> DescendantPreprocessorDirectives(this SyntaxNode node, Func<DirectiveTriviaSyntax, bool> predicate = null)
        {
            return DescendantPreprocessorDirectives(node, node.FullSpan, predicate);
        }

        internal static IEnumerable<DirectiveTriviaSyntax> DescendantPreprocessorDirectives(this SyntaxNode node, TextSpan span, Func<DirectiveTriviaSyntax, bool> predicate = null)
        {
            foreach (SyntaxTrivia trivia in node.DescendantTrivia(span: span, descendIntoTrivia: true))
            {
                if (trivia.IsDirective
                    && trivia.HasStructure
                    && (trivia.GetStructure() is DirectiveTriviaSyntax directive))
                {
                    if (predicate == null
                        || predicate(directive))
                    {
                        yield return directive;
                    }
                }
            }
        }

        internal static IEnumerable<DirectiveTriviaSyntax> DescendantRegionDirectives(this SyntaxNode node)
        {
            return DescendantPreprocessorDirectives(node, predicate: f => f.IsKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia));
        }

        internal static IEnumerable<DirectiveTriviaSyntax> DescendantRegionDirectives(this SyntaxNode node, TextSpan span)
        {
            return DescendantPreprocessorDirectives(node, span, predicate: f => f.IsKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia));
        }

        /// <summary>
        /// Returns true if a node is a descendant of a node with the specified kind.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static bool IsDescendantOf(this SyntaxNode node, SyntaxKind kind, bool ascendOutOfTrivia = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.Ancestors(ascendOutOfTrivia).Any(f => f.IsKind(kind));
        }

        /// <summary>
        /// Returns true if a node's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2;
        }

        /// <summary>
        /// Returns true if a node's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        /// <summary>
        /// Returns true if a node's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        /// <summary>
        /// Returns true if a node's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        /// <summary>
        /// Returns true if a node's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <param name="kind6"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5
                || kind == kind6;
        }

        /// <summary>
        /// Returns true if a node parent's kind is the specified kind.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind)
        {
            return node?.Parent.IsKind(kind) == true;
        }

        /// <summary>
        /// Returns true if a node parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            return IsKind(node?.Parent, kind1, kind2);
        }

        /// <summary>
        /// Returns true if a node parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3);
        }

        /// <summary>
        /// Returns true if a node parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3, kind4);
        }

        /// <summary>
        /// Returns true if a node parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3, kind4, kind5);
        }

        /// <summary>
        /// Returns true if a node parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <param name="kind6"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3, kind4, kind5, kind6);
        }

        internal static bool IsSingleLine(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTree syntaxTree = node.SyntaxTree;

            if (syntaxTree != null)
            {
                TextSpan span = GetSpan(node, includeExteriorTrivia, trim);

                return syntaxTree.IsSingleLineSpan(span, cancellationToken);
            }
            else
            {
                return false;
            }
        }

        internal static bool IsMultiLine(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTree syntaxTree = node.SyntaxTree;

            if (syntaxTree != null)
            {
                TextSpan span = GetSpan(node, includeExteriorTrivia, trim);

                return syntaxTree.IsMultiLineSpan(span, cancellationToken);
            }
            else
            {
                return false;
            }
        }

        private static TextSpan GetSpan(SyntaxNode node, bool includeExteriorTrivia, bool trim)
        {
            return TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia, trim),
                GetEndIndex(node, includeExteriorTrivia, trim));
        }

        private static int GetStartIndex(SyntaxNode node, bool includeExteriorTrivia, bool trim)
        {
            if (!includeExteriorTrivia)
                return node.SpanStart;

            int start = node.FullSpan.Start;

            if (trim)
            {
                SyntaxTriviaList leading = node.GetLeadingTrivia();

                for (int i = 0; i < leading.Count; i++)
                {
                    if (!leading[i].IsWhitespaceOrEndOfLineTrivia())
                        break;

                    start = leading[i].Span.End;
                }
            }

            return start;
        }

        private static int GetEndIndex(SyntaxNode node, bool includeExteriorTrivia, bool trim)
        {
            if (!includeExteriorTrivia)
                return node.Span.End;

            int end = node.FullSpan.End;

            if (trim)
            {
                SyntaxTriviaList trailing = node.GetTrailingTrivia();

                for (int i = trailing.Count - 1; i >= 0; i--)
                {
                    if (!trailing[i].IsWhitespaceOrEndOfLineTrivia())
                        break;

                    end = trailing[i].SpanStart;
                }
            }

            return end;
        }

        /// <summary>
        /// Removes all leading whitespace from the leading trivia and returns a new node with the new leading trivia.
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> and <see cref="SyntaxKind.EndOfLineTrivia"/> is considered to be a whitespace.
        /// Returns the same node if there is nothing to trim.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static TNode TrimLeadingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();

            int count = leadingTrivia.Count;

            if (count == 0)
                return node;

            for (int i = 0; i < count ; i++)
            {
                if (!leadingTrivia[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i == 0)
                    {
                        return node;
                    }
                    else
                    {
                        return node.WithLeadingTrivia(leadingTrivia.Skip(i));
                    }
                }
            }

            return node.WithoutLeadingTrivia();
        }

        /// <summary>
        /// Removes all trailing whitespace from the trailing trivia and returns a new node with the new trailing trivia.
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> and <see cref="SyntaxKind.EndOfLineTrivia"/> is considered to be a whitespace.
        /// Returns the same node if there is nothing to trim.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static TNode TrimTrailingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList trailingTrivia = node.GetTrailingTrivia();

            int count = trailingTrivia.Count;

            if (count == 0)
                return node;

            for (int i = count - 1; i >= 0; i--)
            {
                if (!trailingTrivia[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i == count - 1)
                    {
                        return node;
                    }
                    else
                    {
                        return node.WithTrailingTrivia(trailingTrivia.Take(i + 1));
                    }
                }
            }

            return node.WithoutTrailingTrivia();
        }

        /// <summary>
        /// Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new node with the new trivia.
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> and <see cref="SyntaxKind.EndOfLineTrivia"/> is considered to be a whitespace.
        /// Returns the same node if there is nothing to trim.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static TNode TrimTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .TrimLeadingTrivia()
                .TrimTrailingTrivia();
        }

        internal static TextSpan TrimmedSpan(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia: true, trim: true),
                GetEndIndex(node, includeExteriorTrivia: true, trim: true));
        }

        /// <summary>
        /// Gets the first ancestor of the specified kind.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestor(
            this SyntaxNode node,
            SyntaxKind kind,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestor(node, f => f.IsKind(kind), ascendOutOfTrivia);
        }

        /// <summary>
        /// Gets the first ancestor of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestor(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestor(node, f => f.IsKind(kind1, kind2), ascendOutOfTrivia);
        }

        /// <summary>
        /// Gets the first ancestor of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestor(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            SyntaxKind kind3,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestor(node, f => f.IsKind(kind1, kind2, kind3), ascendOutOfTrivia);
        }

        /// <summary>
        /// Gets the first ancestor that matches the predicate.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestor(this SyntaxNode node, Func<SyntaxNode, bool> predicate, bool ascendOutOfTrivia = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            SyntaxNode parent = GetParent(node, ascendOutOfTrivia);

            if (parent != null)
            {
                return FirstAncestorOrSelf(parent, predicate, ascendOutOfTrivia);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the first ancestor of the specified kind.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestorOrSelf(
            this SyntaxNode node,
            SyntaxKind kind,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestorOrSelf(node, f => f.IsKind(kind), ascendOutOfTrivia);
        }

        /// <summary>
        /// Gets the first ancestor of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestorOrSelf(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestorOrSelf(node, f => f.IsKind(kind1, kind2), ascendOutOfTrivia);
        }

        /// <summary>
        /// Gets the first ancestor of the specified kinds.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestorOrSelf(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            SyntaxKind kind3,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestorOrSelf(node, f => f.IsKind(kind1, kind2, kind3), ascendOutOfTrivia);
        }

        /// <summary>
        /// Gets the first ancestor that matches the predicate.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <param name="ascendOutOfTrivia"></param>
        /// <returns></returns>
        public static SyntaxNode FirstAncestorOrSelf(this SyntaxNode node, Func<SyntaxNode, bool> predicate, bool ascendOutOfTrivia = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            while (node != null)
            {
                if (predicate(node))
                    return node;

                node = GetParent(node, ascendOutOfTrivia);
            }

            return null;
        }

        private static SyntaxNode GetParent(SyntaxNode node, bool ascendOutOfTrivia)
        {
            SyntaxNode parent = node.Parent;

            if (parent == null
                && ascendOutOfTrivia
                && (node is IStructuredTriviaSyntax structuredTrivia))
            {
                parent = structuredTrivia.ParentTrivia.Token.Parent;
            }

            return parent;
        }

        internal static TRoot RemoveNode<TRoot>(this TRoot root, SyntaxNode node) where TRoot : SyntaxNode
        {
            return SyntaxRemover.RemoveNode(root, node);
        }

        internal static TNode RemoveStatement<TNode>(this TNode node, StatementSyntax statement) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return node.RemoveNode(statement);
        }

        internal static TNode RemoveModifier<TNode>(this TNode node, SyntaxKind modifierKind) where TNode : SyntaxNode
        {
            return ModifierList.Remove(node, modifierKind);
        }

        internal static TNode RemoveModifiers<TNode>(this TNode node, SyntaxKind modifierKind1, SyntaxKind modifierKind2) where TNode : SyntaxNode
        {
            return node
                .RemoveModifier(modifierKind1)
                .RemoveModifier(modifierKind2);
        }

        internal static TNode RemoveModifiers<TNode>(this TNode node, SyntaxKind modifierKind1, SyntaxKind modifierKind2, SyntaxKind modifierKind3) where TNode : SyntaxNode
        {
            return node
                .RemoveModifier(modifierKind1)
                .RemoveModifier(modifierKind2)
                .RemoveModifier(modifierKind3);
        }

        internal static TNode RemoveModifier<TNode>(this TNode node, SyntaxToken modifier) where TNode : SyntaxNode
        {
            return ModifierList.Remove(node, modifier);
        }

        internal static TNode InsertModifier<TNode>(this TNode node, SyntaxKind modifierKind, IComparer<SyntaxKind> comparer = null) where TNode : SyntaxNode
        {
            return ModifierList.Insert(node, modifierKind, comparer);
        }

        internal static TNode InsertModifier<TNode>(this TNode node, SyntaxToken modifier, IComparer<SyntaxToken> comparer = null) where TNode : SyntaxNode
        {
            return ModifierList.Insert(node, modifier, comparer);
        }

        /// <summary>
        /// Creates a new node with the trivia removed.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static TNode RemoveTrivia<TNode>(this TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return SyntaxRemover.RemoveTrivia(node, span);
        }

        /// <summary>
        /// Creates a new node with the whitespace removed.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static TNode RemoveWhitespace<TNode>(this TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return SyntaxRemover.RemoveWhitespace(node, span);
        }

        /// <summary>
        /// Creates a new node with the whitespace replaced.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="replacement"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static TNode ReplaceWhitespace<TNode>(this TNode node, SyntaxTrivia replacement, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var replacer = new WhitespaceReplacer(replacement, span);

            return (TNode)replacer.Visit(node);
        }

        internal static bool IsPartOfDocumentationComment(this SyntaxNode node)
        {
            while (node != null)
            {
                if (node.IsStructuredTrivia
                    && SyntaxFacts.IsDocumentationCommentTrivia(node.Kind()))
                {
                    return true;
                }

                node = node.Parent;
            }

            return false;
        }

        internal static bool IsInExpressionTree(
            this SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return IsInExpressionTree(
                node,
                semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Expressions_Expression_1),
                semanticModel,
                cancellationToken);
        }

        internal static bool IsInExpressionTree(
            this SyntaxNode node,
            INamedTypeSymbol expressionType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expressionType != null)
            {
                for (SyntaxNode current = node; current != null; current = current.Parent)
                {
                    if (current.IsKind(SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression))
                    {
                        TypeInfo typeInfo = semanticModel.GetTypeInfo(current, cancellationToken);

                        if (expressionType.Equals(typeInfo.ConvertedType?.OriginalDefinition))
                            return true;
                    }
                }
            }

            return false;
        }

        internal static SyntaxTrivia GetIndentation(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTree tree = node.SyntaxTree;

            if (tree != null)
            {
                TextSpan span = node.Span;

                int lineStartIndex = span.Start - tree.GetLineSpan(span, cancellationToken).StartLinePosition.Character;

                while (!node.FullSpan.Contains(lineStartIndex))
                    node = node.Parent;

                SyntaxToken token = node.FindToken(lineStartIndex);

                if (!token.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

                    if (leadingTrivia.Any()
                        && leadingTrivia.FullSpan.Contains(lineStartIndex))
                    {
                        SyntaxTrivia trivia = leadingTrivia.Last();

                        if (trivia.IsWhitespaceTrivia())
                            return trivia;
                    }
                }
            }

            return EmptyWhitespace();
        }

        internal static SyntaxTriviaList GetIncreasedIndentation(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTrivia trivia = GetIndentation(node, cancellationToken);

            return IncreaseIndentation(trivia);
        }

        internal static SyntaxNode WalkUp(this SyntaxNode node, SyntaxKind kind)
        {
            while (node.Parent?.Kind() == kind)
                node = node.Parent;

            return node;
        }
        #endregion SyntaxNode

        #region SyntaxToken
        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <param name="kind6"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5
                || kind == kind6;
        }

        /// <summary>
        /// Removes all leading whitespace from the leading trivia and returns a new token with the new leading trivia.
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> and <see cref="SyntaxKind.EndOfLineTrivia"/> is considered to be a whitespace.
        /// Returns the same token if there is nothing to trim.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SyntaxToken TrimLeadingTrivia(this SyntaxToken token)
        {
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int count = leadingTrivia.Count;

            if (count == 0)
                return token;

            for (int i = 0; i < count; i++)
            {
                if (!leadingTrivia[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i == 0)
                    {
                        return token;
                    }
                    else
                    {
                        return token.WithLeadingTrivia(leadingTrivia.Skip(i));
                    }
                }
            }

            return token.WithoutLeadingTrivia();
        }

        /// <summary>
        /// Removes all trailing whitespace from the trailing trivia and returns a new token with the new trailing trivia.
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> and <see cref="SyntaxKind.EndOfLineTrivia"/> is considered to be a whitespace.
        /// Returns the same token if there is nothing to trim.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SyntaxToken TrimTrailingTrivia(this SyntaxToken token)
        {
            SyntaxTriviaList trailingTrivia = token.TrailingTrivia;

            int count = trailingTrivia.Count;

            if (count == 0)
                return token;

            for (int i = count - 1; i >= 0; i--)
            {
                if (!trailingTrivia[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i == count - 1)
                    {
                        return token;
                    }
                    else
                    {
                        return token.WithTrailingTrivia(trailingTrivia.Take(i + 1));
                    }
                }
            }

            return token.WithoutTrailingTrivia();
        }

        /// <summary>
        /// Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new token with the new trivia.
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> and <see cref="SyntaxKind.EndOfLineTrivia"/> is considered to be a whitespace.
        /// Returns the same token if there is nothing to trim.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SyntaxToken TrimTrivia(this SyntaxToken token)
        {
            return token
                .TrimLeadingTrivia()
                .TrimTrailingTrivia();
        }

        /// <summary>
        /// Returns true if a token of the specified kind is in the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="tokenList"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static bool Contains(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            return tokenList.IndexOf(kind) != -1;
        }

        /// <summary>
        /// Returns true if a token of the specified kinds is in the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="tokenList"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2)
        {
            return ContainsAny(tokenList, (int)kind1, (int)kind2);
        }

        /// <summary>
        /// Returns true if a token of the specified kinds is in the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="tokenList"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return ContainsAny(tokenList, (int)kind1, (int)kind2, (int)kind3);
        }

        /// <summary>
        /// Returns true if a token of the specified kinds is in the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="tokenList"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
        public static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return ContainsAny(tokenList, (int)kind1, (int)kind2, (int)kind3, (int)kind4);
        }

        /// <summary>
        /// Returns true if a token of the specified kinds is in the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="tokenList"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
        public static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return ContainsAny(tokenList, (int)kind1, (int)kind2, (int)kind3, (int)kind4, (int)kind5);
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, int rawKind1, int rawKind2)
        {
            foreach (SyntaxToken token in tokenList)
            {
                int rawKind = token.RawKind;

                if (rawKind == rawKind1
                    || rawKind == rawKind2)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, int rawKind1, int rawKind2, int rawKind3)
        {
            foreach (SyntaxToken token in tokenList)
            {
                int rawKind = token.RawKind;

                if (rawKind == rawKind1
                    || rawKind == rawKind2
                    || rawKind == rawKind3)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, int rawKind1, int rawKind2, int rawKind3, int rawKind4)
        {
            foreach (SyntaxToken token in tokenList)
            {
                int rawKind = token.RawKind;

                if (rawKind == rawKind1
                    || rawKind == rawKind2
                    || rawKind == rawKind3
                    || rawKind == rawKind4)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, int rawKind1, int rawKind2, int rawKind3, int rawKind4, int rawKind5)
        {
            foreach (SyntaxToken token in tokenList)
            {
                int rawKind = token.RawKind;

                if (rawKind == rawKind1
                    || rawKind == rawKind2
                    || rawKind == rawKind3
                    || rawKind == rawKind4
                    || rawKind == rawKind5)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if a token parent's kind is the specified kind.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind)
        {
            return token.Parent.IsKind(kind);
        }

        /// <summary>
        /// Returns true if a token parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
        {
            return IsKind(token.Parent, kind1, kind2);
        }

        /// <summary>
        /// Returns true if a token parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return IsKind(token.Parent, kind1, kind2, kind3);
        }

        /// <summary>
        /// Returns true if a token parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return IsKind(token.Parent, kind1, kind2, kind3, kind4);
        }

        /// <summary>
        /// Returns true if a token parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return IsKind(token.Parent, kind1, kind2, kind3, kind4, kind5);
        }

        /// <summary>
        /// Returns true if a token parent's kind is one of the specified kinds.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <param name="kind6"></param>
        /// <returns></returns>
        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            return IsKind(token.Parent, kind1, kind2, kind3, kind4, kind5, kind6);
        }
        #endregion SyntaxToken

        #region SyntaxTokenList
        /// <summary>
        /// Searches for a token of the specified kind and returns the first occurrence within the entire <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="tokenList"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static SyntaxToken Find(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            foreach (SyntaxToken token in tokenList)
            {
                if (token.IsKind(kind))
                    return token;
            }

            return default(SyntaxToken);
        }

        internal static SyntaxTokenList Replace(this SyntaxTokenList tokens, SyntaxKind kind, SyntaxToken newToken)
        {
            int i = 0;
            foreach (SyntaxToken token in tokens)
            {
                if (token.Kind() == kind)
                    return tokens.ReplaceAt(i, newToken.WithTriviaFrom(token));

                i++;
            }

            return tokens;
        }

        internal static SyntaxTokenList Replace(this SyntaxTokenList tokens, SyntaxKind kind, SyntaxKind newKind)
        {
            int i = 0;
            foreach (SyntaxToken token in tokens)
            {
                if (token.Kind() == kind)
                    return tokens.ReplaceAt(i, Token(newKind).WithTriviaFrom(token));

                i++;
            }

            return tokens;
        }
        #endregion SyntaxTokenList

        #region SyntaxTrivia
        /// <summary>
        /// Returns true if a trivia's kind is one of the specified kinds.
        /// </summary>
        /// <param name="trivia"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="trivia"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="trivia"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="trivia"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        /// <summary>
        /// Returns true if a token's kind is one of the specified kinds.
        /// </summary>
        /// <param name="trivia"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <param name="kind6"></param>
        /// <returns></returns>
        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5
                || kind == kind6;
        }

        /// <summary>
        /// Returns true if the trivia is <see cref="SyntaxKind.WhitespaceTrivia"/>.
        /// </summary>
        /// <param name="trivia"></param>
        /// <returns></returns>
        public static bool IsWhitespaceTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.WhitespaceTrivia);
        }

        /// <summary>
        /// Returns true if the trivia is <see cref="SyntaxKind.EndOfLineTrivia"/>.
        /// </summary>
        /// <param name="trivia"></param>
        /// <returns></returns>
        public static bool IsEndOfLineTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        /// <summary>
        /// Returns true if the trivia is either <see cref="SyntaxKind.WhitespaceTrivia"/> or <see cref="SyntaxKind.EndOfLineTrivia"/>.
        /// </summary>
        /// <param name="trivia"></param>
        /// <returns></returns>
        public static bool IsWhitespaceOrEndOfLineTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.WhitespaceTrivia, SyntaxKind.EndOfLineTrivia);
        }

        /// <summary>
        /// Returns true if the trivia is a documentation comment trivia.
        /// </summary>
        /// <param name="trivia"></param>
        /// <returns></returns>
        internal static bool IsDocumentationCommentTrivia(this SyntaxTrivia trivia)
        {
            return SyntaxFacts.IsDocumentationCommentTrivia(trivia.Kind());
        }

        internal static bool IsElasticMarker(this SyntaxTrivia trivia)
        {
            return trivia.IsWhitespaceTrivia()
                && trivia.Span.IsEmpty
                && trivia.HasAnnotation(SyntaxAnnotation.ElasticAnnotation);
        }
        #endregion SyntaxTrivia

        #region SyntaxTriviaList
        /// <summary>
        /// Searches for a trivia of the specified kind and returns the zero-based index of the last occurrence within the entire <see cref="SyntaxTriviaList"/>.
        /// </summary>
        /// <param name="triviaList"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static int LastIndexOf(this SyntaxTriviaList triviaList, SyntaxKind kind)
        {
            for (int i = triviaList.Count - 1; i >= 0; i--)
            {
                if (triviaList[i].IsKind(kind))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns true if a trivia of the specified kind is in the <see cref="SyntaxTriviaList"/>.
        /// </summary>
        /// <param name="triviaList"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static bool Contains(this SyntaxTriviaList triviaList, SyntaxKind kind)
        {
            return triviaList.IndexOf(kind) != -1;
        }

        /// <summary>
        /// Searches for a trivia of the specified kind and returns the first occurrence within the entire <see cref="SyntaxList{TNode}"/>.
        /// </summary>
        /// <param name="triviaList"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static SyntaxTrivia Find(this SyntaxTriviaList triviaList, SyntaxKind kind)
        {
            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (trivia.IsKind(kind))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        /// <summary>
        /// Returns true if the list of either empty or contains only whitespace.
        /// </summary>
        /// <param name="triviaList"></param>
        /// <returns></returns>
        public static bool IsEmptyOrWhitespace(this SyntaxTriviaList triviaList)
        {
            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (!trivia.IsWhitespaceOrEndOfLineTrivia())
                    return false;
            }

            return true;
        }

        internal static SyntaxTriviaList EmptyIfWhitespace(this SyntaxTriviaList triviaList)
        {
            return (triviaList.IsEmptyOrWhitespace()) ? default(SyntaxTriviaList) : triviaList;
        }

        internal static bool IsSingleElasticMarker(this SyntaxTriviaList triviaList)
        {
            return triviaList.Count == 1
                && triviaList[0].IsElasticMarker();
        }
        #endregion SyntaxTriviaList

        #region TypeDeclarationSyntax
        internal static TypeDeclarationSyntax WithMembers(this TypeDeclarationSyntax typeDeclaration, SyntaxList<MemberDeclarationSyntax> newMembers)
        {
            if (typeDeclaration == null)
                throw new ArgumentNullException(nameof(typeDeclaration));

            switch (typeDeclaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)typeDeclaration).WithMembers(newMembers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)typeDeclaration).WithMembers(newMembers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)typeDeclaration).WithMembers(newMembers);
                default:
                    {
                        Debug.Fail(typeDeclaration.Kind().ToString());
                        return typeDeclaration;
                    }
            }
        }
        #endregion TypeDeclarationSyntax

        #region TypeParameterConstraintClauseSyntax
        internal static SyntaxList<TypeParameterConstraintClauseSyntax> GetContainingList(this TypeParameterConstraintClauseSyntax constraintClause)
        {
            SyntaxNode parent = constraintClause.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)parent).ConstraintClauses;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)parent).ConstraintClauses;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)parent).ConstraintClauses;
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)parent).ConstraintClauses;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)parent).ConstraintClauses;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)parent).ConstraintClauses;
            }

            return default(SyntaxList<TypeParameterConstraintClauseSyntax>);
        }
        #endregion TypeParameterConstraintClauseSyntax

        #region TypeParameterListSyntax
        internal static TypeParameterSyntax GetTypeParameterByName(this TypeParameterListSyntax typeParameterList, string name)
        {
            foreach (TypeParameterSyntax typeParameter in typeParameterList.Parameters)
            {
                if (string.Equals(typeParameter.Identifier.ValueText, name, StringComparison.Ordinal))
                    return typeParameter;
            }

            return null;
        }
        #endregion TypeParameterListSyntax

        #region TypeSyntax
        /// <summary>
        /// Returns true if the type is <see cref="void"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsVoid(this TypeSyntax type)
        {
            return type.IsKind(SyntaxKind.PredefinedType)
                && ((PredefinedTypeSyntax)type).Keyword.IsKind(SyntaxKind.VoidKeyword);
        }
        #endregion TypeSyntax

        #region UsingStatementSyntax
        /// <summary>
        /// Returns using statement's declaration or an expression if the declaration is null.
        /// </summary>
        /// <param name="usingStatement"></param>
        /// <returns></returns>
        public static CSharpSyntaxNode DeclarationOrExpression(this UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            return usingStatement.Declaration ?? (CSharpSyntaxNode)usingStatement.Expression;
        }

        internal static StatementSyntax EmbeddedStatement(this UsingStatementSyntax usingStatement, bool allowUsingStatement = true)
        {
            StatementSyntax statement = usingStatement.Statement;

            if (statement == null)
                return null;

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.Block)
                return null;

            if (!allowUsingStatement
                && kind == SyntaxKind.UsingStatement)
            {
                return null;
            }

            return statement;
        }
        #endregion UsingStatementSyntax

        #region WhileStatementSyntax
        internal static StatementSyntax EmbeddedStatement(this WhileStatementSyntax whileStatement)
        {
            StatementSyntax statement = whileStatement.Statement;

            return (statement?.Kind() == SyntaxKind.Block) ? null : statement;
        }
        #endregion WhileStatementSyntax

        #region XmlElementSyntax
        internal static bool IsLocalName(this XmlElementSyntax xmlElement, string localName, StringComparison comparison = StringComparison.Ordinal)
        {
            return xmlElement.StartTag?.Name?.IsLocalName(localName, comparison) == true;
        }

        internal static bool IsLocalName(this XmlElementSyntax xmlElement, string localName1, string localName2, StringComparison comparison = StringComparison.Ordinal)
        {
            return xmlElement.StartTag?.Name?.IsLocalName(localName1, localName2, comparison) == true;
        }
        #endregion XmlElementSyntax

        #region XmlNameSyntax
        internal static bool IsLocalName(this XmlNameSyntax xmlName, string localName, StringComparison comparison = StringComparison.Ordinal)
        {
            string name = xmlName.LocalName.ValueText;

            return string.Equals(name, localName, comparison);
        }

        internal static bool IsLocalName(this XmlNameSyntax xmlName, string localName1, string localName2, StringComparison comparison = StringComparison.Ordinal)
        {
            string name = xmlName.LocalName.ValueText;

            return string.Equals(name, localName1, comparison)
                || string.Equals(name, localName2, comparison);
        }
        #endregion XmlNameSyntax

        #region YieldStatementSyntax
        /// <summary>
        /// Returns true if the specified statement is a yield break statement.
        /// </summary>
        /// <param name="yieldStatement"></param>
        /// <returns></returns>
        public static bool IsYieldBreak(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement.IsKind(SyntaxKind.YieldBreakStatement);
        }

        /// <summary>
        /// Returns true if the specified statement is a yield return statement.
        /// </summary>
        /// <param name="yieldStatement"></param>
        /// <returns></returns>
        public static bool IsYieldReturn(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement.IsKind(SyntaxKind.YieldReturnStatement);
        }
        #endregion YieldStatementSyntax
    }
}
