namespace Igor_AIS_Proj.Models.Responses
{
    public class LoginUserResponse
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public string SessionId { get; set; }
        public CreateUserResponse User { get; set; }

    }
}
