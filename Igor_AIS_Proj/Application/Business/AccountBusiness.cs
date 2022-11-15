

using Igor_AIS_Proj.Infrastructure.KafkaServices;
using System.Data.Entity;
using System.Transactions;

namespace Igor_AIS_Proj.Business
{
    public class AccountBusiness : IAccountBusiness
    {
        private ILogger<AccountBusiness> _logger;
        private readonly IAccountPersistence _accountPersistence;
        private readonly IMovementPersistence _movementPersistence;
        private readonly ITransferPersistence _transferPersistence;
        private readonly IUserPersistence _userPersistence;
        private readonly IProducerHandler _producerHandler;
        private readonly IUserBusiness _userBusiness;
        public AccountBusiness(IAccountPersistence accountPersistence, IMovementPersistence movementPersistence, ITransferPersistence transferPersistence, ILogger<AccountBusiness> logger, IUserPersistence userPersistence, IProducerHandler producerHandler, IUserBusiness userBusiness)
        {
            _transferPersistence = transferPersistence;
            _accountPersistence = accountPersistence;
            _movementPersistence = movementPersistence;
            _logger = logger;
            _userPersistence = userPersistence;
            _producerHandler = producerHandler;
            _userBusiness = userBusiness;
        }


        public (bool, Account?) GetById(int id)

        {
            try
            {
                return (true, _accountPersistence.GetById(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Account");
                return (false, null);
            }

        }
         
        public async Task<bool> Delete(int id) => await _accountPersistence.Delete(id);

        public async Task<(bool, string?)> TransferFunds(TransferRequest request)
        {
            if (request.Amount <= 0)
            {
                return (false, "Transfer amount must be positive");
            }
            string topic = "Teste";
            (bool, Account) fromAccount = GetById(request.FromAccountId);
            (bool, Account) toAccount = GetById(request.ToAccountId);
            if (fromAccount.Item2.Currency != toAccount.Item2.Currency)
            {
                return (false, "You can only transfer in the same currency");
            }
            if (fromAccount.Item2.Balance < request.Amount)
            {
                return (false, "Insufficient funds");
            }
            if (fromAccount.Item2.AccountId == toAccount.Item2.AccountId)
            {
                return (false, "You can´t transfer to the same account!");
            }
            if (request.FromAccountId != fromAccount.Item2.AccountId)
            {
                return (false, "You do not own this account! You can only transfer from your accounts.");
            }
            var account = GetById(toAccount.Item2.AccountId);
            if(account.Item2 == null)
            {
                return (false, "Destination Account not valid!");
            }
            try
            {
                using var context = new PostgresContext();
                using var transaction = context.Database.BeginTransaction();
                    fromAccount.Item2.Balance -= request.Amount;
                    toAccount.Item2.Balance += request.Amount;
                    var transfer1 = new Transfer { OriginaccountId = fromAccount.Item2.AccountId, DestinationaccountId = toAccount.Item2.AccountId, Amount = request.Amount, Currency = fromAccount.Item2.Currency };
                    var mov1 = new Movement { AccountId = fromAccount.Item2.AccountId, Amount = -request.Amount, Currency = fromAccount.Item2.Currency, UserId = fromAccount.Item2.UserId };
                    var mov2 = new Movement { AccountId = fromAccount.Item2.AccountId, Amount = +request.Amount, Currency = toAccount.Item2.Currency, UserId = toAccount.Item2.UserId };
                    await _transferPersistence.Create(transfer1);
                    await _movementPersistence.Create(mov1);
                    await _movementPersistence.Create(mov2);
                    await _accountPersistence.Update(fromAccount.Item2);
                    await _accountPersistence.Update(toAccount.Item2);

                var recipientUser = _userBusiness.GetById(toAccount.Item2.UserId);
                var transferUserFrom = _userBusiness.GetById(fromAccount.Item2.UserId);
                var info = new TransferTopicInfo()
                    {
                        Amount = request.Amount,
                        Currency = toAccount.Item2.Currency,
                        FromUserName = transferUserFrom.Username,
                        FromUserMail = transferUserFrom.Email,
                        FromAccountId = request.FromAccountId,
                        RecipientName = recipientUser.Username,
                        RecipientMail = recipientUser.Email,
                        RecipientAccountId = request.ToAccountId
                       
                    };
                    
                transaction.Commit();
                //await _producerHandler.MessageTransfer(topic, info);
                return (true, "Transaction Complete!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Transaction Failed");
                return (false, "Transaction Failed");
            }
            
        }



        public async Task<(bool, List<Account>, String?)> GetAllAccountsUser(int id)
        {
            var accountsUser = _userPersistence.GetById(id);
            if (accountsUser == null)
            {
                return (false, null, "User Not Found");
            }
            
            if (accountsUser.Accounts == null)
            {
                return (false, null, "This User does not have any accounts.");
            }
            try
            {
                return  (true, await _accountPersistence.GetAllAccountsUser(id), null);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get Accounts");
                return (false, null, "Failed to get Accounts");
            }
        }

        public  List<Account> GetAll() => _accountPersistence.GetAll();

        public async Task<(bool, CreateAccountResponse)> Create(CreateAccountRequest request, int userId)
        {
            try
            {
                return (true, await _accountPersistence.Create(request, userId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Account");
                return (false, null);
            }
            
        }
        public async Task<(bool, IEnumerable<Movement>, string?)> GetAccount(int accountId, int userid)
        {
            try
            {
                (bool, Account) account = GetById(accountId);
                if (account.Item2 == null) { return (false, null, "Account Not Found."); }

                //CreateAccountResponse contractsAccount = EntityMapper.MapAccountModelToContract(account.Item2);

                IEnumerable<Movement> movims =
                     _movementPersistence.GetAll().Where(movement => movement.AccountId == accountId && movement.UserId == userid);
                return (true, movims, "Account found!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Account");
                return (false, null, "Failed to get Account");
            }

        }

        public  async Task<bool> Update(Account account) => await _accountPersistence.Update(account);

        public IEnumerable<Account> VerifyTransfer(TransferRequest request, int userId) => 
            _accountPersistence.GetAll().Where(a => a.AccountId == request.FromAccountId && a.UserId == userId);
        
    }
}
