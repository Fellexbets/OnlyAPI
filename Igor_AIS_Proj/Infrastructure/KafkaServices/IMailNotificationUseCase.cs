using Igor_AIS_Proj.MailServices;

namespace Igor_AIS_Proj.Infrastructure.KafkaServices
{
    

    
    public interface IMailNotificationUseCase
    {
        Task MailNotificaiton(TransferTopicInfo info);
    }
}
