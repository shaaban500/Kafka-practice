namespace Producer.API.Services
{
    public class ProducerService
    {
        public async Task ProduceAsync(CancellationToken cancellationToken)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092", // producer url
                AllowAutoCreateTopics = true, // create topic if not exist
                Acks = Acks.All // wait all replicas to have their copies first
            };

            using var producer = new ProducerBuilder<NUll, string>(config).Build();

            try
            {
                var deliveryResult = await producer.ProduceAsync(
                    topic: "test-topic",
                    new Message<Null, string>
                    {
                        Value = $"Helllo from kafka producer at {DateTime.UtcNow}"
                    },
                    cancellationToken);

                // log
            }
            catch(ProduceException<Null, string> e)
            {
                // log
            }

            producer.Flush(cancellationToken);

        }
    }
}
