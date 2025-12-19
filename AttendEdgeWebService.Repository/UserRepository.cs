using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AttendEdgeWebService.Repository
{
    public class UserRepository : IUserRepository
    {
        public Domain.UserLister GetAll(Domain.UserLister mLister)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var query = from user in context.Users
                                join c in context.Companies on user.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join role in context.Roles on user.RoleId equals role.Id
                                where user.DeletedDate == null && user.DeletedBy == null
                                select new
                                {
                                    Id = user.Id,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    EmailAddress = user.EmailAddress,
                                    MobileNumber = user.MobileNumber,
                                    RoleId = user.RoleId,
                                    CompanyId = user.CompanyId,
                                    IsActive = user.IsActive,
                                    Role = role.Name,
                                    Company = c.Name
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.RoleId > 0)
                            query = query.Where(x => x.RoleId == criteria.RoleId);

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }

                        if (criteria.FirstName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToUpper().Contains(criteria.FirstName.ToUpper()));

                        if (criteria.LastName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.ToUpper().Contains(criteria.LastName.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));
                    }

                    // Switch to in-memory for complex string filters
                    var efUsers = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = efUsers.Count();

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efUsers = efUsers.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.User>>(JsonSerializer.Serialize(efUsers.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.User Upsert(Domain.User mUser)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efUser = context.Users.Where(x => x.Id == mUser.Id).FirstOrDefault();
                    if (efUser == null || efUser.Id <= default(int))
                    {
                        efUser = new Persistence.User();
                        efUser.CreatedDate = DateTime.Now;
                        efUser.CreatedBy = mUser.CreatedBy;
                        efUser.Password = mUser.Password;
                        context.Users.Add(efUser);
                    }

                    efUser.FirstName = mUser.FirstName;
                    efUser.LastName = mUser.LastName;
                    efUser.EmailAddress = mUser.EmailAddress;
                    efUser.MobileNumber = mUser.MobileNumber;
                    efUser.RoleId = mUser.RoleId;
                    efUser.CompanyId = mUser.CompanyId;
                    efUser.IsActive = mUser.IsActive;
                    efUser.LastModifiedDate = DateTime.Now;
                    efUser.LastModifiedBy = mUser.LastModifiedBy;

                    context.SaveChanges();

                    mUser.Id = efUser.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public Domain.User Get(int id)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efUser = (from user in context.Users
                                  where user.DeletedDate == null && user.DeletedBy == null && user.Id == id
                                  select new
                                  {
                                      Id = user.Id,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      EmailAddress = user.EmailAddress,
                                      MobileNumber = user.MobileNumber,
                                      RoleId = user.RoleId,
                                      IsActive = user.IsActive,
                                      CompanyId = user.CompanyId
                                  }).FirstOrDefault();

                    if (efUser != null)
                    {
                        mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new AttendEdgeWebService.Persistence.AttendEdgeDBEntities())
                {
                    var efUser = context.Users.Where(x => x.Id == id).FirstOrDefault();
                    if (efUser != null && efUser.Id > default(int))
                    {
                        efUser.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Domain.User GetBy(string emailAddress)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efUser = (from user in context.Users
                                  where user.DeletedDate == null && user.DeletedBy == null && user.EmailAddress.Trim().ToUpper() == emailAddress.Trim().ToUpper()
                                  select new
                                  {
                                      Id = user.Id,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      EmailAddress = user.EmailAddress,
                                      Password = user.Password,
                                      MobileNumber = user.MobileNumber,
                                      RoleId = user.RoleId,
                                      IsActive = user.IsActive,
                                      CompanyId = user.CompanyId
                                  }).AsEnumerable();

                    mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public List<Domain.User> GetActiveUsers(int companyId)
        {
            var userList = new List<Domain.User>();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efUsers = from user in context.Users
                                  where user.DeletedDate == null && user.DeletedBy == null && user.CompanyId == companyId
                                  select new
                                  {
                                      Id = user.Id,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                  };

                    var json = JsonSerializer.Serialize(efUsers.ToList());
                    userList = JsonSerializer.Deserialize<List<Domain.User>>(json);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return userList;
        }

        public string GetPasswordBy(int id)
        {
            string password = string.Empty;
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    password = context.Users.Where(x => x.Id == id && x.DeletedBy == null && x.DeletedDate == null).Select(s => s.Password).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return password;
        }

        public string SetPasswordBy(int id, string password)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efUser = context.Users.Where(x => x.Id == id && x.DeletedBy == null && x.DeletedDate == null).FirstOrDefault();
                    if (efUser != null && efUser.Id > default(int))
                    {
                        efUser.Password = password;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return password;
        }

        public Domain.UserDocument SaveImage(Domain.UserDocument image)
        {
            try
            {

            }catch (Exception)
            {
                throw;
            }
            return image;
        }
    }
}
