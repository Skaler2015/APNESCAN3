using CommandLine;
using ApneScan.Tools.Project;

namespace ApneScan.Tools.Localization;

[Verb("saneopts", HelpText = "Update auto-generated C# files with translations for SANE option values")]
public class SaneOptsOptions : OptionsBase
{
}