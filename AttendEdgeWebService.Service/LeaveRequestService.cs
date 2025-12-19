using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;

namespace AttendEdgeWebService.Service
{
    public class LeaveRequestService : ILeaveRequestService
    {
        #region Declaration
        private readonly ILeaveRequestRepository _repo;

        public LeaveRequestService(ILeaveRequestRepository repo)
        {
            _repo = repo;
        }

        #endregion

        #region Public Methods
        public Domain.LeaveRequestLister GetAll(Domain.LeaveRequestLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.LeaveRequest Upsert(Domain.LeaveRequest mLeave)
        {
            return _repo.Upsert(mLeave);
        }

        public Domain.LeaveRequest Get(int id)
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
