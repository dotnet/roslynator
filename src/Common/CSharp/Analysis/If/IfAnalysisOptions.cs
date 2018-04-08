// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analysis.If
{
    internal class IfAnalysisOptions : AnalysisOptions
    {
        public IfAnalysisOptions()
            : this(true, true, true, true)
        {
        }

        public IfAnalysisOptions(
            bool useCoalesceExpression,
            bool useConditionalExpression,
            bool useBooleanExpression,
            bool useExpression)
        {
            UseCoalesceExpression = useCoalesceExpression;
            UseConditionalExpression = useConditionalExpression;
            UseBooleanExpression = useBooleanExpression;
            UseExpression = useExpression;
        }

        public bool UseCoalesceExpression { get; }

        public bool UseConditionalExpression { get; }

        public bool UseBooleanExpression { get; }

        public bool UseExpression { get; }
    }
}
