namespace ApneScan.Automation;

public class ConsoleOutput
{
    public ConsoleOutput(TextWriter writer)
    {
        Writer = writer;
    }
        
    public TextWriter Writer { get; }
}