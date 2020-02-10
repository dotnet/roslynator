// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum SymbolDefinitionPartFilter
    {
        None = 0,
        Assemblies = 1,
        ContainingNamespace = 2,
        ContainingNamespaceInTypeHierarchy = 4,
        Attributes = 8,
        AssemblyAttributes = 16,
        AttributeArguments = 32,
        Accessibility = 64,
        Modifiers = 128,
        ParameterName = 256,
        ParameterDefaultValue = 512,
        BaseType = 1024,
        BaseInterfaces = 2048,
        BaseList = BaseType | BaseInterfaces,
        Constraints = 4096,
        TrailingSemicolon = 8192,
        TrailingComma = 16384,
        All = Assemblies | ContainingNamespace | ContainingNamespaceInTypeHierarchy | Attributes | AssemblyAttributes | AttributeArguments | Accessibility | Modifiers | ParameterName | ParameterDefaultValue | BaseType | BaseInterfaces | Constraints | TrailingSemicolon | TrailingComma
    }
}
