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
using Roslynator.CSharp.Helpers;
using Roslynator.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class SyntaxExtensions
    {
        #region AccessorDeclarationSyntax
        public static bool IsAutoGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return IsAutoAccessor(accessorDeclaration, SyntaxKind.GetAccessorDeclaration);
        }

        public static bool IsAutoSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return IsAutoAccessor(accessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        }

        private static bool IsAutoAccessor(this AccessorDeclarationSyntax accessorDeclaration, SyntaxKind kind)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.IsKind(kind)
                && IsAutoAccessor(accessorDeclaration);
        }

        internal static bool IsAutoAccessor(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return accessorDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && accessorDeclaration.BodyOrExpressionBody() == null;
        }

        internal static AccessorDeclarationSyntax WithoutSemicolonToken(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return accessorDeclaration.WithSemicolonToken(default(SyntaxToken));
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            BlockSyntax body = accessorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)accessorDeclaration.ExpressionBody;
        }
        #endregion AccessorDeclarationSyntax

        #region AccessorListSyntax
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
        #endregion AccessorListSyntax

        #region BlockSyntax
        internal static StatementSyntax SingleNonBlockStatementOrDefault(this BlockSyntax body, bool recursive = true)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            if (recursive)
            {
                StatementSyntax statement = null;

                do
                {
                    SyntaxList<StatementSyntax> statements = body.Statements;

                    statement = (statements.Count == 1) ? statements[0] : null;

                    body = statement as BlockSyntax;
                }
                while (body != null);

                return statement;
            }
            else
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Count == 1)
                {
                    StatementSyntax statement = statements[0];

                    if (statement.Kind() != SyntaxKind.Block)
                        return statement;
                }

                return null;
            }
        }

        public static TextSpan BracesSpan(this BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return TextSpan.FromBounds(block.OpenBraceToken.SpanStart, block.CloseBraceToken.Span.End);
        }

        internal static BlockSyntax InsertStatement(this BlockSyntax block, StatementSyntax statement)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            int insertIndex = statements.Count;

            if (!statement.IsKind(SyntaxKind.LocalFunctionStatement))
            {
                for (int i = statements.Count - 1; i >= 0; i--)
                {
                    if (statements[i].IsKind(SyntaxKind.LocalFunctionStatement))
                        insertIndex--;
                }
            }

            return block.WithStatements(statements.Insert(insertIndex, statement));
        }

        internal static bool ContainsYield(this BlockSyntax block)
        {
            return block?
                .DescendantNodes(block.Span, node => !node.IsNestedMethod())
                .Any(f => f.IsKind(SyntaxKind.YieldReturnStatement, SyntaxKind.YieldBreakStatement)) == true;
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
        public static TextSpan ParenthesesSpan(this CastExpressionSyntax castExpression)
        {
            if (castExpression == null)
                throw new ArgumentNullException(nameof(castExpression));

            return TextSpan.FromBounds(
                castExpression.OpenParenToken.Span.Start,
                castExpression.CloseParenToken.Span.End);
        }
        #endregion CastExpressionSyntax

        #region ClassDeclarationSyntax
        internal static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax member)
        {
            return classDeclaration.WithMembers(SingletonList(member));
        }

        internal static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return classDeclaration.WithMembers(List(memberDeclarations));
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

        public static MemberDeclarationSyntax RemoveMemberAt(this ClassDeclarationSyntax classDeclaration, int index)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return RemoveMember(classDeclaration, classDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(classDeclaration, member, classDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            classDeclaration = classDeclaration
                .WithMembers(classDeclaration.Members.Replace(member, newMember));

            return classDeclaration
                .RemoveNode(classDeclaration.Members[index], RemoveHelper.GetRemoveOptions(newMember));
        }

        public static ClassDeclarationSyntax InsertMember(this ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member, IMemberDeclarationComparer comparer)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(classDeclaration.Members.InsertMember(member, comparer));
        }
        #endregion ClassDeclarationSyntax

        #region CommonForEachStatementSyntax
        public static TextSpan ParenthesesSpan(this CommonForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            return TextSpan.FromBounds(forEachStatement.OpenParenToken.Span.Start, forEachStatement.CloseParenToken.Span.End);
        }
        #endregion CommonForEachStatementSyntax

        #region CompilationUnitSyntax
        internal static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax member)
        {
            return compilationUnit.WithMembers(SingletonList(member));
        }

        internal static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return compilationUnit.WithMembers(List(memberDeclarations));
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

        public static CompilationUnitSyntax RemoveMemberAt(this CompilationUnitSyntax compilationUnit, int index)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return RemoveMember(compilationUnit, compilationUnit.Members[index], index);
        }

        public static CompilationUnitSyntax RemoveMember(this CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(compilationUnit, member, compilationUnit.Members.IndexOf(member));
        }

        private static CompilationUnitSyntax RemoveMember(
            CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            compilationUnit = compilationUnit
                .WithMembers(compilationUnit.Members.Replace(member, newMember));

            return compilationUnit
                .RemoveNode(compilationUnit.Members[index], RemoveHelper.GetRemoveOptions(newMember));
        }

        public static CompilationUnitSyntax InsertMember(this CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member, IMemberDeclarationComparer comparer)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(compilationUnit.Members.InsertMember(member, comparer));
        }
        #endregion CompilationUnitSyntax

        #region ConstructorDeclarationSyntax
        public static TextSpan HeaderSpan(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.Span.Start,
                constructorDeclaration.ParameterList?.Span.End ?? constructorDeclaration.Identifier.Span.End);
        }

        internal static TextSpan HeaderSpanIncludingInitializer(this ConstructorDeclarationSyntax constructorDeclaration)
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

        public static CSharpSyntaxNode BodyOrExpressionBody(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            BlockSyntax body = constructorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)constructorDeclaration.ExpressionBody;
        }
        #endregion ConstructorDeclarationSyntax

        #region ConversionOperatorDeclarationSyntax
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

            return body ?? (CSharpSyntaxNode)conversionOperatorDeclaration.ExpressionBody;
        }
        #endregion ConversionOperatorDeclarationSyntax

        #region DelegateDeclarationSyntax
        public static bool ReturnsVoid(this DelegateDeclarationSyntax delegateDeclaration)
        {
            return delegateDeclaration?.ReturnType?.IsVoid() == true;
        }
        #endregion DelegateDeclarationSyntax

        #region DestructorDeclarationSyntax
        public static CSharpSyntaxNode BodyOrExpressionBody(this DestructorDeclarationSyntax destructorDeclaration)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            BlockSyntax body = destructorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)destructorDeclaration.ExpressionBody;
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

                    if (element.IsLocalName("summary", "SUMMARY"))
                        return element;
                }
            }

            return null;
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

        #region ElseClauseSyntax
        internal static StatementSyntax GetSingleStatementOrDefault(this ElseClauseSyntax elseClause)
        {
            return GetSingleStatementOrDefault(elseClause.Statement);
        }

        public static IfStatementSyntax GetTopmostIf(this ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            if (elseClause.Parent is IfStatementSyntax ifStatement)
                return ifStatement.GetTopmostIf();

            return null;
        }

        public static bool ContinuesWithIf(this ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            return elseClause.Statement?.IsKind(SyntaxKind.IfStatement) == true;
        }
        #endregion ElseClauseSyntax

        #region EndRegionDirectiveTriviaSyntax
        public static RegionDirectiveTriviaSyntax GetRegionDirective(this EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (endRegionDirective == null)
                throw new ArgumentNullException(nameof(endRegionDirective));

            List<DirectiveTriviaSyntax> relatedDirectives = endRegionDirective.GetRelatedDirectives();

            if (relatedDirectives.Count == 2)
            {
                foreach (DirectiveTriviaSyntax directive in relatedDirectives)
                {
                    if (directive.IsKind(SyntaxKind.RegionDirectiveTrivia))
                        return (RegionDirectiveTriviaSyntax)directive;
                }
            }

            return null;
        }

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

        public static bool HasPreprocessingMessageTrivia(this EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            return GetPreprocessingMessageTrivia(endRegionDirective).IsKind(SyntaxKind.PreprocessingMessageTrivia);
        }
        #endregion EndRegionDirectiveTriviaSyntax

        #region EnumDeclarationSyntax
        public static TextSpan BracesSpan(this EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return TextSpan.FromBounds(
                enumDeclaration.OpenBraceToken.Span.Start,
                enumDeclaration.CloseBraceToken.Span.End);
        }
        #endregion EnumDeclarationSyntax

        #region EventDeclarationSyntax
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
        #endregion EventDeclarationSyntax

        #region EventFieldDeclarationSyntax
        public static bool IsStatic(this EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return eventFieldDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }
        #endregion EventFieldDeclarationSyntax

        #region ExpressionSyntax
        public static ExpressionSyntax WalkUpParentheses(this ExpressionSyntax expression)
        {
            while (expression?.Parent?.Kind() == SyntaxKind.ParenthesizedExpression)
                expression = (ParenthesizedExpressionSyntax)expression.Parent;

            return expression;
        }

        public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
        {
            while (expression?.Kind() == SyntaxKind.ParenthesizedExpression)
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;

            return expression;
        }

        internal static ExpressionSyntax WalkDownParenthesesIf(this ExpressionSyntax expression, bool condition)
        {
            return (condition) ? WalkDownParentheses(expression) : expression;
        }
        #endregion ExpressionSyntax

        #region FieldDeclarationSyntax
        public static bool IsConst(this FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration?.Modifiers.Contains(SyntaxKind.ConstKeyword) == true;
        }

        public static bool IsStatic(this FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }
        #endregion FieldDeclarationSyntax

        #region ForStatementSyntax
        public static TextSpan ParenthesesSpan(this ForStatementSyntax forStatement)
        {
            if (forStatement == null)
                throw new ArgumentNullException(nameof(forStatement));

            return TextSpan.FromBounds(forStatement.OpenParenToken.Span.Start, forStatement.CloseParenToken.Span.End);
        }
        #endregion ForStatementSyntax

        #region IfStatementSyntax
        internal static StatementSyntax GetSingleStatementOrDefault(this IfStatementSyntax ifStatement)
        {
            return GetSingleStatementOrDefault(ifStatement.Statement);
        }

        public static bool IsSimpleIf(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return !ifStatement.IsParentKind(SyntaxKind.ElseClause)
                && ifStatement.Else == null;
        }

        public static bool IsSimpleIfElse(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return !ifStatement.IsParentKind(SyntaxKind.ElseClause)
                && ifStatement.Else?.Statement?.IsKind(SyntaxKind.IfStatement) == false;
        }

        public static IEnumerable<IfStatementOrElseClause> GetChain(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            yield return ifStatement;

            while (true)
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    StatementSyntax statement = elseClause.Statement;

                    if (statement?.IsKind(SyntaxKind.IfStatement) == true)
                    {
                        ifStatement = (IfStatementSyntax)statement;
                        yield return ifStatement;
                    }
                    else
                    {
                        yield return elseClause;
                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

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

        public static bool IsTopmostIf(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return !ifStatement.IsParentKind(SyntaxKind.ElseClause);
        }

        internal static IfStatementSyntax GetNextIf(this IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Else?.Statement;

            if (statement?.IsKind(SyntaxKind.IfStatement) == true)
                return (IfStatementSyntax)statement;

            return null;
        }

        internal static IfStatementSyntax GetPreviousIf(this IfStatementSyntax ifStatement)
        {
            SyntaxNode parent = ifStatement.Parent;

            if (parent?.IsKind(SyntaxKind.ElseClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.IfStatement) == true)
                    return (IfStatementSyntax)parent;
            }

            return null;
        }
        #endregion IfStatementSyntax

        #region IEnumerable<T>
        public static SyntaxList<TNode> ToSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : SyntaxNode
        {
            return List(nodes);
        }

        public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : SyntaxNode
        {
            return SeparatedList(nodes);
        }

        public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<SyntaxNodeOrToken> nodesAndTokens) where TNode : SyntaxNode
        {
            return SeparatedList<TNode>(nodesAndTokens);
        }
        #endregion IEnumerable<T>

        #region IndexerDeclarationSyntax
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

        internal static IndexerDeclarationSyntax WithoutSemicolonToken(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration.WithSemicolonToken(default(SyntaxToken));
        }
        #endregion IndexerDeclarationSyntax

        #region InterfaceDeclarationSyntax
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

        public static MemberDeclarationSyntax RemoveMemberAt(this InterfaceDeclarationSyntax interfaceDeclaration, int index)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return RemoveMember(interfaceDeclaration, interfaceDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(interfaceDeclaration, member, interfaceDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            InterfaceDeclarationSyntax interfaceDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            interfaceDeclaration = interfaceDeclaration
                .WithMembers(interfaceDeclaration.Members.Replace(member, newMember));

            return interfaceDeclaration
                .RemoveNode(interfaceDeclaration.Members[index], RemoveHelper.GetRemoveOptions(newMember));
        }

        internal static InterfaceDeclarationSyntax WithMembers(
            this InterfaceDeclarationSyntax interfaceDeclaration,
            MemberDeclarationSyntax member)
        {
            return interfaceDeclaration.WithMembers(SingletonList(member));
        }

        internal static InterfaceDeclarationSyntax WithMembers(
            this InterfaceDeclarationSyntax interfaceDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return interfaceDeclaration.WithMembers(List(memberDeclarations));
        }

        public static InterfaceDeclarationSyntax InsertMember(this InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member, IMemberDeclarationComparer comparer)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithMembers(interfaceDeclaration.Members.InsertMember(member, comparer));
        }
        #endregion InterfaceDeclarationSyntax

        #region InterpolatedStringExpressionSyntax
        public static bool IsVerbatim(this InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            return interpolatedString.StringStartToken.ValueText.Contains("@");
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
        public static bool IsVerbatimStringLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.Text.StartsWith("@", StringComparison.Ordinal);
        }

        internal static string GetStringLiteralInnerText(this LiteralExpressionSyntax literalExpression)
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
        #endregion LiteralExpressionSyntax

        #region LocalFunctionStatementSyntax
        public static CSharpSyntaxNode BodyOrExpressionBody(this LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                throw new ArgumentNullException(nameof(localFunctionStatement));

            BlockSyntax body = localFunctionStatement.Body;

            return body ?? (CSharpSyntaxNode)localFunctionStatement.ExpressionBody;
        }

        public static bool ReturnsVoid(this LocalFunctionStatementSyntax localFunctionStatement)
        {
            return localFunctionStatement?.ReturnType?.IsVoid() == true;
        }

        public static bool ContainsYield(this LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                throw new ArgumentNullException(nameof(localFunctionStatement));

            return localFunctionStatement.Body?.ContainsYield() == true;
        }
        #endregion LocalFunctionStatementSyntax

        #region MemberDeclarationSyntax
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

        public static SyntaxTrivia GetDocumentationCommentTrivia(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            foreach (SyntaxTrivia trivia in member.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        public static DocumentationCommentTriviaSyntax GetSingleLineDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxTrivia trivia = member.GetSingleLineDocumentationCommentTrivia();

            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true)
                    return comment;
            }

            return null;
        }

        public static DocumentationCommentTriviaSyntax GetDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxTrivia trivia = member.GetDocumentationCommentTrivia();

            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            {
                var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia) == true)
                    return comment;
            }

            return null;
        }

        public static bool HasSingleLineDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member
                .GetLeadingTrivia()
                .Any(f => f.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        }

        public static bool HasDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member
                .GetLeadingTrivia()
                .Any(f => f.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia));
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this MemberDeclarationSyntax containingMember, int index)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            switch (containingMember.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)containingMember).RemoveMemberAt(index);
            }

            return containingMember;
        }

        public static MemberDeclarationSyntax RemoveMember(this MemberDeclarationSyntax containingMember, MemberDeclarationSyntax member)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (containingMember.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)containingMember).RemoveMember(member);
            }

            return containingMember;
        }

        public static SyntaxTokenList GetModifiers(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)member).Modifiers;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)member).Modifiers;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)member).Modifiers;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)member).Modifiers;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)member).Modifiers;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)member).Modifiers;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)member).Modifiers;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)member).Modifiers;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)member).Modifiers;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)member).Modifiers;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)member).Modifiers;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)member).Modifiers;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)member).Modifiers;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)member).Modifiers;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)member).Modifiers;
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)member).Modifiers;
                default:
                    {
                        Debug.Fail(member.Kind().ToString());
                        return default(SyntaxTokenList);
                    }
            }
        }

        public static MemberDeclarationSyntax GetMemberAt(this MemberDeclarationSyntax member, int index)
        {
            SyntaxList<MemberDeclarationSyntax> members = GetMembers(member);

            return members[index];
        }

        public static SyntaxList<MemberDeclarationSyntax> GetMembers(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)member).Members;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)member).Members;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)member).Members;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)member).Members;
                default:
                    {
                        Debug.Fail(member.Kind().ToString());
                        return default(SyntaxList<MemberDeclarationSyntax>);
                    }
            }
        }

        internal static MemberDeclarationSyntax WithMembers(this MemberDeclarationSyntax member, SyntaxList<MemberDeclarationSyntax> newMembers)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)member).WithMembers(newMembers);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)member).WithMembers(newMembers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)member).WithMembers(newMembers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)member).WithMembers(newMembers);
                default:
                    {
                        Debug.Fail(member.Kind().ToString());
                        return member;
                    }
            }
        }

        internal static Accessibility GetDefaultExplicitAccessibility(this MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        if (((ConstructorDeclarationSyntax)member).IsStatic())
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
                        var methodDeclaration = (MethodDeclarationSyntax)member;

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
                        var propertyDeclaration = (PropertyDeclarationSyntax)member;

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
                        var indexerDeclaration = (IndexerDeclarationSyntax)member;

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
                        var eventDeclaration = (EventDeclarationSyntax)member;

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
                        if (member.IsParentKind(SyntaxKind.InterfaceDeclaration))
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
                        if (member.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
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

        public static Accessibility GetDeclaredAccessibility(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)member;

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
                        var methodDeclaration = (MethodDeclarationSyntax)member;

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
                        var propertyDeclaration = (PropertyDeclarationSyntax)member;

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
                        var indexerDeclaration = (IndexerDeclarationSyntax)member;

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
                        var eventDeclaration = (EventDeclarationSyntax)member;

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
                        if (member.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            var eventFieldDeclaration = (EventFieldDeclarationSyntax)member;

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
                        return AccessibilityOrDefault(member, member.GetModifiers());
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

        private static Accessibility AccessibilityOrDefault(MemberDeclarationSyntax member, SyntaxTokenList modifiers)
        {
            Accessibility accessibility = modifiers.GetAccessibility();

            if (accessibility != Accessibility.NotApplicable)
            {
                return accessibility;
            }
            else
            {
                return GetDefaultExplicitAccessibility(member);
            }
        }

        public static bool IsPubliclyVisible(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            do
            {
                if (member.IsKind(SyntaxKind.NamespaceDeclaration))
                    return true;

                Accessibility accessibility = member.GetDeclaredAccessibility();

                if (accessibility == Accessibility.Public
                    || accessibility == Accessibility.Protected
                    || accessibility == Accessibility.ProtectedOrInternal)
                {
                    SyntaxNode parent = member.Parent;

                    if (parent != null)
                    {
                        if (parent.IsKind(SyntaxKind.CompilationUnit))
                            return true;

                        member = parent as MemberDeclarationSyntax;
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

            } while (member != null);

            return false;
        }

        internal static TMember WithNewSingleLineDocumentationComment<TMember>(
            this TMember member,
            DocumentationCommentGeneratorSettings settings = null) where TMember : MemberDeclarationSyntax
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            DocumentationCommentInserter inserter = DocumentationCommentInserter.Create(member);

            settings = settings ?? DocumentationCommentGeneratorSettings.Default;

            settings = settings.WithIndent(inserter.Indent);

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
                BaseDocumentationCommentData info = DocumentationCommentGenerator.GenerateFromBase(member, semanticModel, cancellationToken);

                if (info.Success)
                    return member.WithDocumentationComment(info.Comment, indent: true);
            }

            return WithNewSingleLineDocumentationComment(member, settings);
        }

        public static TMember WithDocumentationComment<TMember>(
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

        public static bool ContainsYield(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.Body?.ContainsYield() == true;
        }

        private static MemberDeclarationSyntax RemoveSingleLineDocumentationComment(MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxTriviaList leadingTrivia = member.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

            int i = 0;
            while (en.MoveNext())
            {
                if (en.Current.IsWhitespaceOrEndOfLineTrivia())
                {
                    i++;
                }
                else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    return member.WithLeadingTrivia(leadingTrivia.Take(leadingTrivia.Count - (i + 1)));
                }
                else
                {
                    return member;
                }
            }

            return member;
        }
        #endregion MemberDeclarationSyntax

        #region MethodDeclarationSyntax
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

        internal static bool ContainsAwait(this MethodDeclarationSyntax methodDeclaration)
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

            return body ?? (CSharpSyntaxNode)methodDeclaration.ExpressionBody;
        }
        #endregion MethodDeclarationSyntax

        #region NamespaceDeclarationSyntax
        public static MemberDeclarationSyntax RemoveMemberAt(this NamespaceDeclarationSyntax namespaceDeclaration, int index)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return RemoveMember(namespaceDeclaration, namespaceDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(namespaceDeclaration, member, namespaceDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            namespaceDeclaration = namespaceDeclaration
                .WithMembers(namespaceDeclaration.Members.Replace(member, newMember));

            return namespaceDeclaration
                .RemoveNode(namespaceDeclaration.Members[index], RemoveHelper.GetRemoveOptions(newMember));
        }

        internal static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member)
        {
            return namespaceDeclaration.WithMembers(SingletonList(member));
        }

        internal static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return namespaceDeclaration.WithMembers(List(memberDeclarations));
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

        public static NamespaceDeclarationSyntax InsertMember(this NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member, IMemberDeclarationComparer comparer)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(namespaceDeclaration.Members.InsertMember(member, comparer));
        }
        #endregion NamespaceDeclarationSyntax

        #region OperatorDeclarationSyntax
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

            return body ?? (CSharpSyntaxNode)operatorDeclaration.ExpressionBody;
        }

        public static bool ReturnsVoid(this OperatorDeclarationSyntax operatorDeclaration)
        {
            return operatorDeclaration?.ReturnType?.IsVoid() == true;
        }
        #endregion OperatorDeclarationSyntax

        #region ParameterSyntax
        public static bool IsThis(this ParameterSyntax parameter)
        {
            return parameter?.Modifiers.Contains(SyntaxKind.ThisKeyword) == true;
        }

        public static bool IsParams(this ParameterSyntax parameter)
        {
            return parameter?.Modifiers.Contains(SyntaxKind.ParamsKeyword) == true;
        }

        internal static SeparatedSyntaxList<ParameterSyntax> GetContainingList(this ParameterSyntax parameter)
        {
            var parameterList = parameter?.Parent as ParameterListSyntax;

            return parameterList?.Parameters ?? default(SeparatedSyntaxList<ParameterSyntax>);
        }
        #endregion ParameterSyntax

        #region PropertyDeclarationSyntax
        internal static PropertyDeclarationSyntax WithAttributeLists(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AttributeListSyntax[] attributeLists)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithAttributeLists(List(attributeLists));
        }

        internal static PropertyDeclarationSyntax WithoutSemicolonToken(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.WithSemicolonToken(default(SyntaxToken));
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
        #endregion PropertyDeclarationSyntax

        #region RegionDirectiveTriviaSyntax
        public static EndRegionDirectiveTriviaSyntax GetEndRegionDirective(this RegionDirectiveTriviaSyntax regionDirective)
        {
            if (regionDirective == null)
                throw new ArgumentNullException(nameof(regionDirective));

            List<DirectiveTriviaSyntax> relatedDirectives = regionDirective.GetRelatedDirectives();

            if (relatedDirectives.Count == 2)
            {
                foreach (DirectiveTriviaSyntax directive in relatedDirectives)
                {
                    if (directive.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
                        return (EndRegionDirectiveTriviaSyntax)directive;
                }
            }

            return null;
        }

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

        public static bool HasPreprocessingMessageTrivia(this RegionDirectiveTriviaSyntax regionDirective)
        {
            return GetPreprocessingMessageTrivia(regionDirective).IsKind(SyntaxKind.PreprocessingMessageTrivia);
        }
        #endregion RegionDirectiveTriviaSyntax

        #region SeparatedSyntaxList<T>
        public static int LastIndexOf<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        public static bool Contains<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        public static bool Contains<TNode>(this SeparatedSyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.IndexOf(node) != -1;
        }

        public static TNode Find<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            int index = list.IndexOf(kind);

            if (index != -1)
                return list[index];

            return default(TNode);
        }

        public static bool IsSingleLine<TNode>(
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

        public static bool IsMultiLine<TNode>(
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
        #endregion SeparatedSyntaxList<T>

        #region StatementSyntax
        private static StatementSyntax GetSingleStatementOrDefault(StatementSyntax statement)
        {
            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                return ((BlockSyntax)statement).Statements.SingleOrDefault(shouldThrow: false);
            }
            else
            {
                return statement;
            }
        }

        public static StatementSyntax PreviousStatementOrDefault(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (statement.TryGetContainingList(out SyntaxList<StatementSyntax> statements))
            {
                int index = statements.IndexOf(statement);

                if (index > 0)
                    return statements[index - 1];
            }

            return null;
        }

        public static StatementSyntax NextStatementOrDefault(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (statement.TryGetContainingList(out SyntaxList<StatementSyntax> statements))
            {
                int index = statements.IndexOf(statement);

                if (index < statements.Count - 1)
                    return statements[index + 1];
            }

            return null;
        }

        public static bool TryGetContainingList(this StatementSyntax statement, out SyntaxList<StatementSyntax> statements)
        {
            SyntaxNode parent = statement?.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        statements = ((BlockSyntax)parent).Statements;
                        return true;
                    }
                case SyntaxKind.SwitchSection:
                    {
                        statements = ((SwitchSectionSyntax)parent).Statements;
                        return true;
                    }
                default:
                    {
                        Debug.Assert(parent == null || statement.IsEmbedded(), parent.Kind().ToString());
                        statements = default(SyntaxList<StatementSyntax>);
                        return false;
                    }
            }
        }

        internal static StatementSyntax SingleNonBlockStatementOrDefault(this StatementSyntax statement, bool recursive = true)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return (statement.Kind() == SyntaxKind.Block)
                ? SingleNonBlockStatementOrDefault((BlockSyntax)statement, recursive)
                : statement;
        }

        public static bool IsEmbedded(
            this StatementSyntax statement,
            bool ifInsideElse = true,
            bool usingInsideUsing = true)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.Block)
                return false;

            SyntaxNode parent = statement.Parent;

            if (parent == null)
                return false;

            SyntaxKind parentKind = parent.Kind();

            return parentKind.CanContainEmbeddedStatement()
                && (ifInsideElse
                    || kind != SyntaxKind.IfStatement
                    || parentKind != SyntaxKind.ElseClause)
                && (usingInsideUsing
                    || kind != SyntaxKind.UsingStatement
                    || parentKind != SyntaxKind.UsingStatement);
        }
        #endregion StatementSyntax

        #region StructDeclarationSyntax
        internal static StructDeclarationSyntax WithMembers(
            this StructDeclarationSyntax structDeclaration,
            MemberDeclarationSyntax member)
        {
            return structDeclaration.WithMembers(SingletonList(member));
        }

        internal static StructDeclarationSyntax WithMembers(
            this StructDeclarationSyntax structDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return structDeclaration.WithMembers(List(memberDeclarations));
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

        public static MemberDeclarationSyntax RemoveMemberAt(this StructDeclarationSyntax structDeclaration, int index)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return RemoveMember(structDeclaration, structDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(structDeclaration, member, structDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            StructDeclarationSyntax structDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            structDeclaration = structDeclaration
                .WithMembers(structDeclaration.Members.Replace(member, newMember));

            return structDeclaration
                .RemoveNode(structDeclaration.Members[index], RemoveHelper.GetRemoveOptions(newMember));
        }

        public static StructDeclarationSyntax InsertMember(this StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member, IMemberDeclarationComparer comparer)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return structDeclaration.WithMembers(structDeclaration.Members.InsertMember(member, comparer));
        }
        #endregion StructDeclarationSyntax

        #region SwitchSectionSyntax
        public static bool ContainsDefaultLabel(this SwitchSectionSyntax switchSection)
        {
            return switchSection?.Labels.Any(f => f.IsKind(SyntaxKind.DefaultSwitchLabel)) == true;
        }
        #endregion SwitchSectionSyntax

        #region SyntaxList<T>
        public static int LastIndexOf<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        public static bool Contains<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        public static TNode Find<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            int index = list.IndexOf(kind);

            if (index != -1)
                return list[index];

            return default(TNode);
        }

        public static SyntaxList<MemberDeclarationSyntax> InsertMember(this SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member, IMemberDeclarationComparer comparer)
        {
            return members.Insert(comparer.GetInsertIndex(members, member), member);
        }

        public static bool IsSingleLine<TNode>(
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

        public static bool IsMultiLine<TNode>(
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

        internal static bool IsLastStatement(
            this SyntaxList<StatementSyntax> statements,
            StatementSyntax statement,
            bool skipLocalFunction = false)
        {
            if (!skipLocalFunction)
                return statements.IsLast(statement);

            for (int i = statements.Count - 1; i >= 0; i--)
            {
                StatementSyntax statement2 = statements[i];

                if (!statement2.IsKind(SyntaxKind.LocalFunctionStatement))
                    return statement2 == statement;
            }

            return false;
        }
        #endregion SyntaxList<T>

        #region SyntaxNode
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
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).Modifiers;
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).Modifiers;
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return default(SyntaxTokenList);
                    }
            }
        }

        internal static TNode WithModifiers<TNode>(this TNode node, SyntaxTokenList modifiers) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)((ClassDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)((ConstructorDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)((OperatorDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)((ConversionOperatorDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)((DelegateDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)((DestructorDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)((EnumDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)((EventDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)((EventFieldDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)((FieldDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)((IndexerDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)((InterfaceDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)((MethodDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)((PropertyDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)((StructDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)((IncompleteMemberSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)((AccessorDeclarationSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)((LocalDeclarationStatementSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)((LocalFunctionStatementSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)((ParameterSyntax)(SyntaxNode)node).WithModifiers(modifiers);
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return node;
                    }
            }
        }

        internal static string GetTitle(this SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return "if statement";
                case SyntaxKind.ElseClause:
                    return "else clause";
                case SyntaxKind.DoStatement:
                    return "do statement";
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                    return "foreach statement";
                case SyntaxKind.ForStatement:
                    return "for statement";
                case SyntaxKind.UsingStatement:
                    return "using statement";
                case SyntaxKind.WhileStatement:
                    return "while statement";
                case SyntaxKind.LockStatement:
                    return "lock statement";
                case SyntaxKind.FixedStatement:
                    return "fixed statement";
                case SyntaxKind.SwitchStatement:
                    return "switch statement";
                case SyntaxKind.BreakStatement:
                    return "break statement";
                case SyntaxKind.ContinueStatement:
                    return "continue statement";
                case SyntaxKind.ReturnStatement:
                    return "return statement";
                case SyntaxKind.YieldReturnStatement:
                    return "yield return statement";
                case SyntaxKind.YieldBreakStatement:
                    return "yield break statement";
                case SyntaxKind.MethodDeclaration:
                    return "method";
                case SyntaxKind.OperatorDeclaration:
                    return "operator method";
                case SyntaxKind.ConversionOperatorDeclaration:
                    return "conversion method";
                case SyntaxKind.ConstructorDeclaration:
                    return "constructor";
                case SyntaxKind.DestructorDeclaration:
                    return "destructor";
                case SyntaxKind.PropertyDeclaration:
                    return "property";
                case SyntaxKind.IndexerDeclaration:
                    return "indexer";
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return "event";
                case SyntaxKind.FieldDeclaration:
                    return (((FieldDeclarationSyntax)node).Modifiers.Contains(SyntaxKind.ConstKeyword)) ? "const" : "field";
                case SyntaxKind.DelegateDeclaration:
                    return "delegate";
                case SyntaxKind.NamespaceDeclaration:
                    return "namespace";
                case SyntaxKind.ClassDeclaration:
                    return "class";
                case SyntaxKind.StructDeclaration:
                    return "struct";
                case SyntaxKind.InterfaceDeclaration:
                    return "interface";
                case SyntaxKind.EnumDeclaration:
                    return "enum";
                case SyntaxKind.IncompleteMember:
                    return "member";
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return "accessor";
                case SyntaxKind.LocalDeclarationStatement:
                    return "local declaration";
                case SyntaxKind.LocalFunctionStatement:
                    return "local function";
                case SyntaxKind.Parameter:
                    return "parameter";
                default:
                    {
                        Debug.Fail(node.Kind().ToString());

                        if (node is StatementSyntax)
                            return "statement";

                        if (node is MemberDeclarationSyntax)
                            return "member";

                        throw new ArgumentException("", nameof(node));
                    }
            }
        }

        internal static bool IsNestedMethod(this SyntaxNode node)
        {
            return node?.Kind().IsNestedMethod() == true;
        }

        internal static IEnumerable<DirectiveTriviaSyntax> DescendantDirectives(this SyntaxNode node)
        {
            foreach (SyntaxTrivia trivia in node.DescendantTrivia(descendIntoTrivia: true))
            {
                if (trivia.IsDirective
                    && trivia.HasStructure
                    && (trivia.GetStructure() is DirectiveTriviaSyntax directive))
                {
                    yield return directive;
                }
            }
        }

        internal static IEnumerable<DirectiveTriviaSyntax> DescendantRegionDirectives(this SyntaxNode node)
        {
            foreach (SyntaxNode descendant in node.DescendantNodes(descendIntoTrivia: true))
            {
                if (descendant.IsKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia))
                    yield return (DirectiveTriviaSyntax)descendant;
            }
        }

        public static bool IsDescendantOf(this SyntaxNode node, SyntaxKind kind, bool ascendOutOfTrivia = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.Ancestors(ascendOutOfTrivia).Any(f => f.IsKind(kind));
        }

        internal static bool IsNumericLiteralExpression(this SyntaxNode node, int value)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                object tokenValue = ((LiteralExpressionSyntax)node).Token.Value;

                return tokenValue is int
                    && (int)tokenValue == value;
            }

            return false;
        }

        internal static bool IsNumericLiteralExpression(this SyntaxNode node, string valueText)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.IsKind(SyntaxKind.NumericLiteralExpression)
                && string.Equals(((LiteralExpressionSyntax)node).Token.ValueText, valueText, StringComparison.Ordinal);
        }

        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

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

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind)
        {
            return node?.Parent?.IsKind(kind) == true;
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            return IsKind(node?.Parent, kind1, kind2);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3, kind4);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3, kind4, kind5);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            return IsKind(node?.Parent, kind1, kind2, kind3, kind4, kind5, kind6);
        }

        public static bool IsSingleLine(
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

        public static bool IsMultiLine(
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
                return node.Span.Start;

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

        public static TNode TrimLeadingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.TrimStart();

            if (leadingTrivia.Count != newLeadingTrivia.Count)
            {
                return node.WithLeadingTrivia(newLeadingTrivia);
            }
            else
            {
                return node;
            }
        }

        public static TNode TrimTrailingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList trailingTrivia = node.GetTrailingTrivia();

            SyntaxTriviaList newTrailingTrivia = trailingTrivia.TrimEnd();

            if (trailingTrivia.Count != newTrailingTrivia.Count)
            {
                return node.WithTrailingTrivia(newTrailingTrivia);
            }
            else
            {
                return node;
            }
        }

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

        public static SyntaxNode FirstAncestor(
            this SyntaxNode node,
            SyntaxKind kind,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestor(node, f => f.IsKind(kind), ascendOutOfTrivia);
        }

        public static SyntaxNode FirstAncestor(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestor(node, f => f.IsKind(kind1, kind2), ascendOutOfTrivia);
        }

        public static SyntaxNode FirstAncestor(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            SyntaxKind kind3,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestor(node, f => f.IsKind(kind1, kind2, kind3), ascendOutOfTrivia);
        }

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

        public static SyntaxNode FirstAncestorOrSelf(
            this SyntaxNode node,
            SyntaxKind kind,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestorOrSelf(node, f => f.IsKind(kind), ascendOutOfTrivia);
        }

        public static SyntaxNode FirstAncestorOrSelf(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestorOrSelf(node, f => f.IsKind(kind1, kind2), ascendOutOfTrivia);
        }

        public static SyntaxNode FirstAncestorOrSelf(
            this SyntaxNode node,
            SyntaxKind kind1,
            SyntaxKind kind2,
            SyntaxKind kind3,
            bool ascendOutOfTrivia = true)
        {
            return FirstAncestorOrSelf(node, f => f.IsKind(kind1, kind2, kind3), ascendOutOfTrivia);
        }

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

        internal static TNode FirstDescendantOrSelf<TNode>(
            this SyntaxNode node,
            Func<SyntaxNode, bool> descendIntoChildren = null,
            bool descendIntoTrivia = false) where TNode : SyntaxNode
        {
            foreach (SyntaxNode descendant in node.DescendantNodesAndSelf(descendIntoChildren: descendIntoChildren, descendIntoTrivia: descendIntoTrivia))
            {
                if (descendant is TNode tnode)
                    return tnode;
            }

            return default(TNode);
        }

        internal static TNode FirstDescendantOrSelf<TNode>(
            this SyntaxNode node,
            TextSpan span,
            Func<SyntaxNode, bool> descendIntoChildren = null,
            bool descendIntoTrivia = false) where TNode : SyntaxNode
        {
            foreach (SyntaxNode descendant in node.DescendantNodesAndSelf(span, descendIntoChildren: descendIntoChildren, descendIntoTrivia: descendIntoTrivia))
            {
                if (descendant is TNode tnode)
                    return tnode;
            }

            return default(TNode);
        }

        internal static TNode RemoveStatement<TNode>(this TNode node, StatementSyntax statement) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return node.RemoveNode(statement, RemoveHelper.GetRemoveOptions(statement));
        }

        internal static TNode RemoveModifier<TNode>(this TNode node, SyntaxKind modifierKind) where TNode : SyntaxNode
        {
            return Modifier.Remove(node, modifierKind);
        }

        internal static TNode RemoveModifier<TNode>(this TNode node, SyntaxToken modifier) where TNode : SyntaxNode
        {
            return Modifier.Remove(node, modifier);
        }

        internal static TNode InsertModifier<TNode>(this TNode node, SyntaxKind modifierKind, IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            return Modifier.Insert(node, modifierKind, comparer);
        }

        internal static TNode InsertModifier<TNode>(this TNode node, SyntaxToken modifier, IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            return Modifier.Insert(node, modifier, comparer);
        }

        public static TNode RemoveComments<TNode>(this TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            return CommentRemover.RemoveComments(node, span);
        }

        public static TNode RemoveComments<TNode>(this TNode node, CommentRemoveOptions removeOptions, TextSpan? span = null) where TNode : SyntaxNode
        {
            return CommentRemover.RemoveComments(node, removeOptions, span);
        }

        public static TNode RemoveTrivia<TNode>(this TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return TriviaRemover.RemoveTrivia(node, span);
        }

        public static TNode RemoveWhitespaceOrEndOfLineTrivia<TNode>(this TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            return WhitespaceOrEndOfLineTriviaRewriter.RemoveWhitespaceOrEndOfLineTrivia(node, span);
        }

        public static TNode ReplaceWhitespaceOrEndOfLineTrivia<TNode>(this TNode node, SyntaxTrivia replacementTrivia, TextSpan? span = null) where TNode : SyntaxNode
        {
            return WhitespaceOrEndOfLineTriviaRewriter.ReplaceWhitespaceOrEndOfLineTrivia(node, replacementTrivia, span);
        }

        internal static bool IsPartOfDocumentationComment(this SyntaxNode node)
        {
            while (node != null)
            {
                if (node.IsStructuredTrivia
                    && node.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
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

        internal static TNode WithAccessibility<TNode>(
            this TNode node,
            Accessibility newAccessibility,
            IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)ChangeAccessibilityHelper.ChangeAccessibility(node, newAccessibility, comparer);
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
        #endregion SyntaxNode

        #region SyntaxToken
        public static bool IsAccessModifier(this SyntaxToken token)
        {
            switch (token.Kind())
            {
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

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

        public static SyntaxToken TrimLeadingTrivia(this SyntaxToken token)
        {
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;
            SyntaxTriviaList newLeadingTrivia = leadingTrivia.TrimStart();

            if (leadingTrivia.Count != newLeadingTrivia.Count)
            {
                return token.WithLeadingTrivia(newLeadingTrivia);
            }
            else
            {
                return token;
            }
        }

        public static SyntaxToken TrimTrailingTrivia(this SyntaxToken token)
        {
            SyntaxTriviaList trailingTrivia = token.TrailingTrivia;
            SyntaxTriviaList newTrailingTrivia = trailingTrivia.TrimEnd();

            if (trailingTrivia.Count != newTrailingTrivia.Count)
            {
                return token.WithTrailingTrivia(newTrailingTrivia);
            }
            else
            {
                return token;
            }
        }

        public static bool Contains(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            return tokenList.IndexOf(kind) != -1;
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2)
        {
            foreach (SyntaxToken token in tokenList)
            {
                SyntaxKind kind = token.Kind();

                if (kind == kind1
                    || kind == kind2)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            foreach (SyntaxToken token in tokenList)
            {
                SyntaxKind kind = token.Kind();

                if (kind == kind1
                    || kind == kind2
                    || kind == kind3)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            foreach (SyntaxToken token in tokenList)
            {
                SyntaxKind kind = token.Kind();

                if (kind == kind1
                    || kind == kind2
                    || kind == kind3
                    || kind == kind4)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ContainsAny(this SyntaxTokenList tokenList, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            foreach (SyntaxToken token in tokenList)
            {
                SyntaxKind kind = token.Kind();

                if (kind == kind1
                    || kind == kind2
                    || kind == kind3
                    || kind == kind4
                    || kind == kind5)
                {
                    return true;
                }
            }

            return false;
        }

        public static SyntaxToken TrimTrivia(this SyntaxToken token)
        {
            return token
                .TrimLeadingTrivia()
                .TrimTrailingTrivia();
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind)
        {
            return token.Parent?.IsKind(kind) == true;
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
        {
            return IsKind(token.Parent, kind1, kind2);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return IsKind(token.Parent, kind1, kind2, kind3);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return IsKind(token.Parent, kind1, kind2, kind3, kind4);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return IsKind(token.Parent, kind1, kind2, kind3, kind4, kind5);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            return IsKind(token.Parent, kind1, kind2, kind3, kind4, kind5, kind6);
        }
        #endregion SyntaxToken

        #region SyntaxTokenList
        public static SyntaxTokenList InsertModifier(this SyntaxTokenList modifiers, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(modifiers, Token(modifierKind), comparer);
        }

        public static SyntaxTokenList InsertModifier(this SyntaxTokenList modifiers, SyntaxToken modifier, IModifierComparer comparer)
        {
            int index = 0;

            if (modifiers.Any())
            {
                index = comparer.GetInsertIndex(modifiers, modifier);

                if (index == 0)
                {
                    SyntaxToken firstModifier = modifiers[index];

                    SyntaxTriviaList trivia = firstModifier.LeadingTrivia;

                    if (trivia.Any())
                    {
                        SyntaxTriviaList leadingTrivia = modifier.LeadingTrivia;

                        if (!leadingTrivia.IsSingleElasticMarker())
                            trivia = trivia.AddRange(leadingTrivia);

                        modifier = modifier.WithLeadingTrivia(trivia);

                        modifiers = modifiers.ReplaceAt(index, firstModifier.WithoutLeadingTrivia());
                    }
                }
            }

            return modifiers.Insert(index, modifier);
        }

        internal static bool ContainsAccessModifier(this SyntaxTokenList tokenList)
        {
            return tokenList.Any(token => token.IsAccessModifier());
        }

        internal static Accessibility GetAccessibility(this SyntaxTokenList tokenList)
        {
            int count = tokenList.Count;

            for (int i = 0; i < count; i++)
            {
                switch (tokenList[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        {
                            return Accessibility.Public;
                        }
                    case SyntaxKind.PrivateKeyword:
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (tokenList[j].Kind() == SyntaxKind.ProtectedKeyword)
                                    return Accessibility.ProtectedAndInternal;
                            }

                            return Accessibility.Private;
                        }
                    case SyntaxKind.InternalKeyword:
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (tokenList[j].Kind() == SyntaxKind.ProtectedKeyword)
                                    return Accessibility.ProtectedOrInternal;
                            }

                            return Accessibility.Internal;
                        }
                    case SyntaxKind.ProtectedKeyword:
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                switch (tokenList[j].Kind())
                                {
                                    case SyntaxKind.InternalKeyword:
                                        return Accessibility.ProtectedOrInternal;
                                    case SyntaxKind.PrivateKeyword:
                                        return Accessibility.ProtectedAndInternal;
                                }
                            }

                            return Accessibility.Protected;
                        }
                }
            }

            return Accessibility.NotApplicable;
        }

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
            int index = tokens.IndexOf(kind);

            if (index == -1)
                return tokens;

            return tokens.ReplaceAt(index, newToken.WithTriviaFrom(tokens[index]));
        }
        #endregion SyntaxTokenList

        #region SyntaxTrivia
        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            SyntaxKind kind = trivia.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

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

        public static bool IsWhitespaceTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.WhitespaceTrivia);
        }

        public static bool IsEndOfLineTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        public static bool IsWhitespaceOrEndOfLineTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.WhitespaceTrivia, SyntaxKind.EndOfLineTrivia);
        }

        internal static bool IsElasticMarker(this SyntaxTrivia trivia)
        {
            return trivia.IsWhitespaceTrivia()
                && trivia.Span.IsEmpty
                && trivia.HasAnnotation(SyntaxAnnotation.ElasticAnnotation);
        }
        #endregion SyntaxTrivia

        #region SyntaxTriviaList
        public static int LastIndexOf(this SyntaxTriviaList triviaList, SyntaxKind kind)
        {
            for (int i = triviaList.Count - 1; i >= 0; i--)
            {
                if (triviaList[i].IsKind(kind))
                    return i;
            }

            return -1;
        }

        public static bool Contains(this SyntaxTriviaList triviaList, SyntaxKind kind)
        {
            return triviaList.IndexOf(kind) != -1;
        }

        public static SyntaxTriviaList TrimStart(this SyntaxTriviaList triviaList)
        {
            for (int i = 0; i < triviaList.Count; i++)
            {
                if (!triviaList[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i > 0)
                    {
                        return TriviaList(triviaList.Skip(i));
                    }
                    else
                    {
                        return triviaList;
                    }
                }
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList TrimEnd(this SyntaxTriviaList triviaList)
        {
            for (int i = triviaList.Count - 1; i >= 0; i--)
            {
                if (!triviaList[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i < triviaList.Count - 1)
                    {
                        return TriviaList(triviaList.Take(i + 1));
                    }
                    else
                    {
                        return triviaList;
                    }
                }
            }

            return SyntaxTriviaList.Empty;
        }

        internal static bool IsEmptyOrWhitespace(this SyntaxTriviaList triviaList)
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
        public static TypeDeclarationSyntax InsertMember(this TypeDeclarationSyntax typeDeclaration, MemberDeclarationSyntax member, IMemberDeclarationComparer comparer)
        {
            if (typeDeclaration == null)
                throw new ArgumentNullException(nameof(typeDeclaration));

            return typeDeclaration.WithMembers(typeDeclaration.Members.InsertMember(member, comparer));
        }

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
        internal static string NameText(this TypeParameterConstraintClauseSyntax constraintClause)
        {
            return constraintClause.Name.Identifier.ValueText;
        }

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
        public static bool IsVoid(this TypeSyntax type)
        {
            return type?.IsKind(SyntaxKind.PredefinedType) == true
                && ((PredefinedTypeSyntax)type).Keyword.IsKind(SyntaxKind.VoidKeyword);
        }
        #endregion TypeSyntax

        #region UsingStatementSyntax
        public static CSharpSyntaxNode DeclarationOrExpression(this UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            CSharpSyntaxNode declaration = usingStatement.Declaration;

            return declaration ?? usingStatement.Expression;
        }
        #endregion UsingStatementSyntax

        #region XmlElementSyntax
        internal static bool IsLocalName(this XmlElementSyntax xmlElement, string localName)
        {
            return xmlElement.StartTag?.Name?.IsLocalName(localName) == true;
        }

        internal static bool IsLocalName(this XmlElementSyntax xmlElement, string localName1, string localName2)
        {
            return xmlElement.StartTag?.Name?.IsLocalName(localName1, localName2) == true;
        }
        #endregion XmlElementSyntax

        #region XmlNameSyntax
        internal static bool IsLocalName(this XmlNameSyntax xmlName, string localName)
        {
            string name = xmlName.LocalName.ValueText;

            return string.Equals(name, localName, StringComparison.Ordinal);
        }

        internal static bool IsLocalName(this XmlNameSyntax xmlName, string localName1, string localName2)
        {
            string name = xmlName.LocalName.ValueText;

            return string.Equals(name, localName1, StringComparison.Ordinal)
                || string.Equals(name, localName2, StringComparison.Ordinal);
        }
        #endregion XmlNameSyntax

        #region YieldStatementSyntax
        public static bool IsYieldReturn(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement?.ReturnOrBreakKeyword.IsKind(SyntaxKind.ReturnKeyword) == true;
        }

        public static bool IsYieldBreak(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement?.ReturnOrBreakKeyword.IsKind(SyntaxKind.BreakKeyword) == true;
        }
        #endregion YieldStatementSyntax
    }
}
