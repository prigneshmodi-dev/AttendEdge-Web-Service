using AttendEdgeWebService.Domain;
using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace AttendEdgeWebService.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        public RoleService(IRoleRepository repo)
        {
            _repo = repo;
        }

        public List<Role> GetAll()
        {
            return _repo.GetAll();
        }
    }
}

