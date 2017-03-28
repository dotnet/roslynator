// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Roslynator.CSharp.Refactorings;
using Roslynator.VisualStudio.TypeConverters;

namespace Roslynator.VisualStudio
{
    public class GeneralOptionsPage : UIElementDialogPage
    {
        public GeneralOptionsPage()
        {
            PrefixFieldIdentifierWithUnderscore = true;
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
        [Browsable(false)]
        public string ApplicationVersion { get; set; }

        public void ApplyTo(RefactoringSettings settings)
        {
            settings.PrefixFieldIdentifierWithUnderscore = PrefixFieldIdentifierWithUnderscore;
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            _control.PrefixFieldIdentifierWithUnderscore = PrefixFieldIdentifierWithUnderscore;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                PrefixFieldIdentifierWithUnderscore = _control.PrefixFieldIdentifierWithUnderscore;
                ApplyTo(RefactoringSettings.Current);
            }

            base.OnApply(e);
        }
    }
}
