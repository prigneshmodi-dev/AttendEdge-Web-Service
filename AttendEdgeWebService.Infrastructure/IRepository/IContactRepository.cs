namespace AttendEdgeWebService.Infrastructure.IRepository
{
    public interface IContactRepository
    {
        Domain.ContactLister GetAll(Domain.ContactLister mLister);

        Domain.Contact Upsert(Domain.Contact mContactUs);

        Domain.Contact Get(int id);

        void Delete(int id);
    }
}
