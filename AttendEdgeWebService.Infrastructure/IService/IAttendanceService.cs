using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IAttendanceService
    {
        Domain.AttendanceLister GetAll(Domain.AttendanceLister mLister);

        Domain.Attendance Upsert(Domain.Attendance mAttendance);

        void Delete(int id);
        List<Domain.Attendance> GetActiveUsers(int companyId);
    }
}
