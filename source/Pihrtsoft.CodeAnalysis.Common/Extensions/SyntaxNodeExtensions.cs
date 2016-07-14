// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxNodeExtensions
    {
        public static TSyntax WithTriviaFrom<TSyntax>(this TSyntax syntax, SyntaxToken token) where TSyntax : SyntaxNode
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            return syntax
                .WithLeadingTrivia(token.LeadingTrivia)
                .WithTrailingTrivia(token.TrailingTrivia);
        }

        public static TextSpan TrimmedSpan(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia: true, trimWhitespace: true),
                GetEndIndex(node, includeExteriorTrivia: true, trimWhitespace: true));
        }

        public static int GetSpanStartLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken).StartLine();

            return -1;
        }

        public static int GetFullSpanStartLine(
            this SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.FullSpan, cancellationToken).StartLine();

            return -1;
        }

        public static int GetSpanEndLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken).EndLine();

            return -1;
        }

        public static int GetFullSpanEndLine(
            this SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
                return node.SyntaxTree.GetLineSpan(node.FullSpan, cancellationToken).EndLine();

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
                    return SyntaxFactory.TokenList();
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

        public static bool IsAnyKind(this SyntaxNode node, SyntaxKind kind, SyntaxKind kind2)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.IsKind(kind)
                || node.IsKind(kind2);
        }

        public static bool IsAnyKind(this SyntaxNode node, SyntaxKind kind, SyntaxKind kind2, SyntaxKind kind3)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.IsKind(kind)
                || node.IsKind(kind2)
                || node.IsKind(kind3);
        }

        public static bool IsAnyKind(this SyntaxNode node, SyntaxKind kind, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.IsKind(kind)
                || node.IsKind(kind2)
                || node.IsKind(kind3)
                || node.IsKind(kind4);
        }

        public static bool IsAnyKind(this SyntaxNode node, SyntaxKind kind, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.IsKind(kind)
                || node.IsKind(kind2)
                || node.IsKind(kind3)
                || node.IsKind(kind4)
                || node.IsKind(kind5);
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

        public static bool IsSingleLine(
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

            return positionSpan.StartLine() == positionSpan.EndLine();
        }

        public static bool IsMultiLine(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trimWhitespace = true)
        {
            return !IsSingleLine(node, includeExteriorTrivia, trimWhitespace);
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
                    if (!leading[i].IsWhitespaceOrEndOfLineTrivia())
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
                    if (!trailing[i].IsWhitespaceOrEndOfLineTrivia())
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
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.EmptyStatement:
                case SyntaxKind.LabeledStatement:
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                case SyntaxKind.BreakStatement:
                case SyntaxKind.ContinueStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.ThrowStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.IfStatement:
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.TryStatement:
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
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                    return true;
                default:
                    return false;
            }
        }

        public static TNode WithFormatterAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(Formatter.Annotation);
        }

        public static TNode WithSimplifierAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(Simplifier.Annotation);
        }

        public static bool IsCompilationUnit(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.CompilationUnit);
        }

        public static bool IsNamespaceDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.NamespaceDeclaration);
        }

        public static bool IsClassDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ClassDeclaration);
        }

        public static bool IsStructDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.StructDeclaration);
        }

        public static bool IsInterfaceDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.InterfaceDeclaration);
        }

        public static bool IsFieldDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.FieldDeclaration);
        }

        public static bool IsPropertyDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.PropertyDeclaration);
        }

        public static bool IsMethodDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.MethodDeclaration);
        }

        public static bool IsGetAccessorDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.GetAccessorDeclaration);
        }

        public static bool IsSetAccessorDeclaration(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.SetAccessorDeclaration);
        }

        public static bool IsBlock(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.Block);
        }

        public static bool IsUsingStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.UsingStatement);
        }

        public static bool IsIfStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.IfStatement);
        }

        public static bool IsElseClause(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ElseClause);
        }

        public static bool IsReturnStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ReturnStatement);
        }

        public static bool IsLocalDeclarationStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.LocalDeclarationStatement);
        }

        public static bool IsExpressionStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ExpressionStatement);
        }

        public static bool IsSwitchStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.SwitchStatement);
        }

        public static bool IsEmptyStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.EmptyStatement);
        }

        public static bool IsLabeledStatement(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.LabeledStatement);
        }

        public static bool IsTrueLiteralExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.TrueLiteralExpression);
        }

        public static bool IsFalseLiteralExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.FalseLiteralExpression);
        }

        public static bool IsStringLiteralExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.StringLiteralExpression);
        }

        public static bool IsNumericLiteralExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.NumericLiteralExpression);
        }

        public static bool IsParenthesizedExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ParenthesizedExpression);
        }

        public static bool IsSimpleMemberAccessExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.SimpleMemberAccessExpression);
        }

        public static bool IsInvocationExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.InvocationExpression);
        }

        public static bool IsEqualsExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.EqualsExpression);
        }

        public static bool IsLogicalNotExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.LogicalNotExpression);
        }

        public static bool IsSimpleAssignmentExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.SimpleAssignmentExpression);
        }

        public static bool IsObjectCreationExpression(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ObjectCreationExpression);
        }

        public static bool IsArgumentList(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ArgumentList);
        }

        public static bool IsParameterList(this SyntaxNode node)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, SyntaxKind.ParameterList);
        }
    }
}
