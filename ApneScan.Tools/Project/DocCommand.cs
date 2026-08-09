namespace ApneScan.Tools.Project;

public class DocCommand : ICommand<DocOptions>
{
    public int Run(DocOptions options)
    {
        // Clean up old build
        Directory.Delete(Path.Combine(Paths.SolutionRoot, "ApneScan.Sdk", "_doc", "_site"), true);
        foreach (var file in new DirectoryInfo(Path.Combine(Paths.SolutionRoot, "ApneScan.Sdk", "_doc", "api"))
                     .EnumerateFiles().Where(x => x.Name != ".gitignore"))
        {
            file.Delete();
        }

        // Copy the SDK readme as index.html
        File.Copy(
            Path.Combine(Paths.SolutionRoot, "ApneScan.Sdk", "README.md"),
            Path.Combine(Paths.SolutionRoot, "ApneScan.Sdk", "_doc", "api", "index.md"),
            true);

        if (options.DocCommand == "build")
        {
            Cli.Run("docfx", "ApneScan.Sdk/_doc/docfx.json", alwaysVerbose: true);
        }
        if (options.DocCommand == "serve")
        {
            Cli.Run("docfx", "ApneScan.Sdk/_doc/docfx.json --serve", alwaysVerbose: true);
        }
        if (options.DocCommand == "push")
        {
            Cli.Run("docfx", "ApneScan.Sdk/_doc/docfx.json", alwaysVerbose: true);
            var deployScriptPath = Path.Combine(Paths.ApneScanUserFolder, "deploy-docs.ps1");
            Cli.Run("powershell", deployScriptPath, alwaysVerbose: true);
        }
        return 0;
    }
}