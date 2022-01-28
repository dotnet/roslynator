// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

#pragma warning disable RS2008 // Enable analyzer release tracking

namespace Roslynator
{
    internal static class CommonDiagnosticRules
    {
        public static readonly DiagnosticDescriptor AnalyzerIsObsolete = DiagnosticDescriptorFactory.Create(
            id: CommonDiagnosticIdentifiers.AnalyzerIsObsolete,
            title: "Analyzer is obsolete",
            messageFormat: "Analyzer {0} is obsolete.{1}",
            category: "",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null,
            customTags: Array.Empty<string>());

        public static readonly DiagnosticDescriptor AnalyzerOptionIsObsolete = DiagnosticDescriptorFactory.Create(
            id: CommonDiagnosticIdentifiers.AnalyzerOptionIsObsolete,
            title: "Analyzer option is obsolete",
            messageFormat: "Analyzer option '{0}' is obsolete{1}",
            category: "",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: null,
            customTags: Array.Empty<string>());

        public static readonly DiagnosticDescriptor RequiredConfigOptionNotSet = DiagnosticDescriptorFactory.Create(
            id: CommonDiagnosticIdentifiers.RequiredConfigOptionNotSet,
            title: "Analyzer requires config option to be specified",
            messageFormat: "Analyzer {0} requires config option to be specified: {1}",
            category: "",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: null,
            customTags: Array.Empty<string>());
    }
}
