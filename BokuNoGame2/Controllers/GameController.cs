using BokuNoGame2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BokuNoGame2.Extensions;
using System.ComponentModel.DataAnnotations;
using BokuNoGame2.Filters;

namespace BokuNoGame2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly UserManager<User> _userManager;

        public GameController(AppDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{gameId}")]
        public async Task<object> Game(int gameId)
        {


            var game = _context.Games.Find(gameId);
            var displayGame = new
            {
                game.Id,
                game.Name,
                game.Description,
                game.Developer,
                game.Publisher,
                Genre = game.Genre.GetAttribute<DisplayAttribute>().GetName(),
                game.AgeRating,
                game.Logo,
                ReleaseDate = game.ReleaseDate.ToShortDateString()
            };
            var user = await _userManager.GetUserAsync(User);

            Catalog catalog = user != null ? _context.GameSummaries.Include(gs => gs.Catalog).Include(gs => gs.Game)
                .FirstOrDefault(gs => gs.UserId.Equals(user.Id) && gs.GameId == gameId)?.Catalog : null;

            var currentRate = GetGameAverageRating(gameId);
            var rate = new
            {
                currentRate,
                currentRateStr = (currentRate / 2).ToString(System.Globalization.CultureInfo.InvariantCulture)
            };

            /*if (catalog != null)
                ViewBag.Catalogs = new SelectList(_context.Catalogs, "Id", "Name", catalog.Id);
            else
                ViewBag.Catalogs = new SelectList(_context.Catalogs, "Id", "Name");

            return View(new GameViewModel() { Game = game, Catalog = catalog });*/

            return new { game = displayGame, catalog, rate };
        }

        [HttpGet("GameList")]
        public object GameList(int page = 1)
        {
            var pageSize = 30;

            var games = _context.Games.AsNoTracking();
            var count = games.Count();
            var pagination = new Page(count, page, 30);
            var gameList = games.Skip((page - 1) * pageSize)
                .Take(pageSize).ToList()
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Logo
                });

            return new { games = gameList, pagination };
        }

        [HttpGet("GetTopMostPopularGames")]
        public object GetTopMostPopularGames(int top)
        {
            return _context.Games
                .OrderByDescending(g => _context.GameSummaries.Where(gs => gs.GameId == g.Id && gs.CatalogId == 2).Count())
                .Take(top)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Logo
                })
                .ToList();
        }


        public List<Review> GetGameReviews(int gameId, bool isApproved)
        {
            return _context.Reviews
                .Where(r => r.GameId == gameId && r.IsApproved == isApproved)
                .ToList();
        }
        public List<Review> GetReviews()
        {
            return _context.Reviews
                .Where(r => !r.IsApproved)
                .ToList();
        }

        public IQueryable<Game> GetFilteredGameList(FilterPanel filter)
        {
            var games = _context.Games.AsNoTracking();
            if (filter == null)
                return games;

            if (!string.IsNullOrEmpty(filter.Name))
                games = games.Where(g => g.Name.Contains(filter.Name));

            if (filter.Genre != Genre.Default)
                games = games.Where(g => g.Genre == filter.Genre);

            if (filter.Rating.HasValue)
                games.Where(g => g.Rating >= filter.Rating);

            if (!string.IsNullOrEmpty(filter.Publisher))
                games = games.Where(g => g.Publisher.Equals(filter.Publisher));

            if (!string.IsNullOrEmpty(filter.Developer))
                games = games.Where(g => g.Developer.Equals(filter.Developer));

            if (filter.ReleaseYearStart.HasValue && filter.ReleaseYearEnd.HasValue)
                games = games.Where(g => g.ReleaseDate.Year >= filter.ReleaseYearStart && g.ReleaseDate.Year <= filter.ReleaseYearEnd);
            else if (filter.ReleaseYearStart.HasValue && !filter.ReleaseYearEnd.HasValue)
                games = games.Where(g => g.ReleaseDate.Year >= filter.ReleaseYearStart);
            else if (!filter.ReleaseYearStart.HasValue && filter.ReleaseYearEnd.HasValue)
                games = games.Where(g => g.ReleaseDate.Year <= filter.ReleaseYearEnd);

            if (filter.Rating.HasValue)
                games = games.Where(g => g.Rating >= filter.Rating);

            if (!string.IsNullOrEmpty(filter.AgeRating))
                games = games.Where(g => g.AgeRating.Equals(filter.AgeRating));


            return games;
        }

        public double GetGameAverageRating(int gameId)
        {
            var gameRates = _context.GameRates.Where(gr => gr.GameId == gameId).ToList();
            var rates = gameRates.Sum(gr => gr.Rate);
            var count = gameRates.Count;
            if (count == 0)
                return 0.0;
            return Math.Round(rates / (double)count, 2);
        }

        [HttpGet("GetNews")]
        public object GetNews(bool isLocal)
        {
            return _context.News.Where(n => n.IsLocal == isLocal)
                .ToList()
                .Select(async n => new
                {
                    n.Reference,
                    n.Text,
                    n.Title,
                    authorName = (await _userManager.FindByIdAsync(n.AuthorId))?.Nickname
                })
                .Select(t => t.Result);
        }

        [HttpPost("CreateNews")]
        public async Task<IActionResult> CreateNews([FromBody] News news)
        {
            var user = await _userManager.GetUserAsync(User);
            news.AuthorId = user?.Id;
            await _context.News.AddAsync(news);
            await _context.SaveChangesAsync();
            var sourceName = news.IsLocal ? "localNews" : "globalNews";
            var data = GetNews(news.IsLocal);
            return Ok(new { sourceName, data });
        }
    }
}
