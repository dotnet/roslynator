// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotMarkdown;
using Microsoft.CodeAnalysis;
using Roslynator.Documentation;
using Roslynator.Documentation.Markdown;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class GenerateDocCommand : MSBuildWorkspaceCommand<CommandResult>
    {
        private static readonly Encoding _defaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public GenerateDocCommand(
            GenerateDocCommandLineOptions options,
            DocumentationDepth depth,
            RootDocumentationParts ignoredRootParts,
            NamespaceDocumentationParts ignoredNamespaceParts,
            TypeDocumentationParts ignoredTypeParts,
            MemberDocumentationParts ignoredMemberParts,
            CommonDocumentationParts ignoredCommonParts,
            OmitMemberParts omitMemberParts,
            IncludeContainingNamespaceFilter includeContainingNamespaceFilter,
            Visibility visibility,
            DocumentationHost documentationHost,
            FilesLayout filesLayout,
            bool groupByCommonNamespace,
            InheritanceStyle inheritanceStyle,
            in ProjectFilter projectFilter) : base(projectFilter)
        {
            Options = options;
            Depth = depth;
            IgnoredRootParts = ignoredRootParts;
            IgnoredNamespaceParts = ignoredNamespaceParts;
            IgnoredTypeParts = ignoredTypeParts;
            IgnoredMemberParts = ignoredMemberParts;
            IgnoredCommonParts = ignoredCommonParts;
            OmitMemberParts = omitMemberParts;
            IncludeContainingNamespaceFilter = includeContainingNamespaceFilter;
            Visibility = visibility;
            DocumentationHost = documentationHost;
            FilesLayout = filesLayout;
            GroupByCommonNamespace = groupByCommonNamespace;
            InheritanceStyle = inheritanceStyle;
        }

        public GenerateDocCommandLineOptions Options { get; }

        public DocumentationDepth Depth { get; }

        public RootDocumentationParts IgnoredRootParts { get; }

        public NamespaceDocumentationParts IgnoredNamespaceParts { get; }

        public TypeDocumentationParts IgnoredTypeParts { get; }

        public MemberDocumentationParts IgnoredMemberParts { get; }

        public CommonDocumentationParts IgnoredCommonParts { get; }

        public OmitMemberParts OmitMemberParts { get; }

        public IncludeContainingNamespaceFilter IncludeContainingNamespaceFilter { get; }

        public Visibility Visibility { get; }

        public DocumentationHost DocumentationHost { get; }

        public FilesLayout FilesLayout { get; }

        public bool GroupByCommonNamespace { get; }

        public InheritanceStyle InheritanceStyle { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var documentationOptions = new DocumentationOptions(
                rootFileHeading: Options.Heading,
                ignoredNames: Options.IgnoredNames,
                preferredCultureName: Options.PreferredCulture,
                maxDerivedTypes: Options.MaxDerivedTypes,
                placeSystemNamespaceFirst: !Options.NoPrecedenceForSystem,
                wrapDeclarationBaseTypes: !Options.NoWrapBaseTypes,
                wrapDeclarationConstraints: !Options.NoWrapConstraints,
                markObsolete: !Options.NoMarkObsolete,
                includeMemberInheritedFrom: (OmitMemberParts & OmitMemberParts.InheritedFrom) == 0,
                includeMemberOverrides: (OmitMemberParts & OmitMemberParts.Overrides) == 0,
                includeMemberImplements: (OmitMemberParts & OmitMemberParts.Implements) == 0,
                includeMemberConstantValue: (OmitMemberParts & OmitMemberParts.ConstantValue) == 0,
                includeInheritedInterfaceMembers: Options.IncludeInheritedInterfaceMembers,
                includeAllDerivedTypes: Options.IncludeAllDerivedTypes,
                includeAttributeArguments: !Options.OmitAttributeArguments,
                includeInheritedAttributes: !Options.OmitInheritedAttributes,
                omitIEnumerable: !Options.IncludeIEnumerable,
                depth: Depth,
                inheritanceStyle: InheritanceStyle,
                ignoredRootParts: IgnoredRootParts,
                ignoredNamespaceParts: IgnoredNamespaceParts,
                ignoredTypeParts: IgnoredTypeParts,
                ignoredMemberParts: IgnoredMemberParts,
                ignoredCommonParts: IgnoredCommonParts,
                includeContainingNamespaceFilter: IncludeContainingNamespaceFilter,
                filesLayout: FilesLayout,
                scrollToContent: Options.ScrollToContent);

            ImmutableArray<Compilation> compilations = await GetCompilationsAsync(projectOrSolution, cancellationToken);

            var documentationModel = new DocumentationModel(compilations, DocumentationFilterOptions.Instance, Options.AdditionalXmlDocumentation);

            List<INamespaceSymbol> commonNamespaces = null;

            if (GroupByCommonNamespace)
            {
                commonNamespaces = DocumentationUtility.FindCommonNamespaces(
                    documentationModel.Types.Concat(documentationModel.GetExtendedExternalTypes()));
            }

            UrlSegmentProvider urlSegmentProvider = new DefaultUrlSegmentProvider(FilesLayout, commonNamespaces);

            var externalProviders = new MicrosoftDocsUrlProvider[] { MicrosoftDocsUrlProvider.Instance };

            DocumentationUrlProvider GetUrlProvider()
            {
                switch (DocumentationHost)
                {
                    case DocumentationHost.GitHub:
                        return new GitHubDocumentationUrlProvider(urlSegmentProvider, externalProviders);
                    case DocumentationHost.Docusaurus:
                        return new DocusaurusDocumentationUrlProvider(urlSegmentProvider, externalProviders);
                    default:
                        throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
                }
            }

            MarkdownWriterSettings GetMarkdownWriterSettings()
            {
                switch (DocumentationHost)
                {
                    case DocumentationHost.GitHub:
                        return MarkdownWriterSettings.Default;
                    case DocumentationHost.Docusaurus:
                        return new MarkdownWriterSettings(new MarkdownFormat(angleBracketEscapeStyle: AngleBracketEscapeStyle.EntityRef));
                    default:
                        throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
                }
            }

            MarkdownWriterSettings markdownWriterSettings = GetMarkdownWriterSettings();

            DocumentationWriter CreateDocumentationWriter(DocumentationContext context)
            {
                MarkdownWriter writer = MarkdownWriter.Create(new StringBuilder(), markdownWriterSettings);

                switch (DocumentationHost)
                {
                    case DocumentationHost.GitHub:
                        return new MarkdownDocumentationWriter(context, writer);
                    case DocumentationHost.Docusaurus:
                        return new DocusaurusDocumentationWriter(context, writer);
                    default:
                        throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
                }
            }

            SourceReferenceProvider sourceReferenceProvider = null;
#if DEBUG
            if (Options.SourceReferences.Any())
                sourceReferenceProvider = SourceReferenceProvider.Load(Options.SourceReferences);
#endif
            var context = new DocumentationContext(
                documentationModel,
                GetUrlProvider(),
                documentationOptions,
                c => CreateDocumentationWriter(c),
                sourceReferenceProvider: sourceReferenceProvider,
                commonNamespaces: commonNamespaces);

            var generator = new DocumentationGenerator(context);

            string directoryPath = Options.Output;

            if (!Options.NoDelete
                && Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
                catch (IOException ex)
                {
                    WriteError(ex);
                    return CommandResults.Fail;
                }
            }

            WriteLine($"Generate documentation to '{Options.Output}'", Verbosity.Minimal);

            foreach (DocumentationGeneratorResult documentationFile in generator.Generate(heading: Options.Heading, cancellationToken))
            {
                string path = Path.Combine(directoryPath, documentationFile.FilePath);

                WriteLine($"  Save '{path}'", ConsoleColors.DarkGray, Verbosity.Detailed);

                Directory.CreateDirectory(Path.GetDirectoryName(path));

                File.WriteAllText(path, documentationFile.Content, _defaultEncoding);
            }

            WriteLine($"Documentation successfully generated to '{Options.Output}'.", Verbosity.Minimal);

            return CommandResults.Success;
        }
    }
}
