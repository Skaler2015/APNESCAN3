using Eto.Drawing;
using Eto.Forms;
using Google.Protobuf;
using ApneScan.ImportExport.Images;

namespace ApneScan.EtoForms.Widgets;

public class ImageListViewBehavior : ListViewBehavior<UiImage>
{
    private readonly UiThumbnailProvider _thumbnailProvider;
    private readonly ApneScanConfig _config;
    private readonly ImageTransfer _imageTransfer = new();

    public ImageListViewBehavior(UiThumbnailProvider thumbnailProvider,
        ColorScheme colorScheme, ApneScanConfig config) : base(colorScheme)
    {
        _thumbnailProvider = thumbnailProvider;
        _config = config;
        MultiSelect = true;
        ShowLabels = false;
        ScrollOnDrag = true;
        UseHandCursor = true;
    }

    public override bool ShowPageNumbers => _config.Get(c => c.ShowPageNumbers);

    /// <summary>
    /// Optional provider that returns an auto-detected document name to show beneath a page (set by the
    /// desktop form). When it returns a non-empty string, that name is drawn instead of the page number.
    /// </summary>
    public Func<UiImage, int, int, string?>? PageLabelProvider { get; set; }

    public override string? GetPageLabel(UiImage item, int index, int count)
    {
        var label = PageLabelProvider?.Invoke(item, index, count);
        return string.IsNullOrWhiteSpace(label) ? null : label;
    }

    public override Image GetImage(IListView<UiImage> listView, UiImage item)
    {
        using var thumbnail = _thumbnailProvider.GetThumbnail(item, listView.ImageSize.Width);
        return thumbnail.ToEtoImage();
    }

    public override bool AllowDragDrop => true;

    public override bool AllowFileDrop => true;

    public override string CustomDragDataType => _imageTransfer.TypeName;

    public override byte[] SerializeCustomDragData(UiImage[] items)
    {
        using var processedImages = items.Select(x => x.GetClonedImage()).ToDisposableList();
        return _imageTransfer.ToBinaryData(processedImages);
    }

    public override DragEffects GetCustomDragEffect(byte[] data)
    {
        var dataObj = _imageTransfer.FromBinaryData(data);
        return dataObj.ProcessId == Process.GetCurrentProcess().Id
            ? DragEffects.Move
            : DragEffects.Copy;
    }

    public override byte[] MergeCustomDragData(byte[][] dataItems)
    {
        // TODO: Move to ImageTransfer?
        var mergedObj = new ImageTransferData();
        foreach (var data in dataItems)
        {
            var dataObj = _imageTransfer.FromBinaryData(data);
            if (mergedObj.ProcessId != 0 && mergedObj.ProcessId != dataObj.ProcessId)
            {
                throw new ArgumentException("Inconsistent process IDs in drag data");
            }
            mergedObj.ProcessId = dataObj.ProcessId;
            mergedObj.SerializedImages.AddRange(dataObj.SerializedImages);
        }
        return mergedObj.ToByteArray();
    }
}