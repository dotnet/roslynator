// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Roslynator.Configuration;

namespace Roslynator
{
    internal static class DiagnosticDescriptorFactory
    {
        public static DiagnosticDescriptor Create(
            string id,
            string title,
            string messageFormat,
            string category,
            DiagnosticSeverity defaultSeverity,
            bool isEnabledByDefault,
            string description = null,
            string helpLinkUri = null,
            params string[] customTags)
        {
            isEnabledByDefault = CodeAnalysisConfig.Instance.IsDiagnosticEnabledByDefault(id, category, isEnabledByDefault);

            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                category: category,
                defaultSeverity: CodeAnalysisConfig.Instance.GetDiagnosticSeverity(id, category, isEnabledByDefault) ?? defaultSeverity,
                isEnabledByDefault: isEnabledByDefault,
                description: description,
                helpLinkUri: DiagnosticDescriptorUtility.GetHelpLinkUri(helpLinkUri),
                customTags: customTags);
        }

        public static DiagnosticDescriptor CreateFadeOut(DiagnosticDescriptor descriptor)
        {
            return new DiagnosticDescriptor(
                descriptor.Id + "FadeOut",
                descriptor.Title,
                descriptor.MessageFormat,
                descriptor.Category,
                DiagnosticSeverity.Hidden,
                isEnabledByDefault: true,
                customTags: new string[] { WellKnownDiagnosticTags.Unnecessary, WellKnownDiagnosticTags.NotConfigurable });
        }
    }
}
