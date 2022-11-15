namespace Igor_AIS_Proj.Infrastructure.KafkaServices
{
    public interface IProducerHandler
    {

        Task<string> MessageTransfer(string topic, TransferTopicInfo info);
    }
}
