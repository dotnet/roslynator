// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis
{
    [Flags]
    public enum BracesAnalysisResult
    {
        None = 0,
        AddBraces = 1,
        RemoveBraces = 2,
    }
}
