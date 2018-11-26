// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslynator.CodeMetrics;

namespace Roslynator.VisualBasic.CodeMetrics
{
    internal class VisualBasicLogicalLinesWalker : VisualBasicLinesWalker
    {
        public int LogicalLineCount { get; set; }

        public VisualBasicLogicalLinesWalker(TextLineCollection lines, CodeMetricsOptions options, CancellationToken cancellationToken)
            : base(lines, options, cancellationToken)
        {
        }

        public override void VisitOptionStatement(OptionStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitOptionStatement(node);
        }

        public override void VisitImportsStatement(ImportsStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitImportsStatement(node);
        }

        public override void VisitNamespaceBlock(NamespaceBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitNamespaceBlock(node);
        }

        public override void VisitModuleBlock(ModuleBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitModuleBlock(node);
        }

        public override void VisitStructureBlock(StructureBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitStructureBlock(node);
        }

        public override void VisitInterfaceBlock(InterfaceBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitInterfaceBlock(node);
        }

        public override void VisitClassBlock(ClassBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitClassBlock(node);
        }

        public override void VisitEnumBlock(EnumBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitEnumBlock(node);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitEnumMemberDeclaration(node);
        }

        public override void VisitMethodBlock(MethodBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitMethodBlock(node);
        }

        public override void VisitConstructorBlock(ConstructorBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitConstructorBlock(node);
        }

        public override void VisitOperatorBlock(OperatorBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitOperatorBlock(node);
        }

        public override void VisitAccessorBlock(AccessorBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitAccessorBlock(node);
        }

        public override void VisitPropertyBlock(PropertyBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitPropertyBlock(node);
        }

        public override void VisitEventBlock(EventBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitEventBlock(node);
        }

        public override void VisitDeclareStatement(DeclareStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitDeclareStatement(node);
        }

        public override void VisitDelegateStatement(DelegateStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitDelegateStatement(node);
        }

        public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            LogicalLineCount++;
            base.VisitIncompleteMember(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            LogicalLineCount++;
            base.VisitFieldDeclaration(node);
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            LogicalLineCount++;
            base.VisitAttributeList(node);
        }

        public override void VisitAttributesStatement(AttributesStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitAttributesStatement(node);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitExpressionStatement(node);
        }

        public override void VisitPrintStatement(PrintStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitPrintStatement(node);
        }

        public override void VisitWhileBlock(WhileBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitWhileBlock(node);
        }

        public override void VisitUsingBlock(UsingBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitUsingBlock(node);
        }

        public override void VisitSyncLockBlock(SyncLockBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitSyncLockBlock(node);
        }

        public override void VisitWithBlock(WithBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitWithBlock(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitLocalDeclarationStatement(node);
        }

        public override void VisitLabelStatement(LabelStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitLabelStatement(node);
        }

        public override void VisitGoToStatement(GoToStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitGoToStatement(node);
        }

        public override void VisitStopOrEndStatement(StopOrEndStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitStopOrEndStatement(node);
        }

        public override void VisitExitStatement(ExitStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitExitStatement(node);
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitContinueStatement(node);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitReturnStatement(node);
        }

        public override void VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitSingleLineIfStatement(node);
        }

        public override void VisitSingleLineElseClause(SingleLineElseClauseSyntax node)
        {
            LogicalLineCount++;
            base.VisitSingleLineElseClause(node);
        }

        public override void VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitMultiLineIfBlock(node);
        }

        public override void VisitElseIfBlock(ElseIfBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitElseIfBlock(node);
        }

        public override void VisitElseBlock(ElseBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitElseBlock(node);
        }

        public override void VisitTryBlock(TryBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitTryBlock(node);
        }

        public override void VisitCatchBlock(CatchBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitCatchBlock(node);
        }

        public override void VisitFinallyBlock(FinallyBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitFinallyBlock(node);
        }

        public override void VisitErrorStatement(ErrorStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitErrorStatement(node);
        }

        public override void VisitOnErrorGoToStatement(OnErrorGoToStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitOnErrorGoToStatement(node);
        }

        public override void VisitOnErrorResumeNextStatement(OnErrorResumeNextStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitOnErrorResumeNextStatement(node);
        }

        public override void VisitResumeStatement(ResumeStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitResumeStatement(node);
        }

        public override void VisitSelectBlock(SelectBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitSelectBlock(node);
        }

        public override void VisitCaseBlock(CaseBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitCaseBlock(node);
        }

        public override void VisitDoLoopBlock(DoLoopBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitDoLoopBlock(node);
        }

        public override void VisitForBlock(ForBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitForBlock(node);
        }

        public override void VisitForEachBlock(ForEachBlockSyntax node)
        {
            LogicalLineCount++;
            base.VisitForEachBlock(node);
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitThrowStatement(node);
        }

        public override void VisitAssignmentStatement(AssignmentStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitAssignmentStatement(node);
        }

        public override void VisitCallStatement(CallStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitCallStatement(node);
        }

        public override void VisitAddRemoveHandlerStatement(AddRemoveHandlerStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitAddRemoveHandlerStatement(node);
        }

        public override void VisitRaiseEventStatement(RaiseEventStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitRaiseEventStatement(node);
        }

        public override void VisitReDimStatement(ReDimStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitReDimStatement(node);
        }

        public override void VisitEraseStatement(EraseStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitEraseStatement(node);
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            LogicalLineCount++;
            base.VisitYieldStatement(node);
        }
    }
}
