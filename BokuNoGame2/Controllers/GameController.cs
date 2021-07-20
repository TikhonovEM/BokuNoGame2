﻿using BokuNoGame2.Models;
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
    }
}