using EasyIdentity.Core;
using LanguageExt;
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

        public Either<DomainError, User> ToDomain()
        {
            var userId = Users.UserId.Create(UserId);
            return 
                from username in (Username is null ?
                    Option<Username>.None
                    : Users.Username.TryCreate(Username).Match<Either<DomainError, Option<Username>>>(u => u.AsOption(), err => err))
                from email in EasyIdentity.Core.EmailAddress.TryCreate(EmailAddress).CastDomainError()
                from password in Users.Password.TryCreate(Password).CastDomainError()
                select new User(userId, email, password, username, EmailVerificationState);
        }
    }
}