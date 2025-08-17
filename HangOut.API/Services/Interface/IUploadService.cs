namespace HangOut.API.Services.Interface;

public interface IUploadService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<bool> DeleteImageAsync(string imageUrl);
    
}