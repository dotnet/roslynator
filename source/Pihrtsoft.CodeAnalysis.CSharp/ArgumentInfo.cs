// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal struct ArgumentInfo
    {
        public ArgumentInfo(ISymbol methodOrProperty, IParameterSymbol parameter)
        {
            MethodOrProperty = methodOrProperty;
            Parameter = parameter;
        }

        public ISymbol MethodOrProperty { get; }
        public IParameterSymbol Parameter { get; }
    }
}
