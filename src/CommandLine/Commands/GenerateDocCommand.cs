// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        SymbolTitleParts ignoredTitleParts,
        OmitMemberParts omitMemberParts,
        IncludeContainingNamespaceFilter includeContainingNamespaceFilter,
        Visibility visibility,
        DocumentationHost documentationHost,
        FilesLayout filesLayout,
        bool groupByCommonNamespace,
        InheritanceStyle inheritanceStyle,
        in ProjectFilter projectFilter,
        FileSystemFilter fileSystemFilter) : base(projectFilter, fileSystemFilter)
    {
        Options = options;
        Depth = depth;
        IgnoredRootParts = ignoredRootParts;
        IgnoredNamespaceParts = ignoredNamespaceParts;
        IgnoredTypeParts = ignoredTypeParts;
        IgnoredMemberParts = ignoredMemberParts;
        IgnoredCommonParts = ignoredCommonParts;
        OmitMemberParts = omitMemberParts;
        IgnoredTitleParts = ignoredTitleParts;
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

    public SymbolTitleParts IgnoredTitleParts { get; }

    public IncludeContainingNamespaceFilter IncludeContainingNamespaceFilter { get; }

    public Visibility Visibility { get; }

    public DocumentationHost DocumentationHost { get; }

    public FilesLayout FilesLayout { get; }

    public bool GroupByCommonNamespace { get; }

    public InheritanceStyle InheritanceStyle { get; }

    public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
    {
        AssemblyResolver.Register();

        var documentationOptions = new DocumentationOptions()
        {
            RootFileHeading = Options.Heading,
            PreferredCultureName = Options.PreferredCulture,
            MaxDerivedTypes = Options.MaxDerivedTypes,
            PlaceSystemNamespaceFirst = !Options.NoPrecedenceForSystem,
            WrapDeclarationBaseTypes = !Options.NoWrapBaseTypes,
            WrapDeclarationConstraints = !Options.NoWrapConstraints,
            MarkObsolete = !Options.NoMarkObsolete,
            IncludeMemberInheritedFrom = (OmitMemberParts & OmitMemberParts.InheritedFrom) == 0,
            IncludeMemberOverrides = (OmitMemberParts & OmitMemberParts.Overrides) == 0,
            IncludeMemberImplements = (OmitMemberParts & OmitMemberParts.Implements) == 0,
            IncludeMemberConstantValue = (OmitMemberParts & OmitMemberParts.ConstantValue) == 0,
            IncludeInheritedInterfaceMembers = Options.IncludeInheritedInterfaceMembers,
            IncludeAllDerivedTypes = Options.IncludeAllDerivedTypes,
            IncludeAttributeArguments = !Options.OmitAttributeArguments,
            IncludeInheritedAttributes = !Options.OmitInheritedAttributes,
            OmitIEnumerable = !Options.IncludeIEnumerable,
            Depth = Depth,
            InheritanceStyle = InheritanceStyle,
            IgnoredRootParts = IgnoredRootParts,
            IgnoredNamespaceParts = IgnoredNamespaceParts,
            IgnoredTypeParts = IgnoredTypeParts,
            IgnoredMemberParts = IgnoredMemberParts,
            IgnoredCommonParts = IgnoredCommonParts,
            IgnoredTitleParts = IgnoredTitleParts,
            IncludeContainingNamespaceFilter = IncludeContainingNamespaceFilter,
            FilesLayout = FilesLayout,
            ScrollToContent = (DocumentationHost == DocumentationHost.GitHub) && Options.ScrollToContent,
            FileSystemFilter = FileSystemFilter,
        };

        if (Options.IgnoredNames is not null)
        {
            foreach (string ignoredName in Options.IgnoredNames)
                documentationOptions.IgnoredNames.Add(MetadataName.Parse(ignoredName));
        }

        ImmutableArray<Compilation> compilations = await GetCompilationsAsync(projectOrSolution, cancellationToken);

        var documentationModel = new DocumentationModel(compilations, DocumentationFilterOptions.Instance, Options.AdditionalXmlDocumentation);

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

        GenerateRootFile(generator);

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

        IEnumerable<DocumentationGeneratorResult> results = generator.Generate(heading: Options.Heading, cancellationToken);

        if (DocumentationHost == DocumentationHost.Sphinx)
        {
            List<DocumentationGeneratorResult> resultList = results.ToList();
            AddTableOfContents(resultList);
            results = resultList;
        }

        foreach (DocumentationGeneratorResult documentationFile in results)
        {
            string path = Path.Combine(directoryPath, documentationFile.FilePath);

            WriteLine($"  Save '{path}'", ConsoleColors.DarkGray, Verbosity.Detailed);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllText(path, documentationFile.Content, _defaultEncoding);
        }

        WriteLine($"Documentation successfully generated to '{Options.Output}'.", Verbosity.Minimal);

        return CommandResults.Success;
    }

    private void GenerateRootFile(DocumentationGenerator generator)
    {
        if (IgnoredRootParts == RootDocumentationParts.All)
        {
            WriteLine("Skipping generation of documentation root file.", Verbosity.Minimal);
            return;
        }

        string outputDirectoryPath = Path.GetFullPath(Options.Output);

        string rootFilePath = (!string.IsNullOrEmpty(Options.RootFilePath))
            ? Options.RootFilePath
            : Path.Combine(outputDirectoryPath, generator.Context.UrlProvider.IndexFileName);

        rootFilePath = Path.GetFullPath(rootFilePath);

        generator.Options.RootDirectoryUrl = FileSystemHelpers.DetermineRelativePath(Options.Output, rootFilePath);

        WriteLine($"Relative path from root file to output directory is '{generator.Options.RootDirectoryUrl}'.", Verbosity.Detailed);

        WriteLine($"Generate documentation root to '{rootFilePath}'.", Verbosity.Minimal);

        DocumentationGeneratorResult result = generator.GenerateRoot(Options.Heading ?? "");

        Directory.CreateDirectory(Path.GetDirectoryName(rootFilePath));
        File.WriteAllText(rootFilePath, result.Content, _defaultEncoding);

        WriteLine($"Documentation root successfully generated to '{rootFilePath}'.", Verbosity.Minimal);
    }

    private static void AddTableOfContents(IEnumerable<DocumentationGeneratorResult> results)
    {
        foreach (DocumentationGeneratorResult result in results)
        {
            string content = result.Content;
            string filePath = result.FilePath;
            string directoryPath = Path.GetDirectoryName(filePath);

            IEnumerable<DocumentationGeneratorResult> children = results.Where(r =>
            {
                if (r != result)
                {
                    string path = r.FilePath;

                    if (path.StartsWith(directoryPath))
                    {
                        string relativePath = path.Substring(directoryPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                        if (relativePath.Count(f => f == Path.DirectorySeparatorChar || f == Path.AltDirectorySeparatorChar) == 1)
                        {
                            return true;
                        }
                    }
                }

                return false;
            });

            if (children.Any())
            {
                var sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine("```{toctree}");
                sb.AppendLine(":hidden:");
                sb.AppendLine(":maxdepth: 1");
                sb.AppendLine();

                foreach (DocumentationGeneratorResult child in children
                    .OrderBy(child => child.Label))
                {
                    Debug.Assert(child.Label is not null);

                    sb.Append(child.Label);
                    sb.Append(" <");

                    sb.Append(child.FilePath
                        .Substring(directoryPath.Length)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Replace('\\', '/'));

                    sb.AppendLine(">");
                }

                sb.AppendLine("```");

                result.Content += sb.ToString();
            }
        }
    }
}
