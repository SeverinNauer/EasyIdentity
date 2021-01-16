using EasyIdentity.Core;
using LanguageExt;
using MediatR;
using Projects.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.ErrorHandler;
using static LanguageExt.Prelude;
using static Users.SignupError;

namespace Users
{
    public record SignupRequest(string EmailAddress, string Password, string? Username);

    public record ProjectData(string ClientId, string ClientSecret);

    public record SignupCommand(ProjectData ProjectData, string EmailAddress, string Password, string? Username) : IRequest<Either<DomainError, User>>;

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

        private async Task<Either<DomainError, User>> SaveUser(User user, Guid projectId)
        {
            var dbUser = UserModel.FromDomain(user, projectId);
            dbContext.Users.Add(dbUser);
            return await TryAsync(async () => await dbContext.SaveChangesAsync())
                    .Match<int, Either<SignupError, User>>(
                        succ => user,
                        exn =>
                            exn
                            .ToDbError<SignupError>()
                            .Catch<UniqueViolationError>(error =>
                                error.ConstraintName switch
                                {
                                    EmailAlreadyUsed.ConstraintName => new EmailAlreadyUsed(),
                                    UsernameAlreadyUsed.ConstraintName => new UsernameAlreadyUsed(),
                                    _ => new SignupError()
                                })
                             .Handle(err => new SignupError())
                     ).CastDomainError();
        }

        public async Task<Either<DomainError, User>> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var projectId = projectQuery.Query(request.ProjectData.ClientId, request.ProjectData.ClientSecret);
            var username = request.Username.AsOption();
            var encrypedPw = passwordHasher.Hash(request.Password);
            return await User.TrySignUp(request.EmailAddress, encrypedPw, username)
                    .BindAsync((user) => SaveUser(user, projectId));
        }
    }

    public class SignupError : DomainError
    {
        public override ErrorSection Section => ErrorSection.Signup;

        public override int ErrorCode => 5;

        public override string Message => "An unhandled error occured while signing up the new user";

        public class EmailAlreadyUsed : SignupError
        {
            public const string ConstraintName = "IX_Users_ProjectId_EmailAddress";
            public override int ErrorCode => 10;
            public override string Message => "User with this email already exists";
        }

        public class UsernameAlreadyUsed : SignupError
        {
            public const string ConstraintName = "IX_Users_ProjectId_Username";
            public override int ErrorCode => 20;
            public override string Message => "User with this username already exists";
        }
    }
}