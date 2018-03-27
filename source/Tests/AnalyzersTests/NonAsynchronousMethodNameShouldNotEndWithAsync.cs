// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

#pragma warning disable RCS1016, RCS1090, RCS1174

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class NonAsynchronousMethodNameShouldNotEndWithAsync
    {
        public static void FooAsync()
        {
        }

        public static (string s1, string s2) Foo2Async()
        {
            return default((string, string));
        }

        public static string Foo3Async()
        {
            return null;
        }

        public static string[] Foo4Async()
        {
            return null;
        }

        public static T Foo5Async<T>()
        {
            return default(T);
        }

        //public static IAsyncAction AsyncActionAsync()
        //{
        //    return null;
        //}

        //public static IAsyncActionWithProgress<object> ActionWithProgressAsync()
        //{
        //    return null;
        //}

        //public static IAsyncOperation<object> OperationAsync()
        //{
        //    return null;
        //}

        //public static IAsyncOperationWithProgress<object, object> OperationWithProgressAsync()
        //{
        //    return null;
        //}

        //public static class Foo<T, T2>
        //{
        //    public static IAsyncActionWithProgress<T> ActionWithProgressAsync()
        //    {
        //        return null;
        //    }

        //    public static IAsyncOperation<T> OperationAsync()
        //    {
        //        return null;
        //    }

        //    public static IAsyncOperationWithProgress<T, T2> OperationWithProgressAsync()
        //    {
        //        return null;
        //    }
        //}

        //n

        public static async Task<object> GetAsync()
        {
            return await Task.FromResult<object>(null);
        }

        public static Task<object> TaskOfTAsync()
        {
            return Task.FromResult<object>(null);
        }

        public static T TaskOfTAsync<T>() where T : Task<object>
        {
            return default(T);
        }

        public static Task TaskAsync()
        {
            return default(Task);
        }

        public static T TaskAsync<T>() where T : Task
        {
            return default(T);
        }

        public static ValueTask<object> ValueTaskOfTAsync()
        {
            return default(ValueTask<object>);
        }

        public static class Foo<T>
        {
            public static Task<T> TaskOfTAsync()
            {
                return Task.FromResult(default(T));
            }

            public static ValueTask<T> ValueTaskOfTAsync()
            {
                return default(ValueTask<T>);
            }
        }
    }
}
