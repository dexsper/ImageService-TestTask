namespace ImageService.Models;

public record AddFriendResponse(List<string> FriendList);

public record AuthenticateResponse(string AccessToken, string TokenType);

public record GetImagesResponse(List<string> Images);

public record ImageUploadResponse(string Name);

public record RegisterResponse(string Id);