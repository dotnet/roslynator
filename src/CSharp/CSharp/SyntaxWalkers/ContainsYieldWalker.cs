// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal class ContainsYieldWalker : SkipFunctionWalker
    {
        private bool _success;
        private readonly TextSpan? _span;

        protected ContainsYieldWalker(TextSpan? span = null)
        {
            _span = span;
        }

        public virtual bool IsSearchingForYieldReturn
        {
            get { return true; }
        }

        public virtual bool IsSearchingForYieldBreak
        {
            get { return true; }
        }

        private static ContainsYieldWalker Create(TextSpan? span = null, bool yieldReturn = true, bool yieldBreak = true)
        {
            if (yieldReturn)
            {
                if (yieldBreak)
                {
                    return new ContainsYieldWalker(span);
                }
                else
                {
                    return new ContainsYieldReturnWalker(span);
                }
            }
            else if (yieldBreak)
            {
                return new ContainsYieldBreakWalker(span);
            }

            throw new InvalidOperationException();
        }

        public static bool ContainsYieldReturn(SyntaxNode node, TextSpan? span = null)
        {
            return ContainsYield(node, span, yieldReturn: true, yieldBreak: false);
        }

        public static bool ContainsYieldBreak(SyntaxNode node, TextSpan? span = null)
        {
            return ContainsYield(node, span, yieldReturn: false, yieldBreak: true);
        }

        public static bool ContainsYield(SyntaxNode node, TextSpan? span = null, bool yieldReturn = true, bool yieldBreak = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Kind() == SyntaxKind.LocalFunctionStatement)
                return ContainsYield((LocalFunctionStatementSyntax)node, span, yieldReturn, yieldBreak);

            ContainsYieldWalker walker = Create(span, yieldReturn, yieldBreak);

            walker.Visit(node);

            return walker._success;
        }

        public static bool ContainsYieldReturn(MethodDeclarationSyntax methodDeclaration, TextSpan? span = null)
        {
            return ContainsYield(methodDeclaration, span, yieldReturn: true, yieldBreak: false);
        }

        public static bool ContainsYieldBreak(MethodDeclarationSyntax methodDeclaration, TextSpan? span = null)
        {
            return ContainsYield(methodDeclaration, span, yieldReturn: false, yieldBreak: true);
        }

        public static bool ContainsYield(MethodDeclarationSyntax methodDeclaration, TextSpan? span = null, bool yieldReturn = true, bool yieldBreak = true)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            BlockSyntax block = methodDeclaration.Body;

            return block != null
                && ContainsYieldCore(block, span, yieldReturn, yieldBreak);
        }

        public static bool ContainsYieldReturn(LocalFunctionStatementSyntax localFunctionStatement, TextSpan? span = null)
        {
            return ContainsYield(localFunctionStatement, span, yieldReturn: true, yieldBreak: false);
        }

        public static bool ContainsYieldBreak(LocalFunctionStatementSyntax localFunctionStatement, TextSpan? span = null)
        {
            return ContainsYield(localFunctionStatement, span, yieldReturn: false, yieldBreak: true);
        }

        public static bool ContainsYield(LocalFunctionStatementSyntax localFunctionStatement, TextSpan? span = null, bool yieldReturn = true, bool yieldBreak = true)
        {
            if (localFunctionStatement == null)
                throw new ArgumentNullException(nameof(localFunctionStatement));

            BlockSyntax block = localFunctionStatement.Body;

            return block != null
                && ContainsYieldCore(block, span, yieldReturn, yieldBreak);
        }

        public static bool ContainsYieldReturn(BlockSyntax block, TextSpan? span = null)
        {
            return ContainsYield(block, span, yieldReturn: true, yieldBreak: false);
        }

        public static bool ContainsYieldBreak(BlockSyntax block, TextSpan? span = null)
        {
            return ContainsYield(block, span, yieldReturn: false, yieldBreak: true);
        }

        public static bool ContainsYield(BlockSyntax block, TextSpan? span = null, bool yieldReturn = true, bool yieldBreak = true)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return ContainsYieldCore(block, span, yieldReturn, yieldBreak);
        }

        private static bool ContainsYieldCore(BlockSyntax block, TextSpan? span, bool yieldReturn, bool yieldBreak)
        {
            ContainsYieldWalker walker = Create(span, yieldReturn, yieldBreak);

            walker.VisitBlock(block);

            return walker._success;
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            SyntaxKind kind = node.Kind();

            if (kind == SyntaxKind.YieldReturnStatement)
            {
                if (IsSearchingForYieldReturn)
                {
                    if (_span == null || _span.Value.Contains(node.FullSpan))
                    {
                        _success = true;
                    }
                }
            }
            else if (kind == SyntaxKind.YieldBreakStatement)
            {
                if (IsSearchingForYieldBreak)
                {
                    if (_span == null || _span.Value.Contains(node.FullSpan))
                    {
                        _success = true;
                    }
                }
            }
        }

        public override void Visit(SyntaxNode node)
        {
            if (!_success)
                base.Visit(node);
        }

        // VisitAccessorDeclaration
        // VisitBlock
        // VisitCatchClause
        // VisitCheckedStatement
        // VisitClassDeclaration
        // VisitCompilationUnit
        // VisitConversionOperatorDeclaration
        // VisitDoStatement
        // VisitElseClause
        // VisitFinallyClause
        // VisitFixedStatement
        // VisitForEachStatement
        // VisitForEachVariableStatement
        // VisitForStatement
        // VisitGlobalStatement
        // VisitIfStatement
        // VisitIncompleteMember
        // VisitIndexerDeclaration
        // VisitInterfaceDeclaration
        // VisitLabeledStatement
        // VisitLockStatement
        // VisitMethodDeclaration
        // VisitNamespaceDeclaration
        // VisitOperatorDeclaration
        // VisitPropertyDeclaration
        // VisitStructDeclaration
        // VisitSwitchSection
        // VisitSwitchStatement
        // VisitTryStatement
        // VisitUnsafeStatement
        // VisitUsingStatement
        // VisitWhileStatement

        public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
        }

        public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
        }

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
        }

        public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
        }

        public override void VisitConstantPattern(ConstantPatternSyntax node)
        {
        }

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
        }

        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
        }

        public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
        }

        public override void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
        }

        public override void VisitCrefParameterList(CrefParameterListSyntax node)
        {
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
        }

        public override void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
        }

        public override void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
        }

        public override void VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
        }

        public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
        }

        public override void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
        }

        public override void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
        }

        public override void VisitEmptyStatement(EmptyStatementSyntax node)
        {
        }

        public override void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
        }

        public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
        }

        public override void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
        }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
        }

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
        }

        public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
        }

        public override void VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
        }

        public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        {
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
        }

        public override void VisitInterpolation(InterpolationSyntax node)
        {
        }

        public override void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
        }

        public override void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
        }

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
        }

        public override void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
        }

        public override void VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
        }

        public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
        }

        public override void VisitNameEquals(NameEqualsSyntax node)
        {
        }

        public override void VisitNameMemberCref(NameMemberCrefSyntax node)
        {
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
        }

        public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
        }

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
        }

        public override void VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
        }

        public override void VisitParameter(ParameterSyntax node)
        {
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
        }

        public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
        }

        public override void VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
        }

        public override void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
        }

        public override void VisitQualifiedCref(QualifiedCrefSyntax node)
        {
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
        }

        public override void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
        }

        public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
        }

        public override void VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
        }

        public override void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
        }

        public override void VisitTupleElement(TupleElementSyntax node)
        {
        }

        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
        }

        public override void VisitTupleType(TupleTypeSyntax node)
        {
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
        }

        public override void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
        }

        public override void VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
        }

        public override void VisitWhenClause(WhenClauseSyntax node)
        {
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
        }

        public override void VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
        }

        public override void VisitXmlComment(XmlCommentSyntax node)
        {
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
        }

        public override void VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
        }

        public override void VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
        }

        public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
        }

        public override void VisitXmlName(XmlNameSyntax node)
        {
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
        }

        public override void VisitXmlPrefix(XmlPrefixSyntax node)
        {
        }

        public override void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
        }

        public override void VisitXmlText(XmlTextSyntax node)
        {
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
        }

        private sealed class ContainsYieldBreakWalker : ContainsYieldWalker
        {
            internal ContainsYieldBreakWalker(TextSpan? span = null) : base(span)
            {
            }

            public override bool IsSearchingForYieldReturn
            {
                get { return false; }
            }
        }

        private sealed class ContainsYieldReturnWalker : ContainsYieldWalker
        {
            internal ContainsYieldReturnWalker(TextSpan? span = null) : base(span)
            {
            }

            public override bool IsSearchingForYieldBreak
            {
                get { return false; }
            }
        }
    }
}
