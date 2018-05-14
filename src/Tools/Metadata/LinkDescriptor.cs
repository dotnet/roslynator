// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Roslynator.Metadata
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public readonly struct LinkDescriptor
    {
        public LinkDescriptor(string url, string text = null, string title = null)
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
