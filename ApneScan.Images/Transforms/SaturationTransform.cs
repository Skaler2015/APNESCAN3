
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace ApneScan.Images.Transforms;

public record SaturationTransform : Transform
{
    public SaturationTransform()
    {
    }

    public SaturationTransform(int saturation)
    {
        Saturation = saturation;
    }

    public int Saturation { get; private set; }

    public override bool IsNull => Saturation == 0;
}