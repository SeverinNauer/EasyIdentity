using EasyIdentity.Core;
using LanguageExt;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Users
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
        private readonly IPasswordHasher passwordHasher;

        public SignupHandler(UserDbContext dbContext, IPasswordHasher passwordHasher)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Either<DomainError, User>> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var username = request.Username is null ? Option<string>.None : request.Username;
            var encrypedPw = passwordHasher.Hash(request.Password);
            var signupResult = User.TrySignUp(request.EmailAddress, encrypedPw, username);
            await signupResult.ToAsync().IfRightAsync(async user =>
            {
                var dbUser = new UserModel
                {
                    EmailAddress = user.EmailAddress,
                    EmailVerificationState = user.EmailVerificationState,
                    Password = user.Password,
                    ProjectId = request.ProjectId,
                    UserId = user.UserId,
                    Username = user.Username.MatchUnsafe<string?>(usname => usname, () => null)
                };
                dbContext.Users.Add(dbUser);
                await dbContext.SaveChangesAsync();
            });
            return signupResult;
        }
    }
}