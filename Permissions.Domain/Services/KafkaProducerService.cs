using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Permissions.Domain.Services
{
    public interface IKafkaProducerService
    {
        Task ProduceMessageAsync(string topic, string operation);
    }

    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(ProducerConfig config, ILogger<KafkaProducerService> logger)
        {
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _logger = logger;
        }

        public async Task ProduceMessageAsync(string topic, string operation)
        {
            var message = JsonSerializer.Serialize(new { Id = Guid.NewGuid(), NameOperation = operation });

            try
            {
                var deliveryReport = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });

                _logger.LogInformation($"Delivered '{deliveryReport.Value}' to '{deliveryReport.TopicPartitionOffset}'");
                _producer.Flush();
            }
            catch (Exception)
            {
                _logger.LogError($"Error al producir mensaje: {message}");
                throw;
            }
        }
    }
}
