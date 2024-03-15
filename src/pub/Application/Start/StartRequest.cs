using Core.AbstractExample;
using MediatR;

namespace pub.Application.Start;

public record StartRequest : IRequest<CommandResult>;