using ApneScan.Sdk.Tests;
using ApneScan.Serialization;
using Xunit;

namespace ApneScan.Lib.Tests.Serialization;

public class SecureStorageTests
{
    [PlatformFact(include: PlatformFlags.Windows)]
    public void EncryptAndDecrypt()
    {
        var text = "Hello, world!";
        var encrypted = SecureStorage.Encrypt(text);
        Assert.StartsWith("encrypted-", encrypted);
        Assert.DoesNotContain("Hello", encrypted);

        var decrypted = SecureStorage.Decrypt(encrypted);
        Assert.Equal(text, decrypted);
    }
    
    [PlatformFact(exclude: PlatformFlags.Windows)]
    public void EncryptAndDecryptNonWindows()
    {
        var text = "Hello, world!";
        var encrypted = SecureStorage.Encrypt(text);
        Assert.Equal(text, encrypted);

        var decrypted = SecureStorage.Decrypt(encrypted);
        Assert.Equal(text, decrypted);
    }
}