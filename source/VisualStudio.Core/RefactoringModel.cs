// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.VisualStudio
{
    public class RefactoringModel
    {
        public RefactoringModel(string id, string title, bool enabled)
        {
            Id = id;
            Title = title;
            Enabled = enabled;
        }

        public string Id { get; }

        public string Title { get; }

        public bool Enabled { get; set; }
    }
}
