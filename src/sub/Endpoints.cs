using System.Text.Json;
using Core.AbstractExample;
using Dapr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace sub;

public static class Endpoints
{
    private static CommandStateCode CreateCommandStateCodeFromResult( CommandResult commandResult)
    {
        
        return commandResult.IsSuccess
            ? CommandStateCode.Success
            : CommandStateCode.Failure;
    }

    public static void MapSubscriptions(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/CreateAbstractExample",
            [Topic("pubsub", "service-command-bus", $"event.type == \"CreateAbstractExample\"", 2)]
            async ([FromBody] JsonDocument command, [FromServices] IMediator mediator,
                [FromServices] ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("Endpoints-CreateAbstractExample");
                try
                {
                    logger.LogInformation("/CreateAbstractExample Endpoint Triggered - Command:{Command}",
                        JsonSerializer.Serialize(command));
                    await mediator.Send(
                        new CreateAbstractExampleRequest(
                            CreateAbstractExample.FromJson(command.RootElement.ToString())));
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "/CreateAbstractExample Error occured with Command:{Command}",
                        JsonSerializer.Serialize(command));
                    return Results.StatusCode(500);
                }
            });

        endpoints.MapPost(
            "/ApplyAbstractExample",
            [Topic("pubsub", "service-command-bus", $"event.type == \"ApplyAbstractExample\"", 3)]
            async ([FromBody] JsonDocument command, [FromServices] IMediator mediator,
                [FromServices] ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("Endpoints-ApplyAbstractExample");
                try
                {
                    logger.LogInformation("/ApplyAbstractExample Endpoint Triggered - Command:{Command}",
                        JsonSerializer.Serialize(command));
                    await mediator.Send(
                        new ApplyAbstractExampleRequest(ApplyAbstractExample.FromJson(command.RootElement.ToString())));
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "/ApplyAbstractExample Error occured with Command:{Command}",
                        JsonSerializer.Serialize(command));
                    return Results.StatusCode(500);
                }
            });
        
    }
}

