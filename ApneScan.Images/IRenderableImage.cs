namespace ApneScan.Images;

public interface IRenderableImage
{
    ImageContext ImageContext { get; }
    IImageStorage Storage { get; }
    TransformState TransformState { get; }
}