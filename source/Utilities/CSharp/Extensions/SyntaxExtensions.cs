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
using Roslynator.CSharp.Internal;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class SyntaxExtensions
    {
        public static IEnumerable<DirectiveTriviaSyntax> DescendantDirectives(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxTrivia trivia in node.DescendantTrivia(descendIntoTrivia: true))
            {
                if (trivia.IsDirective && trivia.HasStructure)
                {
                    var directive = trivia.GetStructure() as DirectiveTriviaSyntax;

                    if (directive != null)
                        yield return directive;
                }
            }
        }

        public static IEnumerable<DirectiveTriviaSyntax> DescendantRegionDirectives(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxNode descendant in node.DescendantNodesAndSelf(descendIntoTrivia: true))
            {
                if (descendant.IsKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia))
                    yield return (DirectiveTriviaSyntax)descendant;
            }
        }

        public static bool IsDescendantOf(this SyntaxNode node, SyntaxKind kind)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.Ancestors().Any(f => f.IsKind(kind));
        }

        public static bool IsBooleanLiteralExpression(this SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression);
        }

        public static bool IsNumericLiteralExpression(this SyntaxNode node, int value)
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

        public static BaseParameterListSyntax GetParameterList(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).ParameterList;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).ParameterList;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).ParameterList;
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return ((ParenthesizedLambdaExpressionSyntax)node).ParameterList;
                case SyntaxKind.AnonymousMethodExpression:
                    return ((AnonymousMethodExpressionSyntax)node).ParameterList;
                default:
                    return null;
            }
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
            return node != null
                && Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node.Parent, kind);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            return node != null
                && IsKind(node.Parent, kind1, kind2);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return node != null
                && IsKind(node.Parent, kind1, kind2, kind3);
        }

        public static SyntaxNode FirstAncestor(
            this SyntaxNode node,
            SyntaxKind kind,
            bool ascendOutOfTrivia = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .Ancestors(ascendOutOfTrivia)
                .FirstOrDefault(f => f.IsKind(kind));
        }

        public static TypeDeclarationSyntax GetEnclosingTypeDeclaration(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.StructDeclaration:
                        return (TypeDeclarationSyntax)ancestor;
                }
            }

            return null;
        }

        public static bool IsYieldStatement(this SyntaxNode node)
        {
            return node?.IsKind(
                SyntaxKind.YieldReturnStatement,
                SyntaxKind.YieldBreakStatement) == true;
        }

        public static bool IsMethodInsideMethod(this SyntaxNode node)
        {
            return !node.IsKind(
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression);
        }

        public static bool IsNamespaceInScope(
            this SyntaxNode node,
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

            foreach (NameSyntax name in NamespacesInScope(node))
            {
                ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                if (symbol?.IsNamespace() == true)
                {
                    string namespaceText = namespaceSymbol.ToString();

                    if (string.Equals(namespaceText, symbol.ToString(), StringComparison.Ordinal))
                    {
                        return true;
                    }
                    else if (name.IsParentKind(SyntaxKind.NamespaceDeclaration))
                    {
                        foreach (INamespaceSymbol containingNamespace in symbol.ContainingNamespaces())
                        {
                            if (string.Equals(namespaceText, containingNamespace.ToString(), StringComparison.Ordinal))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static IEnumerable<NameSyntax> NamespacesInScope(SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)ancestor;

                    NameSyntax namespaceName = namespaceDeclaration.Name;

                    if (namespaceName != null)
                        yield return namespaceName;

                    foreach (NameSyntax name in namespaceDeclaration.Usings.Namespaces())
                        yield return name;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    var compilationUnit = (CompilationUnitSyntax)ancestor;

                    foreach (NameSyntax name in compilationUnit.Usings.Namespaces())
                        yield return name;
                }
            }
        }

        public static bool IsStaticClassInScope(
            this SyntaxNode node,
            INamedTypeSymbol staticClassSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (staticClassSymbol == null)
                throw new ArgumentNullException(nameof(staticClassSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            foreach (NameSyntax name in StaticClassesInScope(node))
            {
                ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                if (symbol?.Equals(staticClassSymbol) == true)
                    return true;
            }

            return false;
        }

        private static IEnumerable<NameSyntax> StaticClassesInScope(SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)ancestor;

                    foreach (NameSyntax name in namespaceDeclaration.Usings.StaticClasses())
                        yield return name;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    var compilationUnit = (CompilationUnitSyntax)ancestor;

                    foreach (NameSyntax name in compilationUnit.Usings.StaticClasses())
                        yield return name;
                }
            }
        }

        public static bool IsSingleLine(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trim = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            TextSpan span = TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia, trim),
                GetEndIndex(node, includeExteriorTrivia, trim));

            FileLinePositionSpan positionSpan = node.SyntaxTree.GetLineSpan(span);

            return positionSpan.StartLine() == positionSpan.EndLine();
        }

        public static bool IsMultiLine(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trim = true)
        {
            return !IsSingleLine(node, includeExteriorTrivia, trim);
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

            return node.WithLeadingTrivia(node.GetLeadingTrivia().TrimStart());
        }

        public static TNode TrimTrailingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithTrailingTrivia(node.GetTrailingTrivia().TrimEnd());
        }

        public static TNode TrimTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .TrimLeadingTrivia()
                .TrimTrailingTrivia();
        }

        public static TextSpan TrimmedSpan(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia: true, trim: true),
                GetEndIndex(node, includeExteriorTrivia: true, trim: true));
        }

        public static bool IsNoneKind(this SyntaxToken token)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(token, SyntaxKind.None);
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

        public static SyntaxToken WithTrailingSpace(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(Space);
        }

        public static SyntaxToken TrimLeadingTrivia(this SyntaxToken token)
        {
            return token.WithLeadingTrivia(token.LeadingTrivia.TrimStart());
        }

        public static SyntaxToken TrimTrailingTrivia(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(token.TrailingTrivia.TrimEnd());
        }

        public static SyntaxToken WithTrailingNewLine(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(NewLineTrivia());
        }

        public static bool Contains(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            return tokenList.IndexOf(kind) != -1;
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(token.Parent, kind);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
        {
            return IsKind(token.Parent, kind1, kind2);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return IsKind(token.Parent, kind1, kind2, kind3);
        }

        public static bool IsNoneKind(this SyntaxTrivia trivia)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(trivia, SyntaxKind.None);
        }

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
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(trivia, SyntaxKind.WhitespaceTrivia);
        }

        public static bool IsEndOfLineTrivia(this SyntaxTrivia trivia)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(trivia, SyntaxKind.EndOfLineTrivia);
        }

        public static bool IsWhitespaceOrEndOfLineTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsWhitespaceTrivia() || trivia.IsEndOfLineTrivia();
        }

        public static bool IsSingleLineCommentTrivia(this SyntaxTrivia trivia)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(trivia, SyntaxKind.SingleLineCommentTrivia);
        }

        public static bool IsMultiLineCommentTrivia(this SyntaxTrivia trivia)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(trivia, SyntaxKind.MultiLineCommentTrivia);
        }

        public static bool IsSingleLineDocumentationCommentTrivia(this SyntaxTrivia trivia)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(trivia, SyntaxKind.SingleLineDocumentationCommentTrivia);
        }

        public static bool IsMultiLineDocumentationCommentTrivia(this SyntaxTrivia trivia)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(trivia, SyntaxKind.MultiLineDocumentationCommentTrivia);
        }

        public static bool IsDocumentationCommentTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxKind.MultiLineDocumentationCommentTrivia);
        }

        public static bool IsCommentTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(
                SyntaxKind.SingleLineCommentTrivia,
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxKind.MultiLineCommentTrivia,
                SyntaxKind.MultiLineDocumentationCommentTrivia);
        }

        public static int LastIndexOf(this SyntaxTriviaList triviaList, SyntaxKind kind)
        {
            for (int i = triviaList.Count - 1; i >= 0; i--)
            {
                if (triviaList[i].IsKind(kind))
                    return i;
            }

            return -1;
        }

        public static bool Contains(this SyntaxTriviaList list, SyntaxKind kind)
        {
            return list.IndexOf(kind) != -1;
        }

        public static SyntaxTriviaList TrimStart(this SyntaxTriviaList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsKind(SyntaxKind.WhitespaceTrivia))
                    continue;

                if (list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                    continue;

                return TriviaList(list.Skip(i));
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList TrimEnd(this SyntaxTriviaList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsKind(SyntaxKind.WhitespaceTrivia))
                    continue;

                if (list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                    continue;

                return TriviaList(list.Take(i + 1));
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList Trim(this SyntaxTriviaList list)
        {
            int startIndex = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    startIndex = i;
                }
            }

            int endIndex = list.Count;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    endIndex = i;
                }
            }

            return TriviaList(list.Skip(startIndex).Take(endIndex + 1 - startIndex));
        }

        public static bool IsAutoGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return IsAutoImplemented(accessorDeclaration, SyntaxKind.GetAccessorDeclaration);
        }

        public static bool IsAutoSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return IsAutoImplemented(accessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        }

        private static bool IsAutoImplemented(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return accessorDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && accessorDeclaration.Body == null;
        }

        private static bool IsAutoImplemented(this AccessorDeclarationSyntax accessorDeclaration, SyntaxKind kind)
        {
            return accessorDeclaration.IsKind(kind)
                && IsAutoImplemented(accessorDeclaration);
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
            this AttributeArgumentSyntax attributeArgument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return DetermineParameterHelper.DetermineParameter(attributeArgument, semanticModel, allowParams, allowCandidate, cancellationToken);
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

        public static BlockSyntax WithoutStatements(this BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.WithStatements(default(SyntaxList<StatementSyntax>));
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

        public static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax memberDeclaration)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(SingletonList(memberDeclaration));
        }

        internal static SyntaxNode AddUsing(this CompilationUnitSyntax compilationUnit, ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsNamedType())
            {
                foreach (ITypeSymbol typeSymbol2 in ((INamedTypeSymbol)typeSymbol).GetAllTypeArgumentsAndSelf())
                    compilationUnit = AddUsingPrivate(compilationUnit, typeSymbol2);

                return compilationUnit;
            }
            else
            {
                return AddUsingPrivate(compilationUnit, typeSymbol);
            }
        }

        private static CompilationUnitSyntax AddUsingPrivate(this CompilationUnitSyntax compilationUnit, ITypeSymbol type)
        {
            INamespaceSymbol containingNamespace = type.ContainingNamespace;

            if (containingNamespace != null)
            {
                return AddUsing(compilationUnit, containingNamespace);
            }
            else
            {
                return compilationUnit;
            }
        }

        internal static CompilationUnitSyntax AddUsing(this CompilationUnitSyntax compilationUnit, INamespaceSymbol namespaceSymbol)
        {
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
                if (en.Current.IsSingleLineCommentTrivia())
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
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return default(SyntaxList<AttributeListSyntax>);
                    }
            }
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
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return node;
                    }
            }
        }

        public static IEnumerable<XmlElementSyntax> Exceptions(this DocumentationCommentTriviaSyntax documentationComment)
        {
            if (documentationComment == null)
                throw new ArgumentNullException(nameof(documentationComment));

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var xmlElement = (XmlElementSyntax)node;

                    XmlNameSyntax name = xmlElement.StartTag?.Name;

                    if (name != null
                        && string.Equals(name.LocalName.ValueText, "exception", StringComparison.Ordinal))
                    {
                        yield return xmlElement;
                    }
                }
            }
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

        public static ExpressionSyntax UnwrapParentheses(this ExpressionSyntax expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            while (expression.IsKind(SyntaxKind.ParenthesizedExpression))
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;

            return expression;
        }

        public static ExpressionSyntax LogicallyNegate(this ExpressionSyntax booleanExpression)
        {
            if (booleanExpression == null)
                throw new ArgumentNullException(nameof(booleanExpression));

            return LogicalNegationHelper.LogicallyNegate(booleanExpression);
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

        public static bool IsVerbatim(this InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            return interpolatedString.StringStartToken.ValueText.Contains("@");
        }

        public static TextSpan GetDollarSpan(this InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            SyntaxToken token = interpolatedString.StringStartToken;

            if (token.Text.StartsWith("$"))
                return new TextSpan(token.SpanStart, 1);

            return new TextSpan(token.SpanStart, 0);
        }

        public static InvocationExpressionSyntax WithArgumentList(
            this InvocationExpressionSyntax invocationExpression,
            params ArgumentSyntax[] arguments)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            return invocationExpression.WithArgumentList(ArgumentList(arguments));
        }

        public static bool IsVerbatimStringLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.Text.StartsWith("@", StringComparison.Ordinal);
        }

        public static bool IsZeroLiteralExpression(this LiteralExpressionSyntax literalExpression)
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
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return default(SyntaxTokenList);
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

        public static bool IsIterator(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                    .DescendantNodes(node => !node.IsMethodInsideMethod())
                    .Any(f => f.IsYieldStatement());
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

            AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

            if (accessorList != null)
            {
                AccessorDeclarationSyntax getter = accessorList.Getter();

                return getter.IsAutoGetter()
                    && accessorList.Setter() == null;
            }

            return false;
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

        public static bool IsQualified(this SimpleNameSyntax identifierName)
        {
            if (identifierName == null)
                throw new ArgumentNullException(nameof(identifierName));

            return identifierName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression);
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

        public static IEnumerable<UsingDirectiveSyntax> RegularUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    && usingDirective.Alias == null)
                {
                    yield return usingDirective;
                }
            }
        }

        public static IEnumerable<NameSyntax> Namespaces(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings.RegularUsings())
            {
                NameSyntax name = usingDirective.Name;

                if (name != null)
                    yield return name;
            }
        }

        public static IEnumerable<UsingDirectiveSyntax> StaticUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    yield return usingDirective;
            }
        }

        public static IEnumerable<NameSyntax> StaticClasses(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings.StaticUsings())
            {
                NameSyntax name = usingDirective.Name;

                if (name != null)
                    yield return name;
            }
        }

        public static IEnumerable<UsingDirectiveSyntax> AliasUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (usingDirective.Alias != null)
                    yield return usingDirective;
            }
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

        public static VariableDeclaratorSyntax FirstVariableOrDefault(this VariableDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            return (variables.Any())
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
