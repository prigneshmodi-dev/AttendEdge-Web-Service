using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IRoleService
    {
        List<Domain.Role> GetAll();
    }
}
