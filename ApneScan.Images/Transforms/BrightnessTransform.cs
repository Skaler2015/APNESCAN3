
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace ApneScan.Images.Transforms;

public record BrightnessTransform : Transform
{
    public BrightnessTransform()
    {
    }

    public BrightnessTransform(int brightness)
    {
        Brightness = brightness;
    }

    public int Brightness { get; private set; }

    public override bool IsNull => Brightness == 0;
}