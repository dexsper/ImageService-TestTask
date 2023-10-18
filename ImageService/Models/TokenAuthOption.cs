using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ImageService.Schemas;

public abstract class TokenAuthOption
{
    private const string Key = "DBA84214DEF50A5BBDB61B5DF580439E622531CA1E8832747DFC4726376D96E7";
    
    public static string Audience => "ImageService";
    public static string Issuer => "ImageServiceClient";
    public static TimeSpan ExpiresSpan => TimeSpan.FromHours(24);
    public static string TokenType => "Bearer";
    

    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
}