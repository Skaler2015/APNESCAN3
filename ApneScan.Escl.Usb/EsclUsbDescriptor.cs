namespace ApneScan.Escl.Usb;

public record EsclUsbDescriptor(int VendorId, int ProductId, string SerialNumber, string Manufacturer, string Product);
