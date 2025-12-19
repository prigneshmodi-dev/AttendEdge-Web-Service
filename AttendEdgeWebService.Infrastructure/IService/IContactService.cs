namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IContactService
    {
        Domain.ContactLister GetAll(Domain.ContactLister mLister);

        Domain.Contact Upsert(Domain.Contact mContactUs);

        Domain.Contact Get(int id);

        void Delete(int id);

    }
}
