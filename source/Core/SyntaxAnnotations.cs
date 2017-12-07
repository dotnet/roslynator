// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace Roslynator
{
    internal static class SyntaxAnnotations
    {
        public const string NavigationAnnotationKind = "CodeAction_Navigation";

        public static SyntaxAnnotation[] FormatterAnnotationArray { get; } = new SyntaxAnnotation[] { Formatter.Annotation };

        public static SyntaxAnnotation[] SimplifierAnnotationArray { get; } = new SyntaxAnnotation[] { Simplifier.Annotation };

        public static SyntaxAnnotation NavigationAnnotation { get; } = new SyntaxAnnotation(NavigationAnnotationKind);

        public static SyntaxAnnotation[] NavigationAnnotationArray { get; } = new SyntaxAnnotation[] { NavigationAnnotation };

        public static SyntaxAnnotation[] FormatterAndSimplifierAnnotations { get; } = new SyntaxAnnotation[] { Formatter.Annotation, Simplifier.Annotation };
    }
}
