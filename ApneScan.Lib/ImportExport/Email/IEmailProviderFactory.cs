namespace ApneScan.ImportExport.Email;

internal interface IEmailProviderFactory
{
    IEmailProvider Create(EmailProviderType type);

    IEmailProvider Default { get; }
}