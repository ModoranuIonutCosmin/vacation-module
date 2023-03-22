namespace VacationsModule.Application.Auth.Config;

public class JwtOptionsProvider
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }

    public JwtOptionsProvider(string secret, string issuer, string audience)
    {
        Secret = secret;
        Issuer = issuer;
        Audience = audience;
    }
}