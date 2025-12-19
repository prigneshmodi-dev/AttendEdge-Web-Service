using System.Collections.Generic;

namespace AttendEdgeWebService.Infrastructure.IRepository
{
    public interface IRoleRepository
    {
        List<Domain.Role> GetAll();

    }
}
