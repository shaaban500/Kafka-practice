using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer.App
{
    public class EventConsumerJob : BackgroundService
    {
        private readonly ILogger<EventConsumerJob> _logger;

        public EventConsumerJob(ILogger<EventConsumerJob> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = "test-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("test-topic");

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(stoppingToken);

                            if (consumeResult != null)
                            {
                                _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'");
                            }
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError($"Consume error: {e.Error.Reason}");
                        }
                        catch (OperationCanceledException)
                        {
                            // This is expected during shutdown
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Unexpected error: {ex.Message}");
                        }
                    }
                }
                finally
                {
                    consumer.Close();
                }
            }

            return Task.CompletedTask;
        }
    }
}
