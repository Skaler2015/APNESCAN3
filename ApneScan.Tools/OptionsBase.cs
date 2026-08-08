using CommandLine;

namespace ApneScan.Tools;

public class OptionsBase
{
    [Option('v', "verbose", Required = false, HelpText = "Show full output")]
    public bool Verbose { get; set; }
}