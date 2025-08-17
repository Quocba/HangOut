namespace HangOut.API.Common.Utils;

public static class OtpUtil
{
    public static string GenerateOtp()
    {
        Random random = new Random();
        return random.Next(1000, 9999).ToString();
    }
}