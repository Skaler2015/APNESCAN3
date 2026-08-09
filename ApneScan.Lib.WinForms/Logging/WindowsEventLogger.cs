namespace ApneScan.Logging;

public class WindowsEventLogger : IEventLogger
{
    private const string SOURCE_NAME = "ApneScan";
    private const string LOG_NAME = "Application";

    private readonly ApneScanConfig _config;

    public WindowsEventLogger(ApneScanConfig config)
    {
        _config = config;
    }

    public void CreateEventSource()
    {
        if (!EventLog.SourceExists(SOURCE_NAME))
        {
            EventLog.CreateEventSource(SOURCE_NAME, LOG_NAME);
        }
    }

    public void LogEvent(EventType eventType, EventParams eventParams)
    {
        if (!_config.Get(c => c.EventLogging).HasFlag(eventType)) return;
        try
        {
            EventLog.WriteEntry(SOURCE_NAME, eventParams.ToString(), EventLogEntryType.Information);
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error writing to windows event log", ex);
        }
    }
}