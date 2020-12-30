using System;

namespace Projects.Contracts
{
    public interface IQueryProjectId
    {
        Guid Query(string clientId, string clientSecret);
    }
}
