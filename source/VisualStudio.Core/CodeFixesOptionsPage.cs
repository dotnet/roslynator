// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Roslynator.Configuration;
using Roslynator.CSharp.CodeFixes;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("B804AA29-91D5-4C54-9B76-C442DA0AE60D")]
    public partial class CodeFixesOptionsPage : UIElementDialogPage
    {
        private bool _isActive;
        private readonly BaseOptionsPageControl _control = new  BaseOptionsPageControl();
        private readonly HashSet<string> _disabledCodeFixes = new HashSet<string>();

        protected override UIElement Child
        {
            get { return _control; }
        }

        [Browsable(false)]
        public string DisabledCodeFixes
        {
            get { return string.Join(",", _disabledCodeFixes); }
            set
            {
                _disabledCodeFixes.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    foreach (string id in value.Split(','))
                        _disabledCodeFixes.Add(id);
                }
            }
        }

        internal IEnumerable<string> GetDisabledCodeFixes()
        {
            foreach (string id in _disabledCodeFixes)
                yield return id;
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (!_isActive)
            {
                Fill(_control.Items);
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
                foreach (BaseModel codeFix in _control.Items)
                    SetIsEnabled(codeFix.Id, codeFix.Enabled);

                SettingsManager.Instance.UpdateVisualStudioSettings(this);
                SettingsManager.Instance.ApplyTo(CodeFixSettings.Current);
            }

            base.OnApply(e);
        }

        private void SetIsEnabled(string id, bool isEnabled)
        {
            if (isEnabled)
            {
                _disabledCodeFixes.Remove(id);
            }
            else
            {
                _disabledCodeFixes.Add(id);
            }
        }

        private bool IsEnabled(string id)
        {
            return !_disabledCodeFixes.Contains(id);
        }
    }
}
