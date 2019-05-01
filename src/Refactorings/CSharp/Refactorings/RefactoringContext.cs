// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
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

        public Workspace Workspace
        {
            get { return Solution.Workspace; }
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

        public void ThrowIfCancellationRequested()
        {
            CancellationToken.ThrowIfCancellationRequested();
        }

        public Task<SemanticModel> GetSemanticModelAsync()
        {
            return Document.GetSemanticModelAsync(CancellationToken);
        }

        public void RegisterRefactoring(
            string title,
            Func<CancellationToken, Task<Document>> createChangedDocument,
            string equivalenceKey)
        {
            RegisterRefactoring(CodeAction.Create(title, createChangedDocument, equivalenceKey));
        }

        public void RegisterRefactoring(
            string title,
            Func<CancellationToken, Task<Solution>> createChangedSolution,
            string equivalenceKey)
        {
            RegisterRefactoring(CodeAction.Create(title, createChangedSolution, equivalenceKey));
        }

        public void RegisterRefactoring(CodeAction codeAction)
        {
            UnderlyingContext.RegisterRefactoring(codeAction);
        }

        public bool IsRefactoringEnabled(string identifier)
        {
            return Settings.IsEnabled(identifier);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2)
        {
            return Settings.IsAnyEnabled(identifier1, identifier2);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2, string identifier3)
        {
            return Settings.IsAnyEnabled(identifier1, identifier2, identifier3);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2, string identifier3, string identifier4)
        {
            return Settings.IsAnyEnabled(identifier1, identifier2, identifier3, identifier4);
        }

        public bool IsAnyRefactoringEnabled(string identifier1, string identifier2, string identifier3, string identifier4, string identifier5)
        {
            return Settings.IsAnyEnabled(identifier1, identifier2, identifier3, identifier4, identifier5);
        }

        public async Task ComputeRefactoringsAsync()
        {
            ComputeRefactoringsForTriviaInsideTrivia();

            ComputeRefactoringsForTokenInsideTrivia();

            ComputeRefactoringsForNodeInsideTrivia();

            await ComputeRefactoringsForTokenAsync().ConfigureAwait(false);

            ComputeRefactoringsForTrivia();

            await ComputeRefactoringsForNodeAsync().ConfigureAwait(false);
        }

        public void ComputeRefactoringsForTriviaInsideTrivia()
        {
            SyntaxTrivia trivia = Root.FindTrivia(Span.Start, findInsideTrivia: true);

            if (trivia.IsPartOfStructuredTrivia())
                CommentTriviaRefactoring.ComputeRefactorings(this, trivia);
        }

        public void ComputeRefactoringsForTokenInsideTrivia()
        {
            SyntaxToken token = Root.FindToken(Span.Start, findInsideTrivia: true);

            switch (token.Kind())
            {
                case SyntaxKind.XmlTextLiteralToken:
                    {
                        if (IsRefactoringEnabled(RefactoringIdentifiers.AddTagToDocumentationComment)
                            && !Span.IsEmpty)
                        {
                            TextSpan span = token.Span;

                            if (span.Contains(Span))
                            {
                                string text = token.Text;

                                if (span.Start == Span.Start
                                    || (!char.IsLetterOrDigit(text[Span.Start - span.Start - 1])
                                        && char.IsLetterOrDigit(text[Span.Start - span.Start])))
                                {
                                    if (span.End == Span.End
                                        || (char.IsLetterOrDigit(text[Span.End - span.Start - 1])
                                            && !char.IsLetterOrDigit(text[Span.End - span.Start])))
                                    {
                                        RegisterRefactoring(
                                            "Add tag 'c'",
                                            ct => Document.WithTextChangeAsync(new TextChange(Span, $"<c>{token.ToString(Span)}</c>"), ct),
                                            RefactoringIdentifiers.AddTagToDocumentationComment);
                                    }
                                }
                            }
                        }

                        break;
                    }
            }
        }

        public void ComputeRefactoringsForNodeInsideTrivia()
        {
            SyntaxNode node = Root.FindNode(Span, findInsideTrivia: true, getInnermostNodeForTie: true);

            for (; node != null; node = node.Parent)
            {
                if (node is DirectiveTriviaSyntax directiveTrivia)
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

                    break;
                }
            }
        }

        public async Task ComputeRefactoringsForTokenAsync()
        {
            SyntaxToken token = Root.FindToken(Span.Start);

            if (!token.Span.Contains(Span))
                return;

            switch (token.Kind())
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
                        if (IsRefactoringEnabled(RefactoringIdentifiers.ChangeAccessibility))
                            await AccessModifierRefactoring.ComputeRefactoringsAsync(this, token).ConfigureAwait(false);

                        break;
                    }
                case SyntaxKind.AsyncKeyword:
                    {
                        if (IsRefactoringEnabled(RefactoringIdentifiers.RemoveAsyncAwait)
                            && Span.IsEmptyAndContainedInSpan(token))
                        {
                            await RemoveAsyncAwaitRefactoring.ComputeRefactoringsAsync(this, token).ConfigureAwait(false);
                        }

                        break;
                    }
            }
        }

        public void ComputeRefactoringsForTrivia()
        {
            SyntaxTrivia trivia = Root.FindTrivia(Span.Start);

            SyntaxKind kind = trivia.Kind();

            if (kind == SyntaxKind.SingleLineCommentTrivia)
            {
                if (IsRefactoringEnabled(RefactoringIdentifiers.UncommentSingleLineComment))
                {
                    RegisterRefactoring(
                        "Uncomment",
                        cancellationToken => UncommentSingleLineCommentRefactoring.RefactorAsync(Document, trivia, cancellationToken),
                        RefactoringIdentifiers.UncommentSingleLineComment);
                }

                if (IsRefactoringEnabled(RefactoringIdentifiers.ConvertCommentToDocumentationComment))
                {
                    TextSpan fixableSpan = ConvertCommentToDocumentationCommentAnalysis.GetFixableSpan(trivia);

                    if (!fixableSpan.IsEmpty)
                    {
                        RegisterRefactoring(
                            ConvertCommentToDocumentationCommentRefactoring.Title,
                            cancellationToken => ConvertCommentToDocumentationCommentRefactoring.RefactorAsync(Document, (MemberDeclarationSyntax)trivia.Token.Parent, fixableSpan, cancellationToken),
                            RefactoringIdentifiers.ConvertCommentToDocumentationComment);
                    }
                }
            }
            else if (kind == SyntaxKind.MultiLineCommentTrivia)
            {
                if (IsRefactoringEnabled(RefactoringIdentifiers.UncommentMultiLineComment))
                    UncommentMultiLineCommentRefactoring.ComputeRefactoring(this, trivia);
            }

            if (!trivia.IsPartOfStructuredTrivia())
                CommentTriviaRefactoring.ComputeRefactorings(this, trivia);
        }

        public async Task ComputeRefactoringsForNodeAsync()
        {
            SyntaxNode node = Root.FindNode(Span, findInsideTrivia: false, getInnermostNodeForTie: true);

            if (node == null)
                return;

            RefactoringFlags flags = RefactoringFlagsCache.GetInstance();

            SyntaxNode firstNode = node;

            for (; node != null; node = node.GetParent(ascendOutOfTrivia: true))
            {
                SyntaxKind kind = node.Kind();

                if (!flags.IsSet(Flag.Expression)
                    && node is ExpressionSyntax expression)
                {
                    await ExpressionRefactoring.ComputeRefactoringsAsync(this, expression).ConfigureAwait(false);
                    flags.Set(Flag.Expression);
                }

                if (!flags.IsSet(Flag.MemberDeclaration)
                    && node is MemberDeclarationSyntax memberDeclaration)
                {
                    await MemberDeclarationRefactoring.ComputeRefactoringsAsync(this, memberDeclaration).ConfigureAwait(false);
                    AttributeListRefactoring.ComputeRefactorings(this, memberDeclaration);
                    await IntroduceConstructorRefactoring.ComputeRefactoringsAsync(this, memberDeclaration).ConfigureAwait(false);
                    flags.Set(Flag.MemberDeclaration);
                }

                switch (kind)
                {
                    case SyntaxKind.AddAccessorDeclaration:
                    case SyntaxKind.RemoveAccessorDeclaration:
                    case SyntaxKind.GetAccessorDeclaration:
                    case SyntaxKind.SetAccessorDeclaration:
                    case SyntaxKind.UnknownAccessorDeclaration:
                        {
                            if (flags.IsSet(Flag.Accessor))
                                continue;

                            AccessorDeclarationRefactoring.ComputeRefactorings(this, (AccessorDeclarationSyntax)node);
                            flags.Set(Flag.Accessor);
                            continue;
                        }
                    case SyntaxKind.Argument:
                        {
                            if (flags.IsSet(Flag.Argument))
                                continue;

                            await ArgumentRefactoring.ComputeRefactoringsAsync(this, (ArgumentSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.Argument);
                            continue;
                        }
                    case SyntaxKind.ArgumentList:
                        {
                            if (flags.IsSet(Flag.ArgumentList))
                                continue;

                            await ArgumentListRefactoring.ComputeRefactoringsAsync(this, (ArgumentListSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ArgumentList);
                            continue;
                        }
                    case SyntaxKind.AttributeArgumentList:
                        {
                            if (flags.IsSet(Flag.AttributeArgumentList))
                                continue;

                            await AttributeArgumentListRefactoring.ComputeRefactoringsAsync(this, (AttributeArgumentListSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.AttributeArgumentList);
                            continue;
                        }
                    case SyntaxKind.ArrowExpressionClause:
                        {
                            if (flags.IsSet(Flag.ArrowExpressionClause))
                                continue;

                            await ArrowExpressionClauseRefactoring.ComputeRefactoringsAsync(this, (ArrowExpressionClauseSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ArrowExpressionClause);
                            continue;
                        }
                    case SyntaxKind.Parameter:
                        {
                            if (flags.IsSet(Flag.Parameter))
                                continue;

                            await ParameterRefactoring.ComputeRefactoringsAsync(this, (ParameterSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.Parameter);
                            continue;
                        }
                    case SyntaxKind.ParameterList:
                        {
                            if (flags.IsSet(Flag.ParameterList))
                                continue;

                            await ParameterListRefactoring.ComputeRefactoringsAsync(this, (ParameterListSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ParameterList);
                            continue;
                        }
                    case SyntaxKind.SwitchSection:
                        {
                            if (flags.IsSet(Flag.SwitchSection))
                                continue;

                            await SwitchSectionRefactoring.ComputeRefactoringsAsync(this, (SwitchSectionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.SwitchSection);
                            continue;
                        }
                    case SyntaxKind.VariableDeclaration:
                        {
                            if (flags.IsSet(Flag.VariableDeclaration))
                                continue;

                            await VariableDeclarationRefactoring.ComputeRefactoringsAsync(this, (VariableDeclarationSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.VariableDeclaration);
                            continue;
                        }
                    case SyntaxKind.VariableDeclarator:
                        {
                            if (flags.IsSet(Flag.VariableDeclarator))
                                continue;

                            await VariableDeclaratorRefactoring.ComputeRefactoringsAsync(this, (VariableDeclaratorSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.VariableDeclarator);
                            continue;
                        }
                    case SyntaxKind.InterpolatedStringText:
                        {
                            if (flags.IsSet(Flag.InterpolatedStringText))
                                continue;

                            await InterpolatedStringTextRefactoring.ComputeRefactoringsAsync(this, (InterpolatedStringTextSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.InterpolatedStringText);
                            continue;
                        }
                    case SyntaxKind.Interpolation:
                        {
                            if (flags.IsSet(Flag.Interpolation))
                                continue;

                            InterpolationRefactoring.ComputeRefactorings(this, (InterpolationSyntax)node);
                            flags.Set(Flag.Interpolation);
                            continue;
                        }
                    case SyntaxKind.ElseClause:
                        {
                            if (flags.IsSet(Flag.ElseClause))
                                continue;

                            ElseClauseRefactoring.ComputeRefactorings(this, (ElseClauseSyntax)node);
                            flags.Set(Flag.ElseClause);
                            continue;
                        }
                    case SyntaxKind.CaseSwitchLabel:
                        {
                            if (flags.IsSet(Flag.CaseSwitchLabel))
                                continue;

                            await CaseSwitchLabelRefactoring.ComputeRefactoringsAsync(this, (CaseSwitchLabelSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.CaseSwitchLabel);
                            continue;
                        }
                    case SyntaxKind.UsingDirective:
                        {
                            if (flags.IsSet(Flag.UsingDirective))
                                continue;

                            UsingDirectiveRefactoring.ComputeRefactoring(this, (UsingDirectiveSyntax)node);
                            flags.Set(Flag.UsingDirective);
                            continue;
                        }
                    case SyntaxKind.DeclarationPattern:
                        {
                            if (flags.IsSet(Flag.DeclarationPattern))
                                continue;

                            await DeclarationPatternRefactoring.ComputeRefactoringAsync(this, (DeclarationPatternSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.DeclarationPattern);
                            continue;
                        }
                    case SyntaxKind.TypeParameterConstraintClause:
                        {
                            if (flags.IsSet(Flag.TypeParameterConstraintClause))
                                continue;

                            TypeParameterConstraintClauseRefactoring.ComputeRefactoring(this, (TypeParameterConstraintClauseSyntax)node);
                            flags.Set(Flag.TypeParameterConstraintClause);
                            continue;
                        }
                    case SyntaxKind.Attribute:
                        {
                            if (flags.IsSet(Flag.Attribute))
                                continue;

                            await AttributeRefactoring.ComputeRefactoringAsync(this, (AttributeSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.Attribute);
                            continue;
                        }
                    case SyntaxKind.SimpleAssignmentExpression:
                    case SyntaxKind.AddAssignmentExpression:
                    case SyntaxKind.SubtractAssignmentExpression:
                    case SyntaxKind.MultiplyAssignmentExpression:
                    case SyntaxKind.DivideAssignmentExpression:
                    case SyntaxKind.ModuloAssignmentExpression:
                    case SyntaxKind.AndAssignmentExpression:
                    case SyntaxKind.ExclusiveOrAssignmentExpression:
                    case SyntaxKind.OrAssignmentExpression:
                    case SyntaxKind.LeftShiftAssignmentExpression:
                    case SyntaxKind.RightShiftAssignmentExpression:
                        {
                            if (flags.IsSet(Flag.AssignmentExpression))
                                continue;

                            await AssignmentExpressionRefactoring.ComputeRefactoringsAsync(this, (AssignmentExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.AssignmentExpression);
                            continue;
                        }
                    case SyntaxKind.AnonymousMethodExpression:
                        {
                            if (flags.IsSet(Flag.AnonymousMethod))
                                continue;

                            AnonymousMethodExpressionRefactoring.ComputeRefactorings(this, (AnonymousMethodExpressionSyntax)node);
                            flags.Set(Flag.AnonymousMethod);
                            continue;
                        }
                    case SyntaxKind.AddExpression:
                    case SyntaxKind.SubtractExpression:
                    case SyntaxKind.MultiplyExpression:
                    case SyntaxKind.DivideExpression:
                    case SyntaxKind.ModuloExpression:
                    case SyntaxKind.LeftShiftExpression:
                    case SyntaxKind.RightShiftExpression:
                    case SyntaxKind.LogicalOrExpression:
                    case SyntaxKind.LogicalAndExpression:
                    case SyntaxKind.BitwiseOrExpression:
                    case SyntaxKind.BitwiseAndExpression:
                    case SyntaxKind.ExclusiveOrExpression:
                    case SyntaxKind.EqualsExpression:
                    case SyntaxKind.NotEqualsExpression:
                    case SyntaxKind.LessThanExpression:
                    case SyntaxKind.LessThanOrEqualExpression:
                    case SyntaxKind.GreaterThanExpression:
                    case SyntaxKind.GreaterThanOrEqualExpression:
                    case SyntaxKind.IsExpression:
                    case SyntaxKind.AsExpression:
                    case SyntaxKind.CoalesceExpression:
                        {
                            if (flags.IsSet(Flag.BinaryExpression))
                                continue;

                            await BinaryExpressionRefactoring.ComputeRefactoringsAsync(this, (BinaryExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.BinaryExpression);
                            continue;
                        }
                    case SyntaxKind.ConditionalExpression:
                        {
                            if (flags.IsSet(Flag.ConditionalExpression))
                                continue;

                            await ConditionalExpressionRefactoring.ComputeRefactoringsAsync(this, (ConditionalExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ConditionalExpression);
                            continue;
                        }
                    case SyntaxKind.QualifiedName:
                        {
                            if (flags.IsSet(Flag.QualifiedName))
                                continue;

                            await QualifiedNameRefactoring.ComputeRefactoringsAsync(this, (QualifiedNameSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.QualifiedName);
                            continue;
                        }
                    case SyntaxKind.GenericName:
                        {
                            if (flags.IsSet(Flag.GenericName))
                                continue;

                            GenericNameRefactoring.ComputeRefactorings(this, (GenericNameSyntax)node);
                            flags.Set(Flag.GenericName);
                            continue;
                        }
                    case SyntaxKind.IdentifierName:
                        {
                            if (flags.IsSet(Flag.IdentifierName))
                                continue;

                            await IdentifierNameRefactoring.ComputeRefactoringsAsync(this, (IdentifierNameSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.IdentifierName);
                            continue;
                        }
                    case SyntaxKind.ArrayInitializerExpression:
                    case SyntaxKind.CollectionInitializerExpression:
                    case SyntaxKind.ComplexElementInitializerExpression:
                    case SyntaxKind.ObjectInitializerExpression:
                        {
                            if (flags.IsSet(Flag.InitializerExpression))
                                continue;

                            await InitializerExpressionRefactoring.ComputeRefactoringsAsync(this, (InitializerExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.InitializerExpression);
                            continue;
                        }
                    case SyntaxKind.InterpolatedStringExpression:
                        {
                            if (flags.IsSet(Flag.InterpolatedStringExpression))
                                continue;

                            InterpolatedStringRefactoring.ComputeRefactorings(this, (InterpolatedStringExpressionSyntax)node);
                            flags.Set(Flag.InterpolatedStringExpression);
                            continue;
                        }
                    case SyntaxKind.InvocationExpression:
                        {
                            if (flags.IsSet(Flag.InvocationExpression))
                                continue;

                            await InvocationExpressionRefactoring.ComputeRefactoringsAsync(this, (InvocationExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.InvocationExpression);
                            continue;
                        }
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        {
                            if (flags.IsSet(Flag.LambdaExpression))
                                continue;

                            await LambdaExpressionRefactoring.ComputeRefactoringsAsync(this, (LambdaExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.LambdaExpression);
                            continue;
                        }
                    case SyntaxKind.CharacterLiteralExpression:
                    case SyntaxKind.DefaultLiteralExpression:
                    case SyntaxKind.FalseLiteralExpression:
                    case SyntaxKind.NullLiteralExpression:
                    case SyntaxKind.NumericLiteralExpression:
                    case SyntaxKind.StringLiteralExpression:
                    case SyntaxKind.TrueLiteralExpression:
                        {
                            if (flags.IsSet(Flag.LiteralExpression))
                                continue;

                            await LiteralExpressionRefactoring.ComputeRefactoringsAsync(this, (LiteralExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.LiteralExpression);
                            continue;
                        }
                    case SyntaxKind.SimpleMemberAccessExpression:
                        {
                            if (flags.IsSet(Flag.SimpleMemberAccessExpression))
                                continue;

                            await SimpleMemberAccessExpressionRefactoring.ComputeRefactoringAsync(this, (MemberAccessExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.SimpleMemberAccessExpression);
                            continue;
                        }
                    case SyntaxKind.ConditionalAccessExpression:
                        {
                            if (flags.IsSet(Flag.ConditionalAccessExpression))
                                continue;

                            await ConditionalAccessExpressionRefactoring.ComputeRefactoringAsync(this, (ConditionalAccessExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ConditionalAccessExpression);
                            continue;
                        }
                    case SyntaxKind.ParenthesizedExpression:
                        {
                            if (flags.IsSet(Flag.ParenthesizedExpression))
                                continue;

                            ParenthesizedExpressionRefactoring.ComputeRefactorings(this, (ParenthesizedExpressionSyntax)node);
                            flags.Set(Flag.ParenthesizedExpression);
                            continue;
                        }
                    case SyntaxKind.PostDecrementExpression:
                    case SyntaxKind.PostIncrementExpression:
                        {
                            if (flags.IsSet(Flag.PostfixUnaryExpression))
                                continue;

                            PostfixUnaryExpressionRefactoring.ComputeRefactorings(this, (PostfixUnaryExpressionSyntax)node);
                            flags.Set(Flag.PostfixUnaryExpression);
                            continue;
                        }
                    case SyntaxKind.UnaryPlusExpression:
                    case SyntaxKind.UnaryMinusExpression:
                    case SyntaxKind.BitwiseNotExpression:
                    case SyntaxKind.LogicalNotExpression:
                    case SyntaxKind.PreIncrementExpression:
                    case SyntaxKind.PreDecrementExpression:
                    case SyntaxKind.AddressOfExpression:
                    case SyntaxKind.PointerIndirectionExpression:
                        {
                            if (flags.IsSet(Flag.PrefixUnaryExpression))
                                continue;

                            PrefixUnaryExpressionRefactoring.ComputeRefactorings(this, (PrefixUnaryExpressionSyntax)node);
                            flags.Set(Flag.PrefixUnaryExpression);
                            continue;
                        }
                    case SyntaxKind.AwaitExpression:
                        {
                            if (flags.IsSet(Flag.AwaitExpression))
                                continue;

                            await AwaitExpressionRefactoring.ComputeRefactoringsAsync(this, (AwaitExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.AwaitExpression);
                            continue;
                        }
                    case SyntaxKind.CastExpression:
                        {
                            if (flags.IsSet(Flag.CastExpression))
                                continue;

                            CastExpressionRefactoring.ComputeRefactorings(this, (CastExpressionSyntax)node);
                            flags.Set(Flag.CastExpression);
                            continue;
                        }
                    case SyntaxKind.ThrowExpression:
                        {
                            if (flags.IsSet(Flag.ThrowExpression))
                                continue;

                            await ThrowExpressionRefactoring.ComputeRefactoringsAsync(this, (ThrowExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ThrowExpression);
                            continue;
                        }
                    case SyntaxKind.DeclarationExpression:
                        {
                            if (flags.IsSet(Flag.DeclarationExpression))
                                continue;

                            await DeclarationExpressionRefactoring.ComputeRefactoringsAsync(this, (DeclarationExpressionSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.DeclarationExpression);
                            continue;
                        }
                    case SyntaxKind.IsPatternExpression:
                        {
                            if (flags.IsSet(Flag.IsPatternExpression))
                                continue;

                            InvertIsExpressionRefactoring.ComputeRefactoring(this, (IsPatternExpressionSyntax)node);
                            flags.Set(Flag.IsPatternExpression);
                            continue;
                        }
                    case SyntaxKind.DoStatement:
                        {
                            if (flags.IsSet(Flag.LoopStatement))
                                break;

                            DoStatementRefactoring.ComputeRefactorings(this, (DoStatementSyntax)node);
                            flags.Set(Flag.LoopStatement);
                            break;
                        }
                    case SyntaxKind.ExpressionStatement:
                        {
                            if (flags.IsSet(Flag.ExpressionStatement))
                                break;

                            await ExpressionStatementRefactoring.ComputeRefactoringsAsync(this, (ExpressionStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ExpressionStatement);
                            break;
                        }
                    case SyntaxKind.ForEachStatement:
                        {
                            if (flags.IsSet(Flag.LoopStatement))
                                break;

                            await ForEachStatementRefactoring.ComputeRefactoringsAsync(this, (ForEachStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.LoopStatement);
                            break;
                        }
                    case SyntaxKind.ForStatement:
                        {
                            if (flags.IsSet(Flag.LoopStatement))
                                break;

                            await ForStatementRefactoring.ComputeRefactoringsAsync(this, (ForStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.LoopStatement);
                            break;
                        }
                    case SyntaxKind.IfStatement:
                        {
                            if (flags.IsSet(Flag.IfStatement))
                                break;

                            await IfStatementRefactoring.ComputeRefactoringsAsync(this, (IfStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.IfStatement);
                            break;
                        }
                    case SyntaxKind.LocalDeclarationStatement:
                        {
                            if (flags.IsSet(Flag.LocalDeclarationStatement))
                                break;

                            await LocalDeclarationStatementRefactoring.ComputeRefactoringsAsync(this, (LocalDeclarationStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.LocalDeclarationStatement);
                            break;
                        }
                    case SyntaxKind.ReturnStatement:
                        {
                            if (flags.IsSet(Flag.ReturnStatement))
                                break;

                            await ReturnStatementRefactoring.ComputeRefactoringsAsync(this, (ReturnStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ReturnStatement);
                            break;
                        }
                    case SyntaxKind.SwitchStatement:
                        {
                            if (flags.IsSet(Flag.SwitchStatement))
                                break;

                            await SwitchStatementRefactoring.ComputeRefactoringsAsync(this, (SwitchStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.SwitchStatement);
                            break;
                        }
                    case SyntaxKind.UsingStatement:
                        {
                            if (flags.IsSet(Flag.UsingStatement))
                                break;

                            UsingStatementRefactoring.ComputeRefactorings(this, (UsingStatementSyntax)node);
                            flags.Set(Flag.UsingStatement);
                            break;
                        }
                    case SyntaxKind.WhileStatement:
                        {
                            if (flags.IsSet(Flag.LoopStatement))
                                break;

                            WhileStatementRefactoring.ComputeRefactorings(this, (WhileStatementSyntax)node);
                            flags.Set(Flag.LoopStatement);
                            break;
                        }
                    case SyntaxKind.YieldBreakStatement:
                    case SyntaxKind.YieldReturnStatement:
                        {
                            if (flags.IsSet(Flag.YieldStatement))
                                break;

                            await YieldStatementRefactoring.ComputeRefactoringsAsync(this, (YieldStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.YieldStatement);
                            break;
                        }
                    case SyntaxKind.LockStatement:
                        {
                            if (flags.IsSet(Flag.LockStatement))
                                break;

                            LockStatementRefactoring.ComputeRefactorings(this, (LockStatementSyntax)node);
                            flags.Set(Flag.LockStatement);
                            break;
                        }
                    case SyntaxKind.Block:
                        {
                            if (flags.IsSet(Flag.Block))
                                break;

                            await BlockRefactoring.ComputeRefactoringAsync(this, (BlockSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.Block);
                            break;
                        }
                    case SyntaxKind.ThrowStatement:
                        {
                            if (flags.IsSet(Flag.ThrowStatement))
                                break;

                            await ThrowStatementRefactoring.ComputeRefactoringAsync(this, (ThrowStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.ThrowStatement);
                            break;
                        }
                    case SyntaxKind.LocalFunctionStatement:
                        {
                            if (flags.IsSet(Flag.LocalFunctionStatement))
                                break;

                            await LocalFunctionStatementRefactoring.ComputeRefactoringsAsync(this, (LocalFunctionStatementSyntax)node).ConfigureAwait(false);
                            flags.Set(Flag.LocalFunctionStatement);
                            break;
                        }
                    case SyntaxKind.UnsafeStatement:
                        {
                            if (flags.IsSet(Flag.UnsafeStatement))
                                break;

                            UnsafeStatementRefactoring.ComputeRefactorings(this, (UnsafeStatementSyntax)node);
                            flags.Set(Flag.UnsafeStatement);
                            break;
                        }
                }

                if (!flags.IsSet(Flag.Statement)
                    && node is StatementSyntax statement)
                {
                    AddBracesRefactoring.ComputeRefactoring(this, statement);
                    RemoveBracesRefactoring.ComputeRefactoring(this, statement);

                    if (IsRefactoringEnabled(RefactoringIdentifiers.ExtractStatement))
                        ExtractStatementRefactoring.ComputeRefactoring(this, statement);

                    EmbeddedStatementRefactoring.ComputeRefactoring(this, statement);
                    flags.Set(Flag.Statement);
                }

                if (!flags.IsSet(Flag.BlockOrSwitchStatement))
                {
                    if (kind == SyntaxKind.Block)
                    {
                        StatementRefactoring.ComputeRefactoring(this, (BlockSyntax)node);
                        flags.Set(Flag.BlockOrSwitchStatement);
                    }
                    else if (kind == SyntaxKind.SwitchStatement)
                    {
                        StatementRefactoring.ComputeRefactoring(this, (SwitchStatementSyntax)node);
                        flags.Set(Flag.BlockOrSwitchStatement);
                    }
                }
            }

            RefactoringFlagsCache.Free(flags);

            await SelectedLinesRefactoring.ComputeRefactoringsAsync(this, firstNode).ConfigureAwait(false);

            CommentTriviaRefactoring.ComputeRefactorings(this, firstNode);
        }

        private class RefactoringFlags
        {
            private readonly BitArray _flags;

            public RefactoringFlags()
            {
                _flags = new BitArray((int)Flag.Count);
            }

            public bool IsSet(Flag flag)
            {
                return _flags.Get((int)flag);
            }

            public void Set(Flag flag)
            {
                _flags.Set((int)flag, true);
            }

            public void Reset()
            {
                _flags.SetAll(false);
            }
        }

        private static class RefactoringFlagsCache
        {
            [ThreadStatic]
            private static RefactoringFlags _cachedInstance;

            public static RefactoringFlags GetInstance()
            {
                RefactoringFlags instance = _cachedInstance;

                if (instance != null)
                {
                    _cachedInstance = null;
                    instance.Reset();
                }
                else
                {
                    instance = new RefactoringFlags();
                }

                return instance;
            }

            public static void Free(RefactoringFlags instance)
            {
                _cachedInstance = instance;
            }
        }

        private enum Flag
        {
            None = 0,
            Accessor = 1,
            Argument = 2,
            ArgumentList = 3,
            AttributeArgumentList = 4,
            ArrowExpressionClause = 5,
            Parameter = 6,
            ParameterList = 7,
            SwitchSection = 8,
            VariableDeclaration = 9,
            VariableDeclarator = 10,
            InterpolatedStringText = 11,
            ElseClause = 12,
            CaseSwitchLabel = 13,
            UsingDirective = 14,
            DeclarationPattern = 15,
            TypeParameterConstraintClause = 16,
            Attribute = 17,

            Expression = 18,
            AnonymousMethod = 19,
            AssignmentExpression = 20,
            BinaryExpression = 21,
            ConditionalExpression = 22,
            QualifiedName = 23,
            GenericName = 24,
            IdentifierName = 25,
            InitializerExpression = 26,
            InterpolatedStringExpression = 27,
            Interpolation = 28,
            InvocationExpression = 29,
            LambdaExpression = 30,
            LiteralExpression = 31,
            SimpleMemberAccessExpression = 32,
            ConditionalAccessExpression = 33,
            ParenthesizedExpression = 34,
            PostfixUnaryExpression = 35,
            PrefixUnaryExpression = 36,
            AwaitExpression = 37,
            CastExpression = 38,
            ThrowExpression = 39,
            DeclarationExpression = 40,
            IsPatternExpression = 41,

            MemberDeclaration = 42,

            Statement = 43,
            ExpressionStatement = 44,
            LoopStatement = 45,
            IfStatement = 46,
            LocalDeclarationStatement = 47,
            ReturnStatement = 48,
            SwitchStatement = 49,
            UsingStatement = 50,
            YieldStatement = 51,
            LockStatement = 52,
            Block = 53,
            BlockOrSwitchStatement = 54,
            ThrowStatement = 55,
            LocalFunctionStatement = 56,
            UnsafeStatement = 57,

            Count = 58,
        }
    }
}
