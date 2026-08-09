using ApneScan.EtoForms.Ui;

namespace ApneScan.EtoForms.Desktop;

public class DesktopFormProvider
{
    private DesktopForm? _desktopForm;

    public DesktopForm DesktopForm
    {
        get => _desktopForm ?? throw new InvalidOperationException();
        set
        {
            _desktopForm = value;
            DesktopFormChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? DesktopFormChanged;
}