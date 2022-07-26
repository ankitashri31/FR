using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using FirstResponseApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using FirstResponseApp.Areas.Ticket.Models;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstResponseApp
{
    public class Startup
    {
        private readonly IWebHostEnvironment environment;
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment _hostingEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            environment = webHostEnvironment;
            _hostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"), o => o.CommandTimeout(180000)));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
               
            });


            services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();
            services.AddTransient<Utility, Utility>(); 
            services.AddTransient<AppService, AppService>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();


            //   services.AddMvc(option => option.EnableEndpointRouting = false).AddSessionStateTempDataProvider();

            services.AddMvc(option => option.EnableEndpointRouting = false).AddRazorPagesOptions(options => {
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            //services.AddSession();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(8);
            });

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton(Configuration);
            services.AddHttpContextAccessor();

            Utility.GlobalHostingEnvironment = _hostingEnvironment;
            ErrorUtility.GlobalHostingEnvironmentError = _hostingEnvironment;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithRedirects("/Error");
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }
            else
            {
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithRedirects("/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("ticket", "ticket/{*index}",
            //             defaults: new { controller = "Ticket", action = "Dashboard" });
            //    routes.MapRoute("default", "{controller=dumy}/{action=Index}/{id?}");
            //});
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("Identity", "Identity/{*index}",
            //             defaults: new { controller = "Account", action = "Login" });
            //    routes.MapRoute("Ticket", "{controller=TicketUser}/{action=Dashboard}/{id?}");
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "areaRoute",
                   template: "{area:exists}/{controller=TicketUser}/{action=Dashboard}/{id?}");

            });

            app.UseCookiePolicy();

        }
    }
}
