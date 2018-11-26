// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.Diagnostics;
using Roslynator.Documentation;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal static class ParseHelpers
    {
        public static bool TryParseMSBuildProperties(IEnumerable<string> values, out Dictionary<string, string> properties)
        {
            properties = null;

            foreach (string property in values)
            {
                int index = property.IndexOf("=");

                if (index == -1)
                {
                    WriteLine($"Unable to parse property '{property}'", ConsoleColor.Red, Verbosity.Quiet);
                    return false;
                }

                string key = property.Substring(0, index);

                if (properties == null)
                    properties = new Dictionary<string, string>();

                properties[key] = property.Substring(index + 1);
            }

            if (properties?.Count > 0)
            {
                int maxLength = properties.Max(f => f.Key.Length);

                foreach (KeyValuePair<string, string> kvp in properties)
                    WriteLine($"Add MSBuild property {kvp.Key.PadRight(maxLength)} = {kvp.Value}", ConsoleColor.DarkGray, Verbosity.Detailed);
            }

            return true;
        }

        public static bool TryParseKeyValuePairs(IEnumerable<string> values, out Dictionary<string, string> properties)
        {
            properties = null;

            foreach (string property in values)
            {
                int index = property.IndexOf("=");

                if (index == -1)
                {
                    WriteLine($"Unable to parse key/value pair '{property}'", ConsoleColor.Red, Verbosity.Quiet);
                    return false;
                }

                string key = property.Substring(0, index);

                if (properties == null)
                    properties = new Dictionary<string, string>();

                properties[key] = property.Substring(index + 1);
            }

            return true;
        }

        public static bool TryParseDiagnosticSeverity(string value, out DiagnosticSeverity severity)
        {
            if (!Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out severity))
            {
                WriteLine($"Unknown diagnostic severity '{value}'.", Verbosity.Quiet);
                return false;
            }

            return true;
        }

        public static bool TryParseIgnoredRootParts(IEnumerable<string> values, out RootDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredRootParts;
                return true;
            }

            parts = RootDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out RootDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown root documentation part '{value}'.", Verbosity.Quiet);
                    return false;
                }
            }

            return true;
        }

        public static bool TryParseIgnoredNamespaceParts(IEnumerable<string> values, out NamespaceDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredNamespaceParts;
                return true;
            }

            parts = NamespaceDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out NamespaceDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown namespace documentation part '{value}'.", Verbosity.Quiet);
                    return false;
                }
            }

            return true;
        }

        public static bool TryParseIgnoredTypeParts(IEnumerable<string> values, out TypeDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredTypeParts;
                return true;
            }

            parts = TypeDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out TypeDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown type documentation part '{value}'.", Verbosity.Quiet);
                    return false;
                }
            }

            return true;
        }

        public static bool TryParseIgnoredMemberParts(IEnumerable<string> values, out MemberDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredMemberParts;
                return true;
            }

            parts = MemberDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out MemberDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown member documentation part '{value}'.", Verbosity.Quiet);
                    return false;
                }
            }

            return true;
        }

        public static bool TryParseIgnoredDeclarationListParts(IEnumerable<string> values, out DeclarationListParts parts)
        {
            if (!values.Any())
            {
                parts = DeclarationListOptions.Default.IgnoredParts;
                return true;
            }

            parts = DeclarationListParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out DeclarationListParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown declaration list part '{value}'.", Verbosity.Quiet);
                    return false;
                }
            }

            return true;
        }

        public static bool TryParseOmitContainingNamespaceParts(IEnumerable<string> values, out OmitContainingNamespaceParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.OmitContainingNamespaceParts;
                return true;
            }

            parts = OmitContainingNamespaceParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out OmitContainingNamespaceParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown omit containing namespace part '{value}'.", Verbosity.Quiet);
                    return false;
                }
            }

            return true;
        }

        public static bool TryParseVisibility(string value, out Visibility visibility)
        {
            if (!Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out visibility))
            {
                WriteLine($"Unknown visibility '{value}'.", Verbosity.Quiet);
                return false;
            }

            return true;
        }

        public static bool TryParseVerbosity(string value, out Verbosity verbosity)
        {
            switch (value)
            {
                case "q":
                    {
                        verbosity = Verbosity.Quiet;
                        return true;
                    }
                case "m":
                    {
                        verbosity = Verbosity.Minimal;
                        return true;
                    }
                case "n":
                    {
                        verbosity = Verbosity.Normal;
                        return true;
                    }
                case "d":
                    {
                        verbosity = Verbosity.Detailed;
                        return true;
                    }
                case "diag":
                    {
                        verbosity = Verbosity.Diagnostic;
                        return true;
                    }
            }

            if (Enum.TryParse(value, ignoreCase: true, out verbosity))
                return true;

            WriteLine($"Unknown verbosity '{value}'.", Verbosity.Quiet);
            return false;
        }

        public static bool TryParseLanguage(string value, out string language)
        {
            switch (value)
            {
                case "cs":
                case "csharp":
                    {
                        language = LanguageNames.CSharp;
                        return true;
                    }
                case "vb":
                case "visual-basic":
                    {
                        language = LanguageNames.VisualBasic;
                        return true;
                    }
            }

            WriteLine($"Unknown language '{value}'.", Verbosity.Quiet);

            language = null;
            return false;
        }

        public static bool TryParseUnusedSymbolKinds(IEnumerable<string> values, out UnusedSymbolKinds kinds)
        {
            if (!values.Any())
            {
                kinds = UnusedSymbolKinds.TypeOrMember;
                return true;
            }

            kinds = UnusedSymbolKinds.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out UnusedSymbolKinds result))
                {
                    kinds |= result;
                }
                else
                {
                    WriteLine($"Unknown unused symbol kind '{value}'.", Verbosity.Quiet);
                    return false;
                }
            }

            return true;
        }
    }
}
