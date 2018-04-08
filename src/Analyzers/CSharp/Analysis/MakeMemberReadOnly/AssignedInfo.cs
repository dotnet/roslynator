// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    internal readonly struct AssignedInfo
    {
        public AssignedInfo(IdentifierNameSyntax name, bool isInInstanceConstructor = false, bool isInStaticConstructor = false)
        {
            Name = name;
            IsInInstanceConstructor = isInInstanceConstructor;
            IsInStaticConstructor = isInStaticConstructor;
        }

        public IdentifierNameSyntax Name { get; }

        public string NameText
        {
            get { return Name.Identifier.ValueText; }
        }

        public bool IsInInstanceConstructor { get; }

        public bool IsInStaticConstructor { get; }
    }
}
