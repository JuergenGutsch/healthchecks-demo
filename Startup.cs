using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using HealthChecks.UI.Client;

namespace HealthCheck.MainApp
{
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
            services.AddHealthChecks()
                .AddCheck("Foo", () =>
                    HealthCheckResult.Healthy("Foo is OK!"), tags: new[] { "foo_tag" })
                .AddCheck("Bar", () =>
                    HealthCheckResult.Degraded("Bar is somewhat OK!"), tags: new[] { "bar_tag" })
                .AddCheck("FooBar", () =>
                    HealthCheckResult.Unhealthy("FooBar is not OK!"), tags: new[] { "foobar_tag" })
                .AddCheck("ping", () =>
                {
                    try
                    {
                        using (var ping = new Ping())
                        {
                            //var reply = ping.Send("twitch.tv");
                            var reply = ping.Send("asp.net-hacker.rocks");
                            if (reply.Status != IPStatus.Success)
                            {
                                return HealthCheckResult.Unhealthy("Ping is unhealthy");
                            }

                            if (reply.RoundtripTime > 100)
                            {
                                return HealthCheckResult.Degraded("Ping is dgraded");
                            }

                            return HealthCheckResult.Healthy("Ping is healthy");
                        }
                    }
                    catch
                    {
                        return HealthCheckResult.Unhealthy("Ping is unhealthy");
                    }
                })
                .AddCheck<ExampleHealthCheck>("class based", null, new[] { "class" });

            services.AddHealthChecksUI()
                    .AddInMemoryStorage();

            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter =  UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }


    public class ExampleHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var healthCheckResultHealthy = true;

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("A healthy result."));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("An unhealthy result."));
        }
    }
}
