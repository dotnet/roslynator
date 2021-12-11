// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseLambdaInsteadOfAnonymousMethodAnalysis
    {
        public static bool IsFixable(AnonymousMethodExpressionSyntax anonymousMethod)
        {
            return anonymousMethod.ParameterList?.IsMissing == false;
        }
    }
}
