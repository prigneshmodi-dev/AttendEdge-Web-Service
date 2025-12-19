using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IAMCService
    {
        Domain.AMCLister GetAll(Domain.AMCLister mLister);

        Domain.AMC Upsert(Domain.AMC mLeave);

        Domain.AMC Get(int id);

        void Delete(int id);
    }
}
