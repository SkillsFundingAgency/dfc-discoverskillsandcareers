using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Diagnostics.CodeAnalysis;
using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;

namespace Dfc.DiscoverSkillsAndCareers.WebApp
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOptions();
            services.AddDistributedMemoryCache();
            services.AddApplicationInsightsTelemetry();
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddCors(opts => opts.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));
            
            services.Configure<HstsOptions>(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSession(options =>
            {
                options.Cookie.Name = ".dysac-session";
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<CosmosSettings>(Configuration.GetSection("CosmosSettings"));

            services.AddHttpClient<IHttpService, HttpService>()
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(3, (e, i) => TimeSpan.FromMilliseconds(600 * i)))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));


            services.AddScoped<IApiServices, ApiServices>();
            services.AddTransient<IErrorController, ErrorController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();   
            }
            else
            {    
                app.UseHsts();
            }
            
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseCors();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
