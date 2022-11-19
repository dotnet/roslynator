// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator.Documentation;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DocumentationGeneratorResult
{
    public DocumentationGeneratorResult(string content, string filePath, DocumentationFileKind kind, string label)
    {
        Content = content;
        FilePath = filePath;
        Kind = kind;
        Label = label;
    }

    public string Content { get; internal set; }

    public string FilePath { get; }

    public DocumentationFileKind Kind { get; }

    public string Label { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Kind} {FilePath} {Content}";
}
