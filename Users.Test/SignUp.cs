using LanguageExt;
using Moq;
using Projects.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Users.Test
{
    public class SignUp: IClassFixture<DbFixture>
    {

        private readonly UserDbContext dbContext;
        public SignUp(DbFixture fixture)
        {
            dbContext = fixture.DbContext; 
        }

        [Fact]
        public async Task SignupSuccess()
        {
            var hasher = new Mock<IPasswordHasher>();
            var query = new Mock<IQueryProjectId>();
            hasher.Setup(o => o.Hash(It.IsAny<string>())).Returns((string val) => val);
            query.Setup(q => q.Query("", "")).Returns(Guid.NewGuid());
            var handler = new SignupHandler(dbContext, hasher.Object, query.Object);
            var signupCommand = new SignupCommand(new ProjectData("", ""), "severin.nauer@gmail.com", "admin1234!", null);
            var res = await handler.Handle(signupCommand, CancellationToken.None);
        }
    }
}