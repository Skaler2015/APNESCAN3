namespace ApneScan.Tools.Localization;

public class TemplatesCommand : ICommand<TemplatesOptions>
{
    public int Run(TemplatesOptions opts)
    {
        var ctx = new TemplatesContext();
        ctx.Load(Path.Combine(Paths.SolutionRoot, "ApneScan.Sdk", "Lang", "Resources"), false);
        ctx.Load(Path.Combine(Paths.SolutionRoot, "ApneScan.Lib", "Lang", "Resources"), false);
        ctx.Save(Paths.TemplatesFile);
        return 0;
    }
}