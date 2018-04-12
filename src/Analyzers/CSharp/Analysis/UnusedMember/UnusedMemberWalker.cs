// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    internal class UnusedMemberWalker : CSharpSyntaxWalker
    {
        private bool _isEmpty;

        private IMethodSymbol _containingMethodSymbol;

        public Collection<NodeSymbolInfo> Nodes { get; } = new Collection<NodeSymbolInfo>();

        public SemanticModel SemanticModel { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public bool IsAnyNodeConst { get; private set; }

        public bool IsAnyNodeDelegate { get; private set; }

        public void Reset()
        {
            Nodes.Clear();
            IsAnyNodeConst = false;
            IsAnyNodeDelegate = false;
            _isEmpty = false;
            _containingMethodSymbol = null;
        }

        private void CheckName(string name, SimpleNameSyntax node)
        {
            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                NodeSymbolInfo info = Nodes[i];

                if (info.Name == name)
                {
                    if (info.Symbol == null)
                    {
                        ISymbol declaredSymbol = SemanticModel.GetDeclaredSymbol(info.Node, CancellationToken);

                        Debug.Assert(declaredSymbol != null, "");

                        if (declaredSymbol == null)
                        {
                            RemoveNodeAt(i);
                            continue;
                        }

                        info = new NodeSymbolInfo(info.Name, info.Node, declaredSymbol);

                        Nodes[i] = info;
                    }

                    ISymbol symbol = SemanticModel.GetSymbol(node, CancellationToken);

                    if (symbol == null)
                        continue;

                    if (symbol is IMethodSymbol methodSymbol)
                        symbol = methodSymbol.ReducedFrom ?? methodSymbol;

                    if (info.Symbol.Equals(symbol.OriginalDefinition)
                        && _containingMethodSymbol?.Equals(symbol.OriginalDefinition) != true)
                    {
                        RemoveNodeAt(i);
                    }
                }
            }
        }

        public void AddDelegate(string name, SyntaxNode node)
        {
            AddNode(name, node);

            IsAnyNodeDelegate = true;
        }

        public void AddNode(string name, SyntaxNode node)
        {
            Nodes.Add(new NodeSymbolInfo(name, node));
        }

        public void AddNodes(VariableDeclarationSyntax declaration, bool isConst = false)
        {
            foreach (VariableDeclaratorSyntax declarator in declaration.Variables)
                AddNode(declarator.Identifier.ValueText, declarator);

            if (isConst)
                IsAnyNodeConst = true;
        }

        private void RemoveNodeAt(int index)
        {
            Nodes.RemoveAt(index);

            if (Nodes.Count == 0)
                _isEmpty = true;
        }

        private void VisitMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (MemberDeclarationSyntax member in members)
                Visit(member);
        }

        private void VisitBodyOrExpressionBody(BlockSyntax body, ArrowExpressionClauseSyntax expressionBody)
        {
            Visit(body);
            Visit(expressionBody);
        }

        private void VisitAccessorListOrExpressionBody(AccessorListSyntax accessorList, ArrowExpressionClauseSyntax expressionBody)
        {
            Visit(accessorList);
            Visit(expressionBody);
        }

        private void VisitAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            if (IsAnyNodeConst)
            {
                foreach (AttributeListSyntax attributeList in attributeLists)
                    Visit(attributeList);
            }
        }

        public override void Visit(SyntaxNode node)
        {
            if (!_isEmpty)
                base.Visit(node);
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            VisitBodyOrExpressionBody(node.Body, node.ExpressionBody);

            //base.VisitAccessorDeclaration(node);
        }

        //public override void VisitAccessorList(AccessorListSyntax node)
        //{
        //    base.VisitAccessorList(node);
        //}

        //public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        //{
        //    base.VisitAliasQualifiedName(node);
        //}

        //public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        //{
        //    base.VisitAnonymousMethodExpression(node);
        //}

        //public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        //{
        //    base.VisitAnonymousObjectCreationExpression(node);
        //}

        //public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        //{
        //    base.VisitAnonymousObjectMemberDeclarator(node);
        //}

        //public override void VisitArgumentList(ArgumentListSyntax node)
        //{
        //    base.VisitArgumentList(node);
        //}

        //public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        //{
        //    base.VisitArrayCreationExpression(node);
        //}

        //public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        //{
        //    base.VisitArrayRankSpecifier(node);
        //}

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            foreach (ArrayRankSpecifierSyntax rankSpecifier in node.RankSpecifiers)
                Visit(rankSpecifier);

            //base.VisitArrayType(node);
        }

        //public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        //{
        //    base.VisitArrowExpressionClause(node);
        //}

        //public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        //{
        //    base.VisitAssignmentExpression(node);
        //}

        public override void VisitAttribute(AttributeSyntax node)
        {
            Visit(node.ArgumentList);
            //base.VisitAttribute(node);
        }

        //public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        //{
        //    base.VisitAttributeArgument(node);
        //}

        //public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        //{
        //    base.VisitAttributeArgumentList(node);
        //}

        //public override void VisitAttributeList(AttributeListSyntax node)
        //{
        //    base.VisitAttributeList(node);
        //}

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitAttributeTargetSpecifier(node);
        }

        //public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        //{
        //    base.VisitAwaitExpression(node);
        //}

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitBadDirectiveTrivia(node);
        }

        //public override void VisitBaseExpression(BaseExpressionSyntax node)
        //{
        //    base.VisitBaseExpression(node);
        //}

        //public override void VisitBaseList(BaseListSyntax node)
        //{
        //    base.VisitBaseList(node);
        //}

        //public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        //{
        //    base.VisitBinaryExpression(node);
        //}

        //public override void VisitBlock(BlockSyntax node)
        //{
        //    base.VisitBlock(node);
        //}

        //public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        //{
        //    base.VisitBracketedArgumentList(node);
        //}

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            if (IsAnyNodeConst
                || IsAnyNodeDelegate)
            {
                base.VisitBracketedParameterList(node);
            }
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            //base.VisitBreakStatement(node);
        }

        //public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        //{
        //    base.VisitCasePatternSwitchLabel(node);
        //}

        //public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        //{
        //    base.VisitCaseSwitchLabel(node);
        //}

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            Visit(node.Expression);
            //base.VisitCastExpression(node);
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            Visit(node.Filter);
            Visit(node.Block);
            //base.VisitCatchClause(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitCatchDeclaration(node);
        }

        //public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        //{
        //    base.VisitCatchFilterClause(node);
        //}

        //public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        //{
        //    base.VisitCheckedExpression(node);
        //}

        //public override void VisitCheckedStatement(CheckedStatementSyntax node)
        //{
        //    base.VisitCheckedStatement(node);
        //}

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            VisitMembers(node.Members);
            //base.VisitClassDeclaration(node);
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitClassOrStructConstraint(node);
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            VisitMembers(node.Members);
            //base.VisitCompilationUnit(node);
        }

        //public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        //{
        //    base.VisitConditionalAccessExpression(node);
        //}

        //public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        //{
        //    base.VisitConditionalExpression(node);
        //}

        //public override void VisitConstantPattern(ConstantPatternSyntax node)
        //{
        //    base.VisitConstantPattern(node);
        //}

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitConstructorConstraint(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.ParameterList);
            VisitBodyOrExpressionBody(node.Body, node.ExpressionBody);
            Visit(node.Initializer);
            //base.VisitConstructorDeclaration(node);
        }

        //public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        //{
        //    base.VisitConstructorInitializer(node);
        //}

        //public override void VisitContinueStatement(ContinueStatementSyntax node)
        //{
        //    base.VisitContinueStatement(node);
        //}

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.ParameterList);
            VisitBodyOrExpressionBody(node.Body, node.ExpressionBody);
            //base.VisitConversionOperatorDeclaration(node);
        }

        public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitConversionOperatorMemberCref(node);
        }

        public override void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitCrefBracketedParameterList(node);
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitCrefParameter(node);
        }

        public override void VisitCrefParameterList(CrefParameterListSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitCrefParameterList(node);
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            //base.VisitDeclarationExpression(node);
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            //base.VisitDeclarationPattern(node);
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            //base.VisitDefaultExpression(node);
        }

        //public override void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        //{
        //    base.VisitDefaultSwitchLabel(node);
        //}

        public override void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitDefineDirectiveTrivia(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.ParameterList);
            //base.VisitDelegateDeclaration(node);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.ParameterList);
            VisitBodyOrExpressionBody(node.Body, node.ExpressionBody);
            //base.VisitDestructorDeclaration(node);
        }

        //public override void VisitDiscardDesignation(DiscardDesignationSyntax node)
        //{
        //    base.VisitDiscardDesignation(node);
        //}

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitDocumentationCommentTrivia(node);
        }

        //public override void VisitDoStatement(DoStatementSyntax node)
        //{
        //    base.VisitDoStatement(node);
        //}

        //public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        //{
        //    base.VisitElementAccessExpression(node);
        //}

        //public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        //{
        //    base.VisitElementBindingExpression(node);
        //}

        public override void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitElifDirectiveTrivia(node);
        }

        //public override void VisitElseClause(ElseClauseSyntax node)
        //{
        //    base.VisitElseClause(node);
        //}

        public override void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitElseDirectiveTrivia(node);
        }

        //public override void VisitEmptyStatement(EmptyStatementSyntax node)
        //{
        //    base.VisitEmptyStatement(node);
        //}

        public override void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitEndIfDirectiveTrivia(node);
        }

        public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitEndRegionDirectiveTrivia(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);

            if (IsAnyNodeConst)
            {
                foreach (EnumMemberDeclarationSyntax member in node.Members)
                    Visit(member);
            }

            //base.VisitEnumDeclaration(node);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.EqualsValue);

            //base.VisitEnumMemberDeclaration(node);
        }

        //public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        //{
        //    base.VisitEqualsValueClause(node);
        //}

        public override void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitErrorDirectiveTrivia(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            //base.VisitEventDeclaration(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            //base.VisitEventFieldDeclaration(node);
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitExplicitInterfaceSpecifier(node);
        }

        //public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        //{
        //    base.VisitExpressionStatement(node);
        //}

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitExternAliasDirective(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.Declaration);
            //base.VisitFieldDeclaration(node);
        }

        //public override void VisitFinallyClause(FinallyClauseSyntax node)
        //{
        //    base.VisitFinallyClause(node);
        //}

        //public override void VisitFixedStatement(FixedStatementSyntax node)
        //{
        //    base.VisitFixedStatement(node);
        //}

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            Visit(node.Expression);
            Visit(node.Statement);
            //base.VisitForEachStatement(node);
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            Visit(node.Expression);
            Visit(node.Statement);
            //base.VisitForEachVariableStatement(node);
        }

        //public override void VisitForStatement(ForStatementSyntax node)
        //{
        //    base.VisitForStatement(node);
        //}

        //public override void VisitFromClause(FromClauseSyntax node)
        //{
        //    base.VisitFromClause(node);
        //}

        public override void VisitGenericName(GenericNameSyntax node)
        {
            CheckName(node.Identifier.ValueText, node);
            //base.VisitGenericName(node);
        }

        public override void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitGlobalStatement(node);
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            //base.VisitGotoStatement(node);
        }

        //public override void VisitGroupClause(GroupClauseSyntax node)
        //{
        //    base.VisitGroupClause(node);
        //}

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            CheckName(node.Identifier.ValueText, node);
            //base.VisitIdentifierName(node);
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitIfDirectiveTrivia(node);
        }

        //public override void VisitIfStatement(IfStatementSyntax node)
        //{
        //    base.VisitIfStatement(node);
        //}

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            Visit(node.Initializer);
            //base.VisitImplicitArrayCreationExpression(node);
        }

        //public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        //{
        //    base.VisitImplicitElementAccess(node);
        //}

        //public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        //{
        //    base.VisitIncompleteMember(node);
        //}

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.ParameterList);
            VisitAccessorListOrExpressionBody(node.AccessorList, node.ExpressionBody);
            //base.VisitIndexerDeclaration(node);
        }

        public override void VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitIndexerMemberCref(node);
        }

        //public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        //{
        //    base.VisitInitializerExpression(node);
        //}

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            VisitMembers(node.Members);
            //base.VisitInterfaceDeclaration(node);
        }

        //public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        //{
        //    base.VisitInterpolatedStringExpression(node);
        //}

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            //base.VisitInterpolatedStringText(node);
        }

        //public override void VisitInterpolation(InterpolationSyntax node)
        //{
        //    base.VisitInterpolation(node);
        //}

        public override void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            //base.VisitInterpolationAlignmentClause(node);
        }

        public override void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            //base.VisitInterpolationFormatClause(node);
        }

        //public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        //{
        //    base.VisitInvocationExpression(node);
        //}

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            Visit(node.Expression);
            //base.VisitIsPatternExpression(node);
        }

        //public override void VisitJoinClause(JoinClauseSyntax node)
        //{
        //    base.VisitJoinClause(node);
        //}

        //public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        //{
        //    base.VisitJoinIntoClause(node);
        //}

        //public override void VisitLabeledStatement(LabeledStatementSyntax node)
        //{
        //    base.VisitLabeledStatement(node);
        //}

        //public override void VisitLetClause(LetClauseSyntax node)
        //{
        //    base.VisitLetClause(node);
        //}

        public override void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitLineDirectiveTrivia(node);
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            //base.VisitLiteralExpression(node);
        }

        public override void VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitLoadDirectiveTrivia(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            Visit(node.Declaration);
            //base.VisitLocalDeclarationStatement(node);
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            Visit(node.ParameterList);
            VisitBodyOrExpressionBody(node.Body, node.ExpressionBody);
            //base.VisitLocalFunctionStatement(node);
        }

        //public override void VisitLockStatement(LockStatementSyntax node)
        //{
        //    base.VisitLockStatement(node);
        //}

        //public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        //{
        //    base.VisitMakeRefExpression(node);
        //}

        //public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        //{
        //    base.VisitMemberAccessExpression(node);
        //}

        //public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        //{
        //    base.VisitMemberBindingExpression(node);
        //}

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Debug.Assert(_containingMethodSymbol == null);

            _containingMethodSymbol = SemanticModel.GetDeclaredSymbol(node, CancellationToken);

            VisitAttributeLists(node.AttributeLists);
            Visit(node.ParameterList);
            VisitBodyOrExpressionBody(node.Body, node.ExpressionBody);
            //base.VisitMethodDeclaration(node);

            _containingMethodSymbol = null;
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
            //base.VisitNameColon(node);
        }

        //public override void VisitNameEquals(NameEqualsSyntax node)
        //{
        //    base.VisitNameEquals(node);
        //}

        public override void VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitNameMemberCref(node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            VisitMembers(node.Members);
            //base.VisitNamespaceDeclaration(node);
        }

        //public override void VisitNullableType(NullableTypeSyntax node)
        //{
        //    base.VisitNullableType(node);
        //}

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            Visit(node.ArgumentList);
            Visit(node.Initializer);
            base.VisitObjectCreationExpression(node);
        }

        //public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        //{
        //    base.VisitOmittedArraySizeExpression(node);
        //}

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitOmittedTypeArgument(node);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.ParameterList);
            VisitBodyOrExpressionBody(node.Body, node.ExpressionBody);
            //base.VisitOperatorDeclaration(node);
        }

        public override void VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitOperatorMemberCref(node);
        }

        //public override void VisitOrderByClause(OrderByClauseSyntax node)
        //{
        //    base.VisitOrderByClause(node);
        //}

        //public override void VisitOrdering(OrderingSyntax node)
        //{
        //    base.VisitOrdering(node);
        //}

        public override void VisitParameter(ParameterSyntax node)
        {
            if (IsAnyNodeConst)
            {
                VisitAttributeLists(node.AttributeLists);
                Visit(node.Default);
            }

            if (IsAnyNodeDelegate)
                Visit(node.Type);

            //base.VisitParameter(node);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            if (IsAnyNodeConst
                || IsAnyNodeDelegate)
            {
                base.VisitParameterList(node);
            }

            //base.VisitParameterList(node);
        }

        //public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        //{
        //    base.VisitParenthesizedExpression(node);
        //}

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            Visit(node.Body);
            //base.VisitParenthesizedLambdaExpression(node);
        }

        //public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        //{
        //    base.VisitParenthesizedVariableDesignation(node);
        //}

        //public override void VisitPointerType(PointerTypeSyntax node)
        //{
        //    base.VisitPointerType(node);
        //}

        //public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        //{
        //    base.VisitPostfixUnaryExpression(node);
        //}

        public override void VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitPragmaChecksumDirectiveTrivia(node);
        }

        public override void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitPragmaWarningDirectiveTrivia(node);
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            //base.VisitPredefinedType(node);
        }

        //public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        //{
        //    base.VisitPrefixUnaryExpression(node);
        //}

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            Visit(node.Initializer);
            VisitAccessorListOrExpressionBody(node.AccessorList, node.ExpressionBody);
            //base.VisitPropertyDeclaration(node);
        }

        public override void VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitQualifiedCref(node);
        }

        //public override void VisitQualifiedName(QualifiedNameSyntax node)
        //{
        //    base.VisitQualifiedName(node);
        //}

        //public override void VisitQueryBody(QueryBodySyntax node)
        //{
        //    base.VisitQueryBody(node);
        //}

        //public override void VisitQueryContinuation(QueryContinuationSyntax node)
        //{
        //    base.VisitQueryContinuation(node);
        //}

        //public override void VisitQueryExpression(QueryExpressionSyntax node)
        //{
        //    base.VisitQueryExpression(node);
        //}

        public override void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitReferenceDirectiveTrivia(node);
        }

        //public override void VisitRefExpression(RefExpressionSyntax node)
        //{
        //    base.VisitRefExpression(node);
        //}

        public override void VisitRefType(RefTypeSyntax node)
        {
            //base.VisitRefType(node);
        }

        //public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        //{
        //    base.VisitRefTypeExpression(node);
        //}

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            Visit(node.Expression);
            //base.VisitRefValueExpression(node);
        }

        public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitRegionDirectiveTrivia(node);
        }

        //public override void VisitReturnStatement(ReturnStatementSyntax node)
        //{
        //    base.VisitReturnStatement(node);
        //}

        //public override void VisitSelectClause(SelectClauseSyntax node)
        //{
        //    base.VisitSelectClause(node);
        //}

        public override void VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitShebangDirectiveTrivia(node);
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitSimpleBaseType(node);
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            Visit(node.Body);
            //base.VisitSimpleLambdaExpression(node);
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitSingleVariableDesignation(node);
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            //base.VisitSizeOfExpression(node);
        }

        public override void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitSkippedTokensTrivia(node);
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            //base.VisitStackAllocArrayCreationExpression(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            VisitMembers(node.Members);
            //base.VisitStructDeclaration(node);
        }

        //public override void VisitSwitchSection(SwitchSectionSyntax node)
        //{
        //    base.VisitSwitchSection(node);
        //}

        //public override void VisitSwitchStatement(SwitchStatementSyntax node)
        //{
        //    base.VisitSwitchStatement(node);
        //}

        //public override void VisitThisExpression(ThisExpressionSyntax node)
        //{
        //    base.VisitThisExpression(node);
        //}

        //public override void VisitThrowExpression(ThrowExpressionSyntax node)
        //{
        //    base.VisitThrowExpression(node);
        //}

        //public override void VisitThrowStatement(ThrowStatementSyntax node)
        //{
        //    base.VisitThrowStatement(node);
        //}

        //public override void VisitTryStatement(TryStatementSyntax node)
        //{
        //    base.VisitTryStatement(node);
        //}

        public override void VisitTupleElement(TupleElementSyntax node)
        {
            //base.VisitTupleElement(node);
        }

        //public override void VisitTupleExpression(TupleExpressionSyntax node)
        //{
        //    base.VisitTupleExpression(node);
        //}

        //public override void VisitTupleType(TupleTypeSyntax node)
        //{
        //    base.VisitTupleType(node);
        //}

        //public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        //{
        //    base.VisitTypeArgumentList(node);
        //}

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitTypeConstraint(node);
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitTypeCref(node);
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            //base.VisitTypeOfExpression(node);
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            //base.VisitTypeParameter(node);
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitTypeParameterConstraintClause(node);
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            //base.VisitTypeParameterList(node);
        }

        public override void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitUndefDirectiveTrivia(node);
        }

        //public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        //{
        //    base.VisitUnsafeStatement(node);
        //}

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            //base.VisitUsingDirective(node);
        }

        //public override void VisitUsingStatement(UsingStatementSyntax node)
        //{
        //    base.VisitUsingStatement(node);
        //}

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            foreach (VariableDeclaratorSyntax variable in node.Variables)
                Visit(variable);
        }

        //public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        //{
        //    base.VisitVariableDeclarator(node);
        //}

        public override void VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitWarningDirectiveTrivia(node);
        }

        //public override void VisitWhenClause(WhenClauseSyntax node)
        //{
        //    base.VisitWhenClause(node);
        //}

        //public override void VisitWhereClause(WhereClauseSyntax node)
        //{
        //    base.VisitWhereClause(node);
        //}

        //public override void VisitWhileStatement(WhileStatementSyntax node)
        //{
        //    base.VisitWhileStatement(node);
        //}

        public override void VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlCDataSection(node);
        }

        public override void VisitXmlComment(XmlCommentSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlComment(node);
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlCrefAttribute(node);
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlElement(node);
        }

        public override void VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlElementEndTag(node);
        }

        public override void VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlElementStartTag(node);
        }

        public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlEmptyElement(node);
        }

        public override void VisitXmlName(XmlNameSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlName(node);
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlNameAttribute(node);
        }

        public override void VisitXmlPrefix(XmlPrefixSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlPrefix(node);
        }

        public override void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlProcessingInstruction(node);
        }

        public override void VisitXmlText(XmlTextSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlText(node);
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            Debug.Fail(node.ToString());
            base.VisitXmlTextAttribute(node);
        }

        //public override void VisitYieldStatement(YieldStatementSyntax node)
        //{
        //    base.VisitYieldStatement(node);
        //}
    }
}
