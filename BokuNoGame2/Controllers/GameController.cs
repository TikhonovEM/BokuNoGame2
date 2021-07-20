using BokuNoGame2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var user = await _userManager.GetUserAsync(User);

            Catalog catalog = user != null ? _context.GameSummaries.Include(gs => gs.Catalog).Include(gs => gs.Game)
                .FirstOrDefault(gs => gs.UserId.Equals(user.Id) && gs.GameId == gameId)?.Catalog : null;

            /*if (catalog != null)
                ViewBag.Catalogs = new SelectList(_context.Catalogs, "Id", "Name", catalog.Id);
            else
                ViewBag.Catalogs = new SelectList(_context.Catalogs, "Id", "Name");

            return View(new GameViewModel() { Game = game, Catalog = catalog });*/

            return new { game, catalog };
        }
    }
}
