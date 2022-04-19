using Quartz;
using VisionAPI.Extensions;

namespace VisionAPI.Jobs
{
    [DisallowConcurrentExecution]
    public class DeleteOldVideosJob : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _timeSpan = TimeSpan.FromDays(-14);
        public DeleteOldVideosJob(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task Execute(IJobExecutionContext context)
        {
            var servers = _configuration.GetSection("Servers").GetChildren();

            var channels = servers.Select(s => s.Key);

            foreach (var channel in channels)
            {
                var basePath = channel.GetOutputPath();

                var directories = Directory.GetDirectories(basePath);

                foreach (var directory in directories)
                {
                    var dirInfo = new DirectoryInfo(directory);

                    if (dirInfo.CreationTime < DateTimeExtensions.GetNow().Add(_timeSpan))
                    {
                        dirInfo.Delete(true);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
