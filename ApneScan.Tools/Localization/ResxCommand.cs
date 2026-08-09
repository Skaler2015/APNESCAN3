using ApneScan.Tools.Project;

namespace ApneScan.Tools.Localization;

public class ResxCommand : ICommand<ResxOptions>
{
    public int Run(ResxOptions opts)
    {
        var langCode = opts.LanguageCode;
        if (langCode == null)
        {
            foreach (var poFile in new DirectoryInfo(Paths.PoFolder).EnumerateFiles("*.po"))
            {
                var poLangCode = poFile.Name.Replace(".po", "");
                TranslateResources(poLangCode);
            }
        }
        else
        {
            TranslateResources(langCode);
        }
        return 0;
    }

    private static void TranslateResources(string langCode)
    {
        var ctx = new ResxContext(langCode);
        ctx.Load(Path.Combine(Paths.PoFolder, $"{langCode}.po"));
        ctx.Translate(Path.Combine(Paths.SolutionRoot, "ApneScan.Sdk", "Lang", "Resources"), false);
        ctx.Translate(Path.Combine(Paths.SolutionRoot, "ApneScan.Lib", "Lang", "Resources"), false);
    }
}