// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxNodeExtensions
    {
        private static readonly SyntaxTokenList _emptySyntaxTokenList = SyntaxFactory.TokenList();

        public static int GetSpanStartLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken).StartLinePosition.Line;

            return -1;
        }

        public static int GetFullSpanStartLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.FullSpan, cancellationToken).StartLinePosition.Line;

            return -1;
        }

        public static int GetSpanEndLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken).EndLinePosition.Line;

            return -1;
        }

        public static int GetFullSpanEndLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.FullSpan, cancellationToken).EndLinePosition.Line;

            return -1;
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

        public static bool HasTrivia(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.HasLeadingTrivia || node.HasTrailingTrivia;
        }

        public static SyntaxTokenList GetDeclarationModifiers(this SyntaxNode declaration)
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
                    return _emptySyntaxTokenList;
            }
        }

        public static bool IsAnyKind(this SyntaxNode syntaxNode, params SyntaxKind[] syntaxKinds)
        {
            if (syntaxNode == null)
                throw new ArgumentNullException(nameof(syntaxNode));

            if (syntaxKinds == null)
                throw new ArgumentNullException(nameof(syntaxKinds));

            for (int i = 0; i < syntaxKinds.Length; i++)
            {
                if (syntaxNode.IsKind(syntaxKinds[i]))
                    return true;
            }

            return false;
        }

        public static bool IsAnyKind(this SyntaxNode syntaxNode, IEnumerable<SyntaxKind> syntaxKinds)
        {
            if (syntaxNode == null)
                throw new ArgumentNullException(nameof(syntaxNode));

            if (syntaxKinds == null)
                throw new ArgumentNullException(nameof(syntaxKinds));

            foreach (SyntaxKind syntaxKind in syntaxKinds)
            {
                if (syntaxNode.IsKind(syntaxKind))
                    return true;
            }

            return false;
        }

        public static SyntaxNode FirstAncestorOrSelf(this SyntaxNode node, params SyntaxKind[] syntaxKinds)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (syntaxKinds == null)
                throw new ArgumentNullException(nameof(syntaxKinds));

            while (node != null)
            {
                if (node.IsAnyKind(syntaxKinds))
                    return node;

                node = node.Parent;
            }

            return null;
        }

        public static SyntaxNode FirstAncestorOrSelf(this SyntaxNode node, IEnumerable<SyntaxKind> syntaxKinds)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (syntaxKinds == null)
                throw new ArgumentNullException(nameof(syntaxKinds));

            while (node != null)
            {
                if (node.IsAnyKind(syntaxKinds))
                    return node;

                node = node.Parent;
            }

            return null;
        }

        public static bool IsSingleline(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trimWhitespace = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            TextSpan span = TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia, trimWhitespace),
                GetEndIndex(node, includeExteriorTrivia, trimWhitespace));

            FileLinePositionSpan positionSpan = node.SyntaxTree.GetLineSpan(span);

            return positionSpan.StartLinePosition.Line == positionSpan.EndLinePosition.Line;
        }

        public static bool IsMultiline(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trimWhitespace = true)
        {
            return !IsSingleline(node, includeExteriorTrivia, trimWhitespace);
        }

        private static int GetStartIndex(SyntaxNode node, bool includeExteriorTrivia, bool trimWhitespace)
        {
            if (!includeExteriorTrivia)
                return node.Span.Start;

            int start = node.FullSpan.Start;

            if (trimWhitespace)
            {
                SyntaxTriviaList leading = node.GetLeadingTrivia();

                for (int i = 0; i < leading.Count; i++)
                {
                    if (!leading[i].IsKind(SyntaxKind.WhitespaceTrivia) && !leading[i].IsKind(SyntaxKind.EndOfLineTrivia))
                        break;

                    start = leading[i].Span.End;
                }
            }

            return start;
        }

        private static int GetEndIndex(SyntaxNode node, bool includeExteriorTrivia, bool trimWhitespace)
        {
            if (!includeExteriorTrivia)
                return node.Span.End;

            int end = node.FullSpan.End;

            if (trimWhitespace)
            {
                SyntaxTriviaList trailing = node.GetTrailingTrivia();

                for (int i = trailing.Count - 1; i >= 0; i--)
                {
                    if (!trailing[i].IsKind(SyntaxKind.WhitespaceTrivia) && !trailing[i].IsKind(SyntaxKind.EndOfLineTrivia))
                        break;

                    end = trailing[i].SpanStart;
                }
            }

            return end;
        }

        public static bool HasExplicitInterfaceSpecifier(this SyntaxNode syntaxNode)
        {
            return GetExplicitInterfaceSpecifier(syntaxNode) != null;
        }

        public static ExplicitInterfaceSpecifierSyntax GetExplicitInterfaceSpecifier(this SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
                throw new ArgumentNullException(nameof(syntaxNode));

            switch (syntaxNode.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)syntaxNode).ExplicitInterfaceSpecifier;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)syntaxNode).ExplicitInterfaceSpecifier;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)syntaxNode).ExplicitInterfaceSpecifier;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)syntaxNode).ExplicitInterfaceSpecifier;
                default:
                    return null;
            }
        }

        public static SyntaxTriviaList GetIndentTrivia(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList triviaList = GetNodeForLeadingTrivia(node).GetLeadingTrivia();

            return SyntaxFactory.TriviaList(
                triviaList
                    .Reverse()
                    .TakeWhile(f => !f.IsEndOfLine()));
        }

        private static SyntaxNode GetNodeForLeadingTrivia(this SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.AncestorsAndSelf())
            {
                if (ancestor.IsKind(SyntaxKind.IfStatement))
                {
                    return ((IfStatementSyntax)ancestor).ParentElse() ?? ancestor;
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

        public static TNode FirstAncestor<TNode>(
            this SyntaxNode node,
            Func<TNode, bool> predicate = null,
            bool ascendOutOfTrivia = true) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.Parent?.FirstAncestorOrSelf<TNode>(predicate, ascendOutOfTrivia);
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

        public static TNode TrimLeadingWhitespace<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithLeadingTrivia(node.GetLeadingTrivia().TrimLeadingWhitespace());
        }

        public static TNode TrimTrailingWhitespace<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithTrailingTrivia(node.GetTrailingTrivia().TrimTrailingWhitespace());
        }

        public static TNode TrimWhitespace<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .TrimLeadingWhitespace()
                .TrimTrailingWhitespace();
        }

        public static bool IsMemberDeclaration(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.IncompleteMember:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsStatement(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    return true;
                case SyntaxKind.ExpressionStatement:
                    return true;
                case SyntaxKind.EmptyStatement:
                    return true;
                case SyntaxKind.LabeledStatement:
                    return true;
                case SyntaxKind.GotoStatement:
                    return true;
                case SyntaxKind.GotoCaseStatement:
                    return true;
                case SyntaxKind.GotoDefaultStatement:
                    return true;
                case SyntaxKind.BreakStatement:
                    return true;
                case SyntaxKind.ContinueStatement:
                    return true;
                case SyntaxKind.ReturnStatement:
                    return true;
                case SyntaxKind.YieldReturnStatement:
                    return true;
                case SyntaxKind.YieldBreakStatement:
                    return true;
                case SyntaxKind.ThrowStatement:
                    return true;
                case SyntaxKind.WhileStatement:
                    return true;
                case SyntaxKind.DoStatement:
                    return true;
                case SyntaxKind.ForStatement:
                    return true;
                case SyntaxKind.ForEachStatement:
                    return true;
                case SyntaxKind.UsingStatement:
                    return true;
                case SyntaxKind.FixedStatement:
                    return true;
                case SyntaxKind.CheckedStatement:
                    return true;
                case SyntaxKind.UncheckedStatement:
                    return true;
                case SyntaxKind.UnsafeStatement:
                    return true;
                case SyntaxKind.LockStatement:
                    return true;
                case SyntaxKind.IfStatement:
                    return true;
                case SyntaxKind.SwitchStatement:
                    return true;
                case SyntaxKind.TryStatement:
                    return true;
                case SyntaxKind.GlobalStatement:
                    return true;
                default:
                    return false;
            }
        }

        public static bool SupportsExpressionBody(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return true;
                case SyntaxKind.PropertyDeclaration:
                    return true;
                case SyntaxKind.IndexerDeclaration:
                    return true;
                case SyntaxKind.OperatorDeclaration:
                    return true;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return true;
                default:
                    return false;
            }
        }
    }
}
