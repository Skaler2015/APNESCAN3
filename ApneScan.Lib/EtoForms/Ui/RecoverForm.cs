using Eto.Drawing;
using Eto.Forms;
using ApneScan.EtoForms.Layout;
using ApneScan.Recovery;

namespace ApneScan.EtoForms.Ui;

public class RecoverForm : EtoDialogBase
{
    private readonly Label _prompt = new();

    public RecoverForm(ApneScanConfig config) : base(config)
    {
    }

    protected override void BuildLayout()
    {
        Title = UiStrings.RecoverFormTitle;

        FormStateController.SaveFormState = false;
        FormStateController.RestoreFormState = false;
        // FormStateController.Resizable = false;

        var recoverButton = C.DialogButton(this, UiStrings.Recover,
            beforeClose: () => SelectedAction = RecoverAction.Recover);
        var deleteButton = C.DialogButton(this, UiStrings.Delete,
            beforeClose: () => SelectedAction = RecoverAction.Delete);
        var notNowButton = C.CancelButton(this, UiStrings.NotNow);

        LayoutController.Content = L.Column(
            _prompt.DynamicWrap(400).MinWidth(300),
            C.Filler(),
            L.Row(
                recoverButton.Scale().Height(32),
                deleteButton.Scale().Height(32),
                notNowButton.Scale().Height(32)
            )
        );
    }

    public RecoverAction SelectedAction { get; private set; }

    public void SetData(int imageCount, DateTime scannedDateTime)
    {
        _prompt.Text = string.Format(UiStrings.RecoverPrompt, imageCount, scannedDateTime.ToShortDateString(),
            scannedDateTime.ToShortTimeString());
    }
}