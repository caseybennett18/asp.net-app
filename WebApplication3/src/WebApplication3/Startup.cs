using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using CryptoHelper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication3.Data;
using WebApplication3.Models;
using WebApplication3.Services;
using OpenIddict;
//using NWebsec.AspNetCore.Middleware;

namespace WebApplication3
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext, Guid>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddEntityFrameworkSqlServer()
               .AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddOpenIddict<ApplicationDbContext>().DisableHttpsRequirement();

            //services.AddOpenIddict<ApplicationUser, IdentityRole<Guid>, ApplicationDbContext, Guid>()
            //    // Register the HTML/CSS assets and MVC modules to handle the interactive flows.
            //    // Note: these modules are not necessary when using your own authorization controller
            //    // or when using non-interactive flows-only like the resource owner password credentials grant.
            //    .AddAssets()
            //    .AddMvc()

            //    // Register the NWebsec module. Note: you can replace the default Content Security Policy (CSP)
            //    // by calling UseNWebsec with a custom delegate instead of using the parameterless extension.
            //    // This can be useful to allow your HTML views to reference remote scripts/images/styles.
            //    .AddNWebsec(options => options.DefaultSources(directive => directive.Self())
            //        .ImageSources(directive => directive.Self()
            //            .CustomSources("*"))
            //        .ScriptSources(directive => directive.Self()
            //            .UnsafeInline()
            //            .CustomSources("https://my.custom.url/"))
            //        .StyleSources(directive => directive.Self()
            //            .UnsafeInline()))

            //    // During development, you can disable the HTTPS requirement.
            //    .DisableHttpsRequirement();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();
            app.UseOpenIddict();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //using (var context = new ApplicationDbContext(
            //    app.ApplicationServices.GetRequiredService<DbContextOptions<ApplicationDbContext>>())) {
            //    context.Database.EnsureCreated();

            //    if (!context.Applications.Any())
            //    {
            //        context.Applications.Add(new OpenIddictApplication<Guid>
            //        {
            //            // Assign a unique identifier to your client app:
            //            ClientId = "myClient",

            //            // Assign a display named used in the consent form page:
            //            DisplayName = "MVC Core client application",

            //            // Register the appropriate redirect_uri and post_logout_redirect_uri:
            //            RedirectUri = "http://localhost:53507/signin-oidc",
            //            LogoutRedirectUri = "http://localhost:53507/",

            //            // Generate a new derived key from the client secret:
            //            ClientSecret = Crypto.HashPassword("secret_secret_secret"),

            //            // Note: use "public" for JS/mobile/desktop applications
            //            // and "confidential" for server-side applications.
            //            Type = OpenIddictConstants.ClientTypes.Confidential
            //        });

            //        context.SaveChanges();
            //    }
            //}
        }
    }
}
