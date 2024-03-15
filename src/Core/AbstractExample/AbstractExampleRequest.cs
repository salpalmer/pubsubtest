using MediatR;

namespace Core.AbstractExample;


public record CreateAbstractExampleRequest(CreateAbstractExample? Command ) : IRequest<CommandResult>;

public record ApplyAbstractExampleRequest(ApplyAbstractExample? Command ) : IRequest<CommandResult>;

public record ApproveAbstractExampleRequest(ApproveAbstractExample? Command ) : IRequest<CommandResult>;

public record RejectAbstractExampleRequest(RejectAbstractExample? Command ) : IRequest<CommandResult>;

public record UpdateAbstractExampleRequest(UpdateAbstractExample? Command ) : IRequest<CommandResult>;
