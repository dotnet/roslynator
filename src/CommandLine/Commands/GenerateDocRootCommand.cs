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

namespace Roslynator.CommandLine;

internal class GenerateDocRootCommand : MSBuildWorkspaceCommand<CommandResult>
{
    private static readonly Encoding _defaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    public GenerateDocRootCommand(
        GenerateDocRootCommandLineOptions options,
        DocumentationDepth depth,
        RootDocumentationParts ignoredParts,
        IncludeContainingNamespaceFilter includeContainingNamespaceFilter,
        Visibility visibility,
        DocumentationHost documentationHost,
        FilesLayout filesLayout,
        bool groupByCommonNamespace,
        in ProjectFilter projectFilter) : base(projectFilter)
    {
        Options = options;
        Depth = depth;
        IgnoredParts = ignoredParts;
        IncludeContainingNamespaceFilter = includeContainingNamespaceFilter;
        Visibility = visibility;
        DocumentationHost = documentationHost;
        FilesLayout = filesLayout;
        GroupByCommonNamespace = groupByCommonNamespace;
    }

    public GenerateDocRootCommandLineOptions Options { get; }

    public DocumentationDepth Depth { get; }

    public RootDocumentationParts IgnoredParts { get; }

    public IncludeContainingNamespaceFilter IncludeContainingNamespaceFilter { get; }

    public Visibility Visibility { get; }

    public DocumentationHost DocumentationHost { get; }

    public FilesLayout FilesLayout { get; }

    public bool GroupByCommonNamespace { get; }

    public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
    {
        AssemblyResolver.Register();

        var documentationOptions = new DocumentationOptions()
        {
            RootFileHeading = Options.Heading,
            RootDirectoryUrl = Options.RootDirectoryUrl,
            IncludeSystemNamespace = Options.IncludeSystemNamespace,
            PlaceSystemNamespaceFirst = !Options.NoPrecedenceForSystem,
            MarkObsolete = !Options.NoMarkObsolete,
            Depth = Depth,
            IgnoredRootParts = IgnoredParts,
            IncludeContainingNamespaceFilter = IncludeContainingNamespaceFilter,
            ScrollToContent = (DocumentationHost == DocumentationHost.GitHub) && Options.ScrollToContent,
            FilesLayout = FilesLayout,
        };

        if (Options.IgnoredNames is not null)
        {
            foreach (string ignoredName in Options.IgnoredNames)
                documentationOptions.IgnoredNames.Add(MetadataName.Parse(ignoredName));
        }

        ImmutableArray<Compilation> compilations = await GetCompilationsAsync(projectOrSolution, cancellationToken);

        var documentationModel = new DocumentationModel(compilations, DocumentationFilterOptions.Instance);

        List<INamespaceSymbol> commonNamespaces = null;

        if (GroupByCommonNamespace)
        {
            commonNamespaces = DocumentationUtility.FindCommonNamespaces(
                documentationModel.Types
                    .Concat(documentationModel.GetExtendedExternalTypes())
                    .Where(f => !documentationOptions.ShouldBeIgnored(f)));
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
                case DocumentationHost.Sphinx:
                    return new SphinxDocumentationUrlProvider(urlSegmentProvider, externalProviders);
                default:
                    throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
            }
        }

        MarkdownWriterSettings GetMarkdownWriterSettings()
        {
            switch (DocumentationHost)
            {
                case DocumentationHost.GitHub:
                case DocumentationHost.Sphinx:
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
                    return new GitHubDocumentationWriter(context, writer);
                case DocumentationHost.Docusaurus:
                    return new DocusaurusDocumentationWriter(context, writer);
                case DocumentationHost.Sphinx:
                    return new SphinxDocumentationWriter(context, writer);
                default:
                    throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
            }
        }

        var context = new DocumentationContext(
            documentationModel,
            GetUrlProvider(),
            documentationOptions,
            c => CreateDocumentationWriter(c));

        var generator = new DocumentationGenerator(context);

        string path = Options.Output;

        WriteLine($"Generate documentation root to '{path}'.", Verbosity.Minimal);

        string heading = Options.Heading;

        if (string.IsNullOrEmpty(heading))
        {
            string fileName = Path.GetFileName(Options.Output);

            heading = (fileName.EndsWith(".dll", StringComparison.Ordinal))
                ? Path.GetFileNameWithoutExtension(fileName)
                : fileName;
        }

        DocumentationGeneratorResult result = generator.GenerateRoot(heading);

        File.WriteAllText(path, result.Content, _defaultEncoding);

        WriteLine($"Documentation root successfully generated to '{path}'.", Verbosity.Minimal);

        return CommandResults.Success;
    }
}
