namespace Consumer.App
{
    internal class EventConsumerJob : BackgroundService
    {
        private readonly ILogger<EventConsumerJob> _logger;

        public EventConuserJob(ILogger<EventConsumerJob> logger) 
        { 
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) 
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "test-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe("test-topic");

            while (!stoppingToken.IsCancellationRequested) 
            { 
                try
                {
                    var consumResult = consumer.Consum(TimeSpan.FromSeconds(5));

                    if (consumResult == null) 
                    {
                        continue;
                    }

                    _logger.LogInformation($"Consumed message '{consumResult.Message.Value}' at: '{consumResult.Offset}'");
                }
                catch(OperationCanceledException)
                {
                    // Ignore
                }
            }

            return Task.CompletedTask;
        }
    }
}
