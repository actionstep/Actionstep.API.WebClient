using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Actionstep.API.WebClient
{

    // Ngrok: from the cmd prompt: ngrok http 50241 -host-header="localhost:50241"


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

            //services.AddBlazoredSessionStorage();
            services.AddHttpClient<ActionstepApi>();

            services.AddSingleton<AppState>();

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                app.UseHttpsRedirection();
            }

            //app.UseHttpsRedirection();
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
