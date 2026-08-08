using System.Net.NetworkInformation;

namespace ApneScan.Scan.Internal;

internal static class LocalIPsHelper
{
    public static Task<HashSet<string>> Get()
    {
        return Task.Run(() =>
            NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                .Select(x => x.Address.ToString())
                .ToHashSet());
    }
}