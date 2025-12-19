using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IRepository
{
    public interface IAttendanceRepository
    {
        Domain.AttendanceLister GetAll(Domain.AttendanceLister mLister);

        Domain.Attendance Upsert(Domain.Attendance mAttendence);

        void Delete(int id);
        List<Domain.Attendance> GetActiveUsers(int companyId);
    }
}
