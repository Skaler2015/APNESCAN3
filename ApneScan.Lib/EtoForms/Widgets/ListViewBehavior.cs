using Eto.Drawing;
using Eto.Forms;

namespace ApneScan.EtoForms.Widgets;

public abstract class ListViewBehavior<T> where T : notnull
{
    protected ListViewBehavior(ColorScheme colorScheme)
    {
        ColorScheme = colorScheme;
    }

    public ColorScheme ColorScheme { get; }

    public bool MultiSelect { get; protected set; }
        
    public bool ShowLabels { get; protected set; }

    public virtual bool ShowPageNumbers => false;

    /// <summary>
    /// Returns a custom label to draw beneath an item (e.g. an auto-detected document name), or null to
    /// fall back to the default page-number label. When non-null, the label is shown even if page numbers
    /// are otherwise off.
    /// </summary>
    public virtual string? GetPageLabel(T item, int index, int count) => null;

    public bool ScrollOnDrag { get; protected set; }

    public bool UseHandCursor { get; protected set; }

    public bool Checkboxes { get; protected set; }

    public virtual string GetLabel(T item) => throw new NotSupportedException();

    public virtual Image GetImage(IListView<T> listView, T item) => throw new NotSupportedException();

    public virtual bool AllowDragDrop => false;

    public virtual bool AllowFileDrop => false;

    public virtual string CustomDragDataType => throw new NotSupportedException();

    public virtual byte[] SerializeCustomDragData(T[] items) => throw new NotSupportedException();

    public virtual byte[] MergeCustomDragData(byte[][] dataItems) => throw new NotSupportedException();

    public virtual DragEffects GetCustomDragEffect(byte[] data) => throw new NotSupportedException();
}