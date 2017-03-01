// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal class SwitchSectionStatementContainer : StatementContainer
    {
        private readonly SwitchSectionSyntax _switchSection;

        public SwitchSectionStatementContainer(SwitchSectionSyntax switchSection)
            : base(switchSection)
        {
            _switchSection = switchSection;
        }

        public override SyntaxList<StatementSyntax> Statements
        {
            get { return _switchSection.Statements; }
        }

        public override SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements)
        {
            return _switchSection.WithStatements(statements);
        }

        public override StatementContainer WithStatements(SyntaxList<StatementSyntax> statements)
        {
            return new SwitchSectionStatementContainer(_switchSection.WithStatements(statements));
        }
    }
}
