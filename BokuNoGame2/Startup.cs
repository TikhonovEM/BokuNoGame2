using BokuNoGame2.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public static IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opts => opts.EnableEndpointRouting = false);
            /*services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });*/

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<UserContext>();

            var defaultConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(defaultConnection));

            var accountConnection = Configuration.GetConnectionString("AccountConnection");
            services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(accountConnection));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
            });

            services.AddAuthorization();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "DefaultApi",
                    template: "api/{controller}/{action}");
                routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
            });
        }
    }
}
