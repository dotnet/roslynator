// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class KeyValuePairOptionValue : OptionValue
    {
        public KeyValuePairOptionValue(
            string key,
            string value,
            string shortKey,
            string helpValue,
            string description = null,
            bool hidden = false,
            bool canContainExpression = false)
            : base(key, helpValue, description, hidden, canContainExpression)
        {
            Key = key;
            ShortKey = shortKey;
            Value = value;
        }

        public override OptionValueKind Kind => OptionValueKind.KeyValuePair;

        public string Key { get; }

        public string ShortKey { get; }

        public string Value { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Kind}  {HelpValue}";

        public bool IsKeyOrShortKey(string value)
        {
            return !string.IsNullOrEmpty(value)
                && (value == Key || value == ShortKey);
        }

        public static KeyValuePairOptionValue Create(
            string key,
            string value,
            string shortKey = null,
            string helpValue = null,
            string description = null,
            bool hidden = false,
            bool canContainExpression = false)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            shortKey ??= key.Substring(0, 1);

            if (helpValue == null)
            {
                if (!string.IsNullOrEmpty(shortKey))
                {
                    if (string.CompareOrdinal(shortKey, 0, key, 0, shortKey.Length) == 0)
                    {
                        helpValue = $"{shortKey}[{key.Substring(shortKey.Length)}]={value}";
                    }
                    else
                    {
                        helpValue = $"{shortKey} [{key}]={value}";
                    }
                }
                else
                {
                    helpValue = $"{key}={value}";
                }
            }

            return new KeyValuePairOptionValue(
                key,
                value,
                shortKey,
                helpValue,
                description,
                hidden: hidden,
                canContainExpression: canContainExpression);
        }
    }
}
