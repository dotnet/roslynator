// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Roslynator.Configuration;
using Roslynator.VisualStudio.TypeConverters;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class GeneralOptionsPage : UIElementDialogPage
    {
        private bool _isActive;

        private readonly GeneralOptionsPageControl _control = new GeneralOptionsPageControl();

        protected override UIElement Child
        {
            get { return _control; }
        }

        [Category("General")]
        [DisplayName("Prefix field identifier with underscore")]
        [Description("")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool PrefixFieldIdentifierWithUnderscore { get; set; }

        [Category("General")]
        [Browsable(false)]
        public string ApplicationVersion { get; set; }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (!_isActive)
            {
                _control.PrefixFieldIdentifierWithUnderscore = PrefixFieldIdentifierWithUnderscore;
                _isActive = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _isActive = false;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                PrefixFieldIdentifierWithUnderscore = _control.PrefixFieldIdentifierWithUnderscore;
            }

            base.OnApply(e);
        }

        internal void UpdateConfig()
        {
            CodeAnalysisConfig.UpdateVisualStudioConfig(f => f.WithPrefixfieldIdentifierWithUnderscore(PrefixFieldIdentifierWithUnderscore));
        }
    }
}
