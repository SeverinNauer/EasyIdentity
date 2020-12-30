using EasyIdentity.Core;
using LanguageExt;
using MediatR;
using Projects.Contracts;
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
    public record ProjectData(string ClientId, string ClientSecret);

    public class SignupCommand : IRequest<Either<DomainError, User>>
    {
        public ProjectData ProjectData { get; }
        public string EmailAddress { get; }
        public string Password { get; }

        public string? Username { get; }

        public SignupCommand(ProjectData projectData, string emailAddress, string password, string? username)
        {
            ProjectData = projectData;
            EmailAddress = emailAddress;
            Password = password;
            Username = username;
        }
    }

    public class SignupHandler : IRequestHandler<SignupCommand, Either<DomainError, User>>
    {
        private readonly UserDbContext dbContext;
        private readonly IPasswordHasher passwordHasher;
        private readonly IQueryProjectId projectQuery;

        public SignupHandler(UserDbContext dbContext, IPasswordHasher passwordHasher, IQueryProjectId projectQuery)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.projectQuery = projectQuery;
        }

        public async Task<Either<DomainError, User>> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var projectId = projectQuery.Query(request.ProjectData.ClientId, request.ProjectData.ClientSecret); //TODO work with clientid and secret
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
                    ProjectId = projectId,
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