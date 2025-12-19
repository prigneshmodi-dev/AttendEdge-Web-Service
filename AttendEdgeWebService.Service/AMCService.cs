using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;

namespace AttendEdgeWebService.Service
{
    public class AMCService : IAMCService
    {
        #region Declaration
        private readonly IAMCRepository _repo;

        public AMCService(IAMCRepository repo)
        {
            _repo = repo;
        }

        #endregion

        #region Public Methods
        public Domain.AMCLister GetAll(Domain.AMCLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.AMC Upsert(Domain.AMC mAMC)
        {
            return _repo.Upsert(mAMC);
        }

        public Domain.AMC Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }
        #endregion
    }
}
