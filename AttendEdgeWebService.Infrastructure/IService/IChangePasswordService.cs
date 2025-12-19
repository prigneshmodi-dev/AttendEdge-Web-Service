using System;
using System.Linq;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IChangePasswordService
    {
        void Upsert(Domain.ChangePassword mChangePassword);
    }
}
