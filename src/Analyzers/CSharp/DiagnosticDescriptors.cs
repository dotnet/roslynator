// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp
{
    public static partial class DiagnosticDescriptors
    {
        private const string HelpLinkUriRoot = "http://pihrt.net/roslynator/analyzer?id=";

        private static DiagnosticDescriptor Create(
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
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                category: category,
                defaultSeverity: AnalyzersConfiguration.GetDiagnosticSeverityOrDefault(id, defaultSeverity),
                isEnabledByDefault: AnalyzersConfiguration.IsDiagnosticEnabledOrDefault(id, isEnabledByDefault),
                description: description,
                helpLinkUri: HelpLinkUriRoot + helpLinkUri,
                customTags: customTags);
        }
    }
}