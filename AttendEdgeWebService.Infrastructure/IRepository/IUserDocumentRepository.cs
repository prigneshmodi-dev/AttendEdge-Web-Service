using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IRepository
{
    public interface IUserDocumentRepository
    {
        Domain.UserDocument Create(Domain.UserDocument mDocument);

        List<Domain.UserDocument> Get(int userId);

        List<Domain.UserDocument> FetchUserDocument(int userId);

        void Delete(int id);
    }
}
