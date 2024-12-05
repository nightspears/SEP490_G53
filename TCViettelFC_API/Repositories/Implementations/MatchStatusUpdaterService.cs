using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class MatchStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<MatchStatusUpdaterService> _logger;

        public MatchStatusUpdaterService(IServiceScopeFactory serviceScopeFactory, ILogger<MatchStatusUpdaterService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Match Status Updater Service is starting.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceScopeFactory.CreateScope()) 
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<Sep490G53Context>(); 

                        
                        await UpdateMatchStatusesAsync(dbContext);
                    }

                    _logger.LogInformation("Waiting for the next run...");

                   
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating match statuses.");
            }
            finally
            {
                _logger.LogInformation("Match Status Updater Service is stopping.");
            }
        }

        private async Task UpdateMatchStatusesAsync(Sep490G53Context dbContext)
        {
            DateTime currentDateTime = DateTime.Now;

           
            _logger.LogInformation($"Current Date and Time: {currentDateTime}");

            
            var matchesToUpdate = await dbContext.Matches
                .Where(x => x.MatchDate.HasValue && x.MatchDate <= currentDateTime && x.Status != 0)
                .ToListAsync();

            _logger.LogInformation($"Found {matchesToUpdate.Count} match(es) to update.");

           
            foreach (var match in matchesToUpdate)
            {
                match.Status = 0;
            }

           
            await dbContext.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated status for {matchesToUpdate.Count} match(es).");
        }
    }
}
