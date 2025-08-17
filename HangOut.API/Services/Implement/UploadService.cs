using HangOut.API.Services.Interface;
using HangOut.Domain.Payload.Settings;
using Microsoft.Extensions.Options;

namespace HangOut.API.Services.Implement;

public class UploadService : IUploadService
{
    private readonly Supabase.Client _client;
    private readonly ILogger _logger;
    private readonly SupabaseSettings _supabaseSettings;
    public UploadService(Supabase.Client client, ILogger logger, IOptions<SupabaseSettings> supabaseSettings)
    {
        _client = client;
        _logger = logger;
        _supabaseSettings = supabaseSettings.Value;
    }
    
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadHttpRequestException("Không tìm thấy file");
        }

        var allowedExtensions = new[] { ".jpeg", ".png", ".jpg", ".gif", ".bmp", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                "Chỉ các định dạng tệp .jpeg, .png, .jpg, .gif, .bmp, và .webp được phép tải lên.");
        try
        {
            var imageGuid = Guid.NewGuid();
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            await _client.Storage.From(_supabaseSettings.SupabaseBucket).Upload(
                memoryStream.ToArray(),
                $"{_supabaseSettings.SupabaseBucket}-{imageGuid}.{extension}");
            var url = _supabaseSettings.SupabaseStorageUrl + $"hangout-{imageGuid}.{extension}";
            return url;
        }
        catch (Exception e)
        {
            _logger.Error("Lỗi khi tải lên tệp lên Supabase Storage");
            throw new BadHttpRequestException("Lỗi khi tải lên tệp lên Supabase Storage");
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        var imageName = imageUrl.Split(_supabaseSettings.SupabaseStorageUrl).Last();
        try
        {
            await _client.Storage.From(_supabaseSettings.SupabaseBucket).Remove(new List<string> { imageName });
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("Lỗi khi xóa tệp khỏi Supabase Storage: {Message}", e.Message);
            return false;
        }
    }
}