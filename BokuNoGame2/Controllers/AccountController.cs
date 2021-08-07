using BokuNoGame2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public AccountController(UserContext userContext, UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, AppDBContext context)
        {
            _userContext = userContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = context;
        }

        [HttpPost("Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginData login)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(login.Login, login.Password, login.RememberMe, false);
                if (result.Succeeded)
                    return Ok(await UserInfo());
                else
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль"); 
            }
            return StatusCode(400);
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
                Roles = roles
            };
        }
    }
}
