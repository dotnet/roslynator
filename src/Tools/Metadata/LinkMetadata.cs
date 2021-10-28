// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Roslynator.Metadata
{
    public readonly struct LinkMetadata
    {
        public LinkMetadata(string url, string text = null, string title = null)
        {
            Url = url;
            Text = text;
            Title = title;
        }

        public string Url { get; }

        public string Text { get; }

        public string Title { get; }
    }
}
