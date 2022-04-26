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
            using var client = new HttpClient();

            client.Timeout = TimeSpan.FromSeconds(2);

            while (true)
            {
                try
                {
                    var result = await client.GetAsync(url);

                    if(result.IsSuccessStatusCode)
                    {
                        var content = await result.Content.ReadAsByteArrayAsync();

                        var pathToSave = channel.GetOutputPath().AppendDay().AppendHour();

                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }

                        File.WriteAllBytes($"{pathToSave.AppendTimestamp()}.png", content);

                        await _imageHub.SendMessage(new Models.Message()
                        {
                            File = content,
                            Channel = channel
                        });
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
