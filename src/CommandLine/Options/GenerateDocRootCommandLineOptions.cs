// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine;

[Verb("generate-doc-root", HelpText = "[deprecated] Generates root documentation file from specified project/solution. This command is obsolete. Use parameter '--root-file-path' of a command 'generate-doc' instead.")]
public class GenerateDocRootCommandLineOptions : AbstractGenerateDocCommandLineOptions
{
    [Option(
        longName: "files-layout",
        Default = "hierarchical",
        HelpText = "Defines layout of documentation files. Allowed values are hierarchical (default) or flat-namespaces.",
        MetaValue = "<LAYOUT>")]
    public string FilesLayout { get; set; }

    [Option(
        longName: "group-by-common-namespace",
        HelpText = "Indicates whether to group namespaces by greatest common namespace.")]
    public bool GroupByCommonNamespace { get; set; }

    [Option(
        longName: OptionNames.IncludeContainingNamespace,
        HelpText = "Defines parts of a documentation that should include containing namespace. Allowed value is class-hierarchy.",
        MetaValue = "<INCLUDE_CONTAINING_NAMESPACE>")]
    public IEnumerable<string> IncludeContainingNamespace { get; set; }

    [Option(
        longName: OptionNames.IncludeSystemNamespace,
        HelpText = "Indicates whether namespace should be included when a type is directly contained in namespace 'System'.")]
    public bool IncludeSystemNamespace { get; set; }

    [Option(
        longName: "ignored-parts",
        HelpText = "Defines parts of a root documentation that should be excluded. Allowed values are content, namespaces, class-hierarchy, types and other",
        MetaValue = "<IGNORED_PARTS>")]
    public IEnumerable<string> IgnoredParts { get; set; }

    [Option(
        longName: OptionNames.RootDirectoryUrl,
        HelpText = "Defines a relative url to the documentation root directory.",
        MetaValue = "<URL>")]
    public string RootDirectoryUrl { get; set; }
}
