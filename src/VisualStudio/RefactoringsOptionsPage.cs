// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Roslynator.Configuration;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public partial class RefactoringsOptionsPage : BaseOptionsPage
    {
        private const string RefactoringCategory = "Refactoring";

        [Category(RefactoringCategory)]
        [Browsable(false)]
        public string DisabledRefactorings { get; set; }

        [Category(RefactoringCategory)]
        [Browsable(false)]
        public string Refactorings
        {
            get { return string.Join(",", Items.Select(f => (f.Value) ? f.Key : (f.Key + "!"))); }
            set
            {
                Items.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    foreach (string id in value.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            if (id.EndsWith("!"))
                            {
                                if (id.Length > 1)
                                    Items[id.Remove(id.Length - 1)] = false;
                            }
                            else
                            {
                                Items[id] = true;
                            }
                        }
                    }
                }
            }
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                UpdateConfig();
            }
        }

        internal void UpdateConfig()
        {
            CodeAnalysisConfig.UpdateVisualStudioConfig(f => f.WithRefactorings(GetItems()));
        }
    }
}
