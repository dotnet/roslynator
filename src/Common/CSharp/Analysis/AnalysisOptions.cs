// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analysis
{
    internal class AnalysisOptions
    {
        public AnalysisOptions(bool canContainDirectives = false, bool canContainComments = true)
        {
            CanContainDirectives = canContainDirectives;
            CanContainComments = canContainComments;
        }

        public bool CanContainDirectives { get; }

        public bool CanContainComments { get; }
    }
}
