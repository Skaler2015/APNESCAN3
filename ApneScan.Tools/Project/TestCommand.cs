using System.Runtime.InteropServices;

namespace ApneScan.Tools.Project;

public class TestCommand : ICommand<TestOptions>
{
    public int Run(TestOptions opts)
    {
        var arch = RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();
        var depsRootPath = OperatingSystem.IsMacOS()
            ? $"ApneScan.App.Mac/bin/Debug/net10.0-macos/osx-{arch}"
            : OperatingSystem.IsLinux()
                ? $"ApneScan.App.Gtk/bin/Debug/net10.0/linux-{arch}"
                : $"ApneScan.App.WinForms/bin/Debug/net10.0-windows/win-{arch}";
        var frameworkArg = OperatingSystem.IsWindows() ? "" : "-f net10.0";
        bool ranTests = false;

        void RunTests(string project, bool isRetry = false)
        {
            try
            {
                ranTests = true;
                Cli.Run("dotnet", $"test -l \"console;verbosity=normal\" {frameworkArg} {project}", new()
                {
                    { "ApneScan_TEST_DEPS", Path.Combine(Paths.SolutionRoot, depsRootPath) },
                    { "ApneScan_TEST_NOGUI", opts.NoGui ? "1" : "0" },
                    { "ApneScan_TEST_NONETWORK", opts.NoNetwork ? "1" : "0" },
                    { "ApneScan_TEST_IMAGES", opts.Images ?? "" }
                });
            }
            catch (Exception)
            {
                if (isRetry) throw;
                Output.Info("Tests failed, retrying once");
                RunTests(project, true);
            }
        }

        var scopes = string.IsNullOrEmpty(opts.Scope) ? ["sdk", "lib", "app"] : opts.Scope.Split('+');
        Output.Info($"Running tests ({string.Join(", ", scopes)})");
        if (scopes.Contains("sdk"))
        {
            RunTests("ApneScan.Sdk.Tests");
        }
        if (scopes.Contains("lib"))
        {
            RunTests("ApneScan.Lib.Tests");
        }
        if (scopes.Contains("app"))
        {
            RunTests("ApneScan.App.Tests");
        }
        Output.Info(ranTests ? "Tests passed." : "Invalid scopes, no tests run.");
        return 0;
    }
}