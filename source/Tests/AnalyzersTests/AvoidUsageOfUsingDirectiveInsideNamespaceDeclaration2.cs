
namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class AvoidUsageOfUsingDirectiveInsideNamespaceDeclaration2
    {
        public static void Foo()
        {
            var items = new List<string>();
        }

        private static Task<object> GetValueAsync()
        {
            return Task.FromResult(new object());
        }
    }
}
