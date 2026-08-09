namespace ApneScan.Escl.Server;

internal class FakeEsclScanJob : IEsclScanJob
{
    private Action<StatusTransition>? _callback;

    public string ContentType => "image/jpeg";

    public void Cancel()
    {
        _callback?.Invoke(StatusTransition.CancelJob);
    }

    public void RegisterStatusTransitionCallback(Action<StatusTransition> callback)
    {
        _callback = callback;
    }

    public Task<bool> WaitForNextDocument(CancellationToken cancelToken) => Task.FromResult(true);

    public async Task WriteDocumentTo(Stream stream)
    {
        var bytes = File.ReadAllBytes(@"C:\Devel\VS\ApneScan\ApneScan.Sdk.Tests\Resources\dog.jpg");
        await stream.WriteAsync(bytes, 0, bytes.Length);
        _callback?.Invoke(StatusTransition.ScanComplete);
    }

    public Task WriteProgressTo(Stream stream) => Task.CompletedTask;

    public Task WriteErrorDetailsTo(Stream stream) => Task.CompletedTask;

    public void Dispose()
    {
    }
}