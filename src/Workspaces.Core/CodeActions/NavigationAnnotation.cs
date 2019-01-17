// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

#pragma warning disable RCS1102

namespace Roslynator.CodeActions
{
    internal class NavigationAnnotation
    {
        public const string Kind = "CodeAction_Navigation";

        public static SyntaxAnnotation Annotation { get; } = new SyntaxAnnotation(Kind);
    }
}
