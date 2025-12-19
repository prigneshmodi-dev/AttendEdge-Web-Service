using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace AttendEdgeWebService.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        public Domain.AttendanceLister GetAll(Domain.AttendanceLister mLister)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var query = from attend in context.Attendances
                                join user in context.Users on attend.UserId equals user.Id
                                join c in context.Companies on attend.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                where attend.DeletedDate == null && attend.DeletedBy == null
                                select new
                                {
                                    Id = attend.Id,
                                    CompanyId = attend.CompanyId,
                                    UserId = attend.UserId,
                                    CheckInTime = attend.CheckInTime,
                                    CheckOutTime = attend.CheckOutTime,
                                    BreakOutTime = attend.BreakOutTime,
                                    BreakInTime = attend.BreakInTime,
                                    LatitudeCheckIn = attend.LatitudeCheckIn,
                                    LatitudeCheckOut = attend.LatitudeCheckOut,
                                    LongitudeCheckIn = attend.LongitudeCheckIn,
                                    LongitudeCheckOut = attend.LongitudeCheckOut,
                                    LatitudeBreakIn = attend.LatitudeBreakIn,
                                    LatitudeBreakOut = attend.LatitudeBreakOut,
                                    LongitudeBreakIn = attend.LongitudeBreakIn,
                                    LongitudeBreakOut = attend.LongitudeBreakOut,
                                    CheckInImagePath = attend.CheckInImagePath,
                                    CheckOutImagePath = attend.CheckOutImagePath,
                                    BreakInImagePath = attend.BreakInImagePath,
                                    BreakOutImagePath = attend.BreakOutImagePath,
                                    LocationDescription = attend.LocationDescription,
                                    DeviceInfo = attend.DeviceInfo,
                                    IPAddress = attend.IPAddress,
                                    Company = c.Name,
                                    Username = user.FirstName + " " + user.LastName
                                };

                    // Optional filtering
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Company.IsNotNullOrEmpty())
                            query = query.Where(x => x.Company != null && x.Company.ToUpper() == criteria.Company.ToUpper());

                        if (criteria.Username.IsNotNullOrEmpty())
                            query = query.Where(x => x.Username != null && x.Username.ToUpper() == criteria.Username.ToUpper());

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);

                        if (criteria.DeviceInfo.IsNotNullOrEmpty())
                            query = query.Where(x => x.DeviceInfo != null &&
                                x.DeviceInfo.ToUpper() == criteria.DeviceInfo.ToUpper());

                        if (criteria.IPAddress.IsNotNullOrEmpty())
                            query = query.Where(x => x.IPAddress != null &&
                                x.IPAddress.ToUpper() == criteria.IPAddress.ToUpper());
                    }


                    var efAttendance = query.AsEnumerable();

                    // Pagination
                    var pagination = mLister.Pagination ?? new Domain.Pagination();
                    pagination.TotalRecord = efAttendance.Count();

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;
                    pagination.TotalPage = pagination.Take > 0
                        ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take)
                        : 1;

                    efAttendance = efAttendance.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Attendance>>(JsonSerializer.Serialize(efAttendance.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.Attendance Upsert(Domain.Attendance mAttendance)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efAttend = context.Attendances.Where(x => x.Id == mAttendance.Id).FirstOrDefault();
                    if (efAttend == null || efAttend.Id <= default(int))
                    {
                        efAttend = new Persistence.Attendance();
                        efAttend.CreatedDate = DateTime.Now;
                        efAttend.CreatedBy = mAttendance.CreatedBy;
                        context.Attendances.Add(efAttend);
                    }

                    efAttend.CompanyId = mAttendance.CompanyId;
                    efAttend.UserId = mAttendance.UserId;
                    efAttend.CheckInTime = mAttendance.CheckInTime;
                    efAttend.CheckOutTime = mAttendance.CheckOutTime;
                    efAttend.BreakOutTime = mAttendance.BreakOutTime;
                    efAttend.BreakInTime = mAttendance.BreakInTime;
                    efAttend.LatitudeCheckIn = mAttendance.LatitudeCheckIn;
                    efAttend.LatitudeCheckOut = mAttendance.LatitudeCheckOut;
                    efAttend.LongitudeCheckIn = mAttendance.LongitudeCheckIn;
                    efAttend.LongitudeCheckOut = mAttendance.LongitudeCheckOut;
                    efAttend.LatitudeBreakIn = mAttendance.LatitudeBreakIn;
                    efAttend.LatitudeBreakOut = mAttendance.LatitudeBreakOut;
                    efAttend.LongitudeBreakIn = mAttendance.LongitudeBreakIn;
                    efAttend.LongitudeBreakOut = mAttendance.LongitudeBreakOut;
                    efAttend.CheckInImagePath = mAttendance.CheckInImagePath.IsNullOrEmpty() ? string.Empty : mAttendance.CheckInImagePath;
                    efAttend.CheckOutImagePath = mAttendance.CheckOutImagePath;
                    efAttend.BreakInImagePath = mAttendance.BreakInImagePath;
                    efAttend.BreakOutImagePath = mAttendance.BreakOutImagePath;
                    efAttend.LocationDescription = mAttendance.LocationDescription.IsNullOrEmpty() ? string.Empty : mAttendance.LocationDescription;
                    efAttend.DeviceInfo = mAttendance.DeviceInfo.IsNullOrEmpty() ? string.Empty : mAttendance.DeviceInfo;
                    efAttend.IPAddress = mAttendance.IPAddress.IsNullOrEmpty() ? string.Empty : mAttendance.IPAddress; ;
                    efAttend.LastModifiedDate = DateTime.Now;
                    efAttend.LastModifiedBy = mAttendance.LastModifiedBy;

                    context.SaveChanges();

                    mAttendance.Id = efAttend.Id;
                }
            }
            catch
            {
                throw;
            }

            return mAttendance;
        }


        public void Delete(int id)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efLog = context.Attendances.FirstOrDefault(x => x.Id == id);
                    if (efLog != null && efLog.Id > default(int))
                    {
                        efLog.DeletedDate = DateTime.Now;
                    }

                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Domain.Attendance> GetActiveUsers(int companyId)
        {
            var userList = new List<Domain.Attendance>();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efUsers = from attend in context.Users
                                  where attend.DeletedDate == null && attend.DeletedBy == null && attend.CompanyId == companyId
                                  select new
                                  {
                                      Id = attend.Id,
                                      FirstName = attend.FirstName,
                                      LastName = attend.LastName,
                                  };

                    var json = JsonSerializer.Serialize(efUsers.ToList());
                    userList = JsonSerializer.Deserialize<List<Domain.Attendance>>(json);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return userList;
        }
    }
}
