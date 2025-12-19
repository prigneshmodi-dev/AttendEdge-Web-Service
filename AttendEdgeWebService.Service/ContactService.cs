using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;

namespace AttendEdgeWebService.Service
{
    public class ContactService : IContactService
    {
        #region Declaration
        private readonly IContactRepository _repo;

        public ContactService(IContactRepository repo)
        {
            _repo = repo;
        }

        #endregion

        #region Public Methods
        public Domain.ContactLister GetAll(Domain.ContactLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Contact Upsert(Domain.Contact mContactUs)
        {
            return _repo.Upsert(mContactUs);
        }

        public Domain.Contact Get(int id)
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
