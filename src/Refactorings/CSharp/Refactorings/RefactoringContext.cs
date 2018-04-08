// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal class RefactoringContext
    {
        public RefactoringContext(CodeRefactoringContext underlyingContext, SyntaxNode root, RefactoringSettings settings)
        {
            UnderlyingContext = underlyingContext;
            Root = root;
            Settings = settings;
        }

        public CodeRefactoringContext UnderlyingContext { get; }

        public SyntaxNode Root { get; }

        public RefactoringSettings Settings { get; }

        public bool SupportsSemanticModel
        {
            get { return Document.SupportsSemanticModel; }
        }

        public CancellationToken CancellationToken
        {
            get { return UnderlyingContext.CancellationToken; }
        }

        public Document Document
        {
            get { return UnderlyingContext.Document; }
        }

        public Project Project
        {
            get { return Document.Project; }
        }

        public Solution Solution
        {
            get { return Project.Solution; }
        }

        public TextSpan Span
        {
            get { return UnderlyingContext.Span; }
        }

        public bool IsRootCompilationUnit
        {
            get { return Root.IsKind(SyntaxKind.CompilationUnit); }
        }

        public bool SupportsCSharp6
        {
            get
            {
                if (Project.Language == LanguageNames.CSharp)
                {
                    switch (((CSharpParseOptions)Project.ParseOptions).LanguageVersion)
                    {
                        case LanguageVersion.CSharp1:
                        case LanguageVersion.CSharp2:
                        case LanguageVersion.CSharp3:
                        case LanguageVersion.CSharp4:
                        case LanguageVersion.CSharp5:
                            return false;
                        default:
                            return true;
                    }
                }

                return false;
            }
        }

        public bool SupportsCSharp7
        {
            get
            {
                if (Project.Language == LanguageNames.CSharp)
                {
                    switch (((CSharpParseOptions)Project.ParseOptions).LanguageVersion)
                    {
                        case LanguageVersion.CSharp1:
                        case LanguageVersion.CSharp2:
                        case LanguageVersion.CSharp3:
                        case LanguageVersion.CSharp4:
                        case LanguageVersion.CSharp5:
                        case LanguageVersion.CSharp6:
                            return false;
                        default:
                            return true;
                    }
                }

                return false;
            }
        }

        public Task<SemanticModel> GetSemanticModelAsync()
        {
            return Document.GetSemanticModelAsync(CancellationToken);
        }

        public void RegisterRefactoring(
            string title,
            Func<CancellationToken, Task<Document>> createChangedDocument,
            string equivalenceKey = null)
        {
            RegisterRefactoring(
                CodeAction.Create(title, createChangedDocument, equivalenceKey));
        }

        public void RegisterRefactoring(
            string title,
            Func<CancellationToken, Task<Solution>> createChangedSolution,
            string equivalenceKey = null)
        {
            RegisterRefactoring(
                CodeAction.Create(title, createChangedSolution, equivalenceKey));
        }

        public void RegisterRefactoring(CodeAction codeAction)
        {
            Debug.WriteLine($"REGISTERING REFACTORING \"{codeAction.Title}\"");

            UnderlyingContext.RegisterRefactoring(codeAction);
        }

        public async Task ComputeRefactoringsAsync()
        {
            ComputeRefactoringsForTriviaInsideTrivia();

            ComputeRefactoringsForNodeInsideTrivia();

            await ComputeRefactoringsForTokenAsync().ConfigureAwait(false);

            ComputeRefactoringsForTrivia();

            await ComputeRefactoringsForNodeAsync().ConfigureAwait(false);
        }

        public void ComputeRefactoringsForTriviaInsideTrivia()
        {
            SyntaxTrivia trivia = Root.FindTrivia(Span.Start, findInsideTrivia: true);

            Debug.WriteLine(trivia.Kind().ToString());

            if (trivia.IsPartOfStructuredTrivia())
                CommentTriviaRefactoring.ComputeRefactorings(this, trivia);
        }

        public void ComputeRefactoringsForNodeInsideTrivia()
        {
            SyntaxNode node = Root.FindNode(Span, findInsideTrivia: true, getInnermostNodeForTie: true);

            if (node == null)
                return;

            bool fDirectiveTrivia = false;

            using (IEnumerator<SyntaxNode> en = node.AncestorsAndSelf().GetEnumerator())
            {
                while (en.MoveNext())
                {
                    node = en.Current;

                    Debug.WriteLine(node.Kind().ToString());

                    if (!fDirectiveTrivia
                        && (node is DirectiveTriviaSyntax directiveTrivia))
                    {
                        DirectiveTriviaRefactoring.ComputeRefactorings(this, directiveTrivia);

                        SyntaxKind kind = node.Kind();

                        if (kind == SyntaxKind.RegionDirectiveTrivia
                            || kind == SyntaxKind.EndRegionDirectiveTrivia)
                        {
                            RegionDirectiveTriviaRefactoring.ComputeRefactorings(this);
                        }

                        RemoveAllPreprocessorDirectivesRefactoring.ComputeRefactorings(this);

                        if (kind == SyntaxKind.RegionDirectiveTrivia)
                        {
                            RegionDirectiveTriviaRefactoring.ComputeRefactorings(this, (RegionDirectiveTriviaSyntax)node);
                        }
                        else if (kind == SyntaxKind.EndRegionDirectiveTrivia)
                        {
                            RegionDirectiveTriviaRefactoring.ComputeRefactorings(this, (EndRegionDirectiveTriviaSyntax)node);
                        }

                        fDirectiveTrivia = true;
                    }
                }
            }
        }

        public async Task ComputeRefactoringsForTokenAsync()
        {
            SyntaxToken token = Root.FindToken(Span.Start);

            SyntaxKind kind = token.Kind();

            if (kind != SyntaxKind.None
                && token.Span.Contains(Span))
            {
                Debug.WriteLine(kind.ToString());

                switch (kind)
                {
                    case SyntaxKind.CloseParenToken:
                        {
                            await CloseParenTokenRefactoring.ComputeRefactoringsAsync(this, token).ConfigureAwait(false);
                            break;
                        }
                    case SyntaxKind.CommaToken:
                        {
                            await CommaTokenRefactoring.ComputeRefactoringsAsync(this, token).ConfigureAwait(false);
                            break;
                        }
                    case SyntaxKind.SemicolonToken:
                        {
                            SemicolonTokenRefactoring.ComputeRefactorings(this, token);
                            break;
                        }
                    case SyntaxKind.PlusToken:
                        {
                            await PlusTokenRefactoring.ComputeRefactoringsAsync(this, token).ConfigureAwait(false);
                            break;
                        }
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        {
                            {
                                if (IsRefactoringEnabled(RefactoringIdentifiers.ChangeAccessibility))
                                    await AccessModifierRefactoring.ComputeRefactoringsAsync(this, token).ConfigureAwait(false);

                                break;
                            }
                        }
                }
            }
        }

        public void ComputeRefactoringsForTrivia()
        {
            SyntaxTrivia trivia = Root.FindTrivia(Span.Start);

            Debug.WriteLine(trivia.Kind().ToString());

            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                    {
                        if (IsRefactoringEnabled(RefactoringIdentifiers.UncommentSingleLineComment))
                        {
                            RegisterRefactoring(
                                "Uncomment",
                                cancellationToken => UncommentSingleLineCommentRefactoring.RefactorAsync(Document, trivia, cancellationToken));
                        }

                        if (IsRefactoringEnabled(RefactoringIdentifiers.ReplaceCommentWithDocumentationComment))
                        {
                            TextSpan fixableSpan = ReplaceCommentWithDocumentationCommentAnalysis.GetFixableSpan(trivia);

                            if (!fixableSpan.IsEmpty)
                            {
                                RegisterRefactoring(
                                    ReplaceCommentWithDocumentationCommentRefactoring.Title,
                                    cancellationToken => ReplaceCommentWithDocumentationCommentRefactoring.RefactorAsync(Document, (MemberDeclarationSyntax)trivia.Token.Parent, fixableSpan, cancellationToken));
                            }
                        }

                        break;
                    }
                case SyntaxKind.MultiLineCommentTrivia:
                    {
                        if (IsRefactoringEnabled(RefactoringIdentifiers.UncommentMultiLineComment))
                            UncommentMultiLineCommentRefactoring.ComputeRefactoring(this, trivia);

                        break;
                    }
            }

            if (!trivia.IsPartOfStructuredTrivia())
                CommentTriviaRefactoring.ComputeRefactorings(this, trivia);
        }

        public async Task ComputeRefactoringsForNodeAsync()
        {
            SyntaxNode node = Root.FindNode(Span, findInsideTrivia: false, getInnermostNodeForTie: true);

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
            bool fVariableDeclarator = false;
            bool fInterpolatedStringText = false;
            bool fElseClause = false;
            bool fCaseSwitchLabel = false;
            bool fUsingDirective = false;
            bool fDeclarationPattern = false;
            bool fTypeParameterConstraintClause = false;

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
            bool fThrowExpression = false;
            bool fDeclarationExpression = false;
            bool fIsPatternExpression = false;

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
            bool fLocalFunctionStatement = false;
            bool fUnsafeStatement = false;

            SyntaxNode firstNode = node;

            using (IEnumerator<SyntaxNode> en = node.AncestorsAndSelf().GetEnumerator())
            {
                while (en.MoveNext())
                {
                    node = en.Current;

                    SyntaxKind kind = node.Kind();

                    Debug.WriteLine(kind.ToString());

                    if (!fAccessor
                        && (node is AccessorDeclarationSyntax accessor))
                    {
                        AccessorDeclarationRefactoring.ComputeRefactorings(this, accessor);
                        fAccessor = true;
                        continue;
                    }

                    if (!fArgument
                        && kind == SyntaxKind.Argument)
                    {
                        await ArgumentRefactoring.ComputeRefactoringsAsync(this, (ArgumentSyntax)node).ConfigureAwait(false);
                        fArgument = true;
                        continue;
                    }

                    if (!fArgumentList
                        && kind == SyntaxKind.ArgumentList)
                    {
                        await ArgumentListRefactoring.ComputeRefactoringsAsync(this, (ArgumentListSyntax)node).ConfigureAwait(false);
                        fArgumentList = true;
                        continue;
                    }

                    if (!fAttributeArgumentList
                        && kind == SyntaxKind.AttributeArgumentList)
                    {
                        await AttributeArgumentListRefactoring.ComputeRefactoringsAsync(this, (AttributeArgumentListSyntax)node).ConfigureAwait(false);
                        fAttributeArgumentList = true;
                        continue;
                    }

                    if (!fArrowExpressionClause
                        && kind == SyntaxKind.ArrowExpressionClause)
                    {
                        await ArrowExpressionClauseRefactoring.ComputeRefactoringsAsync(this, (ArrowExpressionClauseSyntax)node).ConfigureAwait(false);
                        fArrowExpressionClause = true;
                        continue;
                    }

                    if (!fParameter
                        && kind == SyntaxKind.Parameter)
                    {
                        await ParameterRefactoring.ComputeRefactoringsAsync(this, (ParameterSyntax)node).ConfigureAwait(false);
                        fParameter = true;
                        continue;
                    }

                    if (!fParameterList
                        && kind == SyntaxKind.ParameterList)
                    {
                        await ParameterListRefactoring.ComputeRefactoringsAsync(this, (ParameterListSyntax)node).ConfigureAwait(false);
                        fParameterList = true;
                        continue;
                    }

                    if (!fSwitchSection
                        && kind == SyntaxKind.SwitchSection)
                    {
                        await SwitchSectionRefactoring.ComputeRefactoringsAsync(this, (SwitchSectionSyntax)node).ConfigureAwait(false);
                        fSwitchSection = true;
                        continue;
                    }

                    if (!fVariableDeclaration
                        && kind == SyntaxKind.VariableDeclaration)
                    {
                        await VariableDeclarationRefactoring.ComputeRefactoringsAsync(this, (VariableDeclarationSyntax)node).ConfigureAwait(false);
                        fVariableDeclaration = true;
                        continue;
                    }

                    if (!fVariableDeclarator
                        && kind == SyntaxKind.VariableDeclarator)
                    {
                        VariableDeclaratorRefactoring.ComputeRefactorings(this, (VariableDeclaratorSyntax)node);
                        fVariableDeclarator = true;
                        continue;
                    }

                    if (!fInterpolatedStringText
                        && kind == SyntaxKind.InterpolatedStringText)
                    {
                        await InterpolatedStringTextRefactoring.ComputeRefactoringsAsync(this, (InterpolatedStringTextSyntax)node).ConfigureAwait(false);
                        fInterpolatedStringText = true;
                        continue;
                    }

                    if (!fInterpolation
                        && kind == SyntaxKind.Interpolation)
                    {
                        InterpolationRefactoring.ComputeRefactorings(this, (InterpolationSyntax)node);
                        fInterpolation = true;
                        continue;
                    }

                    if (!fElseClause
                        && kind == SyntaxKind.ElseClause)
                    {
                        ElseClauseRefactoring.ComputeRefactorings(this, (ElseClauseSyntax)node);
                        fElseClause = true;
                        continue;
                    }

                    if (!fCaseSwitchLabel
                        && kind == SyntaxKind.CaseSwitchLabel)
                    {
                        await CaseSwitchLabelRefactoring.ComputeRefactoringsAsync(this, (CaseSwitchLabelSyntax)node).ConfigureAwait(false);
                        fCaseSwitchLabel = true;
                        continue;
                    }

                    if (!fUsingDirective
                        && kind == SyntaxKind.UsingDirective)
                    {
                        UsingDirectiveRefactoring.ComputeRefactoring(this, (UsingDirectiveSyntax)node);
                        fUsingDirective = true;
                        continue;
                    }

                    if (!fDeclarationPattern
                        && kind == SyntaxKind.DeclarationPattern)
                    {
                        await DeclarationPatternRefactoring.ComputeRefactoringAsync(this, (DeclarationPatternSyntax)node).ConfigureAwait(false);
                        fDeclarationPattern = true;
                        continue;
                    }

                    if (!fTypeParameterConstraintClause
                        && kind == SyntaxKind.TypeParameterConstraintClause)
                    {
                        TypeParameterConstraintClauseRefactoring.ComputeRefactoring(this, (TypeParameterConstraintClauseSyntax)node);
                        fTypeParameterConstraintClause = true;
                        continue;
                    }

                    if (node is ExpressionSyntax expression)
                    {
                        if (!fExpression)
                        {
                            await ExpressionRefactoring.ComputeRefactoringsAsync(this, expression).ConfigureAwait(false);
                            fExpression = true;
                        }

                        if (!fAssignmentExpression
                            && (node is AssignmentExpressionSyntax assignmentExpression))
                        {
                            await AssignmentExpressionRefactoring.ComputeRefactoringsAsync(this, assignmentExpression).ConfigureAwait(false);
                            fAssignmentExpression = true;
                        }

                        if (!fAnonymousMethod
                            && kind == SyntaxKind.AnonymousMethodExpression)
                        {
                            AnonymousMethodExpressionRefactoring.ComputeRefactorings(this, (AnonymousMethodExpressionSyntax)node);
                            fAnonymousMethod = true;
                        }

                        if (!fBinaryExpression
                            && (node is BinaryExpressionSyntax binaryExpression))
                        {
                            await BinaryExpressionRefactoring.ComputeRefactoringsAsync(this, binaryExpression).ConfigureAwait(false);
                            fBinaryExpression = true;
                        }

                        if (!fConditionalExpression
                            && kind == SyntaxKind.ConditionalExpression)
                        {
                            await ConditionalExpressionRefactoring.ComputeRefactoringsAsync(this, (ConditionalExpressionSyntax)expression).ConfigureAwait(false);
                            fConditionalExpression = true;
                        }

                        if (!fQualifiedName
                            && kind == SyntaxKind.QualifiedName)
                        {
                            await QualifiedNameRefactoring.ComputeRefactoringsAsync(this, (QualifiedNameSyntax)expression).ConfigureAwait(false);
                            fQualifiedName = true;
                        }

                        if (!fGenericName
                            && kind == SyntaxKind.GenericName)
                        {
                            GenericNameRefactoring.ComputeRefactorings(this, (GenericNameSyntax)expression);
                            fGenericName = true;
                        }

                        if (!fIdentifierName
                            && kind == SyntaxKind.IdentifierName)
                        {
                            await IdentifierNameRefactoring.ComputeRefactoringsAsync(this, (IdentifierNameSyntax)expression).ConfigureAwait(false);
                            fIdentifierName = true;
                        }

                        if (!fInitializerExpression
                            && (node is InitializerExpressionSyntax initializer))
                        {
                            await InitializerExpressionRefactoring.ComputeRefactoringsAsync(this, initializer).ConfigureAwait(false);
                            fInitializerExpression = true;
                        }

                        if (!fInterpolatedStringExpression
                            && kind == SyntaxKind.InterpolatedStringExpression)
                        {
                            InterpolatedStringRefactoring.ComputeRefactorings(this, (InterpolatedStringExpressionSyntax)expression);
                            fInterpolatedStringExpression = true;
                        }

                        if (!fInvocationExpression
                            && kind == SyntaxKind.InvocationExpression)
                        {
                            await InvocationExpressionRefactoring.ComputeRefactoringsAsync(this, (InvocationExpressionSyntax)expression).ConfigureAwait(false);
                            fInvocationExpression = true;
                        }

                        if (!fLambdaExpression
                            && (node is LambdaExpressionSyntax lambdaExpression))
                        {
                            LambdaExpressionRefactoring.ComputeRefactorings(this, lambdaExpression);
                            fLambdaExpression = true;
                        }

                        if (!fLiteralExpression
                            && (node is LiteralExpressionSyntax literalExpression))
                        {
                            await LiteralExpressionRefactoring.ComputeRefactoringsAsync(this, literalExpression).ConfigureAwait(false);
                            fLiteralExpression = true;
                        }

                        if (!fSimpleMemberAccessExpression
                            && kind == SyntaxKind.SimpleMemberAccessExpression)
                        {
                            await SimpleMemberAccessExpressionRefactoring.ComputeRefactoringAsync(this, (MemberAccessExpressionSyntax)node).ConfigureAwait(false);
                            fSimpleMemberAccessExpression = true;
                        }

                        if (!fParenthesizedExpression
                            && kind == SyntaxKind.ParenthesizedExpression)
                        {
                            ParenthesizedExpressionRefactoring.ComputeRefactorings(this, (ParenthesizedExpressionSyntax)expression);
                            fParenthesizedExpression = true;
                        }

                        if (!fPostfixUnaryExpression
                            && (node is PostfixUnaryExpressionSyntax postfixUnaryExpression))
                        {
                            PostfixUnaryExpressionRefactoring.ComputeRefactorings(this, postfixUnaryExpression);
                            fPostfixUnaryExpression = true;
                        }

                        if (!fPrefixUnaryExpression
                            && (node is PrefixUnaryExpressionSyntax prefixUnaryExpression))
                        {
                            PrefixUnaryExpressionRefactoring.ComputeRefactorings(this, prefixUnaryExpression);
                            fPrefixUnaryExpression = true;
                        }

                        if (!fAwaitExpression
                            && kind == SyntaxKind.AwaitExpression)
                        {
                            await AwaitExpressionRefactoring.ComputeRefactoringsAsync(this, (AwaitExpressionSyntax)node).ConfigureAwait(false);
                            fAwaitExpression = true;
                        }

                        if (!fCastExpression
                            && kind == SyntaxKind.CastExpression)
                        {
                            CastExpressionRefactoring.ComputeRefactorings(this, (CastExpressionSyntax)node);
                            fCastExpression = true;
                        }

                        if (!fThrowExpression
                            && kind == SyntaxKind.ThrowExpression)
                        {
                            await ThrowExpressionRefactoring.ComputeRefactoringsAsync(this, (ThrowExpressionSyntax)node).ConfigureAwait(false);
                            fThrowExpression = true;
                        }

                        if (!fDeclarationExpression
                            && kind == SyntaxKind.DeclarationExpression)
                        {
                            await DeclarationExpressionRefactoring.ComputeRefactoringsAsync(this, (DeclarationExpressionSyntax)node).ConfigureAwait(false);
                            fDeclarationExpression = true;
                        }

                        if (!fIsPatternExpression
                            && kind == SyntaxKind.IsPatternExpression)
                        {
                            NegateIsExpressionRefactoring.ComputeRefactoring(this, (IsPatternExpressionSyntax)node);
                            fIsPatternExpression = true;
                        }

                        continue;
                    }

                    if (node is MemberDeclarationSyntax memberDeclaration)
                    {
                        if (!fMemberDeclaration)
                        {
                            await MemberDeclarationRefactoring.ComputeRefactoringsAsync(this, memberDeclaration).ConfigureAwait(false);
                            AttributeListRefactoring.ComputeRefactorings(this, memberDeclaration);
                            await IntroduceConstructorRefactoring.ComputeRefactoringsAsync(this, memberDeclaration).ConfigureAwait(false);
                            fMemberDeclaration = true;
                        }

                        continue;
                    }

                    if (node is StatementSyntax statement)
                    {
                        if (!fDoStatement
                            && kind == SyntaxKind.DoStatement)
                        {
                            DoStatementRefactoring.ComputeRefactorings(this, (DoStatementSyntax)statement);
                            fDoStatement = true;
                        }

                        if (!fExpressionStatement
                            && kind == SyntaxKind.ExpressionStatement)
                        {
                            await ExpressionStatementRefactoring.ComputeRefactoringsAsync(this, (ExpressionStatementSyntax)statement).ConfigureAwait(false);
                            fExpressionStatement = true;
                        }

                        if (!fForEachStatement
                            && kind == SyntaxKind.ForEachStatement)
                        {
                            await ForEachStatementRefactoring.ComputeRefactoringsAsync(this, (ForEachStatementSyntax)statement).ConfigureAwait(false);
                            fForEachStatement = true;
                        }

                        if (!fForStatement
                            && kind == SyntaxKind.ForStatement)
                        {
                            await ForStatementRefactoring.ComputeRefactoringsAsync(this, (ForStatementSyntax)statement).ConfigureAwait(false);
                            fForStatement = true;
                        }

                        if (!fIfStatement
                            && kind == SyntaxKind.IfStatement)
                        {
                            await IfStatementRefactoring.ComputeRefactoringsAsync(this, (IfStatementSyntax)statement).ConfigureAwait(false);
                            fIfStatement = true;
                        }

                        if (!fLocalDeclarationStatement
                            && kind == SyntaxKind.LocalDeclarationStatement)
                        {
                            await LocalDeclarationStatementRefactoring.ComputeRefactoringsAsync(this, (LocalDeclarationStatementSyntax)statement).ConfigureAwait(false);
                            fLocalDeclarationStatement = true;
                        }

                        if (!fReturnStatement
                            && kind == SyntaxKind.ReturnStatement)
                        {
                            await ReturnStatementRefactoring.ComputeRefactoringsAsync(this, (ReturnStatementSyntax)statement).ConfigureAwait(false);
                            fReturnStatement = true;
                        }

                        if (!fSwitchStatement
                            && kind == SyntaxKind.SwitchStatement)
                        {
                            await SwitchStatementRefactoring.ComputeRefactoringsAsync(this, (SwitchStatementSyntax)statement).ConfigureAwait(false);
                            fSwitchStatement = true;
                        }

                        if (!fUsingStatement
                            && kind == SyntaxKind.UsingStatement)
                        {
                            UsingStatementRefactoring.ComputeRefactorings(this, (UsingStatementSyntax)statement);
                            fUsingStatement = true;
                        }

                        if (!fWhileStatement
                            && kind == SyntaxKind.WhileStatement)
                        {
                            WhileStatementRefactoring.ComputeRefactorings(this, (WhileStatementSyntax)statement);
                            fWhileStatement = true;
                        }

                        if (!fYieldReturnStatement
                            && (node is YieldStatementSyntax yieldStatement))
                        {
                            await YieldStatementRefactoring.ComputeRefactoringsAsync(this, yieldStatement).ConfigureAwait(false);
                            fYieldReturnStatement = true;
                        }

                        if (!fLockStatement
                            && kind == SyntaxKind.LockStatement)
                        {
                            LockStatementRefactoring.ComputeRefactorings(this, (LockStatementSyntax)node);
                            fLockStatement = true;
                        }

                        if (!fBlock
                            && kind == SyntaxKind.Block)
                        {
                            await BlockRefactoring.ComputeRefactoringAsync(this, (BlockSyntax)node).ConfigureAwait(false);
                            fBlock = true;
                        }

                        if (!fThrowStatement
                            && kind == SyntaxKind.ThrowStatement)
                        {
                            await ThrowStatementRefactoring.ComputeRefactoringAsync(this, (ThrowStatementSyntax)node).ConfigureAwait(false);
                            fThrowStatement = true;
                        }

                        if (!fLocalFunctionStatement
                            && kind == SyntaxKind.LocalFunctionStatement)
                        {
                            await LocalFunctionStatementRefactoring.ComputeRefactoringsAsync(this, (LocalFunctionStatementSyntax)node).ConfigureAwait(false);
                            fLocalFunctionStatement = true;
                        }

                        if (!fUnsafeStatement
                            && kind == SyntaxKind.UnsafeStatement)
                        {
                            UnsafeStatementRefactoring.ComputeRefactorings(this, (UnsafeStatementSyntax)node);
                            fUnsafeStatement = true;
                        }

                        if (!fStatement)
                        {
                            AddBracesRefactoring.ComputeRefactoring(this, statement);
                            RemoveBracesRefactoring.ComputeRefactoring(this, statement);

                            if (IsRefactoringEnabled(RefactoringIdentifiers.ExtractStatement))
                                ExtractStatementRefactoring.ComputeRefactoring(this, statement);

                            EmbeddedStatementRefactoring.ComputeRefactoring(this, statement);
                            fStatement = true;
                        }

                        if (!fStatementRefactoring)
                        {
                            if (kind == SyntaxKind.Block)
                            {
                                StatementRefactoring.ComputeRefactoring(this, (BlockSyntax)node);
                                fStatementRefactoring = true;
                            }
                            else if (kind == SyntaxKind.SwitchStatement)
                            {
                                StatementRefactoring.ComputeRefactoring(this, (SwitchStatementSyntax)node);
                                fStatementRefactoring = true;
                            }
                        }
                    }
                }
            }

            await SyntaxNodeRefactoring.ComputeRefactoringsAsync(this, firstNode).ConfigureAwait(false);

            CommentTriviaRefactoring.ComputeRefactorings(this, firstNode);
        }

        public bool IsRefactoringEnabled(string identifier)
        {
            return Settings.IsRefactoringEnabled(identifier);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2)
        {
            return Settings.IsAnyRefactoringEnabled(identifier1, identifier2);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2, string identifier3)
        {
            return Settings.IsAnyRefactoringEnabled(identifier1, identifier2, identifier3);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2, string identifier3, string identifier4)
        {
            return Settings.IsAnyRefactoringEnabled(identifier1, identifier2, identifier3, identifier4);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2, string identifier3, string identifier4, string identifier5)
        {
            return Settings.IsAnyRefactoringEnabled(identifier1, identifier2, identifier3, identifier4, identifier5);
        }
    }
}
