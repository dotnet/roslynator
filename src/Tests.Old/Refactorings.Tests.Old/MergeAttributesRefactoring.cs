// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.ComponentModel.DataAnnotations;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class MergeAttributesRefactoring
    {
        [Required]
        [StringLength(10)]
        public string Value { get; set; }









    }
}
