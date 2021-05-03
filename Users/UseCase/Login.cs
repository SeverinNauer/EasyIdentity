using EasyIdentity.Core;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Projects.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Users.UseCase.LoginError;

namespace Users.UseCase
{
    public record LoginCommand(string EmailOrUsername, string Password, ProjectData projectData) : IRequest<Either<DomainError, string>>;

    public class LoginHandler : IRequestHandler<LoginCommand, Either<DomainError, string>>
    {
        private readonly UserDbContext userContext;
        private readonly IQueryProjectId queryProject;
        private readonly IPasswordHasher hasher;

        public LoginHandler(UserDbContext userContext, IQueryProjectId queryProject, IPasswordHasher hasher)
        {
            this.userContext = userContext;
            this.queryProject = queryProject;
            this.hasher = hasher;
        }

        private async Task<Either<LoginError, UserModel>> FindUserWithEmail(Guid projectId, string email, string password)
        {
            var user = await userContext.Users.Where(u => u.EmailAddress == email && u.ProjectId == projectId).SingleOrDefaultAsync();
            if (user is null)
            {
                return new NoUserWithEmailFound();
            }
            if (hasher.Verify(password, user.Password))
            {
                return user;
            }
            return new InvalidPassword();
        }

        private async Task<Either<LoginError, UserModel>> FindUserWithUsername(Guid projectId, string username, string password)
        {
            var user = await userContext.Users.Where(u => u.Username == username && u.ProjectId == projectId).SingleOrDefaultAsync();
            if (user is null)
            {
                return new NoUserWithUsernameFound();
            }
            if (hasher.Verify(password, user.Password))
            {
                return user;
            }
            return new InvalidPassword();
        }

        public async Task<Either<DomainError, string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var (emailOrUsername, password, projectdata) = request;
            var projectId = queryProject.Query(projectdata.ClientId, projectdata.ClientSecret);
            var isEmail = EmailAddress.TryCreate(emailOrUsername).IsRight;
            var userEither = isEmail ? await FindUserWithEmail(projectId, emailOrUsername, password) : await FindUserWithUsername(projectId, emailOrUsername, password);
            return
                from dbUser in userEither.CastDomainError()
                from user in dbUser.ToDomain()
                let generator = new JwtGenerator(projectdata.ClientSecret, user)
                select generator.Generate();
        }
    }

    public class LoginError : DomainError
    {
        public override ErrorSection Section => ErrorSection.Login;

        public override int ErrorCode => 5;

        public override string Message => "An error occured while trying to login";

        public class NoUserWithEmailFound : LoginError
        {
            public override int ErrorCode => 10;
            public override string Message => "No User with this email";
        }

        public class NoUserWithUsernameFound : LoginError
        {
            public override int ErrorCode => 20;
            public override string Message => "No User with this username";
        }

        public class InvalidPassword : LoginError
        {
            public override int ErrorCode => 30;
            public override string Message => "Invalid password for this user";
        }
    }
}