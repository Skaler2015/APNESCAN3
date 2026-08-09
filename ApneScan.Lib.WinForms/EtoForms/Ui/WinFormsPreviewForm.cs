using System.Drawing;
using System.Windows.Forms;

namespace ApneScan.EtoForms.Ui;

public class WinFormsPreviewForm : PreviewForm
{
    public WinFormsPreviewForm(ApneScanConfig config, DesktopCommands desktopCommands, UiImageList imageList,
        IIconProvider iconProvider, ColorScheme colorScheme) : base(config,
        desktopCommands, imageList, iconProvider, colorScheme)
    {
    }

    protected override void OnLoad(EventArgs eventArgs)
    {
        base.OnLoad(eventArgs);
        var toolStrip = (ToolStrip) ToolBar.ControlObject;
        EtoPlatform.Current.AttachDpiDependency(this,
            scale => toolStrip.ImageScalingSize = new Size((int) (16 * scale), (int) (16 * scale)));
    }
}