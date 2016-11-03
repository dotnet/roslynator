// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Roslynator.CSharp.Refactorings;
using Roslynator.VisualStudio.TypeConverters;

namespace Roslynator.VisualStudio
{
    public partial class GeneralOptionsPage : DialogPage
    {
        [Category("General")]
        [DisplayName("Prefix field identifier with underscore")]
        [Description("")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool PrefixFieldIdentifierWithUnderscore { get; set; }

        public void Apply()
        {
            DefaultCodeRefactoringProvider.DefaultSettings.PrefixFieldIdentifierWithUnderscore = PrefixFieldIdentifierWithUnderscore;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
                Apply();

            base.OnApply(e);
        }
    }
}
