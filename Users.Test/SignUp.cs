using Moq;
using Projects.Contracts;
using System;
using System.Threading;
using Xunit;

namespace Users.Test
{
    public class SignUp
    {
        [Fact]
        public void SignupSuccess()
        {
            var hasher = new Mock<IPasswordHasher>();
            var query = new Mock<IQueryProjectId>();
            hasher.Setup(o => o.Hash(It.IsAny<string>())).Returns((string val) => val);
            query.Setup(q => q.Query("", "")).Returns(Guid.NewGuid());
            var handler = new SignupHandler(new UserDbContext(), hasher.Object, query.Object);
            var res = handler.Handle(new SignupCommand(new ProjectData("", ""), "", "", ""), CancellationToken.None);
        }
    }
}