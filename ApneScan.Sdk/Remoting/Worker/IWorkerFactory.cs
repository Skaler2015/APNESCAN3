using ApneScan.Scan;

namespace ApneScan.Remoting.Worker;

/// <summary>
/// A factory interface to spawn ApneScan.Worker.exe instances as needed.
/// </summary>
internal interface IWorkerFactory
{
    void Init(ScanningContext scanningContext, WorkerFactoryInitOptions? options = null);
    WorkerContext Create(ScanningContext scanningContext, WorkerType workerType);
    void RecreateSpareWorkers();
    void StopSpareWorkers();
}