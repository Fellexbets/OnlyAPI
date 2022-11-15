namespace Igor_AIS_Proj.Auxiliary
{
     public static class EntityMapper
    {
        public static CreateAccountResponse MapAccountModelToContract(Account modelAccount) =>
              new CreateAccountResponse()
              {
                  AccountId = modelAccount.AccountId,
                  UserId = modelAccount.UserId,
                  Balance = modelAccount.Balance,
                  CreatedAt = modelAccount.CreatedAt,
                  Currency = modelAccount.Currency,
              };

        public static ICollection<CreateAccountResponse> MapAccountListModelToContract(IEnumerable<Account> modelAccounts)
        {
            List<CreateAccountResponse> contractsAccounts = new List<CreateAccountResponse>();
            modelAccounts.ToList().ForEach(account =>
                contractsAccounts.Add(
                    MapAccountModelToContract(account)));
            return contractsAccounts;
        }


        public static Movim MapMovementToMovim(Movement movement) =>
          new Movim()
          {
              Amount = movement.Amount,
              CreatedAt = movement.MovimentTime
          };

        public static ICollection<Movim> MapMovementEnumerableToMovim(IEnumerable<Movement> movements)
        {
            List<Movim> movimsToReturn = new List<Movim>();
            movements.ToList().ForEach(movement =>
                movimsToReturn.Add(
                    MapMovementToMovim(movement)));
            return movimsToReturn;
        }

        public static Account MapRequestToAccountModel(CreateAccountRequest accountRequest, int userId) =>
            new Account()
            {
                Balance = accountRequest.Balance,
                Currency = accountRequest.Currency,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

        public static Transfer MapRequestToTransfer(TransferRequest transferRequest) =>
            new Transfer()
            {
                Amount = transferRequest.Amount,
                OriginaccountId = transferRequest.FromAccountId,
                DestinationaccountId = transferRequest.ToAccountId,
                TransferDate = DateTime.Now
            };
        public static TransferResponse MapTransferToResponse(Transfer transfer) =>
            new TransferResponse()
            {
                Amount = transfer.Amount,
                FromAccountId = transfer.OriginaccountId,
                ToAccountId = transfer.DestinationaccountId
            };


        public static User MapRequestToUser(CreateUserRequest createUserRequest) =>
            new User()
            {
                Email = createUserRequest.Email,
                Username = createUserRequest.Username,
                UserPassword = createUserRequest.UserPassword,
                FullName = createUserRequest.FullName,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };

        public static CreateUserResponse MapUserToResponse(User user) =>
            new CreateUserResponse()
            {
                UserId = user.UserId,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
                FullName = user.FullName,
                UpdatedAt = user.UpdatedAt,
                Username = user.Username
            };

        public static Session MapRequestSession(RenewLoginRequest renewLoginRequest) =>
            new Session()
            {
                RefreshToken = renewLoginRequest.RefreshToken
            };

        public static LoginUserResponse MapLoginToResponse(User user)
            => new LoginUserResponse()
            {
                AccessToken = user.UserToken         
            };
        public static User MapLoginToUser(LoginUserRequest loginUserRequest)
            => new User()
            {
                Email = loginUserRequest.Email,
                UserPassword = loginUserRequest.UserPassword
            };


    }
}
