 
using Csla.Configuration;
using eInvoiceApp.Data;
using eInvoiceApp.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SPC.Services.UI;
using System.Security.Claims;

namespace eInvoiceApp
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
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
             .AddCookie();

            services.AddAuthorization(config =>
            {
                config.AddPolicy("IsAuthenticated", policy => policy.RequireAuthenticatedUser());
                config.AddPolicy("IsAdmin", policy => policy.RequireClaim("Department", "Admin"));
                config.AddPolicy("IsConsultant", policy => policy.RequireAuthenticatedUser());  //.Combine(policy.RequireClaim("Department", "Consulting")));
                config.AddPolicy("IsSPCStaff", policy => policy.RequireClaim("RegCompany", "SPC"));
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddDevExpressBlazor();

            services.AddSingleton<WeatherForecastService>();
                        
            services.AddCsla().WithBlazorServerSupport();

            services.AddHttpContextAccessor();
            services.AddScoped<BlazorPlus.BlazorSession>();
                                                            
            //services.AddSingleton<Ctx.IDeviceInfo, DeviceInfo>();
            
            SPC.SystemServicesRegister.RegisterAll(Configuration);
            
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
            app.UseAuthorization();
                        

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.Map("/_blazorplus_handler", BlazorPlus.BlazorSession.ProcessRequestAsync);
                endpoints.MapFallbackToPage("/_Host");
            });

            app.UseCsla();
            
            
        }
    }
}
