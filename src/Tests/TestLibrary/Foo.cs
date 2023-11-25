using System;
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        using (var disposable = await GetDisposableAsync())
        {
        }
    }

    private Task<Disposable> GetDisposableAsync()
    {
        throw new NotImplementedException();
    }
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
