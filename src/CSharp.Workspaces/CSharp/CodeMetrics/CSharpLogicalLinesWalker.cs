// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeMetrics;

namespace Roslynator.CSharp.CodeMetrics
{
    internal class CSharpLogicalLinesWalker : CSharpLinesWalker
    {
        public int LogicalLineCount { get; set; }

        public CSharpLogicalLinesWalker(TextLineCollection lines, CodeMetricsOptions options, CancellationToken cancellationToken)
            : base(lines, options, cancellationToken)
        {
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitAccessorDeclaration(node);
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            LogicalLineCount++;
            base.VisitAttributeList(node);
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitBreakStatement(node);
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            LogicalLineCount++;
            base.VisitCatchClause(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitClassDeclaration(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitConstructorDeclaration(node);
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitContinueStatement(node);
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitConversionOperatorDeclaration(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitDelegateDeclaration(node);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitDestructorDeclaration(node);
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitDoStatement(node);
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            LogicalLineCount++;
            base.VisitElseClause(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitEnumDeclaration(node);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitEnumMemberDeclaration(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitEventDeclaration(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitEventFieldDeclaration(node);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitExpressionStatement(node);
        }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            LogicalLineCount++;
            base.VisitExternAliasDirective(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitFieldDeclaration(node);
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            LogicalLineCount++;
            base.VisitFinallyClause(node);
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitFixedStatement(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitForEachStatement(node);
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitForEachVariableStatement(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitForStatement(node);
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitGotoStatement(node);
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitCheckedStatement(node);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitIfStatement(node);
        }

        public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            LogicalLineCount++;
            base.VisitIncompleteMember(node);
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitIndexerDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitLabeledStatement(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitLocalDeclarationStatement(node);
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitLocalFunctionStatement(node);
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitLockStatement(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitMethodDeclaration(node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitOperatorDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitReturnStatement(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitStructDeclaration(node);
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            LogicalLineCount++;
            base.VisitSwitchSection(node);
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitSwitchStatement(node);
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitThrowStatement(node);
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitTryStatement(node);
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitUnsafeStatement(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            LogicalLineCount++;
            base.VisitUsingDirective(node);
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitUsingStatement(node);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitWhileStatement(node);
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitYieldStatement(node);
        }
    }
}
