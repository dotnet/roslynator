// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    [DebuggerDisplay("{Flags}")]
    internal readonly struct BracesAnalysis : IEquatable<BracesAnalysis>
    {
        private BracesAnalysis(BracesAnalysisFlags flags)
        {
            Flags = flags;
        }

        public bool AddBraces => Any(BracesAnalysisFlags.AddBraces);

        public bool RemoveBraces => Any(BracesAnalysisFlags.RemoveBraces);

        internal BracesAnalysisFlags Flags { get; }

        public bool Any(BracesAnalysisFlags flags)
        {
            return (Flags & flags) != 0;
        }

        public bool All(BracesAnalysisFlags flags)
        {
            return (Flags & flags) != flags;
        }

        public static BracesAnalysis AnalyzeBraces(SwitchSectionSyntax switchSection)
        {
            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            int count = statements.Count;

            if (count == 0)
                return BracesAnalysisFlags.None;

            if (count > 1)
                return BracesAnalysisFlags.AddBraces;

            if (statements[0].Kind() == SyntaxKind.Block)
                return BracesAnalysisFlags.RemoveBraces;

            return BracesAnalysisFlags.AddBraces;
        }

        public static BracesAnalysis AnalyzeBraces(IfStatementSyntax ifStatement)
        {
            bool anyHasEmbedded = false;
            bool anyHasBlock = false;
            bool allSupportsEmbedded = true;

            int cnt = 0;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                cnt++;

                StatementSyntax statement = ifOrElse.Statement;

                if (!anyHasEmbedded
                    && statement.Kind() != SyntaxKind.Block)
                {
                    anyHasEmbedded = true;
                }

                if (!anyHasBlock
                    && statement.Kind() == SyntaxKind.Block)
                {
                    anyHasBlock = true;
                }

                if (allSupportsEmbedded
                    && !SupportsEmbedded(statement))
                {
                    allSupportsEmbedded = false;
                }

                if (cnt > 1
                    && anyHasEmbedded
                    && !allSupportsEmbedded)
                {
                    return BracesAnalysisFlags.AddBraces;
                }
            }

            if (cnt > 1
                && allSupportsEmbedded
                && anyHasBlock)
            {
                if (anyHasEmbedded)
                {
                    return BracesAnalysisFlags.AddBraces | BracesAnalysisFlags.RemoveBraces;
                }
                else
                {
                    return BracesAnalysisFlags.RemoveBraces;
                }
            }

            return BracesAnalysisFlags.None;

            bool SupportsEmbedded(StatementSyntax statement)
            {
                if (statement.IsParentKind(SyntaxKind.IfStatement)
                    && ((IfStatementSyntax)statement.Parent).Condition?.IsMultiLine() == true)
                {
                    return false;
                }

                if (statement.Kind() == SyntaxKind.Block)
                {
                    var block = (BlockSyntax)statement;

                    statement = block.Statements.SingleOrDefault(shouldThrow: false);

                    if (statement == null)
                        return false;
                }

                return !statement.IsKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.LabeledStatement)
                    && statement.IsSingleLine();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BracesAnalysis other && Equals(other);
        }

        public bool Equals(BracesAnalysis other)
        {
            return Flags == other.Flags;
        }

        public override int GetHashCode()
        {
            return Flags.GetHashCode();
        }

        public static implicit operator BracesAnalysis(BracesAnalysisFlags value)
        {
            return new BracesAnalysis(value);
        }

        public static implicit operator BracesAnalysisFlags(BracesAnalysis value)
        {
            return value.Flags;
        }

        public static bool operator ==(BracesAnalysis analysis1, BracesAnalysis analysis2)
        {
            return analysis1.Equals(analysis2);
        }

        public static bool operator !=(BracesAnalysis analysis1, BracesAnalysis analysis2)
        {
            return !(analysis1 == analysis2);
        }
    }
}
