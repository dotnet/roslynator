// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Roslynator.Documentation;
using static Roslynator.Documentation.DeclarationListOptions;

namespace Roslynator.CommandLine
{
    [Verb("generate-declarations")]
    public class GenerateDeclarationsCommandLineOptions
    {
        [Option(longName: "assemblies", shortName: 'a', Required = true)]
        public IEnumerable<string> Assemblies { get; set; }

        [Option(longName: "output", shortName: 'o', Required = true)]
        public string OutputPath { get; set; }

        [Option(longName: "references", shortName: 'r', Required = true)]
        public IEnumerable<string> References { get; set; }

        [Option(longName: "additional-xml-documentation")]
        public IEnumerable<string> AdditionalXmlDocumentation { get; set; }

        [Option(longName: "depth", Default = DefaultValues.Depth)]
        public DocumentationDepth Depth { get; set; }

        [Option(longName: "empty-line-between-members")]
        public bool EmptyLineBetweenMembers { get; set; }

        [Option(longName: "format-base-list")]
        public bool FormatBaseList { get; set; }

        [Option(longName: "format-constraints")]
        public bool FormatConstraints { get; set; }

        [Option(longName: "format-parameters")]
        public bool FormatParameters { get; set; }

        [Option(longName: "fully-qualified-names")]
        public bool FullyQualifiedNames { get; set; }

        [Option(longName: "ignored-names")]
        public IEnumerable<string> IgnoredNames { get; set; }

        [Option(longName: "ignored-parts")]
        public IEnumerable<string> IgnoredParts { get; set; }

        [Option(longName: "include-ienumerable")]
        public bool IncludeIEnumerable { get; set; }

        [Option(longName: "indent-chars", Default = DefaultValues.IndentChars)]
        public string IndentChars { get; set; }

        [Option(longName: "merge-attributes")]
        public bool MergeAttributes { get; set; }

        [Option(longName: "nest-namespaces")]
        public bool NestNamespaces { get; set; }

        [Option(longName: "no-indent")]
        public bool NoIndent { get; set; }

        [Option(longName: "no-default-literal")]
        public bool NoDefaultLiteral { get; set; }

        [Option(longName: "no-new-line-before-open-brace")]
        public bool NoNewLineBeforeOpenBrace { get; set; }

        [Option(longName: "no-precedence-for-system")]
        public bool NoPrecedenceForSystem { get; set; }

        [Option(longName: "omit-attribute-arguments")]
        public bool OmitAttributeArguments { get; set; }

        [Option(longName: "visibility", Default = nameof(DocumentationVisibility.Publicly))]
        public string Visibility { get; set; }
    }
}
