// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class CommandArgument
    {
        public CommandArgument(
            int index,
            string name,
            string description = null,
            bool isRequired = false)
        {
            Index = index;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            IsRequired = isRequired;
        }

        public int Index { get; }

        public string Name { get; }

        public string Description { get; }

        public bool IsRequired { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder();

                if (!IsRequired)
                    sb.Append("[");

                if (!string.IsNullOrEmpty(Name))
                {
                    sb.Append(Name);
                }
                else
                {
                    sb.Append(Index);
                }

                if (!IsRequired)
                    sb.Append("]");

                if (!string.IsNullOrEmpty(Description))
                {
                    sb.Append("  ");
                    sb.Append(Description);
                }

                return sb.ToString();
            }
        }
    }
}
