﻿using BokuNoGame2.Extensions;
using BokuNoGame2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BokuNoGame2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserContext _userContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDBContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserContext userContext, UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, AppDBContext context, ILogger<AccountController> logger)
        {
            _userContext = userContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = context;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginData login)
        {
            try
            {
                _logger.LogDebug("Login attempt for {0}:{1}", login.Login, login.Password);
                var result = await _signInManager.PasswordSignInAsync(login.Login, login.Password, login.RememberMe, false);
                if (result.Succeeded)
                    return Ok(await UserInfo());
                else
                    return StatusCode(400, new { Errors = "не удалось :(" });
            }
            catch (Exception e)
            {
                _logger.LogError("Login attempt failed for {0}:{1}. Result: {2}", login.Login, login.Password, e.Message);
                return StatusCode(400, new { Errors = e.Message });
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] LoginData data)
        {
            try
            {
                var user = new User { UserName = data.Login };
                var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", "default-avatar.jpg");
                _logger.LogDebug("defaultAvatarPath = '{0}'", defaultAvatarPath);
                user.Photo = System.IO.File.ReadAllBytes(defaultAvatarPath);
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, data.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, false);
                    return Ok(await UserInfo());
                }
                else
                    return StatusCode(400, new { Errors = result.Errors.Select(e => e.Description) });
            }
            catch (Exception e)
            {
                _logger.LogError("Error while Register: {0}", e.Message);
                return StatusCode(400, new { Errors = e.Message });
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("UserInfo")]
        public async Task<object> UserInfo()
        {
            var roles = new List<string>();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                roles.AddRange(await _userManager.GetRolesAsync(user));
            return new
            {
                IsSignedIn = _signInManager.IsSignedIn(User),
                Roles = roles,
                UserId = user?.Id
            };
        }
        [HttpGet("Profile/{userName?}")]
        public async Task<object> Profile(string userName)
        {
            _logger.LogDebug("Attempt to get profile for '{0}'", userName);
            var user = userName != null && !userName.Equals("undefined") ? await _userManager.FindByNameAsync(userName) : await _userManager.GetUserAsync(User);
            _logger.LogDebug("user '{0}' is found", user.UserName);
            var gameSummaries = _dbContext.GetGameSummaries(user.Id);
            var catalogs = _dbContext.Catalogs;
            return new
            {
                user,
                gameSummaries,
                catalogs
            };
        }
        [Authorize]
        [HttpPost("LoadPhoto")]
        public async Task<IActionResult> LoadPhoto(IFormCollection data, IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (file != null)
            {
                using (var reader = new BinaryReader(file.OpenReadStream()))
                {
                    user.Photo = reader.ReadBytes((int)file.Length);
                }

                await _userManager.UpdateAsync(user);
            }
            return Ok(await Profile(null));
        }

        [HttpPost("EditProfile")]
        public async Task<object> EditProfile([FromBody] EditableProfileData data)
        {
            var user = await _userManager.GetUserAsync(User);
            user.Nickname = data.Nickname ?? user.Nickname;
            user.FullName = data.FullName ?? user.FullName;
            user.Email = data.Email ?? user.Email;
            user.BirthDate = data.BirthDate.HasValue ? data.BirthDate.Value : user.BirthDate;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(await Profile(null));
            else
                return StatusCode(400);
        }

        [HttpGet("ExportSummaries")]
        [Authorize]
        public async Task<IActionResult> ExportSummaries(string userId = null)
        {
            userId ??= _userManager.GetUserId(User);
            var gs = await _dbContext.GetGameSummaries(userId)
                .Select(g => new GameSummaryDTO
                {
                    GameName = g.GameName,
                    Rate = g.Rate,
                    Genre = g.Genre,
                    CatalogId = g.CatalogId,
                    GameId = g.GameId
                })
                .ToListAsync();
            var serializedGs = JsonConvert.SerializeObject(gs);
            var fileBytes = Encoding.UTF8.GetBytes(serializedGs);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Json, "ExportedLibrary.json");
        }

        [Authorize]
        public async Task<IActionResult> ImportSummaries(IFormFile jsonfile, string userId = null)
        {
            userId ??= _userManager.GetUserId(User);
            if (jsonfile != null)
            {
                byte[] jsonBytes = null;
                using (var reader = new BinaryReader(jsonfile.OpenReadStream()))
                {
                    jsonBytes = reader.ReadBytes((int)jsonfile.Length);
                }
                var json = Encoding.UTF8.GetString(jsonBytes);
                var gs = JsonConvert.DeserializeObject<List<GameSummaryDTO>>(json);
                var existingGS = _dbContext.GetGameSummaries(userId);
                foreach (var gameSummaryDTO in gs.Where(gs => !existingGS.Any(egs => egs.GameId.Equals(gs.GameId))))
                {
                    var gameSummary = new GameSummary();
                    var game = await _dbContext.Games.FindAsync(gameSummaryDTO.GameId);
                    gameSummary.GameName = game.Name;
                    gameSummary.Game = game;
                    gameSummary.GameId = game.Id;
                    gameSummary.Rate = gameSummaryDTO.Rate;
                    if (gameSummaryDTO.Rate.HasValue)
                    {
                        var gameRate = new GameRate()
                        {
                            AuthorId = userId,
                            GameId = game.Id,
                            Rate = gameSummaryDTO.Rate.Value
                        };
                        await _dbContext.GameRates.AddAsync(gameRate);
                    }
                    gameSummary.Genre = game.Genre;
                    gameSummary.GenreWrapper = game.Genre.GetAttribute<DisplayAttribute>().Name;
                    gameSummary.UserId = userId;
                    var catalog = await _dbContext.Catalogs.FindAsync(gameSummaryDTO.CatalogId);
                    gameSummary.Catalog = catalog;
                    gameSummary.CatalogId = catalog.Id;
                    await _dbContext.GameSummaries.AddAsync(gameSummary);
                }

                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }
    }
}
