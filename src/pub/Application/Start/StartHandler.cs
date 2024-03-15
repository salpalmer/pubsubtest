using Core.AbstractExample;
using Core.Services;
using MediatR;

namespace pub.Application.Start;

public class StartHandler: IRequestHandler<StartRequest, CommandResult>
{
    private readonly ILogger<StartHandler> _logger;
    private readonly ICommandPublisherService _commandPublisherService;

    public StartHandler(ILogger<StartHandler> logger, ICommandPublisherService commandPublisherService )
    {
        _logger = logger;
        _commandPublisherService = commandPublisherService;
    }

    public async Task<CommandResult> Handle( StartRequest request, CancellationToken cancellationToken )
    {
        try
        {
            _logger.LogInformation("/Start Endpoint Triggered");
                
            Guid absId = Guid.NewGuid();
            Guid cr1Id = Guid.NewGuid();
            Guid cr2Id = Guid.NewGuid();
            Guid cr3Id = Guid.NewGuid();
            Guid srcId = Guid.NewGuid();
            Guid tgtId = Guid.NewGuid();

            var cmd = new CreateAbstractExample(srcId, tgtId, new List<Guid> { cr1Id, cr2Id }, absId);
                
            _logger.LogInformation("/Publishing command: {@Cmd}/", cmd);
            await _commandPublisherService.PublishCommand(cmd.ToJson(), "CreateAbstractExample");
            return CommandResult.Success;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "/Start Error occured");
            return CommandResult.Fail($"An error occured in /Start: {e.Message}");
        }
    }
}