using ApneScan.EtoForms.Layout;

namespace ApneScan.EtoForms;

public interface IFormBase
{
    FormStateController FormStateController { get; }

    IFormFactory FormFactory { get; set; }

    ApneScanConfig Config { get; set; }

    LayoutController LayoutController { get; }

    IntPtr NativeHandle { get; }
}