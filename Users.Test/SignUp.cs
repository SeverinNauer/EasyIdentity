using LanguageExt;
using Moq;
using Projects.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Users.Test.Integration
{
    public class SignUp : IClassFixture<DbFixture>, IDisposable
    {
        private readonly UserDbContext dbContext;

        public SignUp(DbFixture fixture)
        {
            dbContext = fixture.DbContext;
        }

        private void ValidateUserdataFromDb(User user)
        {
            var dbUser = dbContext.Users.Where(u => u.UserId == user.UserId).Single();
            Assert.Equal(user.EmailAddress, dbUser.EmailAddress);
            Assert.Equal(user.Password, dbUser.Password);
            Assert.Equal(EmailVerificationState.Unverified, dbUser.EmailVerificationState);
            var username = user.Username.MatchUnsafe<string?>(u => u.Value, () => null);
            Assert.Equal(username, dbUser.Username);
        }

        private SignupHandler CreateHandler()
        {
            var hasher = new Mock<IPasswordHasher>();
            var query = new Mock<IQueryProjectId>();
            hasher.Setup(o => o.Hash(It.IsAny<string>())).Returns((string val) => val);
            query.Setup(q => q.Query("", "")).Returns(Guid.NewGuid());
            return new SignupHandler(dbContext, hasher.Object, query.Object);
        }

        private SignupCommand CreateCommand(string email, string password, string? username)
            => new SignupCommand(new ProjectData("", ""), email, password, username);

        [Fact]
        public async Task SignupSuccess()
        {
            var handler = CreateHandler();
            var signupCommand = CreateCommand("muster.hans@gmail.com", "admin1234!", null);
            var res = await handler.Handle(signupCommand, CancellationToken.None);
            Assert.True(res.IsRight);
            res.IfRight(user =>
            {
                ValidateUserdataFromDb(user);
            });
        }

        [Fact]
        public async Task SignupSuccess_With_Username()
        {
            var handler = CreateHandler();
            var signupCommand = CreateCommand("muster.hans@gmail.com", "admin1234!", "hans.muster");
            var res = await handler.Handle(signupCommand, CancellationToken.None);
            Assert.True(res.IsRight);
            res.IfRight(user =>
            {
                ValidateUserdataFromDb(user);
            });
        }

        [Fact]
        public async Task SignupError_Email_Already_Used()
        {
            var handler = CreateHandler();
            var signup1 = CreateCommand("muster.hans@gmail.com", "admin1234!", "hans.muster");
            var signup2 = CreateCommand("muster.hans@gmail.com", "admin1234!", "muster.hans");
            var res1 = await handler.Handle(signup1, CancellationToken.None);
            var res2 = await handler.Handle(signup2, CancellationToken.None);
            Assert.True(res1.IsRight);
            Assert.True(res2.IsLeft);
            res2.IfLeft(err =>
            {
                Assert.True(err is SignupError.EmailAlreadyUsed);
            });
        }
       
        [Fact]
        public async Task SignupError_Username_Already_Used()
        {
            var handler = CreateHandler();
            var signup1 = CreateCommand("muster.hans@gmail.com", "admin1234!", "hans.muster");
            var signup2 = CreateCommand("hans.muster@gmail.com", "admin1234!", "hans.muster");
            var res1 = await handler.Handle(signup1, CancellationToken.None);
            var res2 = await handler.Handle(signup2, CancellationToken.None);
            Assert.True(res1.IsRight);
            Assert.True(res2.IsLeft);
            res2.IfLeft(err =>
            {
                Assert.True(err is SignupError.UsernameAlreadyUsed);
            });
        }

        public void Dispose()
        {
            dbContext.Users.RemoveRange(dbContext.Users);
            dbContext.SaveChanges();
            GC.SuppressFinalize(this);
        }
    }
}