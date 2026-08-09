using ApneScan.Ocr;
using ApneScan.Scan;

namespace ApneScan.Config;

public static class ConfigExtensions
{
    public static OcrParams DefaultOcrParams(this ApneScanConfig config)
    {
        if (!config.Get(c => c.EnableOcr))
        {
            return OcrParams.Empty;
        }
        return new OcrParams(
            config.Get(c => c.OcrLanguageCode),
            MapOcrMode(config.Get(c => c.OcrMode), config.Get(c => c.OcrPreProcessing)),
            config.Get(c => c.OcrTimeoutInSeconds));
    }

    private static OcrMode MapOcrMode(LocalizedOcrMode ocrMode, bool preProcessing)
    {
        return (ocrMode, preProcessing) switch
        {
            (LocalizedOcrMode.Fast, false) => OcrMode.Fast,
            (LocalizedOcrMode.Fast, true) => OcrMode.FastWithPreProcess,
            (LocalizedOcrMode.Best, false) => OcrMode.Best,
            (LocalizedOcrMode.Best, true) => OcrMode.BestWithPreProcess,
            _ => OcrMode.Default
        };
    }

    public static OcrParams OcrAfterScanningParams(this ApneScanConfig config)
    {
        if (!config.Get(c => c.OcrAfterScanning))
        {
            return OcrParams.Empty;
        }
        return config.DefaultOcrParams();
    }

    public static ScanProfile DefaultProfileSettings(this ApneScanConfig config)
    {
        return config.Get(c => c.DefaultProfileSettings) ??
               new ScanProfile { Version = ScanProfile.CURRENT_VERSION };
    }
}