using System.Net;
using VisionAPI.Extensions;
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
                    var result = await client.DownloadDataTaskAsync(new Uri(url));

                    var pathToSave = channel.GetOutputPath().AppendDay().AppendHour();

                    if (!Directory.Exists(pathToSave))
                    {
                        Directory.CreateDirectory(pathToSave);
                    }

                    File.WriteAllBytes($"{pathToSave.AppendTimestamp()}.png", result);

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
    }
}
