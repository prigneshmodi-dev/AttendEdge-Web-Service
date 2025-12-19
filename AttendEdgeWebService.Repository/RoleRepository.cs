using AttendEdgeWebService.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace AttendEdgeWebService.Repository
{
    public class RoleRepository : IRoleRepository
    {
        public List<Domain.Role> GetAll()
        {
            var mRoles = new List<Domain.Role>();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efRoles = (from role in context.Roles
                                  where role.DeletedDate == null && role.DeletedBy == null && role.IsActive == true
                                  select new
                                  {
                                      Id = role.Id,
                                      Name = role.Name
                                  }).AsEnumerable();

                    mRoles = JsonSerializer.Deserialize<List<Domain.Role>>(JsonSerializer.Serialize(efRoles.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mRoles;
        }
    }
}
