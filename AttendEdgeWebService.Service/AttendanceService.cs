using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AttendEdgeWebService.Service
{
    public class AttendanceService : IAttendanceService
    {
        #region Declaration
        private readonly IAttendanceRepository _repo;

        public AttendanceService(IAttendanceRepository repo)
        {
            _repo = repo;
        }

        #endregion

        #region Public Methods
        public Domain.AttendanceLister GetAll(Domain.AttendanceLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Attendance Upsert(Domain.Attendance mAttendance)
        {
            if (mAttendance == null)
                throw new Infrastructure.CustomException.APIRequestFailedException("Object must have a value!");
            
            if (mAttendance.CheckInImageBase64.IsNotNullOrEmpty())
            {
                mAttendance.CheckInImagePath = StaticMethods.SaveBase64Image(mAttendance.CheckInImageBase64, "~/Uploads/Images/CheckIn", $"{Guid.NewGuid()}.{StaticMethods.GetImageTypeFromBase64(mAttendance.CheckInImageBase64).ToLower()}");
            }
            else if (mAttendance.BreakInImageBase64.IsNotNullOrEmpty())
            {
                mAttendance.BreakInImagePath = StaticMethods.SaveBase64Image(mAttendance.BreakInImageBase64, "~/Uploads/Images/BreakIn", $"{Guid.NewGuid()}.{StaticMethods.GetImageTypeFromBase64(mAttendance.BreakInImageBase64).ToLower()}");
            }
            else if (mAttendance.BreakOutImageBase64.IsNotNullOrEmpty())
            {
                mAttendance.BreakOutImagePath = StaticMethods.SaveBase64Image(mAttendance.BreakOutImageBase64, "~/Uploads/Images/BreakOut", $"{Guid.NewGuid()}.{StaticMethods.GetImageTypeFromBase64(mAttendance.BreakOutImageBase64).ToLower()}");
            }
            else if (mAttendance.CheckOutImageBase64.IsNotNullOrEmpty())
            {
                mAttendance.CheckOutImagePath = StaticMethods.SaveBase64Image(mAttendance.CheckOutImageBase64, "~/Uploads/Images/CheckOut", $"{Guid.NewGuid()}.{StaticMethods.GetImageTypeFromBase64(mAttendance.CheckOutImageBase64).ToLower()}");
            }

            return _repo.Upsert(mAttendance);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<Domain.Attendance> GetActiveUsers(int companyId)
        {
            return _repo.GetActiveUsers(companyId);
        }
        #endregion
    }
}
