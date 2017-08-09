// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roslynator.Metadata
{
    public class CodeFixDescriptor
    {
        public CodeFixDescriptor(
            string id,
            string identifier,
            string title,
            bool isEnabledByDefault,
            IList<string> fixableDiagnosticIds)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            FixableDiagnosticIds = new ReadOnlyCollection<string>(fixableDiagnosticIds);
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public bool IsEnabledByDefault { get; }

        public ReadOnlyCollection<string> FixableDiagnosticIds { get; }
    }
}
