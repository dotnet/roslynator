// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceMethodGroupWithLambdaRefactoring
    {
        public void Foo(Func<string, string> action)
        {
        }

        public void Foo2(Action<string> action)
        {
        }

        public void Foo()
        {
            Function("");

            Foo(Function);

            Foo(FunctionWithExpressionBody);

            Foo(FunctionWithMultipleStatements);

            Foo2(Action);

            Foo2(ActionWithMultipleStatements);

            Foo(this.Function);

            Foo(FooStatic.Action);

            Foo(f => f.ToLower());

            Changed += OnChanged;

            Foo(LocalFunction);

            string LocalFunction(string value)
            {
                return value.ToLower();
            }
        }

        private void OnChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Changed;

        public string Function(string value)
        {
            return value.ToLower();
        }

        public string FunctionWithMultipleStatements(string value)
        {
            value = value.ToLower();
            return value.ToLower();
        }

        public string FunctionWithExpressionBody(string value) => value.ToLower();

        public void Action(string value)
        {
            value.ToLower();
        }

        public void ActionWithMultipleStatements(string value)
        {
            value.ToLower();
            value.ToLower();
        }

        private static class FooStatic
        {
            public static string Action(string value)
            {
                return value.ToLower();
            }
        }
    }
}
