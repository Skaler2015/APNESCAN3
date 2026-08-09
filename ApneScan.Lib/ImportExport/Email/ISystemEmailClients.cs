namespace ApneScan.ImportExport.Email;

public interface ISystemEmailClients
{
    string[] GetNames();
    string? GetDefaultName();
    IMemoryImage? LoadIcon(string clientName);
}