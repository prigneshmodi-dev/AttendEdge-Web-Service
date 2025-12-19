using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface ILeaveRequestService
    {
        Domain.LeaveRequestLister GetAll(Domain.LeaveRequestLister mLister);

        Domain.LeaveRequest Upsert(Domain.LeaveRequest mLeave);

        Domain.LeaveRequest Get(int id);

        void Delete(int id);
    }
}
