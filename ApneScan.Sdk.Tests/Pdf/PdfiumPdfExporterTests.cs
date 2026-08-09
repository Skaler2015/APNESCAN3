using ApneScan.Pdf;
using ApneScan.Sdk.Tests.Asserts;
using Xunit;

namespace ApneScan.Sdk.Tests.Pdf;

public class PdfiumPdfExporterTests : ContextualTests
{
    [Fact]
    public async Task ExportSingleImage()
    {
        var filePath = Path.Combine(FolderPath, "test.pdf");
        using var image = ScanningContext.CreateProcessedImage(LoadImage(ImageResources.dog));
    
        var pdfExporter = new PdfiumPdfExporter(ScanningContext);
        await pdfExporter.Export(filePath, new[] { image });
        
        PdfAsserts.AssertImages(filePath, ImageResources.dog);
    }
}