using Igor_AIS_Proj.MailServices;

namespace Igor_AIS_Proj.Infrastructure.KafkaServices
{
    

    
    public class MailNotificationUseCase : IMailNotificationUseCase
    {
        private IMailService _mailService;

        public MailNotificationUseCase(IMailService mailService)
        {
            _mailService = mailService;
        }
            
        public async Task MailNotificaiton(TransferTopicInfo info)
        {
            var mailtoReceiver = new MailRequest();
            mailtoReceiver.Body = $" Dear {info.RecipientName}, you just received a transfer of {info.Amount} {info.Currency}, from {info.FromUserName} to your Account No. {info.RecipientAccountId}";
            mailtoReceiver.Subject = "Transfer Received";
            mailtoReceiver.ToEmail = info.RecipientMail;
            await _mailService.SendEmailAsync(mailtoReceiver);

            if (info.FromUserName != info.RecipientName)
            {
                var mailToSender = new MailRequest();
                mailToSender.Body = $" Dear {info.FromUserName}, you just executed a transfer of {info.Amount} {info.Currency} to {info.RecipientName}, from your Account No. {info.FromAccountId}";
                mailToSender.Subject = "Transfer Executed";
                mailToSender.ToEmail = info.FromUserMail;
                await _mailService.SendEmailAsync(mailToSender);
            }
            else return;
        }
    }
}
