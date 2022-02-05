using System.Net;
using VisionAPI.Hubs;

namespace VisionAPI.Services
{
    public class ImageScrapperService
    {
        private readonly IConfiguration _configuration;
        private readonly ImageHub _imageHub;

        public ImageScrapperService(IConfiguration configuration, ImageHub imageHub)
        {
            _configuration = configuration;
            _imageHub = imageHub;
        }
        public async void Run()
        {
            var servers = _configuration.GetSection("Servers").GetChildren();

            if(!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            var tasks = servers.Select(s => ScrapImages(s.Value, s.Key));

            await Task.WhenAll(tasks);
        }
        private async Task ScrapImages(string url, string channel)
        {
            using WebClient client = new();

            while (true)
            {
                try
                {
                    
                    //$"{OutputPath}_{channel}.png"
                    var result = await client.DownloadDataTaskAsync(new Uri(url));
                    await _imageHub.SendMessage(new Models.Message()
                    {
                        File = result,
                        Channel = channel
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static string OutputPath
        {
            get
            {
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
                return Path.Combine(fullPath, $"{timestamp}");
            }
        }
    }
}
