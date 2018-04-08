// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal readonly struct ParameterInfo
    {
        public ParameterInfo(IParameterSymbol parameterSymbol, ExpressionSyntax expression, bool isThis = false)
        {
            ParameterSymbol = parameterSymbol;
            Expression = expression;
            IsThis = isThis;
        }

        public ExpressionSyntax Expression { get; }

        public IParameterSymbol ParameterSymbol { get; }

        public bool IsThis { get; }
    }
}
