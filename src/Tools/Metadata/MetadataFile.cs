// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Roslynator.Metadata;

public static class MetadataFile
{
    private static readonly Regex _lfWithoutCr = new(@"(?<!\r)\n");

    private static string NormalizeNewLine(this string value)
    {
        return (value is not null) ? _lfWithoutCr.Replace(value, "\r\n") : null;
    }

    public static IEnumerable<AnalyzerMetadata> ReadAnalyzers(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);

        foreach (XElement element in doc.Root.Elements())
        {
            string id = element.Element("Id").Value;
            string title = element.Element("Title").Value;
            string identifier = element.Attribute("Identifier")?.Value ?? element.Element("Identifier").Value;
            string messageFormat = element.Element("MessageFormat")?.Value ?? title;
            string category = element.Element("Category")?.Value ?? "Roslynator";
            string defaultSeverity = element.Element("DefaultSeverity").Value;
            var isEnabledByDefault = bool.Parse(element.Element("IsEnabledByDefault").Value);
            bool supportsFadeOut = element.ElementValueAsBooleanOrDefault("SupportsFadeOut");
            bool supportsFadeOutAnalyzer = element.ElementValueAsBooleanOrDefault("SupportsFadeOutAnalyzer");
            string minLanguageVersion = element.Element("MinLanguageVersion")?.Value;
            string summary = element.Element("Summary")?.Value.NormalizeNewLine();
            string remarks = element.Element("Remarks")?.Value.NormalizeNewLine();
            IEnumerable<SampleMetadata> samples = LoadSamples(element)?.Select(f => f with { Before = f.Before.Replace("[|Id|]", id) });
            IEnumerable<LinkMetadata> links = LoadLinks(element);
            IEnumerable<LegacyAnalyzerOptionMetadata> options = LoadOptions(element, id);
            IEnumerable<AnalyzerConfigOption> configOptions = LoadConfigOptions(element);
            string[] tags = (element.Element("Tags")?.Value ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            AnalyzerStatus status = ParseStatus(element);
            string obsoleteMessage = element.Element("ObsoleteMessage")?.Value;

            if (obsoleteMessage is not null)
                messageFormat = $"([deprecated] {obsoleteMessage}) {messageFormat}";

            var analyzer = new AnalyzerMetadata()
            {
                Id = id,
                Identifier = identifier,
                Title = title,
                MessageFormat = messageFormat,
                Category = category,
                DefaultSeverity = defaultSeverity,
                IsEnabledByDefault = isEnabledByDefault,
                SupportsFadeOut = supportsFadeOut,
                SupportsFadeOutAnalyzer = supportsFadeOutAnalyzer,
                MinLanguageVersion = minLanguageVersion,
                Summary = summary,
                Remarks = remarks,
                Status = status,
                ObsoleteMessage = obsoleteMessage,
            };

            analyzer.Tags.AddRange(tags);
            analyzer.ConfigOptions.AddRange(configOptions ?? Enumerable.Empty<AnalyzerConfigOption>());
            analyzer.Samples.AddRange(samples ?? Enumerable.Empty<SampleMetadata>());
            analyzer.Links.AddRange(links ?? Enumerable.Empty<LinkMetadata>());
            analyzer.LegacyOptions.AddRange(options ?? Enumerable.Empty<LegacyAnalyzerOptionMetadata>());
            analyzer.LegacyOptionAnalyzers.AddRange(analyzer.LegacyOptions.Select(f => f.CreateAnalyzerMetadata(analyzer)));

            yield return analyzer;
        }
    }

    private static AnalyzerStatus ParseStatus(XElement element)
    {
        var status = AnalyzerStatus.Enabled;

        XAttribute isObsoleteAttribute = element.Attribute("IsObsolete");
        if (isObsoleteAttribute is not null)
        {
            status = (bool.Parse(isObsoleteAttribute.Value)) ? AnalyzerStatus.Disabled : AnalyzerStatus.Enabled;
        }

        if (status == AnalyzerStatus.Enabled)
        {
            XElement statusElement = element.Element("Status");

            if (statusElement is not null)
                status = Enum.Parse<AnalyzerStatus>(statusElement.Value);
        }

        return status;
    }

    public static IEnumerable<RefactoringMetadata> ReadRefactorings(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);

        foreach (XElement element in doc.Root.Elements())
        {
            var refactoring = new RefactoringMetadata()
            {
                Id = element.Attribute("Id")?.Value,
                Identifier = element.Attribute("Identifier").Value,
                OptionKey = element.Element("OptionKey")?.Value,
                Title = element.Attribute("Title").Value,
                Span = element.Element("Span")?.Value,
                Summary = element.Element("Summary")?.Value.NormalizeNewLine(),
                Remarks = element.Element("Remarks")?.Value.NormalizeNewLine(),
                IsEnabledByDefault = element.AttributeValueAsBooleanOrDefault("IsEnabledByDefault", true),
                IsObsolete = element.AttributeValueAsBooleanOrDefault("IsObsolete", false),
            };

            refactoring.Syntaxes.AddRange(element.Element("Syntaxes").Elements("Syntax").Select(f => new SyntaxMetadata(f.Value)));
            refactoring.Samples.AddRange(LoadSamples(element) ?? Enumerable.Empty<SampleMetadata>());
            refactoring.Links.AddRange(LoadLinks(element) ?? Enumerable.Empty<LinkMetadata>());

            yield return refactoring;
        }
    }

    private static IEnumerable<SampleMetadata> LoadSamples(XElement element)
    {
        return element
            .Element("Samples")?
            .Elements("Sample")
            .Select(f =>
            {
                XElement before = f.Element("Before");
                XElement after = f.Element("After");

                ImmutableArray<(string, string)> options = f.Element("ConfigOptions")?
                    .Elements("Option")
                    .Select(f => (f.Attribute("Key").Value, f.Attribute("Value").Value))
                    .ToImmutableArray()
                    ?? ImmutableArray<(string, string)>.Empty;

                return new SampleMetadata()
                {
                    Before = before.Value.NormalizeNewLine(),
                    After = after?.Value.NormalizeNewLine(),
                    ConfigOptions = options,
                };
            });
    }

    private static IEnumerable<LinkMetadata> LoadLinks(XElement element)
    {
        return element
            .Element("Links")?
            .Elements("Link")
            .Select(f => new LinkMetadata() { Url = f.Element("Url").Value, Text = f.Element("Text")?.Value, Title = f.Element("Title")?.Value });
    }

    private static IEnumerable<LegacyAnalyzerOptionMetadata> LoadOptions(XElement element, string parentId)
    {
        return element
            .Element("Options")?
            .Elements("Option")
            .Select(f => LoadOption(f, parentId));
    }

    private static IEnumerable<AnalyzerConfigOption> LoadConfigOptions(XElement element)
    {
        return element
            .Element("ConfigOptions")?
            .Elements("Option")
            .Select(f => new AnalyzerConfigOption("roslynator_" + f.Attribute("Key").Value, bool.Parse(f.Attribute("IsRequired")?.Value ?? bool.FalseString)));
    }

    private static LegacyAnalyzerOptionMetadata LoadOption(XElement element, string parentId)
    {
        string title = element.Element("Title").Value;

        string identifier = element.Attribute("Identifier").Value;
        string id = element.Element("Id")?.Value;
        string optionKey = element.Element("OptionKey").Value;
        string optionValue = element.Element("OptionValue")?.Value;
        var kind = (LegacyAnalyzerOptionKind)Enum.Parse(typeof(LegacyAnalyzerOptionKind), element.Element("Kind").Value);
        bool isEnabledByDefault = element.ElementValueAsBooleanOrDefault("IsEnabledByDefault");
        bool supportsFadeOut = element.ElementValueAsBooleanOrDefault("SupportsFadeOut");
        string minLanguageVersion = element.Element("MinLanguageVersion")?.Value;
        string summary = element.Element("Summary")?.Value.NormalizeNewLine();
        IEnumerable<SampleMetadata> samples = LoadSamples(element)?.Select(f => f with { Before = f.Before.Replace("[|Id|]", parentId) });
        bool isObsolete = element.AttributeValueAsBooleanOrDefault("IsObsolete");
        string[] tags = (element.Element("Tags")?.Value ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        AnalyzerStatus status = ParseStatus(element);

        string newOptionKey = element.Element("NewOptionKey")?.Value;

        if (newOptionKey?.StartsWith("roslynator_") == false)
            newOptionKey = "roslynator_" + newOptionKey;

        var analyzerOption = new LegacyAnalyzerOptionMetadata()
        {
            Identifier = identifier,
            Id = id,
            ParentId = parentId,
            OptionKey = optionKey,
            OptionValue = optionValue,
            NewOptionKey = newOptionKey,
            Kind = kind,
            Title = title,
            IsEnabledByDefault = isEnabledByDefault,
            SupportsFadeOut = supportsFadeOut,
            MinLanguageVersion = minLanguageVersion,
            Summary = summary,
            Status = status,
        };

        if (samples is not null)
            analyzerOption.Samples.AddRange(samples);

        analyzerOption.Tags.AddRange(tags);

        return analyzerOption;
    }

    public static IEnumerable<CodeFixMetadata> ReadCodeFixes(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);

        foreach (XElement element in doc.Root.Elements())
        {
            var fix = new CodeFixMetadata(
                element.Attribute("Id").Value,
                element.Attribute("Identifier").Value,
                element.Attribute("Title").Value,
                element.AttributeValueAsBooleanOrDefault("IsEnabledByDefault", true),
                element.AttributeValueAsBooleanOrDefault("IsObsolete", false));

            fix.FixableDiagnosticIds.AddRange(element.Element("FixableDiagnosticIds")
                .Elements("Id")
                .Select(f => f.Value));

            yield return fix;
        }
    }

    public static IEnumerable<CompilerDiagnosticMetadata> ReadCompilerDiagnostics(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);

        foreach (XElement element in doc.Root.Elements("Diagnostic"))
        {
            yield return new CompilerDiagnosticMetadata(
                element.Attribute("Id").Value,
                element.Attribute("Identifier").Value,
                element.Attribute("Title").Value,
                element.Attribute("Message").Value,
                element.Attribute("Severity").Value,
                element.Attribute("HelpUrl").Value);
        }
    }

    public static IEnumerable<AnalyzerOptionMetadata> ReadOptions(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);

        foreach (XElement element in doc.Root.Elements("Option"))
        {
            string id = element.Attribute("Id").Value;

            string key = (element.Element("Key")?.Value)
                ?? string.Join("_", Regex.Split(id, @"(?<=\p{Ll})(?=\p{Lu})").Select(f => f.ToLowerInvariant()));

            IEnumerable<AnalyzerOptionValueMetadata> values = element.Element("Values")?
                .Elements("Value")
                .Select(e =>
                {
                    bool.TryParse(e.Attribute("IsDefault")?.Value, out bool isDefault);
                    return new AnalyzerOptionValueMetadata(e.Value, isDefault);
                })
                ?? Enumerable.Empty<AnalyzerOptionValueMetadata>();

            string defaultValue = element.Element("DefaultValue")?.Value ?? values.FirstOrDefault(f => f.IsDefault).Value;
            string defaultValuePlaceholder = element.Element("ValuePlaceholder")?.Value ?? string.Join("|", values.Select(f => f.Value).OrderBy(f => f));

            var analyzerOption = new AnalyzerOptionMetadata(
                Id: id,
                Key: "roslynator_" + key,
                DefaultValue: defaultValue,
                DefaultValuePlaceholder: defaultValuePlaceholder,
                Description: element.Element("Description").Value
            );

            analyzerOption.Values.AddRange(values ?? Enumerable.Empty<AnalyzerOptionValueMetadata>());

            yield return analyzerOption;
        }
    }

    public static void SaveCompilerDiagnostics(IEnumerable<CompilerDiagnosticMetadata> diagnostics, string path)
    {
        var doc = new XDocument(
            new XElement(
                "Diagnostics",
                diagnostics.Select(f =>
                {
                    return new XElement(
                        "Diagnostic",
                        new XAttribute("Id", f.Id),
                        new XAttribute("Identifier", f.Identifier),
                        new XAttribute("Severity", f.Severity ?? ""),
                        new XAttribute("Title", f.Title),
                        new XAttribute("Message", f.MessageFormat ?? ""),
                        new XAttribute("HelpUrl", f.HelpUrl ?? "")
                        );
                })));

        using (var fs = new FileStream(path, FileMode.Create))
        using (XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true, NewLineOnAttributes = true }))
            doc.Save(xw);
    }

    public static void CleanAnalyzers(string filePath)
    {
        Debug.WriteLine(filePath);

        XDocument doc = XDocument.Load(filePath);

        foreach (XElement element in doc.Root.Elements())
        {
            string title = element.Element("Title").Value;

            XElement messageFormatElement = element.Element("MessageFormat");

            if (messageFormatElement is not null)
            {
                string messageFormat = messageFormatElement.Value;

                if (string.IsNullOrWhiteSpace(messageFormat)
                    || string.Equals(title, messageFormat, System.StringComparison.Ordinal))

                {
                    messageFormatElement.Remove();
                }
            }

            XElement supportsFadeOutElement = element.Element("SupportsFadeOut");

            if (supportsFadeOutElement is not null
                && !bool.Parse(supportsFadeOutElement.Value))
            {
                supportsFadeOutElement.Remove();
            }

            XElement supportsFadeOutAnalyzerElement = element.Element("SupportsFadeOutAnalyzer");

            if (supportsFadeOutAnalyzerElement is not null
                && !bool.Parse(supportsFadeOutAnalyzerElement.Value))
            {
                supportsFadeOutAnalyzerElement.Remove();
            }

            XElement minLanguageVersionElement = element.Element("MinLanguageVersion");

            if (minLanguageVersionElement is not null
                && string.IsNullOrWhiteSpace(minLanguageVersionElement.Value))
            {
                minLanguageVersionElement.Remove();
            }

            XElement summaryElement = element.Element("Summary");

            if (summaryElement is not null
                && string.IsNullOrWhiteSpace(summaryElement.Value))
            {
                summaryElement.Remove();
            }

            XElement remarksElement = element.Element("Remarks");

            if (remarksElement is not null
                && string.IsNullOrWhiteSpace(remarksElement.Value))
            {
                remarksElement.Remove();
            }

            XElement configurationElement = element.Element("Configuration");

            if (configurationElement is not null
                && string.IsNullOrWhiteSpace(configurationElement.Value))
            {
                configurationElement.Remove();
            }

            XElement samplesElement = element.Element("Samples");

            if (samplesElement is not null)
            {
                foreach (XElement sampleElement in samplesElement.Elements("Sample"))
                {
                    XElement beforeElement = sampleElement.Element("Before");
                    XElement afterElement = sampleElement.Element("After");

                    if (string.IsNullOrEmpty(beforeElement?.Value)
                        && string.IsNullOrEmpty(afterElement?.Value))
                    {
                        sampleElement.Remove();
                        continue;
                    }

                    if (beforeElement is not null)
                    {
                        string before = beforeElement.Value;

                        if (!before.Contains('\n'))
                        {
                            string newBefore = Regex.Replace(before, @"\ *//\ *\[\|Id\|\]\ *\z", "");

                            if (before.Length != newBefore.Length)
                                beforeElement.ReplaceNodes(new XCData(newBefore));
                        }
                    }
                }

                if (samplesElement.IsEmpty)
                    samplesElement.Remove();
            }

            XElement linksElement = element.Element("Links");

            if (linksElement is not null)
            {
                foreach (XElement linkElement in linksElement.Elements("Link"))
                {
                    if (string.IsNullOrWhiteSpace(linkElement.Value))
                        linkElement.Remove();
                }
            }
        }

        using (var sw = new StreamWriter(filePath))
        using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true, Encoding = Encoding.UTF8 }))
            doc.Save(xw);
    }
}
