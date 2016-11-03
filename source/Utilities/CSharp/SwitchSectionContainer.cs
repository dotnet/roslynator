// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public class SwitchSectionContainer : StatementContainer
    {
        private readonly SwitchSectionSyntax _switchSection;

        public SwitchSectionContainer(SwitchSectionSyntax switchSection)
            : base(switchSection)
        {
            _switchSection = switchSection;
        }

        public override bool IsSwitchSection
        {
            get { return true; }
        }

        public override SyntaxList<StatementSyntax> Statements
        {
            get { return _switchSection.Statements; }
        }

        public override SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements)
        {
            return _switchSection.WithStatements(statements);
        }
    }
}
