using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualContestGenerator.Data;
using VirtualContestGenerator.Models;

namespace VirtualContestGenerator.Services
{
    public class MainWorkerService : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly AppSettings _settings;
        private readonly ILogger<MainWorkerService> _logger;
        private readonly AtCoderProblemsContext _context;
        private readonly FetchingJsonService _fetchingJsonService;
        private readonly VirtualContestService _virtualContestService;

        public MainWorkerService(IHostApplicationLifetime lifetime, IConfiguration configuration, ILogger<MainWorkerService> logger,
            AtCoderProblemsContext context, FetchingJsonService fetchingJsonService, VirtualContestService virtualContestService)
        {
            _lifetime = lifetime;
            _settings = configuration.Get<AppSettings>();
            _logger = logger;
            _context = context;
            _fetchingJsonService = fetchingJsonService;
            _virtualContestService = virtualContestService;
        }

        private async void Main()
        {
            try
            {
                await _context.Database.MigrateAsync();

                _logger.LogInformation("[Starting...]");
                await UpdateDatabaseAsync();
                var problemSet = await GetProblemSetAsync();
                await RegisterVirtualContestsAsync(problemSet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
            finally
            {
                _logger.LogInformation("[Exiting...]");
                _lifetime.StopApplication();
            }
        }

        private async Task UpdateDatabaseAsync()
        {
            _logger.LogInformation("[Updating DB...]");
            await UpdateContestsAsync();
            await UpdateProblemsAsync();
            await UpdateDifficultiesAsync();
            _logger.LogInformation("[Updating DB completed!]");
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

        private async Task<Problem[]> GetProblemSetAsync()
        {
            _logger.LogInformation("[Choosing problem set...]");
            var random = new Random();
            var difficultyGroups = _settings.DifficultyGroups;
            var problemSet = new Problem[difficultyGroups.Length - 1];

            for (int i = 0; i < problemSet.Length; i++)
            {
                var lower = difficultyGroups[i];
                var upper = difficultyGroups[i + 1];
                Problem? selected = null;

                var candidates = _context.Problems.Include(p => p.DifficultyInfo)
                                                  .Where(p => p.DifficultyInfo != null &&
                                                              !p.DifficultyInfo.IsExperimental &&
                                                              lower <= p.DifficultyInfo.Difficulty &&
                                                              p.DifficultyInfo.Difficulty < upper);
                var notSelectedCandidates = await candidates.Where(p => !p.HasSelected).ToListAsync();
                if (notSelectedCandidates.Count == 0)
                {
                    foreach (var candidate in candidates)
                    {
                        candidate.HasSelected = false;
                    }
                    notSelectedCandidates = await candidates.ToListAsync();
                }
                selected = notSelectedCandidates[random.Next(notSelectedCandidates.Count)];
                selected.HasSelected = true;
                problemSet[i] = selected;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("[Choosing problem set completed!]");
            return problemSet;
        }

        private async Task RegisterVirtualContestsAsync(IEnumerable<Problem> problems)
        {
            _logger.LogInformation("[Registering virtual contests...]");
            await _virtualContestService.LoginAsync(_settings.GitHubUserInfomation.Id, _settings.GitHubUserInfomation.Password);

            var startTime = DateTimeOffset.Now.Date + new TimeSpan(_settings.VirtualContestTime.BeginHour, 0, 0) - TimeSpan.FromDays(7);
            var duration = new TimeSpan(0, _settings.VirtualContestTime.DurationMinutes, 0);
            var no = (int)(DateTimeOffset.Now - _settings.VirtualContestTime.Since).TotalDays + 1;

            await RegisterSingleVirtualContestAsync(_settings.VirtualContestTitle.Normal, _settings.VirtualContestDescription.Normal,
                problems.Take(6), startTime, duration, no, new int[] { 100, 200, 300, 400, 500, 600 });
            await RegisterSingleVirtualContestAsync(_settings.VirtualContestTitle.Another, _settings.VirtualContestDescription.Another,
                problems.Skip(2), startTime, duration, no, new int[] { 300, 400, 500, 600, 700, 900 });
            _logger.LogInformation("[Registering virtual contests completed!]");
        }

        private async Task RegisterSingleVirtualContestAsync(string title, string description, IEnumerable<Problem> problems,
            DateTime startTime, TimeSpan duration, int no, int[] points)
        {
            var contestTitle = string.Format(title, no);
            _logger.LogInformation($"[Registering {contestTitle}...]");
            var contest = new Models.Json.VirtualContest(contestTitle, startTime, duration, description, false); ;
            var guid = await _virtualContestService.CreateVirtualContestAsync(contest);

            _logger.LogInformation($"[Registering {string.Join(", ", problems)}...]");

            var problemSet = new Models.Json.ProblemSet(guid.ContestId, problems.Zip(points)
                                                                                .Select((pair, index) => new Models.Json.Problem(pair.First.Id, pair.Second, index + 1)));
            await _virtualContestService.UpdateVirtualContestAsync(problemSet);

            _logger.LogInformation($"[Registering {contestTitle} completed!]");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(Main);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
