using EntityFramework.ErrorHandler;
using EntityFramework.ErrorHandler.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Test
{
    public class DbFixture : IDisposable
    {
        public UserDbContext DbContext { get; }

        public DbFixture() 
        {
            var connString = $"Server = 127.0.0.1; Port = 5432; Database = userDb{Guid.NewGuid()}; User Id = postgres; Password = root";
            var builder = new DbContextOptionsBuilder<UserDbContext>();
            builder.UseNpgsql(connString);
            DbContext = new UserDbContext(builder.Options);
            DbContext.Database.Migrate();
            DbErrorHandlerConfiguration.Config.AddPostgres();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
