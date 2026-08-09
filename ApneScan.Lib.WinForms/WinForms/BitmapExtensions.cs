using System.Drawing;

namespace ApneScan.WinForms;

public static class BitmapExtensions
{
    public static Bitmap ToBitmap(this byte[] bytes) => new(new MemoryStream(bytes));
}