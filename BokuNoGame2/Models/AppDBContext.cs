using BokuNoGame2.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame2.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<Game> Games { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<GameSummary> GameSummaries { get; set; }
        public DbSet<IntegrationInfo> IntegrationInfos { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<GameRate> GameRates { get; set; }

        public IQueryable<GameSummary> GetGameSummaries(string userId)
        {
            return GameSummaries.Where(gs => gs.UserId.Equals(userId));
        }
    }
}
