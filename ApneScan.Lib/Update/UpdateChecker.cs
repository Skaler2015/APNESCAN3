using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ApneScan.Update;

public class UpdateChecker : IUpdateChecker
{
    public static readonly TimeSpan CheckInterval = TimeSpan.FromDays(7);

    private const string UPDATE_CHECK_ENDPOINT =
        "https://github.com/Skaler2015/APNESCAN3/releases/download/apnescan-portable/update.json";
#if ZIP
        private const string UPDATE_FILE_EXT = "zip";
#elif MSI
        private const string UPDATE_FILE_EXT = "msi";
#else
    private const string UPDATE_FILE_EXT = "exe";
#endif

    private readonly IOperationFactory _operationFactory;
    private readonly OperationProgress _operationProgress;

    public UpdateChecker(IOperationFactory operationFactory, OperationProgress operationProgress)
    {
        _operationFactory = operationFactory;
        _operationProgress = operationProgress;
    }

    public async Task<UpdateInfo?> CheckForUpdates()
    {
        var json = await GetJson(UPDATE_CHECK_ENDPOINT);
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        foreach (var release in json.Value<JArray>("versions")!)
        {
            var versionName = release.Value<string>("name")!;
            var version = ParseVersion(versionName);

            if (currentVersion >= version) continue;

            var gte = release["requires"]!.Value<string>("gte");
            var gteVersion = gte != null ? ParseVersion(gte) : null;
            if (gteVersion != null && currentVersion < gteVersion) continue;

            var updateFile = release["files"]!.Value<JToken>(UPDATE_FILE_EXT);
            if (updateFile == null) continue;

            var sha256 = updateFile.Value<string>("sha256");
            if (sha256 == null) continue;
            // sig256 is optional; integrity is enforced via the SHA-256 hash over HTTPS.
            var sig256 = updateFile.Value<string>("sig256");

            return new UpdateInfo(versionName, updateFile.Value<string>("url")!, Convert.FromBase64String(sha256),
                sig256 != null ? Convert.FromBase64String(sig256) : Array.Empty<byte>());
        }
        return null;
    }

    public UpdateOperation StartUpdate(UpdateInfo update)
    {
        var op = _operationFactory.Create<UpdateOperation>();
        op.Start(update);
        _operationProgress.ShowModalProgress(op);
        return op;
    }

    private Version ParseVersion(string name)
    {
        return Version.Parse(name.Replace("b", "."));
    }

    private async Task<JObject> GetJson(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        return JObject.Parse(response);
    }
}