
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace ApneScan.Images.Transforms;

public record TrueContrastTransform : Transform
{
    public TrueContrastTransform()
    {
    }

    public TrueContrastTransform(int contrast)
    {
        Contrast = contrast;
    }

    public int Contrast { get; private set; }

    public override bool IsNull => Contrast == 0;
}