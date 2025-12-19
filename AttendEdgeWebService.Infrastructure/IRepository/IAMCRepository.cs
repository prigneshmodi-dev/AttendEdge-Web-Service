using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IRepository
{
    public interface IAMCRepository
    {
        Domain.AMCLister GetAll(Domain.AMCLister mLister);

        Domain.AMC Upsert(Domain.AMC mAMC);

        Domain.AMC Get(int id);

        void Delete(int id);
    }
}
