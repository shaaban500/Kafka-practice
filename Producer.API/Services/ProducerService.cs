using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Producer.API.Services
{
    public class ProducerService
    {
        private readonly ILogger<ProducerService> _logger;

        public ProducerService(ILogger<ProducerService> logger)
        {
            _logger = logger;
        }

        public async Task ProduceAsync(CancellationToken cancellationToken)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092", // producer url
                AllowAutoCreateTopics = true, // create topic if not exist
                Acks = Acks.All // wait all replicas to have their copies first
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var deliveryResult = await producer.ProduceAsync(
                        topic: "test-topic",
                        new Message<Null, string>
                        {
                            Value = $"Hello from Kafka producer at {DateTime.UtcNow}"
                        },
                        cancellationToken);

                    // Log successful message production
                    _logger.LogInformation($"Produced message '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    // Log produce exception
                    _logger.LogError($"Produce error: {e.Error.Reason}");
                }
                catch (Exception ex)
                {
                    // Log general exceptions
                    _logger.LogError($"Unexpected error: {ex.Message}");
                }
                finally
                {
                    producer.Flush(cancellationToken);
                }
            }
        }
    }
}
