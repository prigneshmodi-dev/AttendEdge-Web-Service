using System.Collections.Generic;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IUserService
    {
        Domain.UserLister GetAll(Domain.UserLister mLister);

        Domain.User Upsert(Domain.User mUser);

        Domain.User Get(int id);

        void Delete(int id);

        List<Domain.User> GetActiveUsers(int companyId);

        string SaveDocuments(Domain.UserDocument mImage);
    }
}
