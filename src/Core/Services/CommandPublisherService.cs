using System.Text.Json;
using Dapr;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Services
{

    public class CommandPublisherService : ICommandPublisherService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<CommandPublisherService> _logger;
        private readonly string _daprPubSubName;
        private readonly string _daprTopic;
        private readonly int _daprMessageTimeToLive;

        public CommandPublisherService(DaprClient daprClient, ILogger<CommandPublisherService> logger,
            IConfiguration configuration)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _daprPubSubName = configuration.GetValue<string>("PubSubName", "pubsub");
            _daprTopic = configuration.GetValue<string>("PublishCommandTopic", "service-command-bus");
            _daprMessageTimeToLive = configuration.GetValue("DaprMessageTimeToLiveMinutes", 60);
        }

        public async Task PublishCommand(string eventData, string typeName)
        {
            try
            {
                var cloudEvent = new CloudEvent<JsonDocument>(JsonDocument.Parse(eventData))
                {
                    Type = typeName
                };

                await _daprClient.PublishEventAsync(_daprPubSubName, _daprTopic, cloudEvent,
                    new Dictionary<string, string>
                        { { "ttlInSeconds", TimeSpan.FromMinutes(_daprMessageTimeToLive).Seconds.ToString() } });
                _logger.LogInformation(
                    "Command Published to DaprPubSub:{DaprPubSub} DaprTopic:{DaprTopic} CommandType:{CommandType} Command:{Command}",
                    _daprPubSubName, _daprTopic, cloudEvent.Type, eventData);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Error occured when trying to publish command to DaprTopic:{DaprTopic} CommandContent:{CommandJson}",
                    _daprTopic, eventData);
            }
        }
    }
}