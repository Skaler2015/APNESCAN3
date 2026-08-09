using System.Collections.Immutable;
using ApneScan.Config.Model;

namespace ApneScan.Remoting.Server;

[Config]
public class SharingConfig
{
    /// <summary>
    /// A unique ID for the ApneScan instance, so that if you have the same model of scanner connected to different
    /// computers, they still will have unique derived UUIDs.
    /// </summary>
    public Guid? InstanceId { get; set; }

    public ImmutableList<SharedDevice> SharedDevices { get; set; } = [];
}