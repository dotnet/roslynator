// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.VisualStudio.Shell;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.VisualStudio
{
    public partial class RefactoringsOptionsPage : DialogPage
    {
        private const string RefactoringCategory = "Refactoring";

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
                Apply();

            base.OnApply(e);
        }

        private static void SetIsEnabled(string identifier, bool isEnabled)
        {
            DefaultCodeRefactoringProvider.DefaultSettings.SetIsRefactoringEnabled(identifier, isEnabled);
        }
    }
}
