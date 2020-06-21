using Actionstep.API.WebClient.Domain_Models;
using Blazored.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Actionstep.API.WebClient
{
    // Ngrok: from the cmd prompt => ngrok http 50241 -host-header="localhost:50241" -region au

    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // Load the app's configuration settings from appsettings.json into the AppConfiguration class.
            var appConfiguration = new AppConfiguration();
            Configuration.Bind("AppConfiguration", appConfiguration);
            services.AddSingleton(typeof(AppConfiguration), appConfiguration);

            services.AddHttpClient<ActionstepApi>();
            services.AddSingleton<AppState>();
            services.AddBlazoredToast();

            services.AddRazorPages();
            services.AddServerSideBlazor();
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
                app.UseHsts();
            }

            // NB: If Https redirection is enabled the rest hook controller will not receive the incoming call.
            //     This is only to allow Ngrok to be used to proxy external incoming rest hooks to localhost.
            //     Uncomment in a production environment.
            //
            // app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
