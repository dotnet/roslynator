// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal static class UnusedSyntaxRefactoring
    {
        public static UnusedConstructorParameterRefactoring UnusedConstructorParameter { get; } = new UnusedConstructorParameterRefactoring();
        public static UnusedMethodParameterRefactoring UnusedMethodParameter { get; } = new UnusedMethodParameterRefactoring();
        public static UnusedIndexerParameterRefactoring UnusedIndexerParameter { get; } = new UnusedIndexerParameterRefactoring();
        public static UnusedMethodTypeParameterRefactoring UnusedMethodTypeParameter { get; } = new UnusedMethodTypeParameterRefactoring();
    }
}
