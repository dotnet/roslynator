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
        protected override UIElement Child => Control;

        [Browsable(false)]
        public string LastMaxId { get; set; }

        protected abstract string DisabledByDefault { get; }

        protected abstract string MaxId { get; }

        protected HashSet<string> DisabledItems { get; } = new HashSet<string>();

        internal BaseOptionsPageControl Control { get; } = new BaseOptionsPageControl();

        public bool IsLoaded { get; private set; }

        protected abstract void Fill(ICollection<BaseModel> items);

        public void Load()
        {
            if (!IsLoaded)
            {
                Fill(Control.Items);
                IsLoaded = true;
            }
        }

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

            Load();
        }

        protected override void OnClosed(EventArgs e)
        {
            IsLoaded = false;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                OnApply();
            }

            base.OnApply(e);
        }

        protected virtual void OnApply()
        {
            foreach (BaseModel model in Control.Items)
                SetIsEnabled(model.Id, model.Enabled);
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
