// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Roslynator.Documentation;
using static Roslynator.Documentation.DeclarationListOptions;

namespace Roslynator.CommandLine
{
    [Verb("generate-declarations", HelpText = "Generates a single file that contains all declarations from specified assemblies.")]
    public class GenerateDeclarationsCommandLineOptions
    {
        [Option(shortName: 'a', longName: "assemblies",
            Required = true,
            HelpText = "Defines one or more assemblies that should be used as a source for the documentation.",
            MetaValue = "<ASSEMBLY>")]
        public IEnumerable<string> Assemblies { get; set; }

        [Option(shortName: 'o', longName: "output",
            Required = true,
            HelpText = "Defines a path for the output directory.",
            MetaValue = "<OUTPUT_DIRECTORY>")]
        public string OutputPath { get; set; }

        [Option(shortName: 'r', longName: "references",
            Required = true,
            HelpText = "Defines one or more paths to assembly or a file that contains a list of all assemblies. Each assembly must be on separate line.",
            MetaValue = "<ASSEMBLY_REFERENCE | ASSEMBLY_REFERENCES_FILE>")]
        public IEnumerable<string> References { get; set; }

        [Option(longName: "additional-xml-documentation",
            HelpText = "Defines one or more xml documentation files that should be included. These files can contain a documentation for namespaces, for instance.",
            MetaValue = "<XML_DOCUMENTATION_FILE>")]
        public IEnumerable<string> AdditionalXmlDocumentation { get; set; }

        [Option(longName: "depth",
            Default = DefaultValues.Depth,
            HelpText = "Defines a depth of a documentation. Allowed values are member, type or namespace. Default value is member.",
            MetaValue = "<DEPTH>")]
        public DocumentationDepth Depth { get; set; }

        [Option(longName: "empty-line-between-members",
            HelpText = "Indicates whether an empty line should be added between two member declarations.")]
        public bool EmptyLineBetweenMembers { get; set; }

        [Option(longName: "format-base-list",
            HelpText = "Indicates whether a base list should be formatted on a multiple lines.")]
        public bool FormatBaseList { get; set; }

        [Option(longName: "format-constraints",
            HelpText = "Indicates whether constraints should be formatted on a multiple lines.")]
        public bool FormatConstraints { get; set; }

        [Option(longName: "format-parameters",
            HelpText = "Indicates whether parameters should be formatted on a multiple lines.")]
        public bool FormatParameters { get; set; }

        [Option(longName: "fully-qualified-names",
            HelpText = "Indicates whether type names should be fully qualified.")]
        public bool FullyQualifiedNames { get; set; }

        [Option(longName: "ignored-names",
            HelpText = "Defines a list of metadata names that should be excluded from a documentation. Namespace of type names can be specified.",
            MetaValue = "<FULLY_QUALIFIED_METADATA_NAME>")]
        public IEnumerable<string> IgnoredNames { get; set; }

        [Option(longName: "ignored-parts",
            HelpText = "Defines parts of a declaration list that should be excluded. Allowed values are auto-generated-comment and assembly-attributes.",
            MetaValue = "<IGNORED_PARTS>")]
        public IEnumerable<string> IgnoredParts { get; set; }

        [Option(longName: "include-ienumerable",
            HelpText = "Indicates whether interface System.Collections.IEnumerable should be included in a documentation if a type also implements interface System.Collections.Generic.IEnumerable<T>.")]
        public bool IncludeIEnumerable { get; set; }

        [Option(longName: "indent-chars",
            Default = DefaultValues.IndentChars,
            HelpText = "Defines characters that should be used for indentation. Default value is four spaces.",
            MetaValue = "<INDENT_CHARS>")]
        public string IndentChars { get; set; }

        [Option(longName: "merge-attributes",
            HelpText = "Indicates whether attributes should be displayed in a single attribute list.")]
        public bool MergeAttributes { get; set; }

        [Option(longName: "nest-namespaces",
            HelpText = "Indicates whether namespaces should be nested.")]
        public bool NestNamespaces { get; set; }

        [Option(longName: "no-default-literal",
            HelpText = "Indicates whether default expression should be used instead of default literal.")]
        public bool NoDefaultLiteral { get; set; }

        [Option(longName: "no-indent",
            HelpText = "Indicates whether declarations should not be indented.")]
        public bool NoIndent { get; set; }

        [Option(longName: "no-new-line-before-open-brace",
            HelpText = "Indicates whether opening braced should not be placed on a new line.")]
        public bool NoNewLineBeforeOpenBrace { get; set; }

        [Option(longName: "no-precedence-for-system",
            HelpText = "Indicates whether symbols contained in 'System' namespace should be ordered as any other symbols and not before other symbols.")]
        public bool NoPrecedenceForSystem { get; set; }

        [Option(longName: "omit-attribute-arguments",
            HelpText = "Indicates whether attribute arguments should be omitted when displaying an attribute.")]
        public bool OmitAttributeArguments { get; set; }

        [Option(longName: "visibility",
            Default = nameof(Roslynator.Visibility.Public),
            HelpText = "Defines a visibility of a type or a member. Allowed values are public, internal or private. Default value is public.",
            MetaValue = "<VISIBILITY>")]
        public string Visibility { get; set; }
    }
}
