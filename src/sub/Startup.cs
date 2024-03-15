using System.Reflection;
using Core;
using Core.AbstractExample;
using Core.Services;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace sub;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup( IConfiguration configuration )
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        
        // Add health checks
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

        // Collect assembly information and throw Exception if we fail
        var assemblyName = typeof(Program).AssemblyQualifiedName?.Split(" ");
        if (assemblyName is null) throw new ApplicationException("Unable to fetch assembly information");

        // Set up dependencies
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddDaprClient();
        
        services.AddActors(options => options.Actors.RegisterActor<AbstractExampleActor>());
        services.AddSingleton<IEventStore>(new EventStore());
        services.AddScoped<ICommandPublisherService, CommandPublisherService>();
    }
    
    public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
    {
        if ( env.EnvironmentName == "Development" )
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseCloudEvents();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSubscribeHandler();
            endpoints.MapSubscriptions();
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

        });
    }
}