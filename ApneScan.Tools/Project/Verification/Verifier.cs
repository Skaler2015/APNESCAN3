namespace ApneScan.Tools.Project.Verification;

public static class Verifier
{
    public static void RunVerificationTests(string testRoot)
    {
        Output.Info($"Running verification tests in: {testRoot}");
        Cli.Run("dotnet", "test -l \"console;verbosity=normal\" ApneScan.App.Tests", new()
        {
            { "ApneScan_TEST_DEPS", testRoot },
            { "ApneScan_TEST_ROOT", testRoot },
            { "ApneScan_TEST_VERIFY", "1" }
        });
        Output.Verbose($"Ran verification tests in: {testRoot}");
    }
}