// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

#pragma warning disable RS2008 // Enable analyzer release tracking

namespace Roslynator;

internal static class CommonDiagnosticRules
{
    public static readonly DiagnosticDescriptor RequiredConfigOptionNotSet = DiagnosticDescriptorFactory.Create(
        id: CommonDiagnosticIdentifiers.RequiredConfigOptionNotSet,
        title: "Analyzer requires config option to be specified",
        messageFormat: "Analyzer {0} requires config option to be specified: {1}",
        category: "",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: false,
        description: null,
        helpLinkUri: null,
        customTags: []);
}
