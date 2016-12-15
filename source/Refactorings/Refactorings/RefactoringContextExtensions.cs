// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RefactoringContextExtensions
    {
        public static bool IsRefactoringEnabled(this RefactoringContext context, string identifier)
        {
            return context.Settings.IsRefactoringEnabled(identifier);
        }

        public static bool IsAnyRefactoringEnabled(this RefactoringContext context, string identifier1, string identifier2)
        {
            return context.Settings.IsAnyRefactoringEnabled(identifier1, identifier2);
        }

        public static bool IsAnyRefactoringEnabled(this RefactoringContext context, string identifier1, string identifier2, string identifier3)
        {
            return context.Settings.IsAnyRefactoringEnabled(identifier1, identifier2, identifier3);
        }

        public static bool IsAnyRefactoringEnabled(this RefactoringContext context, string identifier1, string identifier2, string identifier3, string identifier4)
        {
            return context.Settings.IsAnyRefactoringEnabled(identifier1, identifier2, identifier3, identifier4);
        }

        public static bool IsAnyRefactoringEnabled(this RefactoringContext context, string identifier1, string identifier2, string identifier3, string identifier4, string identifier5)
        {
            return context.Settings.IsAnyRefactoringEnabled(identifier1, identifier2, identifier3, identifier4, identifier5);
        }

        public static bool IsAnyRefactoringEnabled(
            this RefactoringContext context,
            string identifier1,
            string identifier2,
            string identifier3,
            string identifier4,
            string identifier5,
            string identifier6)
        {
            return context.Settings.IsAnyRefactoringEnabled(
                identifier1,
                identifier2,
                identifier3,
                identifier4,
                identifier5,
                identifier6);
        }

        public static async Task ComputeRefactoringsAsync(this RefactoringContext context)
        {
            Debug.WriteLine($"START {nameof(ComputeRefactoringsForTriviaInsideTrivia)}");
            ComputeRefactoringsForTriviaInsideTrivia(context);
            Debug.WriteLine($"END {nameof(ComputeRefactoringsForTriviaInsideTrivia)}");

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
            bool fUsingDirective = false;

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
            bool fInterpolation = false;
            bool fInvocationExpression = false;
            bool fLambdaExpression = false;
            bool fLiteralExpression = false;
            bool fSimpleMemberAccessExpression = false;
            bool fParenthesizedExpression = false;
            bool fPostfixUnaryExpression = false;
            bool fPrefixUnaryExpression = false;
            bool fAwaitExpression = false;
            bool fCastExpression = false;

            bool fMemberDeclaration = false;
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
            bool fLockStatement = false;
            bool fBlock = false;
            bool fStatementRefactoring = false;
            bool fThrowStatement = false;

            SyntaxNode firstNode = node;

            using (IEnumerator<SyntaxNode> en = node.AncestorsAndSelf().GetEnumerator())
            {
                while (en.MoveNext())
                {
                    node = en.Current;

                    SyntaxKind kind = node.Kind();

                    Debug.WriteLine(kind.ToString());

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
                        && kind == SyntaxKind.Argument)
                    {
                        await ArgumentRefactoring.ComputeRefactoringsAsync(context, (ArgumentSyntax)node).ConfigureAwait(false);
                        fArgument = true;
                        continue;
                    }

                    if (!fArgumentList
                        && kind == SyntaxKind.ArgumentList)
                    {
                        await ArgumentListRefactoring.ComputeRefactoringsAsync(context, (ArgumentListSyntax)node).ConfigureAwait(false);
                        fArgumentList = true;
                        continue;
                    }

                    if (!fAttributeArgumentList
                        && kind == SyntaxKind.AttributeArgumentList)
                    {
                        await AttributeArgumentListRefactoring.ComputeRefactoringsAsync(context, (AttributeArgumentListSyntax)node).ConfigureAwait(false);
                        fAttributeArgumentList = true;
                        continue;
                    }

                    if (!fArrowExpressionClause
                        && kind == SyntaxKind.ArrowExpressionClause)
                    {
                        await ArrowExpressionClauseRefactoring.ComputeRefactoringsAsync(context, (ArrowExpressionClauseSyntax)node).ConfigureAwait(false);
                        fArrowExpressionClause = true;
                        continue;
                    }

                    if (!fParameter
                        && kind == SyntaxKind.Parameter)
                    {
                        await ParameterRefactoring.ComputeRefactoringsAsync(context, (ParameterSyntax)node).ConfigureAwait(false);
                        fParameter = true;
                        continue;
                    }

                    if (!fParameterList
                        && kind == SyntaxKind.ParameterList)
                    {
                        await ParameterListRefactoring.ComputeRefactoringsAsync(context, (ParameterListSyntax)node).ConfigureAwait(false);
                        fParameterList = true;
                        continue;
                    }

                    if (!fSwitchSection
                        && kind == SyntaxKind.SwitchSection)
                    {
                        await SwitchSectionRefactoring.ComputeRefactoringsAsync(context, (SwitchSectionSyntax)node).ConfigureAwait(false);
                        fSwitchSection = true;
                        continue;
                    }

                    if (!fVariableDeclaration
                        && kind == SyntaxKind.VariableDeclaration)
                    {
                        await VariableDeclarationRefactoring.ComputeRefactoringsAsync(context, (VariableDeclarationSyntax)node).ConfigureAwait(false);
                        fVariableDeclaration = true;
                        continue;
                    }

                    if (!fInterpolatedStringText
                        && kind == SyntaxKind.InterpolatedStringText)
                    {
                        InterpolatedStringTextRefactoring.ComputeRefactorings(context, (InterpolatedStringTextSyntax)node);
                        fInterpolatedStringText = true;
                        continue;
                    }

                    if (!fInterpolation
                        && kind == SyntaxKind.Interpolation)
                    {
                        InterpolationRefactoring.ComputeRefactorings(context, (InterpolationSyntax)node);
                        fInterpolation = true;
                        continue;
                    }

                    if (!fElseClause
                        && kind == SyntaxKind.ElseClause)
                    {
                        ElseClauseRefactoring.ComputeRefactorings(context, (ElseClauseSyntax)node);
                        fElseClause = true;
                        continue;
                    }

                    if (!fCaseSwitchLabel
                        && kind == SyntaxKind.CaseSwitchLabel)
                    {
                        await CaseSwitchLabelRefactoring.ComputeRefactoringsAsync(context, (CaseSwitchLabelSyntax)node).ConfigureAwait(false);
                        fCaseSwitchLabel = true;
                        continue;
                    }

                    if (!fUsingDirective
                        && kind == SyntaxKind.UsingDirective)
                    {
                        UsingDirectiveRefactoring.ComputeRefactoring(context, (UsingDirectiveSyntax)node);
                        fUsingDirective = true;
                        continue;
                    }

                    var expression = node as ExpressionSyntax;
                    if (expression != null)
                    {
                        if (!fExpression)
                        {
                            await ExpressionRefactoring.ComputeRefactoringsAsync(context, expression).ConfigureAwait(false);
                            fExpression = true;
                        }

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
                            && kind == SyntaxKind.AnonymousMethodExpression)
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
                            && kind == SyntaxKind.ConditionalExpression)
                        {
                            await ConditionalExpressionRefactoring.ComputeRefactoringsAsync(context, (ConditionalExpressionSyntax)expression).ConfigureAwait(false);
                            fConditionalExpression = true;
                        }

                        if (!fQualifiedName
                            && kind == SyntaxKind.QualifiedName)
                        {
                            await QualifiedNameRefactoring.ComputeRefactoringsAsync(context, (QualifiedNameSyntax)expression).ConfigureAwait(false);
                            fQualifiedName = true;
                        }

                        if (!fGenericName
                            && kind == SyntaxKind.GenericName)
                        {
                            GenericNameRefactoring.ComputeRefactorings(context, (GenericNameSyntax)expression);
                            fGenericName = true;
                        }

                        if (!fIdentifierName
                            && kind == SyntaxKind.IdentifierName)
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
                            && kind == SyntaxKind.InterpolatedStringExpression)
                        {
                            InterpolatedStringRefactoring.ComputeRefactorings(context, (InterpolatedStringExpressionSyntax)expression);
                            fInterpolatedStringExpression = true;
                        }

                        if (!fInvocationExpression
                            && kind == SyntaxKind.InvocationExpression)
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
                                await LiteralExpressionRefactoring.ComputeRefactoringsAsync(context, literalExpression).ConfigureAwait(false);
                                fLiteralExpression = true;
                            }
                        }

                        if (!fSimpleMemberAccessExpression
                            && kind == SyntaxKind.SimpleMemberAccessExpression)
                        {
                            await SimpleMemberAccessExpressionRefactoring.ComputeRefactoringAsync(context, (MemberAccessExpressionSyntax)node).ConfigureAwait(false);
                            fSimpleMemberAccessExpression = true;
                        }

                        if (!fParenthesizedExpression
                            && kind == SyntaxKind.ParenthesizedExpression)
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
                            && kind == SyntaxKind.AwaitExpression)
                        {
                            await AwaitExpressionRefactoring.ComputeRefactoringsAsync(context, (AwaitExpressionSyntax)node).ConfigureAwait(false);
                            fAwaitExpression = true;
                        }

                        if (!fCastExpression
                            && kind == SyntaxKind.CastExpression)
                        {
                            CastExpressionRefactoring.ComputeRefactorings(context, (CastExpressionSyntax)node);
                            fCastExpression = true;
                        }
                        continue;
                    }

                    var memberDeclaration = node as MemberDeclarationSyntax;
                    if (memberDeclaration != null)
                    {
                        if (!fMemberDeclaration)
                        {
                            await MemberDeclarationRefactoring.ComputeRefactoringsAsync(context, memberDeclaration).ConfigureAwait(false);
                            AttributeListRefactoring.ComputeRefactorings(context, memberDeclaration);
                            await IntroduceConstructorRefactoring.ComputeRefactoringsAsync(context, memberDeclaration).ConfigureAwait(false);
                            fMemberDeclaration = true;
                        }

                        continue;
                    }

                    var statement = node as StatementSyntax;
                    if (statement != null)
                    {
                        if (!fDoStatement
                            && kind == SyntaxKind.DoStatement)
                        {
                            await DoStatementRefactoring.ComputeRefactoringsAsync(context, (DoStatementSyntax)statement).ConfigureAwait(false);
                            fDoStatement = true;
                        }

                        if (!fExpressionStatement
                            && kind == SyntaxKind.ExpressionStatement)
                        {
                            await ExpressionStatementRefactoring.ComputeRefactoringsAsync(context, (ExpressionStatementSyntax)statement).ConfigureAwait(false);
                            fExpressionStatement = true;
                        }

                        if (!fForEachStatement
                            && kind == SyntaxKind.ForEachStatement)
                        {
                            await ForEachStatementRefactoring.ComputeRefactoringsAsync(context, (ForEachStatementSyntax)statement).ConfigureAwait(false);
                            fForEachStatement = true;
                        }

                        if (!fForStatement
                            && kind == SyntaxKind.ForStatement)
                        {
                            await ForStatementRefactoring.ComputeRefactoringsAsync(context, (ForStatementSyntax)statement).ConfigureAwait(false);
                            fForStatement = true;
                        }

                        if (!fIfStatement
                            && kind == SyntaxKind.IfStatement)
                        {
                            await IfStatementRefactoring.ComputeRefactoringsAsync(context, (IfStatementSyntax)statement).ConfigureAwait(false);
                            fIfStatement = true;
                        }

                        if (!fLocalDeclarationStatement
                            && kind == SyntaxKind.LocalDeclarationStatement)
                        {
                            await LocalDeclarationStatementRefactoring.ComputeRefactoringsAsync(context, (LocalDeclarationStatementSyntax)statement).ConfigureAwait(false);
                            fLocalDeclarationStatement = true;
                        }

                        if (!fReturnStatement
                            && kind == SyntaxKind.ReturnStatement)
                        {
                            await ReturnStatementRefactoring.ComputeRefactoringsAsync(context, (ReturnStatementSyntax)statement).ConfigureAwait(false);
                            fReturnStatement = true;
                        }

                        if (!fSwitchStatement
                            && kind == SyntaxKind.SwitchStatement)
                        {
                            await SwitchStatementRefactoring.ComputeRefactoringsAsync(context, (SwitchStatementSyntax)statement).ConfigureAwait(false);
                            fSwitchStatement = true;
                        }

                        if (!fUsingStatement
                            && kind == SyntaxKind.UsingStatement)
                        {
                            await UsingStatementRefactoring.ComputeRefactoringsAsync(context, (UsingStatementSyntax)statement).ConfigureAwait(false);
                            fUsingStatement = true;
                        }

                        if (!fWhileStatement
                            && kind == SyntaxKind.WhileStatement)
                        {
                            await WhileStatementRefactoring.ComputeRefactoringsAsync(context, (WhileStatementSyntax)statement).ConfigureAwait(false);
                            fWhileStatement = true;
                        }

                        if (!fYieldReturnStatement)
                        {
                            var yieldStatement = node as YieldStatementSyntax;
                            if (yieldStatement != null)
                            {
                                await YieldStatementRefactoring.ComputeRefactoringsAsync(context, yieldStatement).ConfigureAwait(false);
                                fYieldReturnStatement = true;
                            }
                        }

                        if (!fLockStatement
                            && kind == SyntaxKind.LockStatement)
                        {
                            LockStatementRefactoring.ComputeRefactorings(context, (LockStatementSyntax)node);
                            fLockStatement = true;
                        }

                        if (!fBlock
                            && kind == SyntaxKind.Block)
                        {
                            await BlockRefactoring.ComputeRefactoringAsync(context, (BlockSyntax)node).ConfigureAwait(false);
                            fBlock = true;
                        }

                        if (!fThrowStatement
                            && kind == SyntaxKind.ThrowStatement)
                        {
                            await ThrowStatementRefactoring.ComputeRefactoringAsync(context, (ThrowStatementSyntax)node).ConfigureAwait(false);
                            fThrowStatement = true;
                        }

                        if (!fStatement)
                        {
                            AddBracesRefactoring.ComputeRefactoring(context, statement);
                            RemoveBracesRefactoring.ComputeRefactoring(context, statement);
                            ExtractStatementRefactoring.ComputeRefactoring(context, statement);
                            fStatement = true;
                        }

                        if (!fStatementRefactoring)
                        {
                            if (kind == SyntaxKind.Block)
                            {
                                StatementRefactoring.ComputeRefactoring(context, (BlockSyntax)node);
                                fStatementRefactoring = true;
                            }
                            else if (kind == SyntaxKind.SwitchStatement)
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

            CommentTriviaRefactoring.ComputeRefactorings(context, firstNode);
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

                            if (node.IsKind(SyntaxKind.RegionDirectiveTrivia,SyntaxKind.EndRegionDirectiveTrivia))
                                RegionDirectiveTriviaRefactoring.ComputeRefactorings(context);

                            RemoveAllPreprocessorDirectivesRefactoring.ComputeRefactorings(context);

                            if (node.IsKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia))
                                RegionDirectiveTriviaRefactoring.ComputeRefactorings(context, (RegionDirectiveTriviaSyntax)node);

                            fDirectiveTrivia = true;
                        }
                    }
                }
            }
        }

        public static async Task ComputeRefactoringsForTokenAsync(this RefactoringContext context)
        {
            SyntaxToken token = context.FindToken();

            SyntaxKind kind = token.Kind();

            if (kind != SyntaxKind.None
                && token.Span.Contains(context.Span))
            {
                Debug.WriteLine(kind.ToString());

                switch (kind)
                {
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
                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.Uncomment))
                        {
                            context.RegisterRefactoring(
                                "Uncomment",
                                cancellationToken => UncommentRefactoring.RefactorAsync(context.Document, trivia, cancellationToken));
                        }

                        break;
                    }
            }

            if (!trivia.IsPartOfStructuredTrivia())
                CommentTriviaRefactoring.ComputeRefactorings(context, trivia);
        }

        public static void ComputeRefactoringsForTriviaInsideTrivia(this RefactoringContext context)
        {
            SyntaxTrivia trivia = context.FindTriviaInsideTrivia();

            Debug.WriteLine(trivia.Kind().ToString());

            if (trivia.IsPartOfStructuredTrivia())
                CommentTriviaRefactoring.ComputeRefactorings(context, trivia);
        }
    }
}
