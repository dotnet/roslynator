// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator;

internal static class DiagnosticDescriptorUtility
{
    private const string HelpLinkUriRoot = "https://josefpihrt.github.io/docs/roslynator/analyzers/";

    public static string GetHelpLinkUri(string analyzerId)
    {
        return HelpLinkUriRoot + analyzerId;
    }
}
