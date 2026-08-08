namespace ApneScan.Ocr;

public class OcrErrorEventArgs
{
    public OcrErrorEventArgs(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}