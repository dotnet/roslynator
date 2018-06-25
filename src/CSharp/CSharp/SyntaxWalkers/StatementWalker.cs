// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal class StatementWalker
    {
        public virtual bool ShouldVisit
        {
            get { return true; }
        }

        private void VisitBlockIfNotNull(BlockSyntax node)
        {
            if (node != null
                && ShouldVisit)
            {
                VisitBlock(node);
            }
        }

        public virtual void VisitBlock(BlockSyntax node)
        {
            foreach (StatementSyntax statement in node.Statements)
            {
                if (!ShouldVisit)
                    return;

                VisitStatement(statement);
            }
        }

        public virtual void VisitBreakStatement(BreakStatementSyntax node)
        {
        }

        public virtual void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            VisitBlockIfNotNull(node.Block);
        }

        public virtual void VisitContinueStatement(ContinueStatementSyntax node)
        {
        }

        public virtual void VisitDoStatement(DoStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitEmptyStatement(EmptyStatementSyntax node)
        {
        }

        public virtual void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
        }

        public virtual void VisitFixedStatement(FixedStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitForEachStatement(ForEachStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitForStatement(ForStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitGotoStatement(GotoStatementSyntax node)
        {
        }

        public virtual void VisitIfStatement(IfStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);

            VisitStatementIfNotNull(node.Else?.Statement);
        }

        public virtual void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
        }

        public virtual void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            VisitBlockIfNotNull(node.Body);
        }

        public virtual void VisitLockStatement(LockStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitReturnStatement(ReturnStatementSyntax node)
        {
        }

        public virtual void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            foreach (SwitchSectionSyntax section in node.Sections)
            {
                if (!ShouldVisit)
                    return;

                foreach (StatementSyntax statement in section.Statements)
                {
                    VisitStatement(statement);

                    if (!ShouldVisit)
                        return;
                }
            }
        }

        public virtual void VisitThrowStatement(ThrowStatementSyntax node)
        {
        }

        public virtual void VisitTryStatement(TryStatementSyntax node)
        {
            VisitBlockIfNotNull(node.Block);

            foreach (CatchClauseSyntax catchClause in node.Catches)
                VisitBlockIfNotNull(catchClause.Block);

            FinallyClauseSyntax finallyClause = node.Finally;

            if (finallyClause != null)
                VisitBlockIfNotNull(finallyClause.Block);
        }

        public virtual void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            VisitBlockIfNotNull(node.Block);
        }

        public virtual void VisitUsingStatement(UsingStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitWhileStatement(WhileStatementSyntax node)
        {
            VisitStatementIfNotNull(node.Statement);
        }

        public virtual void VisitYieldStatement(YieldStatementSyntax node)
        {
        }

        private void VisitStatementIfNotNull(StatementSyntax node)
        {
            if (node != null
                && ShouldVisit)
            {
                VisitStatement(node);
            }
        }

        public virtual void VisitStatement(StatementSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.Block:
                    {
                        VisitBlock((BlockSyntax)node);
                        break;
                    }
                case SyntaxKind.BreakStatement:
                    {
                        VisitBreakStatement((BreakStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.ContinueStatement:
                    {
                        VisitContinueStatement((ContinueStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.DoStatement:
                    {
                        VisitDoStatement((DoStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.EmptyStatement:
                    {
                        VisitEmptyStatement((EmptyStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.ExpressionStatement:
                    {
                        VisitExpressionStatement((ExpressionStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.FixedStatement:
                    {
                        VisitFixedStatement((FixedStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        VisitForEachStatement((ForEachStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.ForEachVariableStatement:
                    {
                        VisitForEachVariableStatement((ForEachVariableStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.ForStatement:
                    {
                        VisitForStatement((ForStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                    {
                        VisitGotoStatement((GotoStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                    {
                        VisitCheckedStatement((CheckedStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.IfStatement:
                    {
                        VisitIfStatement((IfStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.LabeledStatement:
                    {
                        VisitLabeledStatement((LabeledStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        VisitLocalDeclarationStatement((LocalDeclarationStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        VisitLocalFunctionStatement((LocalFunctionStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.LockStatement:
                    {
                        VisitLockStatement((LockStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        VisitReturnStatement((ReturnStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        VisitSwitchStatement((SwitchStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.ThrowStatement:
                    {
                        VisitThrowStatement((ThrowStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.TryStatement:
                    {
                        VisitTryStatement((TryStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.UnsafeStatement:
                    {
                        VisitUnsafeStatement((UnsafeStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.UsingStatement:
                    {
                        VisitUsingStatement((UsingStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.WhileStatement:
                    {
                        VisitWhileStatement((WhileStatementSyntax)node);
                        break;
                    }
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.YieldReturnStatement:
                    {
                        VisitYieldStatement((YieldStatementSyntax)node);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Unknown statement '{node.Kind()}'.", nameof(node));
                    }
            }
        }
    }
}
