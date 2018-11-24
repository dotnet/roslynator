// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    //TODO: make public
    internal readonly struct IfStatementCascadeInfo : IEquatable<IfStatementCascadeInfo>
    {
        public IfStatementCascadeInfo(IfStatementSyntax ifStatement)
        {
            int count = 0;
            IfStatementOrElseClause last = default;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                count++;
                last = ifOrElse;
            }

            IfStatement = ifStatement;
            Count = count;
            Last = last;
        }

        public IfStatementSyntax IfStatement { get; }

        public int Count { get; }

        public IfStatementOrElseClause Last { get; }

        public bool EndsWithIf => Last.IsIf;

        public bool EndsWithElse => Last.IsElse;

        public bool IsSimpleIf => Count == 1;

        public bool IsSimpleIfElse => Count == 2;

        public override string ToString()
        {
            return IfStatement?.ToString() ?? "";
        }

        public override bool Equals(object obj)
        {
            return obj is IfStatementCascadeInfo other && Equals(other);
        }

        public bool Equals(IfStatementCascadeInfo other)
        {
            return EqualityComparer<IfStatementSyntax>.Default.Equals(IfStatement, other.IfStatement);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<IfStatementSyntax>.Default.GetHashCode(IfStatement);
        }

        public static bool operator ==(in IfStatementCascadeInfo info1, in IfStatementCascadeInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in IfStatementCascadeInfo info1, in IfStatementCascadeInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
