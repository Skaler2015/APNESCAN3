using ApneScan.Ocr;

namespace ApneScan.EtoForms.Widgets;

public class OcrLanguagesListViewBehavior : ListViewBehavior<Language>
{
    public OcrLanguagesListViewBehavior(ColorScheme colorScheme) : base(colorScheme)
    {
        ShowLabels = true;
        Checkboxes = true;
    }

    public override string GetLabel(Language item)
    {
        return item.Name;
    }
}