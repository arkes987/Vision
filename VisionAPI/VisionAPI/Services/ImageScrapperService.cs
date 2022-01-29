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
        public void Run()
        {
            var servers = _configuration.GetSection("Servers").GetChildren();

            if(!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            foreach (var server in servers)
            {
                Task.Factory.StartNew(() => 
                { 
                    ScrapImages(server.Value, server.Key); 
                });
            }
        }
        private async Task<bool> ScrapImages(string url, string channel)
        {
            try
            {
                using (WebClient client = new())
                {
                    //$"{OutputPath}_{channel}.png"
                    var result = await client.DownloadDataTaskAsync(new Uri(url));
                    await _imageHub.SendMessage(new Models.Message()
                    {
                        File = result,
                        Channel = channel
                    });
                    return await ScrapImages(url, channel);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
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
