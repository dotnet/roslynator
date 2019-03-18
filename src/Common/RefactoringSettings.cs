// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public sealed class RefactoringSettings : CodeAnalysisSettings<string>
    {
        public static RefactoringSettings Current { get; } = new RefactoringSettings();

        public bool PrefixFieldIdentifierWithUnderscore { get; set; } = true;

        public override void Reset()
        {
            PrefixFieldIdentifierWithUnderscore = true;
            Disabled.Clear();
        }
    }
}
