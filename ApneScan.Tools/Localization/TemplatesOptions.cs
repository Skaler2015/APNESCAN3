using CommandLine;
using ApneScan.Tools.Project;

namespace ApneScan.Tools.Localization;

[Verb("templates", HelpText = "Update templates (.pot) files based on project resources (.resx) files")]
public class TemplatesOptions : OptionsBase
{
}