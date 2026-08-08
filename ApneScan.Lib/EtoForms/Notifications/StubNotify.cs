using ApneScan.Update;

namespace ApneScan.EtoForms.Notifications;

public class StubNotify : INotify
{
    public void PdfSaved(string path)
    {
    }

    public void ImagesSaved(int imageCount, string path)
    {
    }

    public void DonatePrompt()
    {
    }

    public void ReviewPrompt()
    {
    }

    public void OperationProgress(OperationProgress progress, IOperation op)
    {
    }

    public void UpdateAvailable(IUpdateChecker updateChecker, UpdateInfo update)
    {
    }
}