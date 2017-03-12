// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public partial class RefactoringsOptionsPage : UIElementDialogPage
    {
        private const string RefactoringCategory = "Refactoring";

        private RefactoringsControl _refactoringsControl = new RefactoringsControl();
        private HashSet<string> _disabledRefactorings = new HashSet<string>();

        protected override UIElement Child
        {
            get { return _refactoringsControl; }
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
        }

        [Category(RefactoringCategory)]
        [Browsable(false)]
        public string DisabledRefactorings
        {
            get { return string.Join(",", _disabledRefactorings); }
            set
            {
                _disabledRefactorings.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    foreach (string id in value.Split(','))
                        _disabledRefactorings.Add(id);
                }
            }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            Fill(_refactoringsControl.Refactorings);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                foreach (RefactoringModel refactoring in _refactoringsControl.Refactorings)
                    SetIsEnabled(refactoring.Id, refactoring.Enabled);

                ApplyTo(RefactoringSettings.Current);
            }

            base.OnApply(e);
        }

        private void SetIsEnabled(string id, bool isEnabled)
        {
            if (isEnabled)
            {
                _disabledRefactorings.Remove(id);
            }
            else
            {
                _disabledRefactorings.Add(id);
            }
        }

        private bool IsEnabled(string id)
        {
            return !_disabledRefactorings.Contains(id);
        }
    }
}
