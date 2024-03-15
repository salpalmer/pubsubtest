using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using pub.Application.Start;

namespace pub;

public static class Endpoints
{
    public static void MapSubscriptions(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/Start",
        [Topic("pubsub", "service-command-bus", $"event.type == \"Start\"", 1)]
        async ( [FromServices] IMediator mediator, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger( "Endpoints-Start" );
            try
            {
                logger.LogInformation( "/Start Endpoint Activated/" );
                    
                
                var result = await mediator.Send( new StartRequest( ) );
                return Results.StatusCode(result.IsSuccess ? 200 : 400);
            }
            catch ( Exception e )
            {
                logger.LogError(e,"/Start Endpoint Error occured./");
                return Results.StatusCode( 500 );
            }
        });
    }
}

