namespace ApneScan.Images;

internal interface IPdfRendererProvider
{
    IPdfRenderer PdfRenderer { get; }
}