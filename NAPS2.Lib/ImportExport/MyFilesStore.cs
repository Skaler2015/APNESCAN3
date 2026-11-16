namespace NAPS2.ImportExport;

/// <summary>
/// Remembers the files the user has saved (most recent first) and the folder shown in the "My Files" sidebar tab.
/// The data is kept in a small text file in the app data folder so it doesn't touch the main config.
/// </summary>
public class MyFilesStore
{
    public const int MAX_RECENT = 30;

    private const string FOLDER_PREFIX = "folder=";
    private const string RECENT_PREFIX = "recent=";
    private const string STORE_FILE_NAME = "myfiles.txt";

    private readonly object _lock = new();
    private readonly List<string> _recent = [];
    private string? _folder;
    private bool _loaded;

    /// <summary>
    /// Raised (on whatever thread made the change) when the folder or the recent files have changed.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// The folder the user picked for "My Files", or null if none has been picked yet.
    /// </summary>
    public string? Folder
    {
        get
        {
            lock (_lock)
            {
                EnsureLoaded();
                return _folder;
            }
        }
        set
        {
            lock (_lock)
            {
                EnsureLoaded();
                _folder = string.IsNullOrWhiteSpace(value) ? null : value;
                Save();
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets a copy of the recently saved file paths, most recent first.
    /// </summary>
    public List<string> GetRecent()
    {
        lock (_lock)
        {
            EnsureLoaded();
            return _recent.ToList();
        }
    }

    /// <summary>
    /// Records a newly saved file at the top of the recent list.
    /// </summary>
    public void AddRecent(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }
        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(path);
        }
        catch (Exception)
        {
            return;
        }
        lock (_lock)
        {
            EnsureLoaded();
            _recent.RemoveAll(x => string.Equals(x, fullPath, StringComparison.OrdinalIgnoreCase));
            _recent.Insert(0, fullPath);
            if (_recent.Count > MAX_RECENT)
            {
                _recent.RemoveRange(MAX_RECENT, _recent.Count - MAX_RECENT);
            }
            Save();
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private static string StorePath => Path.Combine(Paths.AppData, STORE_FILE_NAME);

    private void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }
        _loaded = true;
        try
        {
            var storePath = StorePath;
            if (!File.Exists(storePath))
            {
                return;
            }
            foreach (var line in File.ReadAllLines(storePath))
            {
                if (line.StartsWith(FOLDER_PREFIX, StringComparison.Ordinal))
                {
                    var folder = line.Substring(FOLDER_PREFIX.Length).Trim();
                    _folder = folder.Length == 0 ? null : folder;
                }
                else if (line.StartsWith(RECENT_PREFIX, StringComparison.Ordinal))
                {
                    var recent = line.Substring(RECENT_PREFIX.Length).Trim();
                    if (recent.Length > 0 && _recent.Count < MAX_RECENT)
                    {
                        _recent.Add(recent);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error loading My Files list", ex);
        }
    }

    private void Save()
    {
        try
        {
            var lines = new List<string> { FOLDER_PREFIX + (_folder ?? "") };
            lines.AddRange(_recent.Select(x => RECENT_PREFIX + x));
            File.WriteAllLines(StorePath, lines);
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error saving My Files list", ex);
        }
    }
}
