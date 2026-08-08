using CommandLine;
using ApneScan.Tools.Project;

namespace ApneScan.Tools.Localization;

[Verb("pullpo", HelpText = "Download .po files from Crowdin")]
public class PullTranslationsOptions : OptionsBase
{
}