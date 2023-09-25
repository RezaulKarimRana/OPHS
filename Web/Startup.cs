using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Cache;
using Infrastructure.Cache.Contracts;
using Infrastructure.Configuration;
using Infrastructure.Configuration.Models;
using Infrastructure.Email;
using Infrastructure.Email.Contracts;
using Infrastructure.Session;
using Infrastructure.Session.Contracts;
using Repositories.UnitOfWork;
using Repositories.UnitOfWork.Contracts;
using Web.Filters;
using Web.Middleware;
using Services;
using Services.Contracts;
using Repositories.ServiceRepos.EmailTemplateRepo.Contracts;
using Repositories.ServiceRepos.EmailTemplateRepo;
using Services.Managers.Contracts;
using Services.Managers;
using System.Globalization;
using Services.Admin;
using Services.Admin.Contracts;
using Web.Authorization;
using Web.Authorization.Requirements;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Web.Authorization.Handlers;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddControllers().AddNewtonsoftJson();
            #region Configuration
            services.Configure<ConnectionStringSettings>(_configuration.GetSection("ConnectionStrings"));
            services.Configure<CacheSettings>(_configuration.GetSection("Cache"));
            services.Configure<EmailSettings>(_configuration.GetSection("Email"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            #endregion

            #region Services

            // Infrastructure Services
            services.AddTransient<ISessionProvider, SessionProvider>();
            services.AddTransient<IEmailProvider, SendGridEmailProvider>();

            services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();

            // Caching
            services.AddDistributedMemoryCache();
            services.AddTransient<ICacheProvider, MemoryCacheProvider>();

            // Business Logic Services
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IDashboardService, DashboardService>();
           

            // Business Logic Admin Service
            services.AddTransient<IPermissionsService, PermissionsService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IAdminService, AdminService>();

            // Business Logic Managers
            services.AddTransient<IAuthenticationManager, AuthenticationManager>();
            services.AddTransient<ICacheManager, CacheManager>();
            services.AddTransient<ISessionManager, SessionManager>();
            services.AddTransient<IAdminManager, AdminManager>();

            // Business Logic Service Repos
            services.AddTransient<IEmailTemplateRepo, EmailTemplateRepo>();

            #endregion

            #region Authentication and Authorization

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.AccessDeniedPath = "/Error/401";
                options.LoginPath = "/Account/Register";
                options.ExpireTimeSpan = TimeSpan.FromSeconds(ApplicationConstants.SessionTimeoutSeconds);
                options.SlidingExpiration = true;

                options.Cookie = ApplicationConstants.SecureNamelessCookie;
                options.Cookie.Name = "Authentication";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConstants.ManageUsers, policy => policy.Requirements.Add(new PermissionRequirement(PermissionKeys.MANAGE_USERS)));
                options.AddPolicy(PolicyConstants.ManageRoles, policy => policy.Requirements.Add(new PermissionRequirement(PermissionKeys.MANAGE_ROLES)));
                options.AddPolicy(PolicyConstants.ManageConfiguration, policy => policy.Requirements.Add(new PermissionRequirement(PermissionKeys.MANAGE_PERMISSIONS)));
            });
            services.AddScoped<IAuthorizationHandler, PermissionsHandler>();

            #endregion

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(ApplicationConstants.SessionTimeoutSeconds);

                options.Cookie = ApplicationConstants.SecureNamelessCookie;
                options.Cookie.Name = "Session";
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                options.EnableForHttps = true; // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression#compression-with-secure-protocol
            });

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(SessionLoggingFilter));
                options.Filters.Add(typeof(OpenGraphPageFilter));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddRazorPagesOptions(options =>
            {
                // apply authorization by default to all pages
                options.Conventions.AuthorizeFolder("/");

                // white-listed routes
                options.Conventions.AllowAnonymousToFolder("/Error");
                options.Conventions.AllowAnonymousToFolder("/Email");

                options.Conventions.AllowAnonymousToPage("/Account/ForgotPassword");
                options.Conventions.AllowAnonymousToPage("/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Account/Register");
                options.Conventions.AllowAnonymousToPage("/Account/ResetPassword");

                //options.Conventions.AllowAnonymousToPage("/Account/Dashboard");

                // custom authorization
                options.Conventions.AuthorizeFolder("/Admin/Users", PolicyConstants.ManageUsers);
                options.Conventions.AuthorizeFolder("/Admin/Roles", PolicyConstants.ManageRoles);
                options.Conventions.AuthorizeFolder("/Admin/Configuration", PolicyConstants.ManageConfiguration);
                options.Conventions.AuthorizeFolder("/Admin/Permissions", PolicyConstants.ManageConfiguration);
                options.Conventions.AuthorizeFolder("/Admin/SessionEvents", PolicyConstants.ManageConfiguration);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // force the application to run under the specified culture
            CultureInfo.DefaultThreadCurrentCulture = ApplicationConstants.Culture;
            CultureInfo.DefaultThreadCurrentUICulture = ApplicationConstants.Culture;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var provider = new FileExtensionContentTypeProvider();

            provider.Mappings[".msg"] = "application/octet-stream";
            provider.Mappings[".kmz"] = "application/vnd.google-earth.kmz";
            provider.Mappings[".kml"] = "application/vnd.google-earth.kml+xml";
            app.UseStaticFiles(
              new StaticFileOptions
              {
                  ContentTypeProvider = provider
              }
            );

            app.UseResponseCompression();

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + ApplicationConstants.StaticFileCachingSeconds;
                }
            });

            app.UseRouting();
            app.UseSession();
            app.UseAdminCreationMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Login}/{id?}");
            //    endpoints.MapRazorPages();
            //});
        }
    }
}
