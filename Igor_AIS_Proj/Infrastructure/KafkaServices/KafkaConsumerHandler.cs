using Confluent.Kafka;
using Igor_AIS_Proj.MailServices;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Igor_AIS_Proj.Infrastructure.KafkaServices
{

    public class KafkaConsumerHandler : IHostedService
    {
        private readonly string topic;
        private ILogger<KafkaConsumerHandler> _logger;
        private readonly IMailNotificationUseCase _mailNotificationUseCase;

        public KafkaConsumerHandler(ILogger<KafkaConsumerHandler> logger, IMailService mailService, IMailNotificationUseCase mailNotificationUseCase)
        {
            topic = "Teste";
            _logger = logger;
            _mailNotificationUseCase = mailNotificationUseCase;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            var builder = new ConsumerBuilder<Ignore, string>(conf).Build();
            builder.Subscribe(topic);
            var cancelToken = new CancellationTokenSource();
            new Thread(async () => await Subscribe(cancellationToken, builder)).Start();

            await Task.CompletedTask;
            return;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task Subscribe(CancellationToken cancellationToken, IConsumer<Ignore, string> builder)
        {
            try
            {
                while (true)
                {
                    var jsonRecord = builder.Consume(cancellationToken).Value;

                    var deserializedInfo = JsonConvert.DeserializeObject<TransferTopicInfo>(jsonRecord);
                    await _mailNotificationUseCase.MailNotificaiton(deserializedInfo);
                    _logger.LogInformation(jsonRecord);        
                    
                }
            }
            catch (Exception)
            {
                builder.Close();
            }
        }


    }





    //public class KafkaConsumerHandler : IHostedService
    //{
    //    private readonly string topic = "Teste";
    //    public Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        var conf = new ConsumerConfig
    //        {
    //            GroupId = "st_consumer_group",
    //            BootstrapServers = "localhost:9092",
    //            AutoOffsetReset = AutoOffsetReset.Earliest
    //        };
    //        using (var builder = new ConsumerBuilder<Ignore,
    //            string>(conf).Build())
    //        {
    //            builder.Subscribe(topic);
    //            var cancelToken = new CancellationTokenSource();
    //            try
    //            {
    //                while (true)
    //                {
    //                    var consumer = builder.Consume(cancelToken.Token);
    //                    Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
    //                }
    //            }
    //            catch (Exception)
    //            {
    //                builder.Close();
    //            }
    //        }
    //        return Task.CompletedTask;
    //    }
    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        return Task.CompletedTask;
    //    }
    //}
}
