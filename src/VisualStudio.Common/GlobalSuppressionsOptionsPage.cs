// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Roslynator.Configuration;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("9851B486-A654-4F6A-B1BA-9E1071DCCA25")]
    public partial class GlobalSuppressionsOptionsPage : BaseOptionsPage
    {
        public GlobalSuppressionsOptionsPage()
        {
            Control.DefaultSortDirection = ListSortDirection.Descending;
            Control.CheckBoxColumnHeaderText = "Suppressed";
            Control.Comment = "IMPORTANT: This option suppresses diagnostics but it does not disable the analyzer. " +
                "It is highly recommended to use standard tool such as ruleset to disable the analyzer.\r\n\r\n" +
                "Global suppressions are not supported when full solution analysis is enabled!";
        }

        [Browsable(false)]
        public string SuppressedAnalyzers
        {
            get { return string.Join(",", DisabledItems); }
            set
            {
                DisabledItems.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    foreach (string id in value.Split(','))
                        DisabledItems.Add(id);
                }
            }
        }

        protected override string DisabledByDefault => "";

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                SettingsManager.Instance.UpdateVisualStudioSettings(this);
                SettingsManager.Instance.ApplyTo(AnalyzerSettings.Current);
            }
        }

        protected override void OnApply()
        {
            foreach (BaseModel item in Control.Items)
                SetIsEnabled(item.Id, !item.Enabled);
        }
    }
}
