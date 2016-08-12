// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RefactoringContextExtensions
    {
        public static async Task ComputeRefactoringsAsync(this RefactoringContext context)
        {
            Debug.WriteLine($"START {nameof(ComputeRefactoringsForNodeInsideTrivia)}");
            ComputeRefactoringsForNodeInsideTrivia(context);
            Debug.WriteLine($"END {nameof(ComputeRefactoringsForNodeInsideTrivia)}");

            Debug.WriteLine($"START {nameof(ComputeRefactoringsForTokenAsync)}");
            await ComputeRefactoringsForTokenAsync(context).ConfigureAwait(false);
            Debug.WriteLine($"END {nameof(ComputeRefactoringsForTokenAsync)}");

            Debug.WriteLine($"START {nameof(ComputeRefactoringsForTrivia)}");
            ComputeRefactoringsForTrivia(context);
            Debug.WriteLine($"END {nameof(ComputeRefactoringsForTrivia)}");

            Debug.WriteLine($"START {nameof(ComputeRefactoringsForNodeAsync)}");
            await ComputeRefactoringsForNodeAsync(context).ConfigureAwait(false);
            Debug.WriteLine($"END {nameof(ComputeRefactoringsForNodeAsync)}");
        }

        public static async Task ComputeRefactoringsForNodeAsync(this RefactoringContext context)
        {
            SyntaxNode node = context.FindNode();

            if (node == null)
                return;

            bool fAccessor = false;
            bool fArgument = false;
            bool fArgumentList = false;
            bool fAttributeArgumentList = false;
            bool fArrowExpressionClause = false;
            bool fParameter = false;
            bool fParameterList = false;
            bool fSwitchSection = false;
            bool fVariableDeclaration = false;
            bool fInterpolatedStringText = false;
            bool fElseClause = false;
            bool fCaseSwitchLabel = false;

            bool fExpression = false;
            bool fAnonymousMethod = false;
            bool fAssignmentExpression = false;
            bool fBinaryExpression = false;
            bool fConditionalExpression = false;
            bool fQualifiedName = false;
            bool fGenericName = false;
            bool fIdentifierName = false;
            bool fInitializerExpression = false;
            bool fInterpolatedStringExpression = false;
            bool fInvocationExpression = false;
            bool fLambdaExpression = false;
            bool fLiteralExpression = false;
            bool fSimpleMemberAccessExpression = false;
            bool fParenthesizedExpression = false;
            bool fPostfixUnaryExpression = false;
            bool fPrefixUnaryExpression = false;
            bool fAwaitExpression = false;

            bool fMemberDeclaration = false;
#if DEBUG
            bool fSortMembers = false;
#endif
            bool fStatement = false;
            bool fDoStatement = false;
            bool fExpressionStatement = false;
            bool fForEachStatement = false;
            bool fForStatement = false;
            bool fIfStatement = false;
            bool fLocalDeclarationStatement = false;
            bool fReturnStatement = false;
            bool fSwitchStatement = false;
            bool fUsingStatement = false;
            bool fWhileStatement = false;
            bool fYieldReturnStatement = false;
            bool fBlock = false;
            bool fStatementRefactoring = false;

            SyntaxNode firstNode = node;

            using (IEnumerator<SyntaxNode> en = node.AncestorsAndSelf().GetEnumerator())
            {
                while (en.MoveNext())
                {
                    node = en.Current;
                    Debug.WriteLine(node.Kind().ToString());

                    if (!fAccessor)
                    {
                        var accessor = node as AccessorDeclarationSyntax;
                        if (accessor != null)
                        {
                            AccessorDeclarationRefactoring.ComputeRefactorings(context, accessor);
                            fAccessor = true;
                            continue;
                        }
                    }

                    if (!fArgument
                        && node.IsKind(SyntaxKind.Argument))
                    {
                        await ArgumentRefactoring.ComputeRefactoringsAsync(context, (ArgumentSyntax)node).ConfigureAwait(false);
                        fArgument = true;
                        continue;
                    }

                    if (!fArgumentList
                        && node.IsKind(SyntaxKind.ArgumentList))
                    {
                        await ArgumentListRefactoring.ComputeRefactoringsAsync(context, (ArgumentListSyntax)node).ConfigureAwait(false);
                        fArgumentList = true;
                        continue;
                    }

                    if (!fAttributeArgumentList
                        && node.IsKind(SyntaxKind.AttributeArgumentList))
                    {
                        await AttributeArgumentListRefactoring.ComputeRefactoringsAsync(context, (AttributeArgumentListSyntax)node).ConfigureAwait(false);
                        fAttributeArgumentList = true;
                        continue;
                    }

                    if (!fArrowExpressionClause
                        && node.IsKind(SyntaxKind.ArrowExpressionClause))
                    {
                        await ArrowExpressionClauseRefactoring.ComputeRefactoringsAsync(context, (ArrowExpressionClauseSyntax)node).ConfigureAwait(false);
                        fArrowExpressionClause = true;
                        continue;
                    }

                    if (!fParameter
                        && node.IsKind(SyntaxKind.Parameter))
                    {
                        await ParameterRefactoring.ComputeRefactoringsAsync(context, (ParameterSyntax)node).ConfigureAwait(false);
                        fParameter = true;
                        continue;
                    }

                    if (!fParameterList
                        && node.IsKind(SyntaxKind.ParameterList))
                    {
                        ParameterListRefactoring.ComputeRefactorings(context, (ParameterListSyntax)node);
                        fParameterList = true;
                        continue;
                    }

                    if (!fSwitchSection
                        && node.IsKind(SyntaxKind.SwitchSection))
                    {
                        SwitchSectionRefactoring.ComputeRefactorings(context, (SwitchSectionSyntax)node);
                        fSwitchSection = true;
                        continue;
                    }

                    if (!fVariableDeclaration
                        && node.IsKind(SyntaxKind.VariableDeclaration))
                    {
                        await VariableDeclarationRefactoring.ComputeRefactoringsAsync(context, (VariableDeclarationSyntax)node).ConfigureAwait(false);
                        fVariableDeclaration = true;
                        continue;
                    }

                    if (!fInterpolatedStringText
                        && node.IsKind(SyntaxKind.InterpolatedStringText))
                    {
                        InterpolatedStringTextRefactoring.ComputeRefactorings(context, (InterpolatedStringTextSyntax)node);
                        fInterpolatedStringText = true;
                        continue;
                    }

                    if (!fElseClause
                        && node.IsKind(SyntaxKind.ElseClause))
                    {
                        ElseClauseRefactoring.ComputeRefactorings(context, (ElseClauseSyntax)node);
                        fElseClause = true;
                        continue;
                    }

                    if (!fCaseSwitchLabel
                        && node.IsKind(SyntaxKind.CaseSwitchLabel))
                    {
                        await CaseSwitchLabelRefactoring.ComputeRefactoringsAsync(context, (CaseSwitchLabelSyntax)node).ConfigureAwait(false);
                        fCaseSwitchLabel = true;
                        continue;
                    }

                    var expression = node as ExpressionSyntax;
                    if (expression != null)
                    {
                        if (!fAssignmentExpression)
                        {
                            var assignmentExpression = node as AssignmentExpressionSyntax;
                            if (assignmentExpression != null)
                            {
                                await AssignmentExpressionRefactoring.ComputeRefactoringsAsync(context, assignmentExpression).ConfigureAwait(false);
                                fAssignmentExpression = true;
                            }
                        }

                        if (!fAnonymousMethod
                            && node.IsKind(SyntaxKind.AnonymousMethodExpression))
                        {
                            AnonymousMethodExpressionRefactoring.ComputeRefactorings(context, (AnonymousMethodExpressionSyntax)node);
                            fAnonymousMethod = true;
                        }

                        if (!fBinaryExpression)
                        {
                            var binaryExpression = node as BinaryExpressionSyntax;
                            if (binaryExpression != null)
                            {
                                await BinaryExpressionRefactoring.ComputeRefactoringsAsync(context, binaryExpression).ConfigureAwait(false);
                                fBinaryExpression = true;
                            }
                        }

                        if (!fConditionalExpression
                            && node.IsKind(SyntaxKind.ConditionalExpression))
                        {
                            ConditionalExpressionRefactoring.ComputeRefactorings(context, (ConditionalExpressionSyntax)expression);
                            fConditionalExpression = true;
                        }

                        if (!fQualifiedName
                            && node.IsKind(SyntaxKind.QualifiedName))
                        {
                            await QualifiedNameRefactoring.ComputeRefactoringsAsync(context, (QualifiedNameSyntax)expression);
                            fQualifiedName = true;
                        }

                        if (!fGenericName
                            && node.IsKind(SyntaxKind.GenericName))
                        {
                            GenericNameRefactoring.ComputeRefactorings(context, (GenericNameSyntax)expression);
                            fGenericName = true;
                        }

                        if (!fIdentifierName
                            && node.IsKind(SyntaxKind.IdentifierName))
                        {
                            await IdentifierNameRefactoring.ComputeRefactoringsAsync(context, (IdentifierNameSyntax)expression).ConfigureAwait(false);
                            fIdentifierName = true;
                        }

                        if (!fInitializerExpression)
                        {
                            var initializer = node as InitializerExpressionSyntax;
                            if (initializer != null)
                            {
                                await InitializerExpressionRefactoring.ComputeRefactoringsAsync(context, initializer).ConfigureAwait(false);
                                fInitializerExpression = true;
                            }
                        }

                        if (!fInterpolatedStringExpression
                            && node.IsKind(SyntaxKind.InterpolatedStringExpression))
                        {
                            InterpolatedStringRefactoring.ComputeRefactorings(context, (InterpolatedStringExpressionSyntax)expression);
                            fInterpolatedStringExpression = true;
                        }

                        if (!fInvocationExpression
                            && node.IsKind(SyntaxKind.InvocationExpression))
                        {
                            await InvocationExpressionRefactoring.ComputeRefactoringsAsync(context, (InvocationExpressionSyntax)expression).ConfigureAwait(false);
                            fInvocationExpression = true;
                        }

                        if (!fLambdaExpression)
                        {
                            var lambdaExpression = node as LambdaExpressionSyntax;
                            if (lambdaExpression != null)
                            {
                                LambdaExpressionRefactoring.ComputeRefactorings(context, lambdaExpression);
                                fLambdaExpression = true;
                            }
                        }

                        if (!fLiteralExpression)
                        {
                            var literalExpression = node as LiteralExpressionSyntax;
                            if (literalExpression != null)
                            {
                                LiteralExpressionRefactoring.ComputeRefactorings(context, literalExpression);
                                fLiteralExpression = true;
                            }
                        }

                        if (!fSimpleMemberAccessExpression
                            && node.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                        {
                            await SimpleMemberAccessExpressionRefactoring.ComputeRefactoringAsync(context, (MemberAccessExpressionSyntax)node).ConfigureAwait(false);
                            fSimpleMemberAccessExpression = true;
                        }

                        if (!fParenthesizedExpression
                            && node.IsKind(SyntaxKind.ParenthesizedExpression))
                        {
                            ParenthesizedExpressionRefactoring.ComputeRefactorings(context, (ParenthesizedExpressionSyntax)expression);
                            fParenthesizedExpression = true;
                        }

                        if (!fPostfixUnaryExpression)
                        {
                            var postfixUnaryExpression = node as PostfixUnaryExpressionSyntax;
                            if (postfixUnaryExpression != null)
                            {
                                PostfixUnaryExpressionRefactoring.ComputeRefactorings(context, postfixUnaryExpression);
                                fPostfixUnaryExpression = true;
                            }
                        }

                        if (!fPrefixUnaryExpression)
                        {
                            var prefixUnaryExpression = node as PrefixUnaryExpressionSyntax;
                            if (prefixUnaryExpression != null)
                            {
                                PrefixUnaryExpressionRefactoring.ComputeRefactorings(context, prefixUnaryExpression);
                                fPrefixUnaryExpression = true;
                            }
                        }

                        if (!fAwaitExpression
                            && node.IsKind(SyntaxKind.AwaitExpression))
                        {
                            await AwaitExpressionRefactoring.ComputeRefactoringsAsync(context, (AwaitExpressionSyntax)node).ConfigureAwait(false);
                            fAwaitExpression = true;
                        }

                        if (!fExpression)
                        {
                            ExpressionRefactoring.ComputeRefactorings(context, expression);
                            fExpression = true;
                        }

                        continue;
                    }

                    var memberDeclaration = node as MemberDeclarationSyntax;
                    if (memberDeclaration != null)
                    {
#if DEBUG
                        if (!fSortMembers
                            && node.IsKind(
                                SyntaxKind.NamespaceDeclaration,
                                SyntaxKind.ClassDeclaration,
                                SyntaxKind.StructDeclaration,
                                SyntaxKind.InterfaceDeclaration))
                        {
                            SortMemberDeclarationsRefactoring.ComputeRefactorings(context, memberDeclaration);
                            fSortMembers = true;
                        }
#endif
                        if (!fMemberDeclaration)
                        {
                            await MemberDeclarationRefactoring.ComputeRefactoringsAsync(context, memberDeclaration).ConfigureAwait(false);

                            await IntroduceConstructorRefactoring.ComputeRefactoringsAsync(context, memberDeclaration).ConfigureAwait(false);
                            fMemberDeclaration = true;
                        }

                        continue;
                    }

                    var statement = node as StatementSyntax;
                    if (statement != null)
                    {
                        if (!fDoStatement
                            && node.IsKind(SyntaxKind.DoStatement))
                        {
                            await DoStatementRefactoring.ComputeRefactoringsAsync(context, (DoStatementSyntax)statement).ConfigureAwait(false);
                            fDoStatement = true;
                        }

                        if (!fExpressionStatement
                            && node.IsKind(SyntaxKind.ExpressionStatement))
                        {
                            await ExpressionStatementRefactoring.ComputeRefactoringsAsync(context, (ExpressionStatementSyntax)statement).ConfigureAwait(false);
                            fExpressionStatement = true;
                        }

                        if (!fForEachStatement
                            && node.IsKind(SyntaxKind.ForEachStatement))
                        {
                            await ForEachStatementRefactoring.ComputeRefactoringsAsync(context, (ForEachStatementSyntax)statement).ConfigureAwait(false);
                            fForEachStatement = true;
                        }

                        if (!fForStatement
                            && node.IsKind(SyntaxKind.ForStatement))
                        {
                            await ForStatementRefactoring.ComputeRefactoringsAsync(context, (ForStatementSyntax)statement).ConfigureAwait(false);
                            fForStatement = true;
                        }

                        if (!fIfStatement
                            && node.IsKind(SyntaxKind.IfStatement))
                        {
                            await IfStatementRefactoring.ComputeRefactoringsAsync(context, (IfStatementSyntax)statement).ConfigureAwait(false);
                            fIfStatement = true;
                        }

                        if (!fLocalDeclarationStatement
                            && node.IsKind(SyntaxKind.LocalDeclarationStatement))
                        {
                            await LocalDeclarationStatementRefactoring.ComputeRefactoringsAsync(context, (LocalDeclarationStatementSyntax)statement).ConfigureAwait(false);
                            fLocalDeclarationStatement = true;
                        }

                        if (!fReturnStatement
                            && node.IsKind(SyntaxKind.ReturnStatement))
                        {
                            await ReturnStatementRefactoring.ComputeRefactoringsAsync(context, (ReturnStatementSyntax)statement).ConfigureAwait(false);
                            fReturnStatement = true;
                        }

                        if (!fSwitchStatement
                            && node.IsKind(SyntaxKind.SwitchStatement))
                        {
                            await SwitchStatementRefactoring.ComputeRefactoringsAsync(context, (SwitchStatementSyntax)statement).ConfigureAwait(false);
                            fSwitchStatement = true;
                        }

                        if (!fUsingStatement
                            && node.IsKind(SyntaxKind.UsingStatement))
                        {
                            await UsingStatementRefactoring.ComputeRefactoringsAsync(context, (UsingStatementSyntax)statement).ConfigureAwait(false);
                            fUsingStatement = true;
                        }

                        if (!fWhileStatement
                            && node.IsKind(SyntaxKind.WhileStatement))
                        {
                            await WhileStatementRefactoring.ComputeRefactoringsAsync(context, (WhileStatementSyntax)statement).ConfigureAwait(false);
                            fWhileStatement = true;
                        }

                        if (!fYieldReturnStatement)
                        {
                            var yieldStatement = node as YieldStatementSyntax;
                            if (yieldStatement != null)
                            {
                                await YieldReturnStatementRefactoring.ComputeRefactoringsAsync(context, yieldStatement).ConfigureAwait(false);
                                fYieldReturnStatement = true;
                            }
                        }

                        if (!fBlock
                            && node.IsKind(SyntaxKind.Block))
                        {
                            await BlockRefactoring.ComputeRefactoringAsync(context, (BlockSyntax)node);
                            fBlock = true;
                        }

                        if (!fStatement)
                        {
                            AddBracesRefactoring.ComputeRefactoring(context, statement);
                            ExtractStatementRefactoring.ComputeRefactoring(context, statement);
                            fStatement = true;
                        }

                        if (!fStatementRefactoring)
                        {
                            if (node.IsKind(SyntaxKind.Block))
                            {
                                StatementRefactoring.ComputeRefactoring(context, (BlockSyntax)node);
                                fStatementRefactoring = true;
                            }
                            else if (node.IsKind(SyntaxKind.SwitchStatement))
                            {
                                StatementRefactoring.ComputeRefactoring(context, (SwitchStatementSyntax)node);
                                fStatementRefactoring = true;
                            }
                        }

                        continue;
                    }
                }
            }

            await SyntaxNodeRefactoring.ComputeRefactoringsAsync(context, firstNode).ConfigureAwait(false);
        }

        public static void ComputeRefactoringsForNodeInsideTrivia(this RefactoringContext context)
        {
            SyntaxNode node = context.FindNode(findInsideTrivia: true);

            if (node == null)
                return;

            bool fDirectiveTrivia = false;

            using (IEnumerator<SyntaxNode> en = node.AncestorsAndSelf().GetEnumerator())
            {
                while (en.MoveNext())
                {
                    node = en.Current;

                    Debug.WriteLine(node.Kind().ToString());

                   if (!fDirectiveTrivia)
                    {
                        var directiveTrivia = node as DirectiveTriviaSyntax;
                        if (directiveTrivia != null)
                        {
                            DirectiveTriviaRefactoring.ComputeRefactorings(context, directiveTrivia);

                            if (node.IsKind(
                                SyntaxKind.RegionDirectiveTrivia,
                                SyntaxKind.EndRegionDirectiveTrivia))
                            {
                                RegionDirectiveTriviaRefactoring.ComputeRefactorings(context);
                            }

                            fDirectiveTrivia = true;
                        }
                    }
                }
            }
        }

        public static async Task ComputeRefactoringsForTokenAsync(this RefactoringContext context)
        {
            SyntaxToken token = context.FindToken();

            if (!token.IsKind(SyntaxKind.None)
                && token.Span.Contains(context.Span))
            {
                Debug.WriteLine(token.Kind().ToString());

                switch (token.Kind())
                {
                    case SyntaxKind.AmpersandAmpersandToken:
                    case SyntaxKind.BarBarToken:
                    case SyntaxKind.EqualsEqualsToken:
                    case SyntaxKind.ExclamationEqualsToken:
                    case SyntaxKind.GreaterThanToken:
                    case SyntaxKind.GreaterThanEqualsToken:
                    case SyntaxKind.LessThanToken:
                    case SyntaxKind.LessThanEqualsToken:
                        {
                            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.NegateOperator))
                            {
                                context.RegisterRefactoring(
                                    "Negate operator",
                                    cancellationToken => NegateOperatorRefactoring.RefactorAsync(context.Document, token, cancellationToken));
                            }

                            break;
                        }
                    case SyntaxKind.CloseParenToken:
                        {
                            await CloseParenTokenRefactoring.ComputeRefactoringsAsync(context, token).ConfigureAwait(false);
                            break;
                        }
                    case SyntaxKind.CommaToken:
                        {
                            await CommaTokenRefactoring.ComputeRefactoringsAsync(context, token).ConfigureAwait(false);
                            break;
                        }
                }
            }
        }

        public static void ComputeRefactoringsForTrivia(this RefactoringContext context)
        {
            SyntaxTrivia trivia = context.FindTrivia();

            Debug.WriteLine(trivia.Kind().ToString());

            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                    {
                        if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.Uncomment))
                        {
                            context.RegisterRefactoring(
                                "Uncomment",
                                cancellationToken => UncommentRefactoring.RefactorAsync(context.Document, trivia, cancellationToken));
                        }

                        break;
                    }
            }

            if (!trivia.IsCommentTrivia())
            {
                SyntaxTrivia trivia2 = context.FindTriviaInsideTrivia();

                if (trivia.Span != trivia2.Span)
                {
                    trivia = trivia2;
                    Debug.WriteLine(trivia.Kind().ToString());
                }
            }

            CommentTriviaRefactoring.ComputeRefactorings(context, trivia);
        }
    }
}
