using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NutritionWebClient.AsyncDataServices.Grpc;
using NutritionWebClient.Services;
using NutritionWebClient.Services.InformationDialog;
using NutritionWebClient.SyncDataService.Login;
using NutritionWebClient.SyncDataService.Does;
using NutritionWebClient.SyncDataService.Meals;
using NutritionWebClient.SyncDataService.Products;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "MyNutritionCookie";
                    options.Cookie.HttpOnly = true;
                    options.LoginPath = new PathString("/api/login");
                    options.LogoutPath = new PathString("/api/logout");
                    options.AccessDeniedPath = new PathString("/api/login");
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.SlidingExpiration = false;
                });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSingleton<JwtTokenRefreshProvider>();
            services.AddHttpClient<IMealDataClient, HttpMealDataClient>();
            services.AddHttpClient<ILoginDataClient, HttpDataClient>();
            services.AddHttpClient<IProductDataClient, HttpProductDataClient>();
            services.AddHttpClient<IDoeDataClient, HttpDoeDataClient>();
            services.AddScoped<ILogsDataClient, LogsDataClient>();

            services.AddRazorPages();
            services.AddControllers();
            services.AddServerSideBlazor();
            services.AddMemoryCache();
            services.AddSingleton<ITokenStorage, TokenStorage>();

            services.AddScoped<AuthenticationStateProvider, IdentityValidationProvider>();

            services.AddScoped<InformationDialogService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            //app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });
        }
    }
}
