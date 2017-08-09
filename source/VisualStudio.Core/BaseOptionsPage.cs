// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Roslynator.VisualStudio
{
    public abstract class BaseOptionsPage : UIElementDialogPage
    {
        private bool _isActive;

        private readonly BaseOptionsPageControl _control = new BaseOptionsPageControl();

        protected override UIElement Child
        {
            get { return _control; }
        }

        [Browsable(false)]
        public string LastMaxId { get; set; }

        protected abstract string DisabledByDefault { get; }

        protected abstract string MaxId { get; }

        protected HashSet<string> DisabledItems { get; } = new HashSet<string>();

        protected abstract void Fill(ICollection<BaseModel> codeFixes);

        internal IEnumerable<string> GetDisabledItems()
        {
            foreach (string id in DisabledItems)
                yield return id;
        }

        public void CheckNewItemsDisabledByDefault()
        {
            bool shouldSave = false;

            if (string.IsNullOrEmpty(LastMaxId))
            {
                if (DisabledItems.Count == 0)
                {
                    foreach (string id in DisabledByDefault.Split(','))
                        DisabledItems.Add(id);
                }

                shouldSave = true;
            }
            else if (string.Compare(LastMaxId, MaxId, StringComparison.Ordinal) < 0)
            {
                foreach (string id in DisabledByDefault
                    .Split(',')
                    .Where(f => string.Compare(LastMaxId, f, StringComparison.Ordinal) < 0))
                {
                    DisabledItems.Add(id);
                }

                shouldSave = true;
            }

            if (shouldSave)
            {
                LastMaxId = MaxId;
                SaveSettingsToStorage();
            }
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
                foreach (BaseModel item in _control.Items)
                    SetIsEnabled(item.Id, item.Enabled);
            }

            base.OnApply(e);
        }

        protected void SetIsEnabled(string id, bool isEnabled)
        {
            if (isEnabled)
            {
                DisabledItems.Remove(id);
            }
            else
            {
                DisabledItems.Add(id);
            }
        }

        protected bool IsEnabled(string id)
        {
            return !DisabledItems.Contains(id);
        }
    }
}
