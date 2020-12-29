using System;

namespace Users
{
    public sealed class UserModel
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; } = "";
        public string? Username { get; set; }
        public string Password { get; set; } = "";
        public EmailVerificationState EmailVerificationState { get; set; } = EmailVerificationState.Unverified;

        public static UserModel FromDomain(User user, Guid projectId)
        {
            return new UserModel
            {
                EmailAddress = user.EmailAddress,
                EmailVerificationState = user.EmailVerificationState,
                Password = user.Password,
                ProjectId = projectId,
                UserId = user.UserId,
                Username = user.Username.MatchUnsafe<string?>(usname => usname, () => null)
            };
        }
    }
}