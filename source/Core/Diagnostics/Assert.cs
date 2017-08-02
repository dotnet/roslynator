// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.Diagnostics
{
    internal static class Assert
    {
        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string name, SyntaxKind kind = SyntaxKind.None)
        {
            Debug.Assert(condition, (kind == SyntaxKind.None) ? $"{name} is {kind}" : $"{name} is not {kind}");
        }

        [Conditional("DEBUG")]
        public static void NotNull<T>(T value) where T : class
        {
            Debug.Assert(value != null, $"{nameof(value)} is null");
        }

        [Conditional("DEBUG")]
        public static void HasNotAnnotation(SyntaxNode node, SyntaxAnnotation annotation)
        {
            Debug.Assert(!node.HasAnnotation(annotation), GetAnnotationMessage(nameof(node), annotation));
        }

        [Conditional("DEBUG")]
        public static void HasNotAnnotation(SyntaxToken token, SyntaxAnnotation annotation)
        {
            Debug.Assert(!token.HasAnnotation(annotation), GetAnnotationMessage(nameof(token), annotation));
        }

        private static string GetAnnotationMessage(string name, SyntaxAnnotation annotation)
        {
            return $"{name} contains annotation - Kind: {annotation.Kind}, Data: {annotation.Data}";
        }
    }
}