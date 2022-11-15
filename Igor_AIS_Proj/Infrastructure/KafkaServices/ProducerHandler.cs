using Confluent.Kafka;
using Newtonsoft.Json;

namespace Igor_AIS_Proj.Infrastructure.KafkaServices
{
        public class ProducerHandler : IProducerHandler
    {

        private ProducerConfig _config;
        private ILogger<ProducerHandler> _logger;

        public ProducerHandler(ProducerConfig config, ILogger<ProducerHandler> logger)
        {
            _config = config;
            _logger = logger;
        }


        public async Task<string> MessageTransfer(string topic, TransferTopicInfo info)
        {
            string serializedTransfer = JsonConvert.SerializeObject(info);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())

            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = serializedTransfer });
                producer.Flush()/*(TimeSpan.FromSeconds(10))*/;
                _logger.LogInformation(serializedTransfer);
                return serializedTransfer;
            }
        }
    }
}
