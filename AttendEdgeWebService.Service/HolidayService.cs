using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;

namespace AttendEdgeWebService.Service
{
    public class HolidayService : IHolidayService
    {
        #region Declaration
        private readonly IHolidayRepository _repo;

        public HolidayService(IHolidayRepository repo)
        {
            _repo = repo;
        }

        #endregion

        #region Public Methods
        public Domain.HolidayLister GetAll(Domain.HolidayLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Holiday Upsert(Domain.Holiday mHoliday)
        {
            return _repo.Upsert(mHoliday);
        }

        public Domain.Holiday Get(int id)
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
