namespace ApneScan.Tools;

public interface ICommand<in TOptions> where TOptions : OptionsBase
{
    public int Run(TOptions options);
}