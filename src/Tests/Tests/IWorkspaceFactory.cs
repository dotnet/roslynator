// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public interface IWorkspaceFactory
    {
        string Language { get; }

        Document Document(string source, params string[] additionalSources);

        Project Project(string source);

        Project Project(IEnumerable<string> sources);
    }
}