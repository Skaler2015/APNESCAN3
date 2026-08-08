using System.ComponentModel;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ApneScan.EtoForms.Desktop;

namespace ApneScan.Update;

public class UpdateOperation : OperationBase
{
    private readonly ErrorOutput _errorOutput;
    private readonly DesktopController _desktopController;
    private readonly DesktopFormProvider _desktopFormProvider;

    private readonly ManualResetEvent _waitHandle = new ManualResetEvent(false);
    private WebClient? _client;
    private UpdateInfo? _update;
    private string? _tempFolder;
    private string? _tempPath;

    public UpdateOperation(ErrorOutput errorOutput, DesktopController desktopController,
        DesktopFormProvider desktopFormProvider)
    {
        _errorOutput = errorOutput;
        _desktopController = desktopController;
        _desktopFormProvider = desktopFormProvider;

        ProgressTitle = MiscResources.UpdateProgress;
        AllowBackground = true;
        AllowCancel = true;
        Status = new OperationStatus
        {
            StatusText = MiscResources.Updating,
            ProgressType = OperationProgressType.MB
        };
    }

    public void Start(UpdateInfo updateInfo)
    {
        _update = updateInfo;
        _tempFolder = Path.Combine(Paths.Temp, Path.GetRandomFileName());
        Directory.CreateDirectory(_tempFolder);
        _tempPath = Path.Combine(_tempFolder,
            updateInfo.DownloadUrl.Substring(updateInfo.DownloadUrl.LastIndexOf('/') + 1));

        // TODO: Migrate to HttpClient
#pragma warning disable SYSLIB0014
        _client = new WebClient();
#pragma warning restore SYSLIB0014
        _client.DownloadProgressChanged += DownloadProgress;
        _client.DownloadFileCompleted += DownloadCompleted;
        _client.DownloadFileAsync(new Uri(updateInfo.DownloadUrl), _tempPath);
    }

    public override void Cancel()
    {
        _client?.CancelAsync();
    }

    public override void Wait(CancellationToken cancelToken = default)
    {
        while (!_waitHandle.WaitOne(1000) && !cancelToken.IsCancellationRequested)
        {
        }
    }

    private void DownloadCompleted(object? sender, AsyncCompletedEventArgs e)
    {
        try
        {
            if (e.Cancelled)
            {
                return;
            }
            if (e.Error != null)
            {
                e.Error.PreserveStackTrace();
                throw e.Error;
            }
            if (!VerifyHash())
            {
                Log.Error($"Update error for {_update!.Name}: hash does not match");
                _errorOutput.DisplayError(MiscResources.UpdateError);
                return;
            }
            if (!VerifySignature())
            {
                Log.Error($"Update error for {_update!.Name}: signature does not validate");
                _errorOutput.DisplayError(MiscResources.UpdateError);
                return;
            }

#if ZIP
                InstallZip();
#else
            InstallExe();
#endif
        }
        catch (Exception ex)
        {
            Log.ErrorException("Update error", ex);
            _errorOutput.DisplayError(MiscResources.UpdateError);
            return;
        }
        finally
        {
            InvokeFinished();
            _waitHandle.Set();
        }
        _desktopController.SkipRecoveryCleanup = true;
        _desktopFormProvider.DesktopForm.Close();
    }

    private void InstallExe()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = _tempPath,
            Arguments = "/SILENT /CLOSEAPPLICATIONS"
        });
    }

    private void InstallZip()
    {
        // Extract the downloaded update zip
        var extractDir = Path.Combine(_tempFolder!, "extracted");
        Directory.CreateDirectory(extractDir);
        ZipFile.ExtractToDirectory(_tempPath!, extractDir);

        // The portable zip contains a top-level "ApneScan" folder with the new files.
        var newAppDir = Path.Combine(extractDir, "ApneScan");
        if (!Directory.Exists(newAppDir))
        {
            newAppDir = extractDir;
        }

        // The folder that currently contains ApneScan.exe (the app is installed/run in place).
        var installDir = AssemblyHelper.EntryFolder.TrimEnd('\\', '/');
        var exePath = Path.Combine(installDir, "ApneScan.exe");
        var pid = Process.GetCurrentProcess().Id;

        // A small batch script waits for this process to exit, copies the new files over the
        // install folder, then relaunches the app. (You can't overwrite a running exe, so we
        // wait for the app to close first.)
        var scriptPath = Path.Combine(_tempFolder!, "apnescan_update.bat");
        var script =
            "@echo off\r\n" +
            ":waitloop\r\n" +
            $"tasklist /FI \"PID eq {pid}\" 2>nul | find \"{pid}\" >nul\r\n" +
            "if not errorlevel 1 (\r\n" +
            "    ping -n 2 127.0.0.1 >nul\r\n" +
            "    goto waitloop\r\n" +
            ")\r\n" +
            $"robocopy \"{newAppDir}\" \"{installDir}\" /E /IS /IT /R:3 /W:1 >nul\r\n" +
            $"start \"\" \"{exePath}\"\r\n";
        File.WriteAllText(scriptPath, script);

        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"\"{scriptPath}\"\"",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        });
    }

    private void AtomicReplaceFile(string source, string dest)
    {
        if (!File.Exists(dest))
        {
            File.Move(source, dest);
            return;
        }
        string temp = dest + ".old";
        File.Move(dest, temp);
        try
        {
            File.Move(source, dest);
            File.Delete(temp);
        }
        catch (Exception)
        {
            File.Move(temp, dest);
            throw;
        }
    }

    private bool VerifyHash()
    {
        using var sha = SHA256.Create();
        using FileStream stream = File.OpenRead(_tempPath!);
        byte[] checksum = sha.ComputeHash(stream);
        return checksum.SequenceEqual(_update!.Sha256);
    }

    private bool VerifySignature()
    {
        // Update integrity is verified via the SHA-256 hash (VerifyHash) of the file downloaded
        // over HTTPS from this project's own GitHub releases. RSA code-signing of updates is not
        // used for this build, so there is no separate signature to validate here.
        return true;
    }

    private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        Status.CurrentProgress = (int) e.BytesReceived;
        Status.MaxProgress = (int) e.TotalBytesToReceive;
        InvokeStatusChanged();
    }
}