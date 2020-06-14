using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualContestGenerator.Data;

namespace VirtualContestGenerator.Services
{
    public class MainWorkerService : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MainWorkerService> _logger;
        private readonly AtCoderProblemsContext _context;
        private readonly FetchingJsonService _fetchingJsonService;
        private readonly VirtualContestService _virtualContestService;

        public MainWorkerService(IHostApplicationLifetime lifetime, IConfiguration configuration, ILogger<MainWorkerService> logger,
            AtCoderProblemsContext context, FetchingJsonService fetchingJsonService, VirtualContestService virtualContestService)
        {
            _lifetime = lifetime;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _fetchingJsonService = fetchingJsonService;
            _virtualContestService = virtualContestService;
        }

        private async void Main()
        {
            try
            {
                Console.WriteLine("starting...");
                await UpdateDatabaseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
            finally
            {
                _lifetime.StopApplication();
            }
        }

        private async Task UpdateDatabaseAsync()
        {
            await UpdateContestsAsync();
            await UpdateProblemsAsync();
            await UpdateDifficultiesAsync();
        }

        private async Task UpdateProblemsAsync()
        {
            var problems = await _fetchingJsonService.GetProblemsAsync();

            foreach (var problem in problems)
            {
                var storedProblem = _context.Problems.Find(problem.Id);

                if (storedProblem == null)  // 存在しない場合だけ追加
                {
                    await _context.Problems.AddAsync(problem);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpdateContestsAsync()
        {
            var contests = await _fetchingJsonService.GetContestsAsync();

            foreach (var contest in contests)
            {
                var storeadContest = _context.Contests.Find(contest.Id);

                if (storeadContest == null)  // 存在しない場合だけ追加
                {
                    await _context.Contests.AddAsync(contest);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpdateDifficultiesAsync()
        {
            var difficulties = await _fetchingJsonService.GetDifficultyInfoAsync();

            foreach (var difficulty in difficulties)
            {
                var storedDifficulty = _context.DifficultyInfos.Find(difficulty.Id);

                if (storedDifficulty != null)   // 存在する場合も更新（diffモデル更新の可能性があるため）
                {
                    storedDifficulty.Difficulty = difficulty.Difficulty;
                    storedDifficulty.IsExperimental = difficulty.IsExperimental;
                }
                else
                {
                    await _context.DifficultyInfos.AddAsync(difficulty);
                }
            }

            await _context.SaveChangesAsync();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(Main);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
