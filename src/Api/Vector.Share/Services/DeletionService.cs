using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vector.Share.Configuration;
using Vector.Share.Data.Models;
using Timer = System.Timers.Timer;

namespace Vector.Share.Services
{
    public sealed class DeletionService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<DeletionServiceConfiguration> _configuration;
        private readonly ILogger<DeletionService> _logger;
        private Timer _timer;

        public DeletionService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<DeletionServiceConfiguration> configuration,
            ILogger<DeletionService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_configuration.Value.PollingRate)
            {
                AutoReset = true
            };

            _timer.Elapsed += DoWork;
            _timer.Start();

            _logger.LogInformation("Service started.");
            return Task.CompletedTask;
        }

        private async void DoWork(object sender, ElapsedEventArgs e)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var schedulerService = scope.ServiceProvider.GetRequiredService<ISchedulerService>();
            var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

            _logger.LogInformation("Checking for files to delete.");

            ScheduledDeletion[] filesToDelete = schedulerService.GetScheduledDeletionsToDelete();
            if (!filesToDelete.Any())
            {
                _logger.LogInformation("No files to delete.");
                return;
            }

            string[] identifiers = filesToDelete.Select(i => i.Identifier).ToArray();
            await fileService.DeleteMultipleFilesAsync(identifiers);
            await schedulerService.DeleteMultipleScheduledDeletionsAsync(identifiers);
            _logger.LogInformation($"{filesToDelete.Length} files deleted.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Stop();
            _logger.LogInformation("Service stopped.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
