using Core.AbstractExample;
using MediatR;

namespace sub;

public class CreateAbstractExampleHandler: IRequestHandler<CreateAbstractExampleRequest, CommandResult>
{
    private readonly ILogger<CreateAbstractExampleHandler> _logger;
    
    public CreateAbstractExampleHandler(
        ILogger<CreateAbstractExampleHandler> logger )
    {
        _logger = logger;
    }

    public Task<CommandResult> Handle( CreateAbstractExampleRequest request, CancellationToken cancellationToken )
    {
        _logger.LogInformation("CreateAbstractExample handled");
        return Task.FromResult(CommandResult.Success);
    }
}

public class ApplyAbstractExampleHandler: IRequestHandler<ApplyAbstractExampleRequest, CommandResult>
{
    private readonly ILogger<ApplyAbstractExampleHandler> _logger;
    
    public ApplyAbstractExampleHandler(
        ILogger<ApplyAbstractExampleHandler> logger )
    {
        _logger = logger;
    }

    public Task<CommandResult> Handle( ApplyAbstractExampleRequest request, CancellationToken cancellationToken )
    {
        _logger.LogInformation("ApplyAbstractExample handled");
        return Task.FromResult(CommandResult.Success);
    }
}