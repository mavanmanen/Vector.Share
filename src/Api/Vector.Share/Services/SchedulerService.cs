using System;
using System.Threading.Tasks;
using Vector.Share.Data.Models;
using Vector.Share.DTO;
using Vector.Share.Providers.Timestamp;
using Vector.Share.Repositories;

namespace Vector.Share.Services
{
    public interface ISchedulerService
    {
        Task ScheduleDeletionAsync(string identifier, FileLifetime lifetime);
        ScheduledDeletion[] GetScheduledDeletionsToDelete();
        Task DeleteScheduledDeletionAsync(string identifier);
        Task DeleteMultipleScheduledDeletionsAsync(string[] identifiers);
    }

    public class SchedulerService : ISchedulerService
    {
        private readonly IScheduledDeletionRepository _repository;
        private readonly ITimestampProvider _timestampService;

        public SchedulerService(
            IScheduledDeletionRepository repository,
            ITimestampProvider timestampService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _timestampService = timestampService ?? throw new ArgumentNullException(nameof(timestampService));
        }

        public async Task ScheduleDeletionAsync(string identifier, FileLifetime lifetime)
        {
            DateTime now = _timestampService.UtcNow;
            DateTime deletionTime = lifetime switch
            {
                FileLifetime.D1 => now.AddDays(1),
                FileLifetime.D7 => now.AddDays(7),
                FileLifetime.M1 => now.AddMonths(1)
            };

            var entity = new ScheduledDeletion
            {
                Identifier = identifier,
                DeletionTime = deletionTime
            };

            await _repository.AddAsync(entity);
        }

        public ScheduledDeletion[] GetScheduledDeletionsToDelete()
        {
            DateTime now = _timestampService.UtcNow;
            return _repository.FindWhere(sd => sd.DeletionTime <= now);
        }

        public async Task DeleteScheduledDeletionAsync(string identifier)
        {
            ScheduledDeletion scheduledDeletion = await _repository.FindAsync(identifier);
            await _repository.DeleteAsync(scheduledDeletion);
        }

        public async Task DeleteMultipleScheduledDeletionsAsync(string[] identifiers)
        {
            ScheduledDeletion[] scheduledDeletions = await _repository.FindMultipleAsync(identifiers);
            await _repository.DeleteMultipleAsync(scheduledDeletions);
        }
    }
}
