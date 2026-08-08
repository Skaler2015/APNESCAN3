using ApneScan.Serialization;

namespace ApneScan.ImportExport.Email.Oauth;

public class OauthToken
{
    public SecureString? AccessToken { get; set; }

    public SecureString? RefreshToken { get; set; }

    public DateTime Expiry { get; set; }
}