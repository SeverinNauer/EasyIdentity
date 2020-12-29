using EasyIdentity.Core;
using LanguageExt;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UserManagement
{
    public class SignupRequest
    {
        public string EmailAddress { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Username { get; set; }
    }
    public class SignupCommand : IRequest<Either<DomainError, User>>
    {
        public Guid ProjectId { get; }
        public string EmailAddress { get; }
        public string Password { get; }

        public string? Username { get; }

        public SignupCommand(Guid projectId, string emailAddress, string password, string? username)
        {
            ProjectId = projectId;
            EmailAddress = emailAddress;
            Password = password;
            Username = username;
        }
    }

    public class SignupHandler : IRequestHandler<SignupCommand, Either<DomainError, User>>
    {
        private readonly UserDbContext dbContext;

        public SignupHandler(UserDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<Either<DomainError, User>> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var users = dbContext.Users.ToList();
            var username = request.Username is null ? Option<string>.None : Option<string>.Some(request.Username);
            var signupResult = User.TrySignUp(request.EmailAddress, request.Password, username);
            signupResult.IfRight(user =>
            {
                var dbUser = new UserEntity
                {
                    EmailAddress = user.EmailAddress,
                    EmailVerificationState = user.EmailVerificationState,
                    Password = user.Password,
                    ProjectId = request.ProjectId,
                    UserId = user.UserId,
                    Username = user.Username.MatchUnsafe<string?>(usname => usname, () => null)
                };
                dbContext.Users.Add(dbUser);
                dbContext.SaveChanges();
            });
            return Task.FromResult(signupResult);
        }
    }
}