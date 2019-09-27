// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Roslynator.Documentation;
using static Roslynator.Documentation.DocumentationOptions;

namespace Roslynator.CommandLine
{
    [Verb("generate-doc", HelpText = "Generates documentation files from specified assemblies.")]
    public class GenerateDocCommandLineOptions : AbstractGenerateDocCommandLineOptions
    {
        [Option(longName: "additional-xml-documentation",
            HelpText = "Defines one or more xml documentation files that should be included. These files can contain a documentation for namespaces, for instance.",
            MetaValue = "<XML_DOCUMENTATION_FILE>")]
        public IEnumerable<string> AdditionalXmlDocumentation { get; set; }

        [Option(longName: ParameterNames.IgnoredMemberParts,
            HelpText = "Defines parts of a member documentation that should be excluded. Allowed values are overloads, containing-type, containing-assembly, obsolete-message, summary, declaration, type-parameters, parameters, return-value, implements, attributes, exceptions, examples, remarks and see-also.",
            MetaValue = "<IGNORED_MEMBER_PARTS>")]
        public IEnumerable<string> IgnoredMemberParts { get; set; }

        [Option(longName: ParameterNames.IgnoredNamespaceParts,
            HelpText = "Defines parts of a namespace documentation that should be excluded. Allowed values are content, containing-namespace, summary, examples, remarks, classes, structs, interfaces, enums, delegates and see-also.",
            MetaValue = "<IGNORED_NAMESPACE_PARTS>")]
        public IEnumerable<string> IgnoredNamespaceParts { get; set; }

        [Option(longName: ParameterNames.IgnoredRootParts,
            HelpText = "Defines parts of a root documentation that should be excluded. Allowed values are content, namespaces, class-hierarchy, types and other.",
            MetaValue = "<IGNORED_ROOT_PARTS>")]
        public IEnumerable<string> IgnoredRootParts { get; set; }

        [Option(longName: ParameterNames.IgnoredTypeParts,
            HelpText = "Defines parts of a type documentation that should be excluded. Allowed values are content, containing-namespace, containing-assembly, obsolete-message, summary, declaration, type-parameters, parameters, return-value, inheritance, attributes, derived, implements, examples, remarks, constructors, fields, indexers, properties, methods, operators, events, explicit-interface-implementations, extension-methods, classes, structs, interfaces, enums, delegates and see-also.",
            MetaValue = "<IGNORED_TYPE_PARTS>")]
        public IEnumerable<string> IgnoredTypeParts { get; set; }

        [Option(longName: "include-all-derived-types",
            HelpText = "Indicates whether all derived types should be included in the list of derived types. By default only types that directly inherits from a specified type are displayed.")]
        public bool IncludeAllDerivedTypes { get; set; }

        [Option(longName: ParameterNames.IncludeContainingNamespace,
            HelpText = "Defines parts of a documentation that should include containing namespace. Allowed values are class-hierarchy, containing-type, parameter, return-type, base-type, attribute, derived-type, implemented-interface, implemented-member, exception, see-also and all.",
            MetaValue = "<INCLUDE_CONTAINING_NAMESPACE>")]
        public IEnumerable<string> IncludeContainingNamespace { get; set; }

        [Option(longName: "include-ienumerable",
            HelpText = "Indicates whether interface System.Collections.IEnumerable should be included in a documentation if a type also implements interface System.Collections.Generic.IEnumerable<T>.")]
        public bool IncludeIEnumerable { get; set; }

        [Option(longName: "include-inherited-interface-members",
            HelpText = "Indicates whether inherited interface members should be displayed in a list of members.")]
        public bool IncludeInheritedInterfaceMembers { get; set; }

        [Option(longName: ParameterNames.IncludeSystemNamespace,
            HelpText = "Indicates whether namespace should be included when a type is directly contained in namespace 'System'.")]
        public bool IncludeSystemNamespace { get; set; }

        [Option(longName: "inheritance-style",
            Default = DefaultValues.InheritanceStyle,
            HelpText = "Defines a style of a type inheritance. Allowed values are horizontal or vertical. Default value is horizontal.",
            MetaValue = "<INHERITANCE_STYLE>")]
        public InheritanceStyle InheritanceStyle { get; set; }

        [Option(longName: "max-derived-types",
            Default = DefaultValues.MaxDerivedTypes,
            HelpText = "Defines maximum number derived types that should be displayed. Default value is 5.",
            MetaValue = "<MAX_DERIVED_TYPES>")]
        public int MaxDerivedTypes { get; set; }

        [Option(longName: "no-delete",
            HelpText = "Indicates whether output directory should not be deleted at the beginning of the process.")]
        public bool NoDelete { get; set; }

        [Option(longName: "no-wrap-base-types",
            HelpText = "Indicates whether base types should not be wrapped.")]
        public bool NoWrapBaseTypes { get; set; }

        [Option(longName: "no-wrap-constraints",
            HelpText = "Indicates whether constraints should not be wrapped.")]
        public bool NoWrapConstraints { get; set; }

        [Option(longName: "omit-attribute-arguments",
            HelpText = "Indicates whether attribute arguments should be omitted when displaying an attribute.")]
        public bool OmitAttributeArguments { get; set; }

        [Option(longName: "omit-inherited-atttributes",
            HelpText = "Indicates whether inherited attributes should be omitted.")]
        public bool OmitInheritedAttributes { get; set; }

        [Option(longName: ParameterNames.OmitMemberParts,
            HelpText = "Defines parts of member definition that should be omitted. Allowed values are constant-value, implements, inherited-from and overrides.")]
        public IEnumerable<string> OmitMemberParts { get; set; }

        [Option(longName: "preferred-culture",
            HelpText = "Defines culture that should be used when searching for xml documentation files.",
            MetaValue = "<CULTURE_ID>")]
        public string PreferredCulture { get; set; }
#if DEBUG
        [Option(longName: "source-references")]
        public IEnumerable<string> SourceReferences { get; set; }
#endif
    }
}
