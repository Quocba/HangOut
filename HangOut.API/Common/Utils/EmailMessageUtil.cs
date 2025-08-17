namespace HangOut.API.Common.Utils
{
    public static class EmailMessageUtil
    {
        private static readonly IWebHostEnvironment _env;
        public static string GetBookingTemplate(string name)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "html", name);

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("Không tìm thấy file BookingTemplate.html", filePath);
            }

            string htmlContent = System.IO.File.ReadAllText(filePath);
            return htmlContent;
        }
    }
}
