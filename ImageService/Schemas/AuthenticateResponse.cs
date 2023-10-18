namespace ImageService.Schemas;

public class AuthenticateResponse
{
    public string AccessToken { get; set; } = null!;
    
    public string TokenType { get; set; } = null!;
}