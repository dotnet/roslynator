// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.VisualStudio.TypeConverters
{
    public class YesNoConverter : TrueFalseConverter
    {
        public override string TrueText
        {
            get { return "Yes"; }
        }

        public override string FalseText
        {
            get { return "No"; }
        }
    }
}
