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
                var deliveryResultTask = _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });

                // Esperar a que la operación se complete o se cancele después de un cierto tiempo
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2), new CancellationToken());

                // Esperar a que ocurra la primera de las dos tareas (envío del mensaje o timeout)
                var completedTask = await Task.WhenAny(deliveryResultTask, timeoutTask);

                // Si la operación de envío del mensaje se completó, manejar el resultado
                if (completedTask == deliveryResultTask)
                {
                    var deliveryResult = await deliveryResultTask;
                    _logger.LogInformation($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                }
                else
                {
                    // Se alcanzó el tiempo de espera máximo, cancelar la operación de producción
                    _logger.LogError("Tiempo de espera excedido al enviar el mensaje a Kafka.");
                }
            }
            catch (Exception)
            {
                _logger.LogError($"Error al producir mensaje: {message}");
                throw;
            }
        }
    }
}
