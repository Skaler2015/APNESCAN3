using CommandLine;
using ApneScan.Tools.Project;

namespace ApneScan.Tools.Localization;

[Verb("pushpot", HelpText = "Upload templates.pot to Crowdin")]
public class PushTemplatesOptions : OptionsBase
{
}