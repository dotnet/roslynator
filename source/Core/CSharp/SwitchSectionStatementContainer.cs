// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal struct SwitchSectionStatementContainer : IStatementContainer
    {
        public SwitchSectionStatementContainer(SwitchSectionSyntax switchSection)
        {
            SwitchSection = switchSection;
            Statements = switchSection.Statements;
        }

        SyntaxNode IStatementContainer.Node
        {
            get { return SwitchSection; }
        }

        public SwitchSectionSyntax SwitchSection { get; }

        public SyntaxList<StatementSyntax> Statements { get; }

        public SyntaxNode NodeWithStatements(IEnumerable<StatementSyntax> statements)
        {
            return NodeWithStatements(List(statements));
        }

        public SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements)
        {
            return SwitchSection.WithStatements(statements);
        }

        public IStatementContainer WithStatements(IEnumerable<StatementSyntax> statements)
        {
            return WithStatements(List(statements));
        }

        public IStatementContainer WithStatements(SyntaxList<StatementSyntax> statements)
        {
            return new SwitchSectionStatementContainer(SwitchSection.WithStatements(statements));
        }
    }
}
