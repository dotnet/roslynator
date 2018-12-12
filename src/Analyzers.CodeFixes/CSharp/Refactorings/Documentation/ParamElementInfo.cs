// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal class ParamElementInfo : ElementInfo<ParameterSyntax>
    {
        public ParamElementInfo(ParameterSyntax node, int insertIndex, NewLinePosition newLinePosition)
            : base(node, insertIndex, newLinePosition)
        {
        }

        public override string Name
        {
            get { return Node.Identifier.ValueText; }
        }
    }
}