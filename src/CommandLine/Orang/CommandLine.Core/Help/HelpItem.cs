// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine.Help
{
    public class HelpItem
    {
        public HelpItem(string syntax, string description)
        {
            Syntax = syntax;
            Description = description;
        }

        public string Syntax { get; }

        public string Description { get; }

        public string Text => Syntax + Description;

        public override string ToString() => Text;
    }
}
