// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Roslynator.Documentation;

namespace Roslynator.CommandLine
{
    public abstract class AbstractGenerateDocCommandLineOptions : AbstractCommandLineOptions
    {
        [Option(shortName: 'a', longName: "assemblies",
            Required = true,
            HelpText = "Defines one or more assemblies that should be used as a source for the documentation.",
            MetaValue = "<ASSEMBLY>")]
        public IEnumerable<string> Assemblies { get; set; }

        [Option(shortName: 'h', longName: "heading",
            Required = true,
            HelpText = "Defines a heading of the root documentation file.",
            MetaValue = "<ROOT_FILE_HEADING>")]
        public string Heading { get; set; }

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

        [Option(longName: "depth",
            Default = DocumentationOptions.DefaultValues.Depth,
            HelpText = "Defines a depth of a documentation. Allowed values are member, type or namespace. Default value is member.",
            MetaValue = "<DEPTH>")]
        public DocumentationDepth Depth { get; set; }

        [Option(longName: "ignored-names",
            HelpText = "Defines a list of metadata names that should be excluded from a documentation. Namespace of type names can be specified.",
            MetaValue = "<FULLY_QUALIFIED_METADATA_NAME>")]
        public IEnumerable<string> IgnoredNames { get; set; }

        [Option(longName: "no-class-hierarchy",
            HelpText = "Indicates whether classes should be displayed as a list instead of hierarchy tree.")]
        public bool NoClassHierarchy { get; set; }

        [Option(longName: "no-mark-obsolete",
            HelpText = "Indicates whether obsolete types and members should not be marked as '[deprecated]'.")]
        public bool NoMarkObsolete { get; set; }

        [Option(longName: "no-precedence-for-system",
            HelpText = "Indicates whether symbols contained in 'System' namespace should be ordered as any other symbols and not before other symbols.")]
        public bool NoPrecedenceForSystem { get; set; }

        [Option(longName: "scroll-to-content",
            HelpText = "Indicates whether a link should lead to the top of the documentation content.")]
        public bool ScrollToContent { get; set; }

        [Option(longName: "visibility",
            Default = nameof(Roslynator.Visibility.Public),
            HelpText = "Defines a visibility of a type or a member. Allowed values are public, internal or private. Default value is public.",
            MetaValue = "<VISIBILITY>")]
        public string Visibility { get; set; }
    }
}
