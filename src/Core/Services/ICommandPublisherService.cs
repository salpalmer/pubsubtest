namespace Core.Services
{
    public interface ICommandPublisherService
    {
        Task PublishCommand(string eventData, string typeName);
    }
}