// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Roslynator.Documentation;

namespace Roslynator.CommandLine
{
    // OmitAssemblies, PreferredCulture
    [Verb("list-symbols", HelpText = "Lists symbols from the specified project or solution.")]
    public class ListSymbolsCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: ParameterNames.Depth,
            HelpText = "Defines a depth of a list of symbols. Allowed values are member, type or namespace. Default value is member.",
            MetaValue = "<DEPTH>")]
        public string Depth { get; set; }

        [Option(longName: "empty-line-between-members",
            HelpText = "Indicates whether an empty line should be added between two member definitions.")]
        public bool EmptyLineBetweenMembers { get; set; }

        [Option(longName: ParameterNames.Format,
            HelpText = "Specifies parts of a symbol definition that should be formatted. Allowed values are attributes, parameters, base-list and constraints.")]
        public IEnumerable<string> Format { get; set; }

        [Option(longName: "group-by-assembly",
            HelpText = "Indicates whether symbols should be grouped by assembly.")]
        public bool GroupByAssembly { get; set; }

        [Option(longName: "ignored-attributes",
            HelpText = "Defines a list of attributes that should be ignored.",
            MetaValue = "<FULLY_QUALIFIED_METADATA_NAME>")]
        public IEnumerable<string> IgnoredAttributes { get; set; }

        [Option(longName: ParameterNames.IgnoredParts,
            HelpText = "Defines parts of a symbol definition that should be excluded. Allowed values are containing-namespace, attributes, assembly-attributes, attribute-arguments, accessibility, modifiers, parameter-name, parameter-default-value, base-type, base-interfaces, constraints, trailing-semicolon, trailing-comma.",
            MetaValue = "<IGNORED_PARTS>")]
        public IEnumerable<string> IgnoredParts { get; set; }

        [Option(longName: "ignored-symbols",
            HelpText = "Defines a list of symbols that should be ignored. Namespace of types can be specified.",
            MetaValue = "<FULLY_QUALIFIED_METADATA_NAME>")]
        public IEnumerable<string> IgnoredSymbols { get; set; }

        [Option(longName: "documentation",
            HelpText = "Indicates whether a documentation should be included.")]
        public bool Documentation { get; set; }

        [Option(longName: "indent-chars",
            Default = DefinitionListFormat.DefaultValues.IndentChars,
            HelpText = "Defines characters that should be used for indentation. Default value is two spaces.",
            MetaValue = "<INDENT_CHARS>")]
        public string IndentChars { get; set; }

        [Option(longName: ParameterNames.Layout,
            HelpText = "Defines layout of a list of symbol definitions. Allowed values are namespace-list, namespace-hierarchy or type-hierarchy. Default value is namespace-list.")]
        public string Layout { get; set; }

        [Option(shortName: 'o', longName: "output",
            HelpText = "Defines path to file(s) that will store a list of symbol definitions. Available data formats are XML ('xml' extension), JSON ('json' extension), Markdown ('md' extension) or plain text format.",
            MetaValue = "<OUTPUT_FILE>")]
        public IEnumerable<string> Output { get; set; }

        [Option(longName: "references",
            HelpText = "Defines file name/path to assembly(ies) that should be included.",
                MetaValue = "<ASSEMBLY_FILE>")]
        public IEnumerable<string> References { get; set; }

        [Option(longName: ParameterNames.Visibility,
            Default = nameof(Roslynator.Visibility.Private),
            HelpText = "Defines one or more visibility of a type or a member. Allowed values are public, internal or private.",
            MetaValue = "<VISIBILITY>")]
        public IEnumerable<string> Visibility { get; set; }

        //[Option(longName: "include-ienumerable",
        //    HelpText = "Indicates whether interface System.Collections.IEnumerable should be included in a documentation if a type also implements interface System.Collections.Generic.IEnumerable<T>.")]
        //public bool IncludeIEnumerable { get; set; }

        //[Option(longName: "no-precedence-for-system",
        //    HelpText = "Indicates whether symbols contained in 'System' namespace should be ordered as any other symbols and not before other symbols.")]
        //public bool NoPrecedenceForSystem { get; set; }
    }
}
