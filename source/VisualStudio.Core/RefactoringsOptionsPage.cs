// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Roslynator.Configuration;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public partial class RefactoringsOptionsPage : UIElementDialogPage
    {
        private const string RefactoringCategory = "Refactoring";

        private bool _isActive;
        private readonly RefactoringsOptionsPageControl _control = new RefactoringsOptionsPageControl();
        private readonly HashSet<string> _disabledRefactorings = new HashSet<string>();

        protected override UIElement Child
        {
            get { return _control; }
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

        internal IEnumerable<string> GetDisabledRefactorings()
        {
            foreach (string id in _disabledRefactorings)
                yield return id;
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (!_isActive)
            {
                Fill(_control.Refactorings);
                _isActive = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _isActive = false;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                foreach (RefactoringModel refactoring in _control.Refactorings)
                    SetIsEnabled(refactoring.Id, refactoring.Enabled);

                SettingsManager.Instance.UpdateVisualStudioSettings(this);
                SettingsManager.Instance.ApplyTo(RefactoringSettings.Current);
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
