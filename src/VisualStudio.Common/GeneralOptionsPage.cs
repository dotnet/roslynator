// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Roslynator.VisualStudio.TypeConverters;
using Roslynator.Configuration;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class GeneralOptionsPage : UIElementDialogPage
    {
        private bool _isActive;

        public GeneralOptionsPage()
        {
            PrefixFieldIdentifierWithUnderscore = true;
            UseConfigFile = true;
        }

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
        [DisplayName("UseConfigFile")]
        [Description("")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool UseConfigFile { get; set; }

        [Category("General")]
        [Browsable(false)]
        public string ApplicationVersion { get; set; }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (!_isActive)
            {
                _control.PrefixFieldIdentifierWithUnderscore = PrefixFieldIdentifierWithUnderscore;
                _control.UseConfigFile = UseConfigFile;
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
                UseConfigFile = _control.UseConfigFile;

                SettingsManager.Instance.UpdateVisualStudioSettings(this);
                SettingsManager.Instance.ApplyTo(RefactoringSettings.Current);
            }

            base.OnApply(e);
        }
    }
}
