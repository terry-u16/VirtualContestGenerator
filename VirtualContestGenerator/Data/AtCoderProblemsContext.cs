using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp;
using Microsoft.EntityFrameworkCore;
using VirtualContestGenerator.Models;

namespace VirtualContestGenerator.Data
{
    public class AtCoderProblemsContext : DbContext
    {
        public DbSet<Problem> Problems { get; set; } = default!;
        public DbSet<Contest> Contests { get; set; } = default!;
        public DbSet<DifficultyInfo> DifficultyInfos { get; set; } = default!;

        public AtCoderProblemsContext(DbContextOptions<AtCoderProblemsContext> options) : base(options)
        {
        }
    }
}
