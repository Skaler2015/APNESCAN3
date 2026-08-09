namespace ApneScan.EtoForms;

public interface IDarkModeProvider
{
    bool IsDarkModeEnabled { get; }

    event EventHandler? DarkModeChanged;
}