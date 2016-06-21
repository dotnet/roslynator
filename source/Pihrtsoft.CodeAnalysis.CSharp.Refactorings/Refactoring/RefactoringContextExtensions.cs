// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RefactoringContextExtensions
    {
        public static void ComputeRefactoringsForNodeInsideTrivia(this RefactoringContext context)
        {
            SyntaxNode node = context.FindNode(findInsideTrivia: true);

            if (node == null)
                return;

            bool fRegionDirectiveTrivia = false;

            using (IEnumerator<SyntaxNode> en = node.AncestorsAndSelf().GetEnumerator())
            {
                while (en.MoveNext())
                {
                    node = en.Current;

                    if (!fRegionDirectiveTrivia
                        && node.IsAnyKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia)
                        && context.Root.IsKind(SyntaxKind.CompilationUnit))
                    {
                        RegionDirectiveTriviaRefactoring.ComputeRefactorings(context);
                        fRegionDirectiveTrivia = true;
                        continue;
                    }
                }
            }
        }

        public static async Task ComputeRefactoringsForNodeAsync(this RefactoringContext context)
        {
            SyntaxNode node = context.FindNode();

            if (node == null)
                return;

            bool fAccessor = false;
            bool fArgument = false;
            bool fArgumentList = false;
            bool fArrowExpressionClause = false;
            bool fParameter = false;
            bool fParameterList = false;
            bool fSwitchSection = false;
            bool fVariableDeclaration = false;

            bool fExpression = false;
            bool fAssignmentExpression = false;
            bool fBinaryExpression = false;
            bool fConditionalExpression = false;
            bool fFormatBinaryExpression = false;
            bool fGenericName = false;
            bool fIdentifierName = false;
            bool fInitializerExpression = false;
            bool fInterpolatedStringExpression = false;
            bool fInvocationExpression = false;
            bool fLambdaExpression = false;
            bool fLiteralExpression = false;
            bool fMemberAccessExpression = false;
            bool fParenthesizedExpression = false;
            bool fPostfixUnaryExpression = false;
            bool fPrefixUnaryExpression = false;
            bool fSimpleLambdaExpression = false;

            bool fMemberDeclaration = false;
            bool fFieldDeclaration = false;
#if DEBUG
            bool fReorderMembers = false;
#endif
            bool fStatement = false;
            bool fDoStatement = false;
            bool fForEachStatement = false;
            bool fForStatement = false;
            bool fIfStatement = false;
            bool fLocalDeclarationStatement = false;
            bool fReturnStatement = false;
            bool fSwitchStatement = false;
            bool fUsingStatement = false;
            bool fWhileStatement = false;
            bool fYieldReturnStatement = false;

            using (IEnumerator<SyntaxNode> en = node.AncestorsAndSelf().GetEnumerator())
            {
                while (en.MoveNext())
                {
                    node = en.Current;

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
                        await ArgumentRefactoring.ComputeRefactoringsAsync(context, (ArgumentSyntax)node);
                        fArgument = true;
                        continue;
                    }

                    if (!fArgumentList
                        && node.IsKind(SyntaxKind.ArgumentList))
                    {
                        await ArgumentListRefactoring.ComputeRefactoringsAsync(context, (ArgumentListSyntax)node);
                        fArgumentList = true;
                        continue;
                    }

                    if (!fArrowExpressionClause
                        && node.IsKind(SyntaxKind.ArrowExpressionClause))
                    {
                        ArrowExpressionClauseRefactoring.ComputeRefactorings(context, (ArrowExpressionClauseSyntax)node);
                        fArrowExpressionClause = true;
                        continue;
                    }

                    if (!fParameter
                        && node.IsKind(SyntaxKind.Parameter))
                    {
                        await ParameterRefactoring.ComputeRefactoringsAsync(context, (ParameterSyntax)node);
                        fParameter = true;
                        continue;
                    }

                    if (!fParameterList
                        && node.IsKind(SyntaxKind.ParameterList))
                    {
                        SyntaxToken token = context.FindToken();

                        switch (token.Kind())
                        {
                            case SyntaxKind.CloseParenToken:
                                await CloseParenTokenRefactoring.ComputeRefactoringsAsync(context, token);
                                break;
                            case SyntaxKind.CommaToken:
                                await CommaTokenRefactoring.ComputeRefactoringsAsync(context, token);
                                break;
                        }

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
                        await VariableDeclarationRefactoring.ComputeRefactoringsAsync(context, (VariableDeclarationSyntax)node);
                        fVariableDeclaration = true;
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
                                await AssignmentExpressionRefactoring.ComputeRefactoringsAsync(context, assignmentExpression);
                                fAssignmentExpression = true;
                            }
                        }

                        if (!fBinaryExpression || !fFormatBinaryExpression)
                        {
                            var binaryExpression = node as BinaryExpressionSyntax;
                            if (binaryExpression != null)
                            {
                                if (!fFormatBinaryExpression)
                                {
                                    switch (binaryExpression.Parent?.Kind())
                                    {
                                        case SyntaxKind.IfStatement:
                                            {
                                                FormatBinaryExpressionRefactoring.ComputeRefactorings(context, (IfStatementSyntax)binaryExpression.Parent);
                                                fFormatBinaryExpression = true;
                                                break;
                                            }
                                        case SyntaxKind.DoStatement:
                                            {
                                                FormatBinaryExpressionRefactoring.ComputeRefactorings(context, (DoStatementSyntax)binaryExpression.Parent);
                                                fFormatBinaryExpression = true;
                                                break;
                                            }
                                        case SyntaxKind.WhileStatement:
                                            {
                                                FormatBinaryExpressionRefactoring.ComputeRefactorings(context, (WhileStatementSyntax)binaryExpression.Parent);
                                                fFormatBinaryExpression = true;
                                                break;
                                            }
                                    }
                                }

                                if (!fBinaryExpression)
                                {
                                    await BinaryExpressionRefactoring.ComputeRefactoringsAsync(context, binaryExpression);
                                    fBinaryExpression = true;
                                }
                            }
                        }

                        if (!fConditionalExpression
                            && node.IsKind(SyntaxKind.ConditionalExpression))
                        {
                            ConditionalExpressionRefactoring.ComputeRefactorings(context, (ConditionalExpressionSyntax)expression);
                            fConditionalExpression = true;
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
                            await IdentifierNameRefactoring.ComputeRefactoringsAsync(context, (IdentifierNameSyntax)expression);
                            fIdentifierName = true;
                        }

                        if (!fInitializerExpression)
                        {
                            var initializer = node as InitializerExpressionSyntax;
                            if (initializer != null)
                            {
                                InitializerExpressionRefactoring.ComputeRefactorings(context, initializer);
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
                            await InvocationExpressionRefactoring.ComputeRefactoringsAsync(context, (InvocationExpressionSyntax)expression);
                            fInvocationExpression = true;
                        }

                        var lambdaExpression = node as LambdaExpressionSyntax;
                        if (lambdaExpression != null)
                        {
                            if (!fSimpleLambdaExpression
                                && node.IsKind(SyntaxKind.SimpleLambdaExpression))
                            {
                                await SimpleLambdaExpressionRefactoring.ComputeRefactoringsAsync(context, (SimpleLambdaExpressionSyntax)expression);
                                fSimpleLambdaExpression = true;
                            }

                            if (!fLambdaExpression)
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

                                if (node.IsKind(SyntaxKind.StringLiteralExpression))
                                    StringLiteralRefactoring.ComputeRefactorings(context, literalExpression);

                                fLiteralExpression = true;
                            }
                        }

                        if (!fMemberAccessExpression)
                        {
                            var memberAccess = node as MemberAccessExpressionSyntax;
                            if (memberAccess != null)
                            {
                                await MemberAccessExpressionRefactoring.ComputeRefactoringsAsync(context, memberAccess);
                                fMemberAccessExpression = true;
                            }
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
                        if (!fFieldDeclaration
                            && node.IsKind(SyntaxKind.FieldDeclaration))
                        {
                            await FieldDeclarationRefactoring.ComputeRefactoringsAsync(context, (FieldDeclarationSyntax)memberDeclaration);
                            fFieldDeclaration = true;
                        }
#if DEBUG
                        if (!fReorderMembers
                            && node.IsAnyKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration))
                        {
                            ReorderMembersRefactoring.ComputeRefactorings(context, memberDeclaration);
                            fReorderMembers = true;
                        }
#endif
                        if (!fMemberDeclaration)
                        {
                            await MemberDeclarationRefactoring.ComputeRefactoringsAsync(context, memberDeclaration);
                            await IntroduceConstructorRefactoring.ComputeRefactoringsAsync(context, memberDeclaration);
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
                            await DoStatementRefactoring.ComputeRefactoringsAsync(context, (DoStatementSyntax)statement);
                            fDoStatement = true;
                        }

                        if (!fForEachStatement
                            && node.IsKind(SyntaxKind.ForEachStatement))
                        {
                            await ForEachStatementRefactoring.ComputeRefactoringsAsync(context, (ForEachStatementSyntax)statement);
                            fForEachStatement = true;
                        }

                        if (!fForStatement
                            && node.IsKind(SyntaxKind.ForStatement))
                        {
                            await ForStatementRefactoring.ComputeRefactoringsAsync(context, (ForStatementSyntax)statement);
                            fForStatement = true;
                        }

                        if (!fIfStatement
                            && node.IsKind(SyntaxKind.IfStatement))
                        {
                            await IfStatementRefactoring.ComputeRefactoringsAsync(context, (IfStatementSyntax)statement);
                            fIfStatement = true;
                        }

                        if (!fLocalDeclarationStatement
                            && node.IsKind(SyntaxKind.LocalDeclarationStatement))
                        {
                            await LocalDeclarationStatementRefactoring.ComputeRefactoringsAsync(context, (LocalDeclarationStatementSyntax)statement);
                            fLocalDeclarationStatement = true;
                        }

                        if (!fReturnStatement
                            && node.IsKind(SyntaxKind.ReturnStatement))
                        {
                            await ReturnStatementRefactoring.ComputeRefactoringsAsync(context, (ReturnStatementSyntax)statement);
                            fReturnStatement = true;
                        }

                        if (!fSwitchStatement
                            && node.IsKind(SyntaxKind.SwitchStatement))
                        {
                            await SwitchStatementRefactoring.ComputeRefactoringsAsync(context, (SwitchStatementSyntax)statement);
                            fSwitchStatement = true;
                        }

                        if (!fUsingStatement
                            && node.IsKind(SyntaxKind.UsingStatement))
                        {
                            await UsingStatementRefactoring.ComputeRefactoringsAsync(context, (UsingStatementSyntax)statement);
                            fUsingStatement = true;
                        }

                        if (!fWhileStatement
                            && node.IsKind(SyntaxKind.WhileStatement))
                        {
                            await WhileStatementRefactoring.ComputeRefactoringsAsync(context, (WhileStatementSyntax)statement);
                            fWhileStatement = true;
                        }

                        if (!fYieldReturnStatement)
                        {
                            var yieldStatement = node as YieldStatementSyntax;
                            if (yieldStatement != null)
                            {
                                await YieldReturnStatementRefactoring.ComputeRefactoringsAsync(context, yieldStatement);
                                fYieldReturnStatement = true;
                            }
                        }

                        if (!fStatement)
                        {
                            StatementRefactoring.ComputeRefactorings(context, statement);
                            fStatement = true;
                        }

                        continue;
                    }
                }
            }
        }

        public static void ComputeRefactoringsForToken(this RefactoringContext context)
        {
            SyntaxToken token = context.FindToken();

            if (!token.IsKind(SyntaxKind.None))
                NegateOperatorRefactoring.ComputeRefactorings(context, token);
        }

        public static void ComputeRefactoringsForTrivia(this RefactoringContext context)
        {
            SyntaxTrivia trivia = context.FindTrivia();

            if (!trivia.IsKind(SyntaxKind.None))
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        {
                            SingleLineCommentTriviaRefactoring.ComputeRefactorings(context, trivia);
                            break;
                        }
                }

                if (context.Root.IsKind(SyntaxKind.CompilationUnit))
                    RemoveCommentRefactoring.ComputeRefactorings(context, trivia);
            }
        }
    }
}
