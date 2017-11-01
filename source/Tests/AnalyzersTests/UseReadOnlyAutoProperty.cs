// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

#pragma warning disable RCS1016, RCS1048, RCS1060, RCS1081, RCS1163, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public partial class Foo
    {
        public static string StaticProperty { get; private set; }
        public string Property { get; private set; }
        public int IntProperty { get; private set; }
        public StringSplitOptions EnumProperty { get; private set; }

        //n

        public static string StaticAssignedInInstanceConstructor { get; private set; }
        public string Assigned { get; private set; }
        public string InSimpleLambda { get; private set; }
        public string InParenthesizedLambda { get; private set; }
        public string InAnonymousMethod { get; private set; }
        public string InLocalFunction { get; private set; }
        public int AssignedTuple1 { get; private set; }
        public int AssignedTuple2 { get; private set; }

        [DataMember]
        public string PropertyWithDataMemberAttribute { get; private set; }

        public string PrivateSetHasAttribute { get; [DebuggerStepThrough]private set; }

        static Foo()
        {
            StaticProperty = null;
        }

        public Foo()
        {
            Property = null;
            IntProperty = 0;
            EnumProperty = StringSplitOptions.None;
            StaticAssignedInInstanceConstructor = null;
        }

        private void Bar()
        {
            Assigned = null;
            (AssignedTuple1, AssignedTuple2) = default((int, int));
        }

        private class BaseClassName
        {
        }

        private class ClassName<T> : BaseClassName
        {
            public BaseClassName Property { get; private set; }

            public ClassName<TResult> MethodName<TResult>()
            {
                return new ClassName<TResult>() { Property = this };
            }
        }
    }

    public partial class Foo
    {
        public Foo(object parameter)
        {
            var items = new List<string>();

            IEnumerable<string> q = items.Select(f =>
            {
                InSimpleLambda = null;
                return f;
            });

            IEnumerable<string> q2 = items.Select((f) =>
            {
                InParenthesizedLambda = null;
                return f;
            });

            IEnumerable<string> q3 = items.Select(delegate (string f)
            {
                InAnonymousMethod = null;
                return f;
            });

            LocalFunction();

            void LocalFunction()
            {
                InLocalFunction = null;
            }
        }
    }
}

namespace System.Runtime.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal class DataMemberAttribute : Attribute
    {
    }
}
