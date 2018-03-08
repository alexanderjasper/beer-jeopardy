using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oljeopardy.Data;
using Oljeopardy.DataAccess;
using Oljeopardy.Models;
using Oljeopardy.Models.Email;
using Oljeopardy.Services;

namespace Oljeopardy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig.AutoMapperProfileConfiguration>();
            });
        }

        public IConfiguration Configuration { get; }

        private MapperConfiguration MapperConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IGameRepository, GameRepository>();

            services.AddMvc();
            services.AddMemoryCache();
            services.AddSignalR();

            services.AddAutoMapper();
            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<GameUpdateHub>("gameUpdate");
            });
        }
    }
}
