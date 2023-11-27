// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Text.RegularExpressions;
using DotMarkdown;

namespace Roslynator.CommandLine.Documentation;

internal class DocumentationWriter : MarkdownDocumentationWriter
{
    private static readonly Regex _optionValuesRegex = new(
        @"
Allowed\ value(\ is|s\ are)\ 
(?<x>[\w[\]-]+)(\ \(default\))?
(
    (,|\ and|\ or)\ 
    (?<x>[\w[\]-]+)(\ \(default\))?
)*
",
        RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

    public DocumentationWriter(MarkdownWriter writer) : base(writer)
    {
    }

    public override void WriteOptionDescription(CommandOption option)
    {
        string description = option.Description;

        if (!string.IsNullOrEmpty(description))
        {
            Match match = _optionValuesRegex.Match(description);

            if (match.Success)
            {
                int prevEndIndex = 0;

                foreach (Capture capture in match.Groups["x"].Captures)
                {
                    _writer.WriteString(description.Substring(prevEndIndex, capture.Index - prevEndIndex));
                    _writer.WriteInlineCode(description.Substring(capture.Index, capture.Length));
                    prevEndIndex = capture.Index + capture.Length;
                }

                _writer.WriteString(description.Substring(match.Index + match.Length));
            }
            else
            {
            _writer.WriteString(description);
            }
        }

        if (!string.IsNullOrEmpty(option.AdditionalDescription))
            _writer.WriteRaw(option.AdditionalDescription);
    }
}
