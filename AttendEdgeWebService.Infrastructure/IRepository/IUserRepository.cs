using System.Collections.Generic;

namespace AttendEdgeWebService.Infrastructure.IRepository
{
    public interface IUserRepository
    {
        Domain.UserLister GetAll(Domain.UserLister mLister);

        Domain.User Upsert(Domain.User mUser);

        Domain.User Get(int id);

        void Delete(int id);

        Domain.User GetBy(string emailAddress);

        List<Domain.User> GetActiveUsers(int companyId);
        
        string GetPasswordBy(int id);

        string SetPasswordBy(int id, string password);

       
    }
}
