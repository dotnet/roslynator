// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Roslynator.Documentation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct DocumentationGeneratorResult : IEquatable<DocumentationGeneratorResult>
    {
        public DocumentationGeneratorResult(string content, string filePath, DocumentationFileKind kind)
        {
            Content = content;
            FilePath = filePath;
            Kind = kind;
        }

        public string Content { get; }

        public string FilePath { get; }

        public DocumentationFileKind Kind { get; }

        internal bool HasContent
        {
            get { return !string.IsNullOrWhiteSpace(Content); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{Kind} {FilePath} {Content}"; }
        }

        public override bool Equals(object obj)
        {
            return obj is DocumentationGeneratorResult other && Equals(other);
        }

        public bool Equals(DocumentationGeneratorResult other)
        {
            return Kind == other.Kind
                && FilePath == other.FilePath
                && Content == other.Content;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(StringComparer.Ordinal.GetHashCode(Content),
                Hash.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(FilePath), (int)Kind));
        }

        public static bool operator ==(in DocumentationGeneratorResult file1, in DocumentationGeneratorResult file2)
        {
            return file1.Equals(file2);
        }

        public static bool operator !=(in DocumentationGeneratorResult file1, in DocumentationGeneratorResult file2)
        {
            return !(file1 == file2);
        }
    }
}
