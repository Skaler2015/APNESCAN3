using System.Threading;
using ApneScan.App.Tests.Targets;
using ApneScan.App.Tests.Verification;
using ApneScan.Sdk.Tests;
using ApneScan.Sdk.Tests.Asserts;
using Xunit;

namespace ApneScan.App.Tests.Appium;

[Collection("appium")]
public class ImportAndSaveTests : AppiumTests
{
    [VerifyTheory(AllowDebug = true, WindowsAppium = true)]
    [ClassData(typeof(AppiumTestData))]
    public void ImportVariousAndSavePdfWithOcr(IAppTestTarget target)
    {
        Init(target);
        CopyResourceToFile(PdfResources.word_generated_pdf, "word.pdf");
        CopyResourceToFile(PdfResources.word_patcht_pdf, "patcht.pdf");
        CopyResourceToFile(PdfResources.image_pdf, "image.pdf");
        CopyResourceToFile(BinaryResources.ocr_test, "text.jpg");
        var tessdata = Path.Combine(FolderPath, "components", "tesseract4", "fast");
        Directory.CreateDirectory(tessdata);
        CopyResourceToFile(BinaryResources.eng_traineddata, tessdata, "eng.traineddata");
        
        ImportFile("word.pdf");
        ImportFile("patcht.pdf");
        ImportFile("image.pdf");
        ImportFile("text.jpg");
        
        ClickAtName("OCR");
        ClickAtName("Make PDFs searchable using OCR");
        ClickAtName("OK");
        
        ClickAtName("Save PDF");
        ResetMainWindow();
        var fileTextBox = WaitFor(() => _session.FindElementsByName("File name:").Last());
        ClickAt(fileTextBox);
        fileTextBox.SendKeys("test.pdf");
        ClickAtName("Save");
        // Wait for the save to finish
        Thread.Sleep(100);
        WaitFor(() => !HasElementWithName("Cancel"), 30_000);

        var path = Path.Combine(FolderPath, "test.pdf");
        PdfAsserts.AssertImages(path, 
            PdfResources.word_p1,
            PdfResources.word_p2,
            PdfResources.word_patcht_p1,
            ImageResources.dog,
            ImageResources.ocr_test);
        PdfAsserts.AssertContainsTextOnce("Page one.", path);
        PdfAsserts.AssertContainsTextOnce("Page two.", path);
        PdfAsserts.AssertContainsTextOnce("ADVERTISEMENT.", path);
        PdfAsserts.AssertContainsTextOnce("Sized for printing unscaled", path);
        AppTestHelper.AssertNoErrorLog(FolderPath);
    }

    private void ImportFile(string fileName)
    {
        ClickAtName("Import");
        DoubleClickAtName(fileName);
        ResetMainWindow();
        Thread.Sleep(100);
    }
}