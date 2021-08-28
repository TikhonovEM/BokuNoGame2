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
using BokuNoGame2.IntegrationServices;

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

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UserContext>();

            var defaultConnection = Configuration.GetConnectionString("DefaultConnection");
            var accountConnection = Configuration.GetConnectionString("AccountConnection");

            var dbEngine = Configuration["DatabaseEngine"];
            if (Equals(dbEngine.ToLower().Trim(), "mssql"))
            {
                services.AddDbContext<AppDBContext>(options =>
                    options.UseSqlServer(defaultConnection));


                services.AddDbContext<UserContext>(options =>
                    options.UseSqlServer(accountConnection));
            }
            else
            {
                services.AddDbContext<AppDBContext>(options =>
                    options.UseNpgsql(defaultConnection));


                services.AddDbContext<UserContext>(options =>
                    options.UseNpgsql(accountConnection));
            }

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Account/Login");
                options.SlidingExpiration = true;
            });

            services.AddAuthorization();

            #region Интеграция
            services.AddTransient<IntegrationJobFactory>();
            services.AddScoped<SteamIntegrationJob>();
            services.AddScoped<IBaseIntegrationService, SteamIntegratonService>();
            #endregion
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware();
            }
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            
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
