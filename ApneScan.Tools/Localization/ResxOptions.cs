using CommandLine;
using ApneScan.Tools.Project;

namespace ApneScan.Tools.Localization;

[Verb("resx", HelpText = "Update language resource (.resx) files based on the translation (.po) files")]
public class ResxOptions : OptionsBase
{
    [Option('l', "lang", Required = false, HelpText = "Language to update (otherwise all)")]
    public string? LanguageCode { get; set; }
}