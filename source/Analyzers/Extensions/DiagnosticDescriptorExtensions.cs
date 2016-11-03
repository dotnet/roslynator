// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp
{
    internal static class DiagnosticDescriptorExtensions
    {
        private const string IdSuffix = "FadeOut";

        public static DiagnosticDescriptor CreateFadeOut(this DiagnosticDescriptor descriptor)
        {
            return new DiagnosticDescriptor(
                descriptor.Id + IdSuffix,
                descriptor.Title,
                descriptor.MessageFormat,
                DiagnosticCategories.FadeOut,
                DiagnosticSeverity.Hidden,
                isEnabledByDefault: descriptor.IsEnabledByDefault,
                customTags: WellKnownDiagnosticTags.Unnecessary);
        }
    }
}
