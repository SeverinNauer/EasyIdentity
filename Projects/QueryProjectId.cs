using Projects.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects
{
    public class QueryProjectId : IQueryProjectId
    {
        public Guid Query(string clientId, string clientSecret)
        {
            return Guid.Parse("2418a920-3ab4-4ae9-853f-15d66fb2266a");
        }
    }
}
