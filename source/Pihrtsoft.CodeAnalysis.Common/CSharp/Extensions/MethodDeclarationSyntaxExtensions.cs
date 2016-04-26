// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class MethodDeclarationSyntaxExtensions
    {
        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            if (methodDeclaration.ReturnType == null)
                return false;

            return methodDeclaration.ReturnType.IsVoid();
        }
    }
}
