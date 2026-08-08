using ApneScan.Remoting;

namespace ApneScan.Platform;

public class LinuxApplicationLifecycle(
    ProcessCoordinator processCoordinator,
    IOsServiceManager serviceManager,
    ApneScanConfig config)
    : ApplicationLifecycle(processCoordinator, serviceManager, config)
{
}