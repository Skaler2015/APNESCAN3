using Eto.Drawing;
using ApneScan.EtoForms.Widgets;

namespace ApneScan.EtoForms.Ui;

public class SharpenForm : UnaryImageFormBase
{
    private readonly SliderWithTextBox _sharpenSlider = new();

    public SharpenForm(ApneScanConfig config, UiImageList imageList, ThumbnailController thumbnailController,
        IIconProvider iconProvider) :
        base(config, imageList, thumbnailController)
    {
        IconName = "sharpen_small";
        Title = UiStrings.Sharpen;

        EtoPlatform.Current.AttachDpiDependency(this,
            scale => _sharpenSlider.Icon = iconProvider.GetIcon("sharpen_small", scale));
        Sliders = [_sharpenSlider];
    }

    protected override List<Transform> Transforms => [new SharpenTransform(_sharpenSlider.IntValue)];
}