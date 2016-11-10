// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class CopyCommentFromBaseRefactoring
    {
        public class BaseClass
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public BaseClass(string value)
            {

            }

            /// <summary>
            /// method
            /// </summary>
            /// <returns></returns>
            public virtual string FooMethod()
            {
                return null;
            }

            /// <summary>
            /// property
            /// </summary>
            public virtual string FooProperty { get; set; }

            /// <summary>
            /// indexer
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public virtual string this[int index]
            {
                get { return null; }
            }

            /// <summary>
            /// event field
            /// </summary>
            public virtual event EventHandler FooEventField;

            /// <summary>
            /// event
            /// </summary>
            public virtual event EventHandler FooEvent
            {
                add { }
                remove { }
            }
        }

        public class DerivedClass : BaseClass
        {
            public DerivedClass(string value)
                : base(value)
            {
            }

            public override string FooMethod()
            {
                return base.FooMethod();
            }

            public override string this[int index]
            {
                get { return base[index]; }
            }

            public override string FooProperty
            {
                get { return base.FooProperty; }

                set { base.FooProperty = value; }
            }

            public override event EventHandler FooEventField;

            public override event EventHandler FooEvent;
        }
    }
}
